namespace StaticProxy.Interceptor.IntegrationTests
{
    using System;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class When_class_proxy_has_interceptor
    {
        private readonly DemoInterceptor interceptor;
        private readonly DemoClassProxy demoClassProxy;

        private readonly DynamicInterceptorManager testee;

        public When_class_proxy_has_interceptor()
        {
            this.interceptor = new DemoInterceptor();

            this.demoClassProxy = new DemoClassProxy();

            this.testee = new DynamicInterceptorManager(new FakeDynamicInterceptorCollection { this.interceptor });
        }

        [Fact]
        public void Initialize_WhenTargetIsNull_MustThrow()
        {
            this.testee.Invoking(x => x.Initialize(null, false))
                .ShouldThrow<ArgumentNullException>()
                .Where(ex => ex.ParamName == "implementationMethodTarget");
        }

        [Fact]
        public void Initialize_WhenTargetIsNotNull_MustNotThrow()
        {
            this.testee.Invoking(x => x.Initialize(new object(), false))
                .ShouldNotThrow();
        }

        [Fact]
        public void Intercept_MethodReturningReferenceType_WhenInterceptorSetsReturnValueToNull_ShouldReturnNull()
        {
            this.interceptor.InterceptionLogic = invocation => invocation.ReturnValue = null;

            this.testee.Initialize(this.demoClassProxy, false);

            var returnValue = this.testee.Intercept(
                MethodInfos.ClassProxy.DecoratedMethodReturningReferenceType,
                MethodInfos.ClassProxy.ImplementedMethodReturningReferenceType,
                new Type[0],
                new object[] { Mock.Of<IDisposable>() });

            returnValue.Should().BeNull();
        }

        [Fact]
        public void Intercept_MethodReturningReferenceType_WhenInterceptorSetsReturnValue_ShouldReturnValue()
        {
            var expectedReturnValue = Mock.Of<IDisposable>();
            this.interceptor.InterceptionLogic = invocation => invocation.ReturnValue = expectedReturnValue;

            this.testee.Initialize(this.demoClassProxy, false);

            var returnValue = this.testee.Intercept(
                MethodInfos.ClassProxy.DecoratedMethodReturningReferenceType,
                MethodInfos.ClassProxy.ImplementedMethodReturningReferenceType,
                new Type[0],
                new object[] { null });

            returnValue.Should().Be(expectedReturnValue);
        }

        [Fact]
        public void Intercept_MethodReturningValueType_WhenInterceptorSetsReturnValueToNull_ShouldThrow()
        {
            this.interceptor.InterceptionLogic = invocation => invocation.ReturnValue = null;

            this.testee.Initialize(this.demoClassProxy, false);

            var returnValue = this.testee.Invoking(x => x.Intercept(
                MethodInfos.ClassProxy.DecoratedMethodReturningValueType,
                MethodInfos.ClassProxy.ImplementedMethodReturningValueType,
                new Type[0],
                new object[] { 123 }))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void Intercept_MethodReturningValueType_WhenInterceptorSetsReturnValue_ShouldReturnValue()
        {
            const int expectedReturnValue = 27389;
            this.interceptor.InterceptionLogic = invocation => invocation.ReturnValue = expectedReturnValue;

            this.testee.Initialize(this.demoClassProxy, false);

            var returnValue = this.testee.Intercept(
                MethodInfos.ClassProxy.DecoratedMethodReturningValueType,
                MethodInfos.ClassProxy.ImplementedMethodReturningValueType,
                new Type[0],
                new object[] { 332 });

            returnValue.Should().Be(expectedReturnValue);
        }

        [Fact]
        public void Intercept_WhenInterceptorManipulatesReturnValue_ShouldReturnManipulatedValue()
        {
            const int initialValue = 2222;
            this.interceptor.InterceptionLogic = invocation =>
            {
                invocation.Proceed();
                invocation.ReturnValue = (int)invocation.ReturnValue * 3;
            };

            this.testee.Initialize(this.demoClassProxy, false);

            var returnValue = this.testee.Intercept(
                MethodInfos.ClassProxy.DecoratedMethodReturningValueType,
                MethodInfos.ClassProxy.ImplementedMethodReturningValueType,
                new Type[0],
                new object[] { initialValue });

            returnValue.Should().Be(2 * 3 * initialValue); //original implementation multiplies by 2
        }

        [Fact]
        public void Intercept_WhenInterceptorDoesNotManipulateReturnValue_ShouldReturnOriginalImplementationValue()
        {
            const int initialValue = 2222;
            this.interceptor.InterceptionLogic = invocation =>
            {
                invocation.Proceed();
            };

            this.testee.Initialize(this.demoClassProxy, false);

            var returnValue = this.testee.Intercept(
                MethodInfos.ClassProxy.DecoratedMethodReturningValueType,
                MethodInfos.ClassProxy.ImplementedMethodReturningValueType,
                new Type[0],
                new object[] { initialValue });

            returnValue.Should().Be(2 * initialValue); //original implementation multiplies by 2
        }

        [Fact]
        public void Intercept_WhenInterceptorManipulatesArgument()
        {
            const int initialValue = 2222;
            const int manipulatedValue = 3123;
            this.interceptor.InterceptionLogic = invocation =>
            {
                invocation.Arguments[0] = manipulatedValue;
                invocation.Proceed();
            };

            this.testee.Initialize(this.demoClassProxy, false);

            var returnValue = this.testee.Intercept(
                MethodInfos.ClassProxy.DecoratedMethodReturningValueType,
                MethodInfos.ClassProxy.ImplementedMethodReturningValueType,
                new Type[0],
                new object[] { initialValue });

            returnValue.Should().Be(2 * manipulatedValue); //original implementation multiplies by 2
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

            this.testee.Initialize(this.demoClassProxy, false);

            this.testee.Intercept(
                MethodInfos.ClassProxy.DecoratedMethodReturningValueType,
                MethodInfos.ClassProxy.ImplementedMethodReturningValueType,
                new Type[0],
                new object[] { initialValue });

            actualInvocation.Should().NotBeNull();
            actualInvocation.Method.Should().BeSameAs(MethodInfos.ClassProxy.DecoratedMethodReturningValueType);
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

            this.testee.Initialize(this.demoClassProxy, false);

            this.testee.Invoking(x => x.Intercept(
                MethodInfos.ClassProxy.DecoratedGenericMethod,
                MethodInfos.ClassProxy.ImplementedGenericMethod,
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
                invocation.Proceed();
            };
            var genericArguments = new[] { typeof(float), typeof(int) };
            var arguments = new object[] { 23.4f, 333 };

            this.testee.Initialize(this.demoClassProxy, false);

            this.testee.Intercept(
                MethodInfos.ClassProxy.DecoratedGenericMethod,
                MethodInfos.ClassProxy.ImplementedGenericMethod,
                genericArguments,
                arguments);

            actualInvocation.Should().NotBeNull();
            actualInvocation.Method.Should().BeSameAs(MethodInfos.ClassProxy.DecoratedGenericMethod.MakeGenericMethod(genericArguments));
            actualInvocation.GenericArguments.Should().BeSameAs(genericArguments);
            actualInvocation.Arguments.Should().BeSameAs(arguments);
        }
    }
}