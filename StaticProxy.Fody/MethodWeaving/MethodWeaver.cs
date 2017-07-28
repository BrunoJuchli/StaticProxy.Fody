namespace StaticProxy.Fody.MethodWeaving
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public class MethodWeaver
    {
        private readonly MethodReference getMethodFromHandleRef;
        private readonly TypeReference methodBaseTypeRef;
        private readonly TypeReference objectArrayTypeRef;
        private readonly TypeReference typeArrayTypeRef;

        private readonly MethodReference interceptMethod;

        public MethodWeaver()
        {
            this.getMethodFromHandleRef = WeavingInformation.ReferenceFinder.GetMethodReference(typeof(MethodBase), md => md.Name == "GetMethodFromHandle" && md.Parameters.Count == 2);
            this.methodBaseTypeRef = WeavingInformation.ReferenceFinder.GetTypeReference(typeof(MethodBase));
            this.objectArrayTypeRef = WeavingInformation.ReferenceFinder.GetTypeReference(typeof(object[]));
            this.typeArrayTypeRef = WeavingInformation.ReferenceFinder.GetTypeReference(typeof(Type[]));

            this.interceptMethod = ImportInterceptMethod();
        }

        public void DecorateMethod(MethodDefinition method, FieldDefinition interceptorManager)
        {
            string newMethodName = string.Format(CultureInfo.InvariantCulture, "{0}<SP>", method.Name);
            MethodDefinition implementationMethod = method.CreateCopy(newMethodName);

            method.DeclaringType.Methods.Add(implementationMethod);

            DeleteMethodImplementation(method);

            this.WeaveInterceptionCall(method, method, implementationMethod, interceptorManager);
        }

        public void ImplementMethod(MethodDefinition interfaceMethod, FieldDefinition interceptorManager)
        {
            MethodDefinition interceptingMethod = interfaceMethod.CreateImplementation();

            this.WeaveInterceptionCall(interceptingMethod, interfaceMethod, null, interceptorManager);

            interceptorManager.DeclaringType.Methods.Add(interceptingMethod);
        }

        private static void DeleteMethodImplementation(MethodDefinition method)
        {
            method.Body.Instructions.Clear();
            method.Body.ExceptionHandlers.Clear();
            method.Body.Variables.Clear();
        }

        private static MethodReference ImportInterceptMethod()
        {
            TypeDefinition interceptorManagerDefinition = WeavingInformation.DynamicInterceptorManagerReference.Resolve();
            return WeavingInformation.ModuleDefinition.ImportReference(
                interceptorManagerDefinition.Methods.Single(x1 => x1.Name == "Intercept"));
        }

        private static void HandleInterceptReturnValue(MethodDefinition method, ILProcessor processor)
        {
            if (method.ReturnType.IsValueType || method.ReturnType.IsGenericParameter)
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
            var genericArgumentsVar = methodToExtend.AddVariableDefinition("__fody$genericParameters", this.typeArrayTypeRef);
            var parametersVar = methodToExtend.AddVariableDefinition("__fody$parameters", this.objectArrayTypeRef);

            this.SaveMethodBaseToVariable(processor, decoratedMethodParameter, decoratedMethodVar);
            if (implementationMethodParameter != null)
            {
                this.SaveMethodBaseToVariable(processor, implementationMethodParameter, implementationMethodVar);
            }

            processor.SaveGenericParametersToNewTypeArray(methodToExtend.GenericParameters);
            processor.SaveCurrentStackValueToVariable(genericArgumentsVar);

            processor.SaveParametersToNewObjectArray(methodToExtend.Parameters.ToArray());
            processor.SaveCurrentStackValueToVariable(parametersVar);

            this.CallInterceptMethod(interceptorManager, processor, decoratedMethodVar, implementationMethodVar, genericArgumentsVar, parametersVar);

            HandleInterceptReturnValue(methodToExtend, processor);

            // write method end
            processor.Emit(OpCodes.Ret);
            methodToExtend.Body.OptimizeMacros();
        }

        private void SaveMethodBaseToVariable(ILProcessor processor, MethodDefinition decoratedMethod, VariableDefinition methodBaseVar)
        {
            processor.Emit(OpCodes.Ldtoken, decoratedMethod);

            if (decoratedMethod.DeclaringType.HasGenericParameters)
            {
                processor.Emit(OpCodes.Ldarg_0); // load "this" onto the stack
                processor.Emit(OpCodes.Call, WeavingInformation.GetTypeMethodReference); // call "GetType()" on "this"
                processor.Emit(OpCodes.Callvirt, WeavingInformation.GetInterfacesMethodReference); // call "GetInterfaces()" on the Type
                processor.Emit(OpCodes.Ldc_I4_0); // load index 0 to the stack
                processor.Emit(OpCodes.Ldelem_Ref); // load the interface-Type with the index 0 from the stack
                processor.Emit(OpCodes.Callvirt, WeavingInformation.TypeHandlePropertyGetMethodReference);  // call "get_TypeHandle" on the Type on the stack
            }
            else
            {
                processor.Emit(OpCodes.Ldtoken, decoratedMethod.DeclaringType);
            }

            processor.Emit(OpCodes.Call, this.getMethodFromHandleRef); // Push method onto the stack, GetMethodFromHandle, result on stack
            processor.Emit(OpCodes.Stloc_S, methodBaseVar); // Store method in __fody$method
        }

        private void CallInterceptMethod(
            FieldDefinition interceptorManager,
            ILProcessor processor,
            VariableDefinition decoratedMethodVar,
            VariableDefinition implementationMethodVar,
            VariableDefinition genericArgumentsVar,
            VariableDefinition parametersVar)
        {
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, interceptorManager);
            processor.Emit(OpCodes.Ldloc_S, decoratedMethodVar);
            processor.Emit(OpCodes.Ldloc_S, implementationMethodVar);
            processor.Emit(OpCodes.Ldloc_S, genericArgumentsVar);
            processor.Emit(OpCodes.Ldloc_S, parametersVar);
            processor.Emit(OpCodes.Callvirt, this.interceptMethod);
        }
    }
}