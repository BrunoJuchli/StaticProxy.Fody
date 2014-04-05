namespace SimpleTest.Integration
{
    using FluentAssertions;

    [StaticProxy]
    public class IntegrationWithoutConstructorArgument
    {
        private object[] lastCallArguments;

        public void DoSomething(object arg1, object arg2)
        {
            this.lastCallArguments = new[] { arg1, arg2 };
        }

        public void AssertDoSomethingArgumentsWere(object arg1, object arg2)
        {
            this.lastCallArguments[0].Should().Be(arg1);
            this.lastCallArguments[1].Should().Be(arg2);
        }
    }
}