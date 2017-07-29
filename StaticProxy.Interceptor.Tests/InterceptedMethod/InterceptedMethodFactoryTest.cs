namespace StaticProxy.Interceptor.InterceptedMethod
{
    using Fasterflect;
    using FluentAssertions;
    using Moq;
    using StaticProxy.Interceptor.TargetInvocation;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using Xunit;

    public class InterceptedMethodFactoryTest
    {
        private static readonly MethodInfo GenericClassProxyDecoratedMethod = typeof(DemoGenericClassProxy).GetMethod("DecoratedMethod");
        private static readonly MethodInfo GenericClassProxyImplementationMethod = typeof(DemoGenericClassProxy).GetMethod("ImplementationMethod");
        private static readonly MethodInfo GenericInterfaceProxyDecoratedMethod = typeof(IDemoGenericProxyInterface).GetMethod("DecoratedMethod");

        private readonly Mock<ITargetInvocationFactory> targetInvocationFactory;

        private readonly InterceptedMethodFactory testee;

        public InterceptedMethodFactoryTest()
        {
            this.targetInvocationFactory = new Mock<ITargetInvocationFactory>();

            this.testee = new InterceptedMethodFactory(this.targetInvocationFactory.Object);
        }

        [Fact]
        public void Create_WhenNonGenericMethodOnClassProxy_MustCreateTargetInvocation()
        {
            var expectedTarget = new object();
            var decoratedMethod = new DynamicMethod("decorated", typeof(object), new Type[0]);
            var implementationMethod = new DynamicMethod("implementation", typeof(object), new Type[0]);

            this.testee.Create(expectedTarget, decoratedMethod, implementationMethod, new Type[0]);

            this.targetInvocationFactory.Verify(x => x.Create(expectedTarget, implementationMethod));
        }

        [Fact]
        public void Create_WhenGenericMethodOnClassProxy_MustCreateTargetInvocation()
        {
            var expectedTarget = new object();
            var genericArguments = new[] { typeof(string), typeof(IDisposable), typeof(IComparable) };
            var expectedImplementationMethod = GenericClassProxyImplementationMethod.MakeGenericMethod(genericArguments);

            this.testee.Create(expectedTarget, GenericClassProxyDecoratedMethod, GenericClassProxyImplementationMethod, genericArguments);

            this.targetInvocationFactory.Verify(x => x.Create(expectedTarget, expectedImplementationMethod));
        }

        [Fact]
        public void Create_WhenNonGenericMethodOnInterfaceProxy_MustCreateTargetInvocation()
        {
            var expectedTarget = new object();
            var decoratedMethod = new DynamicMethod("decorated", typeof(object), new Type[0]);

            this.testee.Create(expectedTarget, decoratedMethod, null, new Type[0]);

            this.targetInvocationFactory.Verify(x => x.Create(expectedTarget, null));
        }

        [Fact]
        public void Create_WhenGenericMethodOnInterfaceProxy_MustCreateTargetInvocation()
        {
            var expectedTarget = new object();
            var genericArguments = new[] { typeof(string), typeof(IDisposable), typeof(int) };

            this.testee.Create(expectedTarget, GenericInterfaceProxyDecoratedMethod, null, genericArguments);

            this.targetInvocationFactory.Verify(x => x.Create(expectedTarget, null));
        }

        [Fact]
        public void Create_WhenHasGenericArguments_MustReturnGenericArguments()
        {
            var genericArguments = new[] { typeof(string), typeof(IDisposable), typeof(int) };

            var interceptedMethod = this.testee.Create(null, GenericInterfaceProxyDecoratedMethod, null, genericArguments);

            interceptedMethod.GenericArguments.Should().BeEquivalentTo(genericArguments);
        }

        [Fact]
        public void Create_WhenNotHasGenericArguments_MustReturnEmptyGenericArguments()
        {
            var genericArguments = new Type[0];

            var interceptedMethod = this.testee.Create(null, null, null, genericArguments);

            interceptedMethod.GenericArguments.Should().BeEmpty();
        }

        [Fact]
        public void Create_WhenNonGenericMethodOnClassProxy_MustReturnInterceptedMethod()
        {
            var expectedTarget = new object();
            var decoratedMethod = new DynamicMethod("decorated", typeof(object), new Type[0]);
            var implementationMethod = new DynamicMethod("implementation", typeof(object), new Type[0]);
            var genericArguments = new Type[0];
            var expectedTargetInvocation = this.SetupTargetInvocationFactory();

            var result = this.testee.Create(expectedTarget, decoratedMethod, implementationMethod, genericArguments);

            result.DecoratedMethod.Should().BeSameAs(decoratedMethod);
            result.GenericArguments.Should().BeSameAs(genericArguments);
            result.TargetInvocation.Should().Be(expectedTargetInvocation);
        }

        [Fact]
        public void Create_WhenGenericMethodOnClassProxy_MustReturnInterceptedMethod()
        {
            var expectedTarget = new object();
            var genericArguments = new[] { typeof(string), typeof(IDisposable), typeof(IComparable) };
            var expectedDecoratedMethod = GenericClassProxyDecoratedMethod.MakeGenericMethod(genericArguments);
            var expectedTargetInvocation = this.SetupTargetInvocationFactory();

            var result = this.testee.Create(expectedTarget, GenericClassProxyDecoratedMethod, GenericClassProxyImplementationMethod, genericArguments);

            result.DecoratedMethod.Should().BeSameAs(expectedDecoratedMethod);
            result.GenericArguments.Should().BeSameAs(genericArguments);
            result.TargetInvocation.Should().Be(expectedTargetInvocation);
        }

        [Fact]
        public void Create_WhenNonGenericMethodOnInterfaceProxy_MustReturnInterceptedMethod()
        {
            var expectedTarget = new object();
            var decoratedMethod = new DynamicMethod("decorated", typeof(object), new Type[0]);
            var genericArguments = new Type[0];
            var expectedTargetInvocation = this.SetupTargetInvocationFactory();

            var result = this.testee.Create(expectedTarget, decoratedMethod, null, genericArguments);

            result.DecoratedMethod.Should().BeSameAs(decoratedMethod);
            result.GenericArguments.Should().BeSameAs(genericArguments);
            result.TargetInvocation.Should().Be(expectedTargetInvocation);
        }

        [Fact]
        public void Create_WhenGenericMethodOnInterfaceProxy_MustReturnInterceptedMethod()
        {
            var expectedTarget = new object();
            var genericArguments = new[] { typeof(IComparable), typeof(IDisposable), typeof(float) };
            var expectedDecoratedMethod = GenericInterfaceProxyDecoratedMethod.MakeGenericMethod(genericArguments);
            var expectedTargetInvocation = this.SetupTargetInvocationFactory();

            var result = this.testee.Create(expectedTarget, GenericInterfaceProxyDecoratedMethod, null, genericArguments);

            result.DecoratedMethod.Should().BeSameAs(expectedDecoratedMethod);
            result.GenericArguments.Should().BeSameAs(genericArguments);
            result.TargetInvocation.Should().Be(expectedTargetInvocation);
        }

        private ITargetInvocation SetupTargetInvocationFactory()
        {
            var result = Mock.Of<ITargetInvocation>();
            this.targetInvocationFactory
                .Setup(x => x.Create(It.IsAny<object>(), It.IsAny<MethodBase>()))
                .Returns(result);
            return result;
        }

        private class DemoGenericClassProxy
        {
            public void DecoratedMethod<X, Y, Z>() { }

            public void ImplementationMethod<X, Y, Z>() { }
        }

        private interface IDemoGenericProxyInterface
        {
            void DecoratedMethod<X, Y, Z>();
        }
    }
}
