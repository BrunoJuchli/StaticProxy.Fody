namespace SimpleTest
{
    [StaticProxy]
    public class ConstructorWithArguments
    {
        private readonly int foo;
        private readonly object bar;

        public ConstructorWithArguments(int foo, object bar)
        {
            this.foo = foo;
            this.bar = bar;
        }
    }
}