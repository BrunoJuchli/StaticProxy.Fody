namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using System;
    using System.Reflection;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class When_implementing_method_returning_value_type : InterfaceWithMethodsTestBase
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