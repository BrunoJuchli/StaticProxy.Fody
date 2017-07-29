namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Reflection;
    using Xunit;

    public class When_implementing_generic_method : InterfaceWithGenericMethodsTestBase
    {
        [Fact]
        public void CallingMethod_MustPassDecoratedMethodWithCorrectGenericArguments()
        {
            MethodBase decoratedMethod = null;
            Type[] genericArguments = null;

            var expectedResult = new object();
            this.InterceptorManager.Setup(
                    x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<Type[]>(), It.IsAny<object[]>()))
                    .Callback<MethodBase, MethodBase, Type[], object[]>((dM, iM, gA, p) =>
                    {
                        decoratedMethod = dM;
                        genericArguments = gA;
                    });

            this.Instance.GenericMethod<object,string,int>();

            decoratedMethod.Should().NotBeNull();
            decoratedMethod.GetGenericArguments().Should()
                .HaveCount(3)
                .And.OnlyContain(x => x.IsGenericParameter == true);
            decoratedMethod.IsGenericMethod.Should().BeTrue();
            genericArguments.Should()
                .HaveCount(3)
                .And.ContainInOrder(typeof(object), typeof(string), typeof(int));
        }
    }
}