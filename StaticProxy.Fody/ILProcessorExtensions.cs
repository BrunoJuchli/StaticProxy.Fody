namespace StaticProxy.Fody
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;

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

        public static void SaveGenericParametersToNewTypeArray(this ILProcessor processor, Collection<GenericParameter> genericParameters)
        {
            processor.SaveCollectionToNewArray(
                WeavingInformation.TypeTypeReference,
                genericParameters,
                (p, genericParameter) =>
                {
                    p.Emit(OpCodes.Ldtoken, genericParameter);
                    p.Emit(OpCodes.Call, WeavingInformation.GetTypeFromHandleMethodReference);
                });
        }

        public static void SaveParametersToNewObjectArray(this ILProcessor processor, IList<ParameterDefinition> parameters)
        {
            processor.SaveCollectionToNewArray(
                WeavingInformation.ObjectTypeReference,
                parameters,
                (p, parameter) =>
                {
                    p.Emit(OpCodes.Ldarg_S, parameter); // push the parameter to the stack

                    if (parameter.ParameterType.IsValueType || parameter.ParameterType.IsGenericParameter)
                    {
                        // box value types
                        p.Emit(OpCodes.Box, parameter.ParameterType);
                    }
                });
        }

        public static void SaveCollectionToNewArray<T>(
            this ILProcessor processor,
            TypeReference arrayPayloadType,
            IList<T> collection,
            Action<ILProcessor, T> loadElementToStack)
        {
            const byte MaxArraySize = sbyte.MaxValue - 1;
            if (collection.Count > MaxArraySize)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "There are {0} items in {1}, but the supported maximum is {2}",
                        collection.Count,
                        collection,
                        MaxArraySize));
            }

            processor.CreateArray(arrayPayloadType, (sbyte)collection.Count);

            for (sbyte index = 0; index < collection.Count; index++)
            {
                processor.Emit(OpCodes.Dup); // duplicate the array address onto the stack so we have it for the next call (and it's still on the stack when we return)
                T item = collection[index];
                processor.SaveValueToArray(index, p => loadElementToStack(p, item));
            }
        }

        public static void SaveCurrentStackValueToVariable(this ILProcessor processor, VariableDefinition variable)
        {
            processor.Emit(OpCodes.Stloc_S, variable);
        }

        public static void CreateArray(this ILProcessor processor, TypeReference elementType, sbyte arraySize)
        {
            processor.Emit(OpCodes.Ldc_I4_S, arraySize);
            processor.Emit(OpCodes.Newarr, elementType);
        }

        public static void SaveGenericArgumentToTypeArray(this ILProcessor processor, Collection<GenericParameter> genericParameters, sbyte index)
        {
            processor.SaveValueToArray(index, p =>
            {
                p.Emit(OpCodes.Ldtoken, genericParameters[index]);
                p.Emit(OpCodes.Call, WeavingInformation.GetTypeFromHandleMethodReference);
            });
        }

        public static void SaveValueToArray(this ILProcessor processor, sbyte index, Action<ILProcessor> loadElementToStack)
        {
            processor.Emit(OpCodes.Ldc_I4_S, index); // push the index to the stack
            loadElementToStack(processor);
            processor.Emit(OpCodes.Stelem_Ref); // pop the parameter from the stack and store it in the array
        }
    }
}