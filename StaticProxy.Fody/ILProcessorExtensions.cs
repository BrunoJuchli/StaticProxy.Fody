namespace StaticProxy.Fody
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Mono.Cecil;
    using Mono.Cecil.Cil;
    
    // ReSharper disable once InconsistentNaming
    public static class ILProcessorExtensions
    {
        public static void InsertBefore(this ILProcessor processor, Instruction target, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                processor.InsertBefore(target, instruction);
            }
        }

        public static void InsertAfter(this ILProcessor processor, Instruction target, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                processor.InsertAfter(target, instruction);
                target = instruction;
            }
        }

        public static void SaveGenericArgumentsToNewObjectArray(this ILProcessor processor, VariableDefinition array, MethodDefinition method)
        {
            const byte MaxArraySize = sbyte.MaxValue - 1;
            if (method.GenericParameters.Count > MaxArraySize)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "There are {0} generic parameters, but the supported maximum is {1}",
                        method.GenericParameters.Count,
                        MaxArraySize));
            }

            processor.CreateArrayAndStoreToVariable(array, WeavingInformation.TypeTypeReference, (sbyte)method.GenericParameters.Count);
            for (sbyte i = 0; i < method.GenericParameters.Count; i++)
            {
                processor.SaveGenericArgumentToTypeArray(array, method, i);
            }
        }

        public static void SaveParametersToNewObjectArray(this ILProcessor processor, VariableDefinition array, IList<ParameterDefinition> parameters)
        {
            const byte MaxArraySize = sbyte.MaxValue - 1;
            if (parameters.Count > MaxArraySize)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "There are {0} method parameters, but the supported maximum is {1}",
                        parameters.Count,
                        MaxArraySize));
            }

            processor.CreateArrayAndStoreToVariable(array, WeavingInformation.ObjectTypeReference, (sbyte)parameters.Count);
            for (sbyte i = 0; i < parameters.Count; i++)
            {
                processor.SaveParameterValueToObjectArray(array, parameters[i], i);
            }
        }

        public static void CreateArrayAndStoreToVariable(this ILProcessor processor, VariableDefinition variable, TypeReference elementType, sbyte arraySize)
        {
            processor.Emit(OpCodes.Ldc_I4_S, arraySize);
            processor.Emit(OpCodes.Newarr, elementType);
            processor.Emit(OpCodes.Stloc_S, variable);
        }

        public static void SaveParameterValueToObjectArray(this ILProcessor processor, VariableDefinition array, ParameterDefinition parameter, sbyte index)
        {
            processor.Emit(OpCodes.Ldloc_S, array); // load the array variable
            processor.Emit(OpCodes.Ldc_I4_S, index); // push the index to the stack
            processor.Emit(OpCodes.Ldarg_S, parameter); // push the parameter to the stack
            
            if (parameter.ParameterType.IsValueType || parameter.ParameterType.IsGenericParameter)
            {
                // box value types
                processor.Emit(OpCodes.Box, parameter.ParameterType);
            }

            processor.Emit(OpCodes.Stelem_Ref); // pop the parameter from the stack and store it in the array
        }

        public static void SaveGenericArgumentToTypeArray(this ILProcessor processor, VariableDefinition array, MethodDefinition method, sbyte index)
        {
            processor.Emit(OpCodes.Ldloc_S, array); // load the array variable
            processor.Emit(OpCodes.Ldc_I4_S, index); // push the index to the stack
            processor.Emit(OpCodes.Ldtoken, method.GenericParameters[index]);
            processor.Emit(OpCodes.Call, WeavingInformation.GetTypeFromHandleMethodReference);
            processor.Emit(OpCodes.Stelem_Ref); // pop the parameter from the stack and store it in the array
        }
    }
}