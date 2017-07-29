namespace IntegrationTests.ProxyWithoutTarget.Generic
{
    using Moq;
    using Ninject;
    using System;
    using Xunit;

    public class InterceptionCallTests : GenericProxyWithoutTargetIntegrationTestBase
    {
        private readonly Mock<IDynamicInterceptor> interceptor;

        private readonly IGenericProxy<IDisposable, string> testee;

        public InterceptionCallTests()
        {
            this.interceptor = new Mock<IDynamicInterceptor>();
            this.BindInterceptorCollection(this.interceptor.Object);

            this.testee = this.Kernel.Get<IGenericProxy<IDisposable,string>>();
        }

        [Fact]
        public void WhenInterceptingMethodWithGenericParameters_MustUseInterceptors()
        {
            const string ExpectedArgument = "123";

            this.testee.Foo(ExpectedArgument);

            this.interceptor.VerifyIntercepted(Reflector<IGenericProxy<IDisposable, string>>.GetMethod(x => x.Foo(null)), ExpectedArgument);
        }
    }
}