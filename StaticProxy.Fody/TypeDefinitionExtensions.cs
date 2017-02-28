using Mono.Cecil;
using Mono.Cecil.Rocks;
using System.Collections.Generic;
using System.Linq;

namespace StaticProxy.Fody
{
    static class TypeDefinitionExtensions
    {
        public static IEnumerable<MethodDefinition> GetNonStaticConstructors(this TypeDefinition self)
        {
            return self
                .GetConstructors()
                .Where(x => x.IsStatic != true);
        }
    }
}
