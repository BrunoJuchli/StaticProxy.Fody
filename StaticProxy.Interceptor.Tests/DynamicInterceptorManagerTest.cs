namespace StaticProxy.Interceptor.Tests
{
    using System;
    using System.Linq;
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
        private readonly FakeDynamicInterceptorCollection interceptorCollection;
        private readonly Mock<ITargetInvocationFactory> targetInvocationFactory;
        private readonly Mock<IInvocationFactory> invocationFactory;
        private readonly Mock<ITypeInformation> typeInformation;

        private readonly DynamicInterceptorManager testee;

        public DynamicInterceptorManagerTest()
        {
            this.interceptorCollection = new FakeDynamicInterceptorCollection();
            this.targetInvocationFactory = new Mock<ITargetInvocationFactory> { DefaultValue = DefaultValue.Mock };
            this.invocationFactory = new Mock<IInvocationFactory> { DefaultValue = DefaultValue.Mock };
            this.typeInformation = new Mock<ITypeInformation>();

            this.testee = new DynamicInterceptorManager(
                this.interceptorCollection,
                this.targetInvocationFactory.Object, 
                this.invocationFactory.Object,
                this.typeInformation.Object);
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
        public void Initialize_WhenInterceptorsNotRequiredAndThereIsNoInterceptor_MustNotThrow()
        {
        }

        [Fact]
        public void Initialize_WhenInterceptorsNotRequiredAndThereIsOneInterceptor_MustNotThrow()
        {
        }

        [Fact]
        public void Initialize_WhenInterceptorsRequiredButThereIsNoIntereceptor_MustThrow()
        {
            
        }

        [Fact]
        public void Initialize_WhenInterceptorsRequiredAndThereIsRequired_MustNotThrow()
        {
            
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
            this.testee.Initialize(expectedTarget, false);

            this.testee.Intercept(expectedDecoratedMethod, expectedImplementationMethod, expectedArguments);

            this.targetInvocationFactory.Verify(x => x.Create(expectedTarget, expectedImplementationMethod));
        }

        [Fact]
        public void Intercept_ShouldCreateInvocation()
        {
            var interceptor1 = Mock.Of<IDynamicInterceptor>();
            var interceptor2 = Mock.Of<IDynamicInterceptor>();
            this.interceptorCollection.Add(interceptor1);
            this.interceptorCollection.Add(interceptor2);
            var expectedInterceptors = new[] { interceptor1, interceptor2 };

            var expectedTargetInvocation = Mock.Of<ITargetInvocation>();
            this.targetInvocationFactory.Setup(x => x.Create(It.IsAny<object>(), It.IsAny<MethodBase>()))
                .Returns(expectedTargetInvocation);

            var expectedDecoratedMethod = new DynamicMethod("decorated", typeof(object), new Type[0]);
            var expectedArguments = new[] { new object(), new object() };

            this.testee.Initialize(new object(), false);
            
            this.testee.Intercept(expectedDecoratedMethod, null, expectedArguments);

            this.invocationFactory.Verify(x =>
                x.Create(expectedTargetInvocation, expectedDecoratedMethod, expectedArguments, It.Is<IDynamicInterceptor[]>(i => i.SequenceEqual(expectedInterceptors))));
        }

        [Fact]
        public void Intercept_ShouldProceedCreatedInvocation()
        {
            var invocation = new Mock<IInvocation> { DefaultValue = DefaultValue.Mock };
            this.SetupInvocationFactory(invocation.Object);
            this.testee.Initialize(new object(), false);

            this.testee.Intercept(null, null, null);

            invocation.Verify(x => x.Proceed());
        }

        [Fact]
        public void Intercept_ShouldReturnReturnValueOfInvocation()
        {
            var expectedReturnValue = new object();
            var invocation = new Mock<IInvocation>();
            this.SetupInvocationFactory(invocation.Object);
            this.testee.Initialize(new object(), false);

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
            this.testee.Initialize(new object(), false);

            this.testee.Intercept(decoratedMethod, null, null);

            this.typeInformation.Verify(x => x.IsNullable(expectedReturnType));
        }

        [Fact]
        public void Intercept_WhenReturnTypeIsNotNullableAndReturnValueIsNull_MustThrow()
        {
            this.SetupInvocationFactory(Mock.Of<IInvocation>(x => x.ReturnValue == null));
            var decoratedMethod = new DynamicMethod("anyName", null, new Type[0]);
            this.typeInformation.Setup(x => x.IsNullable(It.IsAny<Type>())).Returns(false);
            this.testee.Initialize(new object(), false);

            this.testee.Invoking(x => x.Intercept(decoratedMethod, null, null))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void Intercept_WhenReturnTypeIsNullableTypeAndReturnValueIsNull_MustNotThrow()
        {
            this.SetupInvocationFactory(Mock.Of<IInvocation>(x => x.ReturnValue == null));
            this.testee.Initialize(new object(), false);
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