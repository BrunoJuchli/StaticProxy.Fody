namespace IntegrationTests.ProxyWithoutTarget.Generic
{
    using System;
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Xunit;

    public class ReturnValueTests : GenericProxyWithoutTargetIntegrationTestBase
    {
        private readonly Mock<IDynamicInterceptor> interceptor;

        private readonly IGenericProxy<IDisposable, string> testee;

        public ReturnValueTests()
        {
            this.interceptor = new Mock<IDynamicInterceptor>();
            this.BindInterceptorCollection(this.interceptor.Object);

            this.testee = this.Kernel.Get<IGenericProxy<IDisposable, string>>();
        }

        [Fact]
        public void WhenMethodsReturnsReferenceType_MustReturnReturnValueOfInterceptor()
        {
            var expectedResult = Mock.Of<IDisposable>();

            this.interceptor.SetupReturnValue(expectedResult);

            this.testee.Foo("foobar").Should().Be(expectedResult);
        }

    }
}