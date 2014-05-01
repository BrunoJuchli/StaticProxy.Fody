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
            MethodDefinition implementationMethod = CopyMethod(method, method.DeclaringType);
            
            ILProcessor processor = ReplaceOriginalMethod(method);
            method.Body.InitLocals = true;

            var decoratedMethodVar = method.AddVariableDefinition("__fody$originalMethod", this.methodBaseTypeRef);
            var implementationMethodVar = method.AddVariableDefinition("__fody$implementationMethod", this.methodBaseTypeRef);
            var parametersVar = method.AddVariableDefinition("__fody$parameters", this.objectArrayTypeRef);

            this.SaveMethodBaseToVariable(processor, method, decoratedMethodVar);
            this.SaveMethodBaseToVariable(processor, implementationMethod, implementationMethodVar);
            processor.SaveParametersToNewObjectArray(parametersVar, method.Parameters.ToArray());

            this.CallInterceptMethod(interceptorManager, processor, decoratedMethodVar, implementationMethodVar, parametersVar);

            HandleInterceptReturnValue(method, processor);

            // write method end
            processor.Emit(OpCodes.Ret);
            method.Body.OptimizeMacros();
        }

        private static ILProcessor ReplaceOriginalMethod(MethodDefinition method)
        {
            method.Body.Instructions.Clear();
            method.Body.ExceptionHandlers.Clear();
            method.Body.Variables.Clear();
            ILProcessor processor = method.Body.GetILProcessor();

            processor.Emit(OpCodes.Nop);

            return processor;
        }

        private static MethodDefinition CopyMethod(MethodDefinition templateMethod, TypeDefinition targetType)
        {
            var newMethod = new MethodDefinition(
                string.Format(CultureInfo.InvariantCulture, "{0}<SP>", templateMethod.Name), 
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
            // todo replace by check for WeavingInformation.ModuleDefinition.TypeSystem.Void ?
            else if (method.ReturnType.FullName == "System.Void") 
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