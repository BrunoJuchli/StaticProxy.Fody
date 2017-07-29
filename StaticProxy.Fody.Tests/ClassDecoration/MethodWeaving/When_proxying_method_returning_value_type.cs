namespace StaticProxy.Fody.Tests.ClassDecoration.MethodWeaving
{
    using System.Reflection;
    using FluentAssertions;
    using Moq;
    using Xunit;
    using System;

    public class When_proxying_method_returning_value_type : ClassWithMethodsTestBase
    {
        [Fact]
        public void MustReturnValueReturnedByDynamicInterceptorManager()
        {
            const int ExpectedResult = 34820;
            this.InterceptorManager.Setup(
                x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<Type[]>(), It.IsAny<object[]>()))
                .Returns(ExpectedResult);

            var actualResult = this.Instance.ReturnsInteger();

            ((int)actualResult).Should().Be(ExpectedResult);
        }
    }
}