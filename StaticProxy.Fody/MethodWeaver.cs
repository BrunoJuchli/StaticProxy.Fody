namespace StaticProxy.Fody
{
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;

    public class MethodWeaver
    {
        private readonly MethodReference getMethodFromHandleRef;
        private readonly TypeReference methodBaseTypeRef;
        private readonly TypeReference objectArrayTypeRef;

        private readonly MethodReference interceptMethod;

        public MethodWeaver()
        {
            this.getMethodFromHandleRef = WeavingInformation.ReferenceFinder.GetMethodReference(typeof(MethodBase), md => md.Name == "GetMethodFromHandle" && md.Parameters.Count == 2);
            this.methodBaseTypeRef = WeavingInformation.ReferenceFinder.GetTypeReference(typeof(MethodBase));
            this.objectArrayTypeRef = WeavingInformation.ReferenceFinder.GetTypeReference(typeof(object[]));

            this.interceptMethod = ImportInterceptMethod();
        }

        public void DecorateMethod(MethodDefinition method, FieldDefinition interceptorManager)
        {
            string newMethodName = string.Format(CultureInfo.InvariantCulture, "{0}<SP>", method.Name);
            MethodDefinition implementationMethod = CopyMethod(method, newMethodName, method.DeclaringType);
            
            DeleteMethodImplementation(method);

            this.WeaveInterceptionCall(method, method, implementationMethod, interceptorManager);
        }

        public void ImplementMethod(MethodDefinition interfaceMethod, FieldDefinition interceptorManager)
        {
            MethodDefinition interceptingMethod = CopyMethod(interfaceMethod, interfaceMethod.Name, interceptorManager.DeclaringType);
            
            this.WeaveInterceptionCall(interceptingMethod, interfaceMethod, null, interceptorManager);
        }

        private static void DeleteMethodImplementation(MethodDefinition method)
        {
            method.Body.Instructions.Clear();
            method.Body.ExceptionHandlers.Clear();
            method.Body.Variables.Clear();
        }

        private static MethodDefinition CopyMethod(MethodDefinition templateMethod, string newMethodName, TypeDefinition targetType)
        {
            var newMethod = new MethodDefinition(
                newMethodName, 
                templateMethod.Attributes, 
                templateMethod.ReturnType);

            newMethod.Body.InitLocals = true;

            foreach (var parameterDefinition in templateMethod.Parameters)
            {
                newMethod.Parameters.Add(parameterDefinition);
            }

            foreach (var variableDefinition in templateMethod.Body.Variables)
            {
                newMethod.Body.Variables.Add(variableDefinition);
            }

            foreach (ExceptionHandler exceptionHandler in templateMethod.Body.ExceptionHandlers)
            {
                newMethod.Body.ExceptionHandlers.Add(exceptionHandler);
            }

            foreach (var instruction in templateMethod.Body.Instructions)
            {
                newMethod.Body.Instructions.Add(instruction);
            }

            newMethod.Body.OptimizeMacros();

            targetType.Methods.Add(newMethod);

            return newMethod;
        }

        private static MethodReference ImportInterceptMethod()
        {
            TypeDefinition interceptorManagerDefinition = WeavingInformation.DynamicInterceptorManagerReference.Resolve();
            return WeavingInformation.ModuleDefinition.Import(
                interceptorManagerDefinition.Methods.Single(x1 => x1.Name == "Intercept"));
        }

        private static void HandleInterceptReturnValue(MethodDefinition method, ILProcessor processor)
        {
            if (method.ReturnType.IsValueType)
            {
                // unbox
                processor.Emit(OpCodes.Unbox_Any, method.ReturnType);
            }
            else if (method.ReturnType == WeavingInformation.ModuleDefinition.TypeSystem.Void) 
            {
                // remove return value of intercept method from stack
                processor.Emit(OpCodes.Pop);
            }
            else
            {
                // cast to reference type
                processor.Emit(OpCodes.Castclass, method.ReturnType);
            }
        }

        private void WeaveInterceptionCall(
            MethodDefinition methodToExtend,
            MethodDefinition decoratedMethodParameter,
            MethodDefinition implementationMethodParameter,
            FieldDefinition interceptorManager)
        {
            methodToExtend.Body.InitLocals = true;
            ILProcessor processor = methodToExtend.Body.GetILProcessor();
            processor.Emit(OpCodes.Nop);

            var decoratedMethodVar = methodToExtend.AddVariableDefinition("__fody$originalMethod", this.methodBaseTypeRef);
            var implementationMethodVar = methodToExtend.AddVariableDefinition("__fody$implementationMethod", this.methodBaseTypeRef);
            var parametersVar = methodToExtend.AddVariableDefinition("__fody$parameters", this.objectArrayTypeRef);

            this.SaveMethodBaseToVariable(processor, decoratedMethodParameter, decoratedMethodVar);
            if (implementationMethodParameter != null)
            {
                this.SaveMethodBaseToVariable(processor, implementationMethodParameter, implementationMethodVar);
            }
            
            processor.SaveParametersToNewObjectArray(parametersVar, methodToExtend.Parameters.ToArray());

            this.CallInterceptMethod(interceptorManager, processor, decoratedMethodVar, implementationMethodVar, parametersVar);

            HandleInterceptReturnValue(methodToExtend, processor);

            // write method end
            processor.Emit(OpCodes.Ret);
            methodToExtend.Body.OptimizeMacros();
        }

        private void SaveMethodBaseToVariable(ILProcessor processor, MethodDefinition decoratedMethod, VariableDefinition methodBaseVar)
        {
            processor.Emit(OpCodes.Ldtoken, decoratedMethod);
            processor.Emit(OpCodes.Ldtoken, decoratedMethod.DeclaringType);
            processor.Emit(OpCodes.Call, this.getMethodFromHandleRef); // Push method onto the stack, GetMethodFromHandle, result on stack
            processor.Emit(OpCodes.Stloc_S, methodBaseVar); // Store method in __fody$method
        }

        private void CallInterceptMethod(
            FieldDefinition interceptorManager, 
            ILProcessor processor,
            VariableDefinition decoratedMethodVar,
            VariableDefinition implementationMethodVar, 
            VariableDefinition parametersVar)
        {
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, interceptorManager);
            processor.Emit(OpCodes.Ldloc_S, decoratedMethodVar);
            processor.Emit(OpCodes.Ldloc_S, implementationMethodVar);
            processor.Emit(OpCodes.Ldloc_S, parametersVar);
            processor.Emit(OpCodes.Callvirt, this.interceptMethod);
        }
    }
}