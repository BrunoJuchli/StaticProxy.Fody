namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using Xunit;

    public class When_implementing_method_without_arguments : InterfaceWithMethodsTestBase
    {
        [Fact]
        public void CallingMethod_MustUseInterceptorManager()
        {
            this.Instance.NoArguments();

            this.InterceptorManager.VerifyImplementedMethodCallIntercepted("NoArguments");
        }
    }
}