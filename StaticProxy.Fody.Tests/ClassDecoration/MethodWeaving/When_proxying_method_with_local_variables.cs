namespace StaticProxy.Fody.Tests.ClassDecoration.MethodWeaving
{
    using FluentAssertions;

    using Xunit;

    public class When_proxying_method_with_local_variables : ClassWithMethodsTestBase
    {
        [Fact]
        public void Calling_method_must_not_throw()
        {
            this.Invoking(x => this.Instance.WithLocalVariables())
                .ShouldNotThrow();
        }

        [Fact]
        public void Calling_method_must_call_original_method()
        {
            this.Instance.WithLocalVariables();

            this.VerifyMethodCalled("WithLocalVariables");
        }
    }
}