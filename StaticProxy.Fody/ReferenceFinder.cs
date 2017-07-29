using Mono.Cecil;
using System;
using System.Linq;

namespace StaticProxy.Fody
{
    public class ReferenceFinder
    {
        private readonly ModuleDefinition moduleDefinition;

        public ReferenceFinder(ModuleDefinition moduleDefinition)
        {
            this.moduleDefinition = moduleDefinition;
        }

        public MethodReference GetMethodReference(Type declaringType, Func<MethodDefinition, bool> predicate)
        {
            return GetMethodReference(GetTypeReference(declaringType), predicate);
        }

        public MethodReference GetMethodReference(TypeReference typeReference, Func<MethodDefinition, bool> predicate)
        {
            var typeDefinition = typeReference.Resolve();

            MethodDefinition methodDefinition;
            do
            {
                methodDefinition = typeDefinition.Methods.FirstOrDefault(predicate);
                typeDefinition = typeDefinition.BaseType?.Resolve();
            } while (methodDefinition == null && typeDefinition != null);

            return moduleDefinition.ImportReference(methodDefinition);
        }

        public TypeReference GetTypeReference(Type type)
        {
            return moduleDefinition.ImportReference(type);
        }
    }
}