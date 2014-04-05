namespace StaticProxy.Fody
{
    public static class ExceptionMessages
    {
        public const string MultipleConstructors = "Static Proxy issue with type '{0}'. Static Proxy does not support proxying classes with multiple constructors.";

        public const string NotSingleTypeMatchingFullName = "There needs to exactly one type with FullName '{0}', but {1} were found.";
    }
}