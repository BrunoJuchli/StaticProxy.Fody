namespace StaticProxy.Fody.Tests.ClassDecoration.MethodWeaving
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Reflection;
    using Xunit;

    public class When_proxying_generic_method : ClassWithGenericMethodsTestBase
    {
        [Fact]
        public void CallingMethod_MustPassDecoratedAndImplementedMethodWithCorrectGenericArguments()
        {
            MethodBase decoratedMethod = null;
            MethodBase implementationMethod = null;
            Type[] genericArguments = null;

            var expectedResult = new object();
            this.InterceptorManager.Setup(
                    x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<Type[]>(), It.IsAny<object[]>()))
                    .Callback<MethodBase, MethodBase, Type[], object[]>((dM, iM, gA, p) =>
                    {
                        decoratedMethod = dM;
                        implementationMethod = iM;
                        genericArguments = gA;
                    });

            this.Instance.GenericMethod<object,string,int>();

            decoratedMethod.Should().NotBeNull();
            decoratedMethod.GetGenericArguments().Should()
                .HaveCount(3)
                .And.OnlyContain(x => x.IsGenericParameter == true);
            decoratedMethod.IsGenericMethod.Should().BeTrue();

            implementationMethod.Should().NotBeNull();
            implementationMethod.GetGenericArguments().Should()
                .HaveCount(3)
                .And.OnlyContain(x => x.IsGenericParameter == true);
            implementationMethod.IsGenericMethod.Should().BeTrue();

            genericArguments.Should()
                .HaveCount(3)
                .And.ContainInOrder(typeof(object), typeof(string), typeof(int));
        }
    }
}