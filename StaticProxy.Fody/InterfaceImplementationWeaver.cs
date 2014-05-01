namespace StaticProxy.Fody
{
    using System.Globalization;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public class InterfaceImplementationWeaver
    {
        public const string ClassNameSuffix = "Implementation";

        private readonly TypeReference objectTypeReference;
        private readonly MethodReference objectConstructorReference;

        public InterfaceImplementationWeaver()
        {
            this.objectTypeReference = WeavingInformation.ReferenceFinder.GetTypeReference(typeof(object));
            this.objectConstructorReference = WeavingInformation.ReferenceFinder.GetMethodReference(this.objectTypeReference, x => x.IsConstructor);
        }

        public TypeDefinition CreateImplementationOf(TypeDefinition interfaceToImplement)
        {
            var classType = new TypeDefinition(
                interfaceToImplement.Namespace,
                GenerateImplementationName(interfaceToImplement),
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed,
                this.objectTypeReference);
            // todo copy generic parameters

            AddEmptyConstructor(classType, this.objectConstructorReference);
            
            // todo add methods and enable this
            // classType.Interfaces.Add(interfaceToImplement);
            
            WeavingInformation.ModuleDefinition.Types.Add(classType);

            return classType;
        }

        private static string GenerateImplementationName(TypeDefinition interfaceToImplement)
        {
            if (interfaceToImplement.HasGenericParameters)
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    interfaceToImplement.Name.Replace("`", "{0}`"),
                    ClassNameSuffix);
            }
            
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