namespace SimpleTest.ClassDecoration
{
    using System.Globalization;

    [StaticProxy]
    public class ConstructorWithoutArguments
    {
        private readonly int foo;

        public ConstructorWithoutArguments()
        {
            this.foo = 5;
        }

        public override string ToString()
        {
            return this.foo.ToString(CultureInfo.InvariantCulture);
        }
    }
}