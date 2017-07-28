namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using FluentAssertions;
    using Moq;
    using StaticProxy.Fody.InterfaceImplementation;
    using System;
    using System.Reflection;
    using Xunit;

    public class When_implementing_method_of_generic_interface : SimpleTestBase
    {
        private static readonly Type[] TypeGenericArguments = new[] { typeof(object), typeof(string), typeof(Uri) };

        protected readonly Type Clazz;
        protected readonly Mock<IDynamicInterceptorManager> InterceptorManager;

        protected readonly dynamic Instance;

        public When_implementing_method_of_generic_interface()
        {
            this.Clazz = this.WovenSimpleTestAssembly.GetType("SimpleTest.InterfaceImplementation.IGenericProxy" + InterfaceImplementationWeaver.ClassNameSuffix + "`3")
                            .MakeGenericType(TypeGenericArguments);

            this.InterceptorManager = new Mock<IDynamicInterceptorManager>();

            this.Instance = Activator.CreateInstance(this.Clazz, this.InterceptorManager.Object);
        }

        [Fact]
        public void CallingMethod_MustPassDecoratedMethod()
        {
            MethodBase decoratedMethod = null;

            this.InterceptorManager.Setup(
                    x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<Type[]>(), It.IsAny<object[]>()))
                    .Callback<MethodBase, MethodBase, Type[], object[]>((dM, iM, gA, a) =>
                    {
                        decoratedMethod = dM;
                    });

            this.Instance.DoSomething(new object());

            decoratedMethod.Should().NotBeNull();
            decoratedMethod.GetGenericArguments().Should().BeEmpty();
            decoratedMethod.Name.Should().Be("DoSomething");
            decoratedMethod.DeclaringType.GetGenericArguments().Should()
                .ContainInOrder(TypeGenericArguments)
                .And.HaveSameCount(TypeGenericArguments);
        }

        [Fact]

        public void CallingMethod_MustPassEmptyGenericMethodArguments()
        {
            Type[] genericArguments = null;

            this.InterceptorManager.Setup(
                    x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<Type[]>(), It.IsAny<object[]>()))
                    .Callback<MethodBase, MethodBase, Type[], object[]>((dM, iM, gA, a) =>
                    {
                        genericArguments = gA;
                    });

            this.Instance.DoSomething(new object());

            genericArguments.Should().BeEmpty();
        }

        [Fact]

        public void CallingMethod_MustNotPassImplementedMethod()
        {
            MethodBase implementedMethod = null;

            this.InterceptorManager.Setup(
                    x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<Type[]>(), It.IsAny<object[]>()))
                    .Callback<MethodBase, MethodBase, Type[], object[]>((dM, iM, gA, a) =>
                    {
                        implementedMethod = iM;
                    });

            this.Instance.DoSomething(new object());

            implementedMethod.Should().BeNull();
        }

        [Fact]

        public void CallingMethod_MustPassArguments()
        {
            object expectedArgument = new object();
            object[] actualArguments = null;

            this.InterceptorManager.Setup(
                    x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<Type[]>(), It.IsAny<object[]>()))
                    .Callback<MethodBase, MethodBase, Type[], object[]>((dM, iM, gA, a) =>
                    {
                        actualArguments = a;
                    });

            this.Instance.DoSomething(expectedArgument);

            actualArguments.Should()
                .HaveCount(1)
                .And.Contain(expectedArgument);
        }
    }
}