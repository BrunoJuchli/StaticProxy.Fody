namespace StaticProxy.Fody.Tests.ClassDecoration.MethodWeaving
{
    using Xunit;

    public class When_proxying_generic_method_taking_T : ClassWithGenericMethodsTestBase
    {
        [Fact]
        public void CallingMethod_MustUseInterceptorManager()
        {
            const string expectedValue = "Hello World Takes<T> Test";

            this.Instance.TakesT<string>(expectedValue);

            this.InterceptorManager.VerifyGenericDecoratedMethodCallIntercepted("TakesT", new[] { typeof(string) }, expectedValue);
        }
    }
}