namespace StaticProxy.Fody
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using System.Linq;


    public class ConstructorWeaver
    {
        private readonly TypeReference interceptorManagerReference;
        private readonly MethodReference managerInitializeMethodReference;

        private readonly MethodReference ensureDynamicInterceptorManagerNotNull;

        public ConstructorWeaver()
        {
            this.interceptorManagerReference = WeavingInformation.DynamicInterceptorManagerReference;
            this.managerInitializeMethodReference = WeavingInformation.ModuleDefinition.ImportReference(this.interceptorManagerReference.Resolve().GetMethods().Single(x => x.Name == "Initialize"));

            TypeDefinition originalExceptionsTypeDefinition =
                WeavingInformation.InterceptorModuleDefinition.GetTypeDefinition("StaticProxy.Interceptor.Exceptions");
            TypeDefinition importedExceptionsTypeDefinition = WeavingInformation.ModuleDefinition.ImportReference(originalExceptionsTypeDefinition).Resolve();

            this.ensureDynamicInterceptorManagerNotNull =
                WeavingInformation.ModuleDefinition.ImportReference(importedExceptionsTypeDefinition.GetMethods().Single(x => x.Name == "EnsureDynamicInterceptorManagerNotNull"));
        }

        public FieldDefinition ExtendConstructorWithDynamicInterceptorManager(TypeDefinition typeToProxy, bool requiresInterceptor)
        {
            FieldDefinition field = AddPrivateReadonlyField(typeToProxy, this.interceptorManagerReference);

            MethodDefinition constructor = typeToProxy.GetNonStaticConstructors().Single();
            constructor.Body.InitLocals = true;

            ParameterDefinition parameter = AddParameter(constructor, this.interceptorManagerReference);

            Instruction firstInstruction = FindFirstInstruction(constructor);

            ILProcessor processor = constructor.Body.GetILProcessor();

            processor.InsertBefore(firstInstruction, InstructionHelper.CallMethodAndPassParameter(this.ensureDynamicInterceptorManagerNotNull, parameter));
            processor.InsertBefore(firstInstruction, InstructionHelper.SetInstanceFieldToMethodParameter(field, parameter));
            processor.InsertBefore(firstInstruction, InstructionHelper.CallMethodAndPassThisAndBoolean(field, this.managerInitializeMethodReference, requiresInterceptor));

            return field;
        }

        private static Instruction FindFirstInstruction(MethodDefinition constructor)
        {
            Instruction instruction = constructor.Body.Instructions.First(i => i.OpCode == OpCodes.Call).Next;
            while (instruction.OpCode == OpCodes.Nop)
            {
                instruction = instruction.Next;
            }

            return instruction;
        }

        private static FieldDefinition AddPrivateReadonlyField(TypeDefinition typeToProxy, TypeReference typeOfField)
        {
            var field = new FieldDefinition(
                typeOfField.Name,
                FieldAttributes.Private | FieldAttributes.InitOnly,
                typeOfField);
            typeToProxy.Fields.Add(field);
            return field;
        }

        private static ParameterDefinition AddParameter(MethodDefinition method, TypeReference typeOfParameter)
        {
            var parameter = new ParameterDefinition(
                typeOfParameter.Name,
                ParameterAttributes.None,
                typeOfParameter);
            method.Parameters.Add(parameter);
            return parameter;
        }
    }
}