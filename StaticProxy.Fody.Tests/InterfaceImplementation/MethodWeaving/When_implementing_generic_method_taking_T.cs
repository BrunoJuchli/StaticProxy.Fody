namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using Xunit;

    public class When_implementing_generic_method_taking_T : InterfaceWithGenericMethodsTestBase
    {
        [Fact]
        public void CallingMethod_MustUseInterceptorManager()
        {
            const string expectedValue = "Hello World Takes<T> Test";

            this.Instance.TakesT<string>(expectedValue);

            this.InterceptorManager.VerifyImplementedGenericMethodCallIntercepted("TakesT", new[] { typeof(string) }, expectedValue);
        }
    }
}