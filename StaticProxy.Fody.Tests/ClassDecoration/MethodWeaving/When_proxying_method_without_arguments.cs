namespace StaticProxy.Fody.Tests.ClassDecoration.MethodWeaving
{
    using Xunit;

    public class When_proxying_method_without_arguments : MethodsTestBase
    {
        [Fact]
        public void CallingMethod_MustUseInterceptorManager()
        {
            this.Instance.NoArguments();

            this.InterceptorManager.VerifyMethodCallIntercepted("NoArguments", new object[0]);
        }

        [Fact]
        public void CallingMethod_MustExecuteOriginalMethod()
        {
            this.Instance.NoArguments();

            this.VerifyMethodCalled("NoArguments");
        }
    }
}