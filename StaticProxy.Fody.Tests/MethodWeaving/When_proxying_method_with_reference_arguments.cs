namespace StaticProxy.Fody.Tests.MethodWeaving
{
    using Xunit;

    public class When_proxying_method_with_reference_arguments : MethodsTestBase
    {
        [Fact]
        public void Calling_method_must_use_interceptor_manager()
        {
            var arg1 = new object();
            var arg2 = new object();
            var expectedArguments = new[] { arg1, arg2 };

            this.Instance.ReferenceArguments(arg1, arg2);

            this.InterceptorManager.VerifyMethodCallIntercepted("ReferenceArguments", expectedArguments);
        }

        [Fact]
        public void CallingMethod_MustExecuteOriginalMethod()
        {
            this.Instance.ReferenceArguments(null, null);

            this.VerifyMethodCalled("ReferenceArguments");
        }
    }
}