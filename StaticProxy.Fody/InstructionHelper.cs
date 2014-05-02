namespace StaticProxy.Fody
{
    using System.Collections.Generic;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public static class InstructionHelper
    {
        public static IEnumerable<Instruction> CallMethodAndPassParameter(MethodReference method, ParameterDefinition parameter)
        {
            return new[]
                       {
                           Instruction.Create(OpCodes.Ldarg_0),
                           Instruction.Create(OpCodes.Ldarg_S, parameter), 
                           Instruction.Create(OpCodes.Call, method),
                           Instruction.Create(OpCodes.Nop),
                       };
        }

        public static IEnumerable<Instruction> SetInstanceFieldToMethodParameter(FieldDefinition field, ParameterDefinition parameter)
        {
            return new[]
                        {
                            Instruction.Create(OpCodes.Ldarg_0),
                            Instruction.Create(OpCodes.Ldarg_S, parameter), 
                            Instruction.Create(OpCodes.Stfld, field)
                        };
        }

        public static IEnumerable<Instruction> CallMethodAndPassThisAndBoolean(FieldDefinition instanceOfMethod, MethodReference method, bool boolean)
        {
            OpCode loadBooleanValue = boolean ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0;

            return new[]
                       {
                           Instruction.Create(OpCodes.Ldfld, instanceOfMethod), 
                           Instruction.Create(OpCodes.Ldarg_0),
                           Instruction.Create(loadBooleanValue),
                           Instruction.Create(OpCodes.Callvirt, method),
                           Instruction.Create(OpCodes.Nop),
                       };
        }
    }
}