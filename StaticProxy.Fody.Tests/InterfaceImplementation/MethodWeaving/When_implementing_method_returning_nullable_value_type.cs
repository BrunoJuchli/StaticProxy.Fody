namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using System;
    using System.Reflection;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class When_implementing_method_returning_nullable_value_type : InterfaceWithMethodsTestBase
    {
        [Fact]
        public void WhenValueIsNotNull_MustReturnValueReturnedByDynamicInterceptorManager()
        {
            const float ExpectedResult = 38290.8f;
            this.InterceptorManager.Setup(
                x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<Type[]>(), It.IsAny<object[]>()))
                .Returns(ExpectedResult);

            var actualResult = this.Instance.ReturnsNullableFloat();

            ((float?)actualResult).Should().Be(ExpectedResult);
        }

        [Fact]
        public void WhenValueIsNull_MustReturnValueReturnedByDynamicInterceptorManager()
        {
            this.InterceptorManager.Setup(
                x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<Type[]>(), It.IsAny<object[]>()))
                .Returns(null);

            var actualResult = this.Instance.ReturnsNullableFloat();

            ((float?)actualResult).Should().Be(null);
        } 
    }
}