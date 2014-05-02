namespace StaticProxy.Fody.InterfaceImplementation
{
    using System.Globalization;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public class InterfaceImplementationWeaver
    {
        public const string ClassNameSuffix = "Implementation";

        private readonly ConstructorWeaver constructorWeaver;
        private readonly TypeReference objectTypeReference;
        private readonly MethodReference objectConstructorReference;

        public InterfaceImplementationWeaver(ConstructorWeaver constructorWeaver)
        {
            this.constructorWeaver = constructorWeaver;
            this.objectTypeReference = WeavingInformation.ReferenceFinder.GetTypeReference(typeof(object));
            this.objectConstructorReference = WeavingInformation.ReferenceFinder.GetMethodReference(this.objectTypeReference, x => x.IsConstructor);
        }

        public TypeDefinition CreateImplementationOf(TypeDefinition interfaceToImplement)
        {
            if (interfaceToImplement.HasGenericParameters)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "interface '{0}' has generic parameters which is currently not supported.",
                    interfaceToImplement.FullName);
                throw new WeavingException(message);
            }

            var classType = new TypeDefinition(
                interfaceToImplement.Namespace,
                GenerateImplementationName(interfaceToImplement),
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed,
                this.objectTypeReference);

            AddEmptyConstructor(classType, this.objectConstructorReference);

            this.constructorWeaver.ExtendConstructorWithDynamicInterceptorRetriever(classType);

            // todo add methods and enable this
            // classType.Interfaces.Add(interfaceToImplement);

            WeavingInformation.ModuleDefinition.Types.Add(classType);

            return classType;
        }

        private static string GenerateImplementationName(TypeDefinition interfaceToImplement)
        {
            return string.Concat(interfaceToImplement.Name, ClassNameSuffix);
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