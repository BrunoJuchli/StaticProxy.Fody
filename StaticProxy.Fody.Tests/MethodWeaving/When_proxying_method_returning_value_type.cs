namespace StaticProxy.Fody.Tests.MethodWeaving
{
    using System.Reflection;

    using FluentAssertions;

    using Moq;

    using Xunit;

    public class When_proxying_method_returning_value_type : MethodsTestBase
    {
        [Fact]
        public void MustReturnValueReturnedByDynamicInterceptorManager()
        {
            const int ExpectedResult = 34820;
            this.InterceptorManager.Setup(
                x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<object[]>()))
                .Returns(ExpectedResult);

            var actualResult = this.Instance.ReturnsInteger();

            ((int)actualResult).Should().Be(ExpectedResult);
        }
    }
}