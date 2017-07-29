namespace IntegrationTests.ProxyWithoutTarget
{
    using System;

    using FluentAssertions;

    using Moq;

    using Ninject;

    using Xunit;

    public class ReturnValueTests : ProxyWithoutTargetIntegrationTestBase
    {
        private readonly Mock<IDynamicInterceptor> interceptor;

        private readonly IProxy testee;

        public ReturnValueTests()
        {
            this.interceptor = new Mock<IDynamicInterceptor>();
            this.BindInterceptorCollection(this.interceptor.Object);

            this.testee = this.Kernel.Get<IProxy>();
        }

        [Fact]
        public void WhenMethodsReturnsReferenceType_MustReturnReturnValueOfInterceptor()
        {
            var expectedResult = new object();

            this.interceptor.SetupReturnValue(expectedResult);

            this.testee.ReturnsObject().Should().Be(expectedResult);
        }

        [Fact]
        public void WhenMethodReturnsValueType_InterceptorValueIsNotNull_MustReturnReturnValueOfInterceptor()
        {
            const int ExpectedResult = 34820;

            this.interceptor.SetupReturnValue(ExpectedResult);

            this.testee.ReturnsInteger().Should().Be(ExpectedResult);
        }

        [Fact]
        public void WhenMethodReturnValueType_InterceptorValueIsNull_MustThrow()
        {
            this.interceptor.SetupReturnValue(null);

            this.testee.Invoking(x => x.ReturnsInteger())
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void WhenMethodReturnsNullableValueType_InterceptorValueIsNotNull_MustReturnReturnValueOfInterceptor()
        {
            const float ExpectedResult = 15.6f;

            this.interceptor.SetupReturnValue(ExpectedResult);

            this.testee.ReturnsNullableFloat().Should().Be(ExpectedResult);
        }

        [Fact]
        public void WhenMethodReturnsNullableValueType_InterceptorValueIsNull_MustReturnReturnValueOfInterceptor()
        {
            this.interceptor.SetupReturnValue(null);

            this.testee.ReturnsNullableFloat().Should().NotHaveValue();
        }
    }
}