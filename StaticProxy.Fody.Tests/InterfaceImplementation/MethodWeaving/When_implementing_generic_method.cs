namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using FluentAssertions;
    using Moq;
    using System.Reflection;
    using Xunit;

    public class When_implementing_generic_method : InterfaceWithGenericMethodsTestBase
    {
        [Fact]
        public void CallingMethod_MustPassDecoratedMethodWithCorrectGenericArguments()
        {
            MethodBase decoratedMethod = null;

            var expectedResult = new object();
            this.InterceptorManager.Setup(
                    x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<object[]>()))
                    .Callback<MethodBase, MethodBase, object[]>((dM, iM, p) => decoratedMethod = dM);

            this.Instance.GenericMethod<object,string,int>();

            decoratedMethod.Should().NotBeNull();
            decoratedMethod.GetGenericArguments().Should().HaveCount(3)
                .And.ContainInOrder(typeof(object), typeof(string), typeof(int));
            decoratedMethod.IsGenericMethodDefinition.Should().BeFalse();
        }
    }
}