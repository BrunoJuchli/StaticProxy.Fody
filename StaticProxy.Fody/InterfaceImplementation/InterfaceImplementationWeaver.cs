namespace StaticProxy.Fody.InterfaceImplementation
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    using StaticProxy.Fody.MethodWeaving;

    public class InterfaceImplementationWeaver
    {
        public const string ClassNameSuffix = "Implementation";

        private readonly ConstructorWeaver constructorWeaver;
        private readonly MethodWeaver methodWeaver;

        private readonly TypeReference objectTypeReference;
        private readonly MethodReference objectConstructorReference;

        public InterfaceImplementationWeaver(ConstructorWeaver constructorWeaver, MethodWeaver methodWeaver)
        {
            this.constructorWeaver = constructorWeaver;
            this.methodWeaver = methodWeaver;
            this.objectTypeReference = WeavingInformation.ReferenceFinder.GetTypeReference(typeof(object));
            this.objectConstructorReference = WeavingInformation.ReferenceFinder.GetMethodReference(this.objectTypeReference, x => x.IsConstructor);
        }

        public TypeDefinition CreateImplementationOf(TypeDefinition interfaceToImplement)
        {
            var classType = new TypeDefinition(
                interfaceToImplement.Namespace,
                GenerateImplementationName(interfaceToImplement),
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
                this.objectTypeReference);

            foreach(GenericParameter genericParameter in interfaceToImplement.GenericParameters)
            {
                classType.GenericParameters.Add(genericParameter.CreateCopy(classType));
            }

            AddEmptyConstructor(classType, this.objectConstructorReference);

            FieldDefinition dynamicInterceptorManager = this.constructorWeaver.ExtendConstructorWithDynamicInterceptorManager(classType, true);

            foreach (MethodDefinition interfaceMethod in interfaceToImplement.Methods)
            {
                this.methodWeaver.ImplementMethod(interfaceMethod, dynamicInterceptorManager);
            }

            if (interfaceToImplement.HasGenericParameters)
            {
                var genericInterface = new GenericInstanceType(interfaceToImplement);
                foreach (var argument in interfaceToImplement.GenericParameters)
                {
                    genericInterface.GenericArguments.Add(argument);
                }

                classType.Interfaces.Add(new InterfaceImplementation(genericInterface));
            }
            else
            {
                classType.Interfaces.Add(new InterfaceImplementation(interfaceToImplement));
            }

            WeavingInformation.ModuleDefinition.Types.Add(classType);

            return classType;
        }

        private static string GenerateImplementationName(TypeReference interfaceToImplement)
        {
            var originalNameParts = interfaceToImplement.Name.Split('`');
            if(originalNameParts.Length == 1)
            {
                return originalNameParts[0] + ClassNameSuffix;
            }
            else if(originalNameParts.Length == 2)
            {
                return originalNameParts[0] + ClassNameSuffix + "`" + originalNameParts[1];
            }
            else
            {
                throw new WeavingException($"Interface Name '{interfaceToImplement.Name}' unexpectedly contains more than one '`' character.");
            }
        }

        private static void AddEmptyConstructor(TypeDefinition type, MethodReference baseEmptyConstructor)
        {
            const MethodAttributes MethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            var method = new MethodDefinition(".ctor", MethodAttributes, WeavingInformation.ModuleDefinition.TypeSystem.Void);
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, baseEmptyConstructor));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            type.Methods.Add(method);
        }
    }
}