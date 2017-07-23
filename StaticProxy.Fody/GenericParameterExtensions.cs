namespace StaticProxy.Fody
{
    using Mono.Cecil;

    static class GenericParameterExtensions
    {
        public static GenericParameter CreateCopy(this GenericParameter original, IGenericParameterProvider owner)
        {
            var copy = new GenericParameter(original.Name, owner);
            copy.Attributes = original.Attributes;
            return copy;
        }
    }
}
