namespace StaticProxy.Fody.Tests.MethodWeaving
{
    using Xunit;

    public class When_proxying_method_with_value_arguments : MethodsTestBase
    {
        [Fact]
        public void Calling_method_must_use_interceptor_manager()
        {
            const int Arg1 = 48;
            const float Arg2 = (float)3.4;
            var expectedArguments = new object[] { Arg1, Arg2 };

            this.Instance.ValueArguments(Arg1, Arg2);

            this.InterceptorManager.VerifyMethodCallIntercepted("ValueArguments", expectedArguments);
        }

        [Fact]
        public void CallingMethod_MustExecuteOriginalMethod()
        {
            this.Instance.ValueArguments(0, 9.8f);

            this.VerifyMethodCalled("ValueArguments");
        }
    }
}