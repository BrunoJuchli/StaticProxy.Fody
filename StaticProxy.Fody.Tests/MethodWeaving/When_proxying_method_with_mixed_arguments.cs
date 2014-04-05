namespace StaticProxy.Fody.Tests.MethodWeaving
{
    using Xunit;

    public class When_proxying_method_with_mixed_arguments : MethodsTestBase
    {
        [Fact]
        public void Calling_method_must_use_interceptor_manager()
        {
            const int Arg1 = 5;
            var arg2 = new object();
            const string Arg3 = "blub";

            var expectedArguments = new object[] { Arg1, arg2, Arg3 };

            this.Instance.MixedArguments(Arg1, arg2, Arg3);

            this.InterceptorManager.VerifyMethodCallIntercepted("MixedArguments", expectedArguments);
        }

        [Fact]
        public void CallingMethod_MustExecuteOriginalMethod()
        {
            this.Instance.MixedArguments(0, null, string.Empty);

            this.VerifyMethodCalled("MixedArguments");
        }
    }
}