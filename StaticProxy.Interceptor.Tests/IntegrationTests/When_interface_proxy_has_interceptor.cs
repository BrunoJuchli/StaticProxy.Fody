namespace StaticProxy.Interceptor.IntegrationTests
{
    using System;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class When_interface_proxy_has_interceptor
    {
        private readonly DemoInterceptor interceptor;
        private readonly object demoInterfaceProxy;

        private readonly DynamicInterceptorManager testee;

        public When_interface_proxy_has_interceptor()
        {
            this.interceptor = new DemoInterceptor();

            this.demoInterfaceProxy = new object();

            this.testee = new DynamicInterceptorManager(new DynamicInterceptorCollection { this.interceptor });
        }

        [Fact]
        public void Initialize_WhenTargetIsNull_MustThrow()
        {
            this.testee.Invoking(x => x.Initialize(null, true))
                .ShouldThrow<ArgumentNullException>()
                .Where(ex => ex.ParamName == "implementationMethodTarget");
        }

        [Fact]
        public void Initialize_WhenTargetIsNotNull_MustNotThrow()
        {
            this.testee.Invoking(x => x.Initialize(new object(), true))
                .ShouldNotThrow();
        }

        [Fact]
        public void Intercept_MethodReturningReferenceType_WhenInterceptorSetsReturnValueToNull_ShouldReturnNull()
        {
            this.interceptor.InterceptionLogic = invocation => invocation.ReturnValue = null;

            this.testee.Initialize(this.demoInterfaceProxy, true);

            var returnValue = this.testee.Intercept(
                MethodInfos.InterfaceProxy.DecoratedMethodReturningReferenceType,
                null,
                new Type[0],
                new object[] { Mock.Of<IDisposable>() });

            returnValue.Should().BeNull();
        }

        [Fact]
        public void Intercept_MethodReturningReferenceType_WhenInterceptorSetsReturnValue_ShouldReturnValue()
        {
            var expectedReturnValue = Mock.Of<IDisposable>();
            this.interceptor.InterceptionLogic = invocation => invocation.ReturnValue = expectedReturnValue;

            this.testee.Initialize(this.demoInterfaceProxy, true);

            var returnValue = this.testee.Intercept(
                MethodInfos.InterfaceProxy.DecoratedMethodReturningReferenceType,
                null,
                new Type[0],
                new object[] { null });

            returnValue.Should().Be(expectedReturnValue);
        }

        [Fact]
        public void Intercept_MethodReturningValueType_WhenInterceptorSetsReturnValueToNull_ShouldThrow()
        {
            this.interceptor.InterceptionLogic = invocation => invocation.ReturnValue = null;

            this.testee.Initialize(this.demoInterfaceProxy, true);

            var returnValue = this.testee.Invoking(x => x.Intercept(
                MethodInfos.InterfaceProxy.DecoratedMethodReturningValueType,
                null,
                new Type[0],
                new object[] { 123 }))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void Intercept_MethodReturningValueType_WhenInterceptorSetsReturnValue_ShouldReturnValue()
        {
            const int expectedReturnValue = 27389;
            this.interceptor.InterceptionLogic = invocation => invocation.ReturnValue = expectedReturnValue;

            this.testee.Initialize(this.demoInterfaceProxy, true);

            var returnValue = this.testee.Intercept(
                MethodInfos.InterfaceProxy.DecoratedMethodReturningValueType,
                null,
                new Type[0],
                new object[] { 332 });

            returnValue.Should().Be(expectedReturnValue);
        }

        [Fact]
        public void Intercept_WhenInterceptorManipulatesReturnValue_ShouldReturnManipulatedValue()
        {
            const int initialValue = 2222;
            const int valueSetByInterceptor = 48492;
            this.interceptor.InterceptionLogic = invocation =>
            {
                invocation.Proceed();
                invocation.ReturnValue = valueSetByInterceptor;
            };

            this.testee.Initialize(this.demoInterfaceProxy, true);

            var returnValue = this.testee.Intercept(
                MethodInfos.InterfaceProxy.DecoratedMethodReturningValueType,
                null,
                new Type[0],
                new object[] { initialValue });

            returnValue.Should().Be(valueSetByInterceptor); //original implementation multiplies by 2
        }

        [Fact]
        public void Intercept_WhenInterceptorDoesNotSetValueTypeReturnValue_ShouldThrow()
        {
            const int initialValue = 2222;
            this.interceptor.InterceptionLogic = invocation =>
            {
                invocation.Proceed();
            };

            this.testee.Initialize(this.demoInterfaceProxy, true);

            this.testee.Invoking(x => x.Intercept(
                MethodInfos.InterfaceProxy.DecoratedMethodReturningValueType,
                null,
                new Type[0],
                new object[] { initialValue }))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void Intercept_WhenNonGenericMethod_MustPassInvocationDataToInterceptor()
        {
            IInvocation actualInvocation = null;
            this.interceptor.InterceptionLogic = invocation =>
            {
                actualInvocation = invocation;
                invocation.Proceed();
            };
            const int initialValue = 333;

            this.testee.Initialize(this.demoInterfaceProxy, true);

            this.testee.Intercept(
                MethodInfos.InterfaceProxy.DecoratedMethodReturningReferenceType,
                null,
                new Type[0],
                new object[] { initialValue });

            actualInvocation.Should().NotBeNull();
            actualInvocation.Method.Should().BeSameAs(MethodInfos.InterfaceProxy.DecoratedMethodReturningReferenceType);
            actualInvocation.GenericArguments.Should().BeEmpty();
            actualInvocation.Arguments.Should()
                .HaveCount(1)
                .And.Contain(initialValue);
        }

        [Fact]
        public void Intercept_WhenGenericMethodReturnsValueTypeAndReturnValueIsNull_MustThrow()
        {
            this.interceptor.InterceptionLogic = invocation => invocation.ReturnValue = null;
            var genericArguments = new[] { typeof(float), typeof(int) };
            var arguments = new object[] { 23.4f, 333 };

            this.testee.Initialize(this.demoInterfaceProxy, true);

            this.testee.Invoking(x => x.Intercept(
                MethodInfos.InterfaceProxy.DecoratedGenericMethod,
                null,
                genericArguments,
                arguments))
            .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void Intercept_WhenGenericMethod_MustPassInvocationDataToInterceptor()
        {
            IInvocation actualInvocation = null;
            this.interceptor.InterceptionLogic = invocation =>
            {
                actualInvocation = invocation;
                invocation.ReturnValue = 232;
            };
            var genericArguments = new[] { typeof(float), typeof(int) };
            var arguments = new object[] { 23.4f, 333 };

            this.testee.Initialize(this.demoInterfaceProxy, true);

            this.testee.Intercept(
                MethodInfos.InterfaceProxy.DecoratedGenericMethod,
                null,
                genericArguments,
                arguments);

            actualInvocation.Should().NotBeNull();
            actualInvocation.Method.Should().BeSameAs(MethodInfos.InterfaceProxy.DecoratedGenericMethod.MakeGenericMethod(genericArguments));
            actualInvocation.GenericArguments.Should().BeSameAs(genericArguments);
            actualInvocation.Arguments.Should().BeSameAs(arguments);
        }

        [Fact]
        public void Intercept_WhenInterceptorProceeds_MustNotThrow()
        {
            this.interceptor.InterceptionLogic = invocation => invocation.Proceed();

            this.testee.Initialize(this.demoInterfaceProxy, true);

            this.testee.Invoking(x => x.Intercept(
                MethodInfos.InterfaceProxy.DecoratedMethodReturningReferenceType,
                null,
                new Type[0],
                new object[] { Mock.Of<IDisposable>() }))
                .ShouldNotThrow();
        }
    }
}