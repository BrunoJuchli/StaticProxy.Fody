namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using System;
    using System.Reflection;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class When_implementing_generic_method_returning_T : InterfaceWithGenericMethodsTestBase
    {
        [Fact]
        public void CallingMethod_MustUseInterceptorManager()
        {
            var expectedResult = new object();
            this.InterceptorManager.Setup(
                    x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<Type[]>(), It.IsAny<object[]>()))
                    .Returns(expectedResult);

            var actualResult = this.Instance.ReturnsT<object>();

            ((object)actualResult).Should().Be(expectedResult);
        }
    }
}