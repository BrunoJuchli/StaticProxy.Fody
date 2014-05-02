namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using Xunit;

    public class When_implementing_method_with_value_arguments : InterfaceWithMethodsTestBase
    {
        [Fact]
        public void Calling_method_must_use_interceptor_manager()
        {
            const int Arg1 = 48;
            const float Arg2 = (float)3.4;

            this.Instance.ValueArguments(Arg1, Arg2);

            this.InterceptorManager.VerifyImplementedMethodCallIntercepted("ValueArguments", Arg1, Arg2);
        }
    }
}