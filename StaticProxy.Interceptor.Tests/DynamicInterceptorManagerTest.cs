namespace StaticProxy.Interceptor.Tests
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    using FluentAssertions;

    using Moq;

    using StaticProxy.Interceptor;
    using StaticProxy.Interceptor.Reflection;
    using StaticProxy.Interceptor.TargetInvocation;

    using Xunit;

    public class DynamicInterceptorManagerTest
    {
        private readonly Mock<IDynamicInterceptor> interceptor1;
        private readonly Mock<IDynamicInterceptor> interceptor2;

        private readonly Mock<ITargetInvocationFactory> targetInvocationFactory;
        private readonly Mock<IInvocationFactory> invocationFactory;
        private readonly Mock<ITypeInformation> typeInformation;

        private readonly DynamicInterceptorManager testee;

        public DynamicInterceptorManagerTest()
        {
            this.interceptor1 = new Mock<IDynamicInterceptor>();
            this.interceptor2 = new Mock<IDynamicInterceptor>();

            this.targetInvocationFactory = new Mock<ITargetInvocationFactory> { DefaultValue = DefaultValue.Mock };
            this.invocationFactory = new Mock<IInvocationFactory> { DefaultValue = DefaultValue.Mock };
            this.typeInformation = new Mock<ITypeInformation>();

            this.testee = new DynamicInterceptorManager(
                new FakeDynamicInterceptorCollection { this.interceptor1.Object, this.interceptor2.Object },
                this.targetInvocationFactory.Object, 
                this.invocationFactory.Object,
                this.typeInformation.Object);
        }

        [Fact]
        public void Initialize_WhenTargetIsNull_MustThrow()
        {
            this.testee.Invoking(x => x.Initialize(null))
                .ShouldThrow<ArgumentNullException>()
                .Where(ex => ex.ParamName == "target");
        }

        [Fact]
        public void Initialize_WhenTargetIsNotNull_MustNotThrow()
        {
            this.testee.Invoking(x => x.Initialize(new object()))
                .ShouldNotThrow();
        }

        [Fact]
        public void Intercept_WhenNotInitialized_MustThrow()
        {
            this.testee.Invoking(x => x.Intercept(null, null, null))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void Intercept_ShouldCreateTargetInvocation()
        {
            var expectedTarget = new object();
            var expectedDecoratedMethod = new DynamicMethod("decorated", typeof(object), new Type[0]);
            var expectedImplementationMethod = new DynamicMethod("implementation", typeof(object), new Type[0]);
            var expectedArguments = new[] { new object(), new object() };
            this.testee.Initialize(expectedTarget);

            this.testee.Intercept(expectedDecoratedMethod, expectedImplementationMethod, expectedArguments);

            this.targetInvocationFactory.Verify(x => x.Create(expectedTarget, expectedImplementationMethod));
        }

        [Fact]
        public void Intercept_ShouldCreateInvocation()
        {
            var expectedTargetInvocation = Mock.Of<ITargetInvocation>();
            this.targetInvocationFactory.Setup(x => x.Create(It.IsAny<object>(), It.IsAny<MethodBase>()))
                .Returns(expectedTargetInvocation);

            var expectedDecoratedMethod = new DynamicMethod("decorated", typeof(object), new Type[0]);
            var expectedArguments = new[] { new object(), new object() };
            var expectedInterceptors = new[] { this.interceptor1.Object, this.interceptor2.Object };

            this.testee.Initialize(new object());
            
            this.testee.Intercept(expectedDecoratedMethod, null, expectedArguments);

            this.invocationFactory.Verify(x => 
                x.Create(expectedTargetInvocation, expectedDecoratedMethod, expectedArguments, expectedInterceptors));
        }

        [Fact]
        public void Intercept_ShouldProceedCreatedInvocation()
        {
            var invocation = new Mock<IInvocation> { DefaultValue = DefaultValue.Mock };
            this.SetupInvocationFactory(invocation.Object);
            this.testee.Initialize(new object());

            this.testee.Intercept(null, null, null);

            invocation.Verify(x => x.Proceed());
        }

        [Fact]
        public void Intercept_ShouldReturnReturnValueOfInvocation()
        {
            var expectedReturnValue = new object();
            var invocation = new Mock<IInvocation>();
            this.SetupInvocationFactory(invocation.Object);
            this.testee.Initialize(new object());

            invocation.Setup(x => x.Proceed())
                .Callback(() => invocation.SetupGet(x => x.ReturnValue).Returns(expectedReturnValue));

            object actualReturnValue = this.testee.Intercept(null, null, null);

            actualReturnValue.Should().Be(expectedReturnValue);
        }

        [Fact]
        public void Intercept_WhenReturnValueIsNull_MustDetermineWhetherReturnTypeIsNullable()
        {
            Type expectedReturnType = typeof(int);
            this.SetupInvocationFactory(Mock.Of<IInvocation>(x => x.ReturnValue == null));
            var decoratedMethod = new DynamicMethod("anyName", expectedReturnType, new Type[0]);
            this.typeInformation.Setup(x => x.IsNullable(It.IsAny<Type>())).Returns(true);
            this.testee.Initialize(new object());

            this.testee.Intercept(decoratedMethod, null, null);

            this.typeInformation.Verify(x => x.IsNullable(expectedReturnType));
        }

        [Fact]
        public void Intercept_WhenReturnTypeIsNotNullableAndReturnValueIsNull_MustThrow()
        {
            this.SetupInvocationFactory(Mock.Of<IInvocation>(x => x.ReturnValue == null));
            var decoratedMethod = new DynamicMethod("anyName", null, new Type[0]);
            this.typeInformation.Setup(x => x.IsNullable(It.IsAny<Type>())).Returns(false);
            this.testee.Initialize(new object());

            this.testee.Invoking(x => x.Intercept(decoratedMethod, null, null))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void Intercept_WhenReturnTypeIsNullableTypeAndReturnValueIsNull_MustNotThrow()
        {
            this.SetupInvocationFactory(Mock.Of<IInvocation>(x => x.ReturnValue == null));
            this.testee.Initialize(new object());
            var decoratedMethod = new DynamicMethod("anyName", null, new Type[0]);
            this.typeInformation.Setup(x => x.IsNullable(It.IsAny<Type>())).Returns(true);

            this.testee.Invoking(x => x.Intercept(decoratedMethod, null, null))
                .ShouldNotThrow();
        }

        private void SetupInvocationFactory(IInvocation invocation)
        {
            this.invocationFactory
                .Setup(x => x.Create(It.IsAny<ITargetInvocation>(), It.IsAny<MethodInfo>(), It.IsAny<object[]>(), It.IsAny<IDynamicInterceptor[]>()))
                .Returns(invocation);
        }
    }
}