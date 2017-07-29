namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using System.Reflection;

    using FluentAssertions;

    using Moq;

    using Xunit;
    using System;

    public class When_implementing_method_returning_reference_type : InterfaceWithMethodsTestBase
    {
        [Fact]
        public void MustReturnValueReturnedByDynamicInterceptorManager()
        {
            var expectedResult = new object();
            this.InterceptorManager.Setup(
                x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<Type[]>(), It.IsAny<object[]>()))
                .Returns(expectedResult);

            var actualResult = this.Instance.ReturnsObject();

            ((object)actualResult).Should().Be(expectedResult);
        } 
    }
}