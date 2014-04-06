namespace StaticProxy.Fody
{
    using System.Globalization;

    using Mono.Cecil;

    public static class ModuleDefinitionExtensions
    {
        public static TypeReference GetTypeReference(this ModuleDefinition module, string fullName)
        {
            TypeReference result;
            if (module.TryGetTypeReference(fullName, out result))
            {
                return result;
            }

            throw BuildNotSIngleTypeMatchingFullNameException(fullName, 0);
        }

        public static TypeDefinition GetTypeDefinition(this ModuleDefinition module, string fullName)
        {
            TypeDefinition type = module.GetType(fullName);
            if (type == null)
            {
                throw BuildNotSIngleTypeMatchingFullNameException(fullName, 0);
            }

            return type;
        }

        private static WeavingException BuildNotSIngleTypeMatchingFullNameException(string fullName, int matchingCount)
        {
            return new WeavingException(
                string.Format(CultureInfo.InvariantCulture, ExceptionMessages.NotSingleTypeMatchingFullName, fullName, 0));
        }
    }
}