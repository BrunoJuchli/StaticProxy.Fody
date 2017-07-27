namespace StaticProxy.Fody.Tests.ClassDecoration.MethodWeaving
{
    using System.Reflection;
    using FluentAssertions;
    using Moq;
    using Xunit;
    using System;

    public class When_proxying_method_returning_reference_type : ClassWithMethodsTestBase
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