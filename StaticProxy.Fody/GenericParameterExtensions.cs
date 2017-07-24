namespace StaticProxy.Fody
{
    using Mono.Cecil;

    static class GenericParameterExtensions
    {
        public static GenericParameter CreateCopy(this GenericParameter original, IGenericParameterProvider owner)
        {
            var copy = new GenericParameter(original.Name, owner);

            foreach(var constraint in original.Constraints) {
                copy.Constraints.Add(constraint);
            }
            
            copy.Attributes = original.Attributes;
            return copy;
        }
    }
}
