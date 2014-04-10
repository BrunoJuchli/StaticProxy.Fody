namespace StaticProxy.Interceptor.Tests
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    using FluentAssertions;

    using Moq;

    using StaticProxy.Interceptor;

    using Xunit;

    public class DynamicInterceptorManagerTest
    {
        private readonly Mock<IDynamicInterceptor> interceptor1;
        private readonly Mock<IDynamicInterceptor> interceptor2;

        private readonly Mock<IInvocationFactory> invocationFactory;

        private readonly DynamicInterceptorManager testee;

        public DynamicInterceptorManagerTest()
        {
            this.interceptor1 = new Mock<IDynamicInterceptor>();
            this.interceptor2 = new Mock<IDynamicInterceptor>();

            this.invocationFactory = new Mock<IInvocationFactory> { DefaultValue = DefaultValue.Mock };

            this.testee = new DynamicInterceptorManager(
                new FakeDynamicInterceptorCollection { this.interceptor1.Object, this.interceptor2.Object },
                this.invocationFactory.Object);
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
        public void Intercept_ShouldCreateInvocation()
        {
            var expectedTarget = new object();
            var expectedDecoratedMethod = new DynamicMethod("decorated", typeof(object), new Type[0]);
            var expectedImplementationMethod = new DynamicMethod("implementation", typeof(object), new Type[0]);
            var expectedArguments = new[] { new object(), new object() };
            var expectedInterceptors = new[] { this.interceptor1.Object, this.interceptor2.Object };
            this.testee.Initialize(expectedTarget);
            
            this.testee.Intercept(expectedDecoratedMethod, expectedImplementationMethod, expectedArguments);

            this.invocationFactory.Verify(x => 
                x.Create(expectedTarget, expectedDecoratedMethod, expectedImplementationMethod, expectedArguments, expectedInterceptors));
        }

        [Fact]
        public void Intercept_ShouldProceedCreatedInvocation()
        {
            var invocation = new Mock<IInvocation>();
            this.invocationFactory
                .Setup(x => x.Create(It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<object[]>(), It.IsAny<IDynamicInterceptor[]>()))
                .Returns(invocation.Object);
            this.testee.Initialize(new object());

            this.testee.Intercept(null, null, null);

            invocation.Verify(x => x.Proceed());
        }

        [Fact]
        public void Intercept_ShouldReturnReturnValueOfInvocation()
        {
            var expectedReturnValue = new object();
            var invocation = new Mock<IInvocation>();
            this.invocationFactory
                .Setup(x => x.Create(It.IsAny<object>(), It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<object[]>(), It.IsAny<IDynamicInterceptor[]>()))
                .Returns(invocation.Object);
            this.testee.Initialize(new object());

            invocation.Setup(x => x.Proceed())
                .Callback(() => invocation.SetupGet(x => x.ReturnValue).Returns(expectedReturnValue));

            object actualReturnValue = this.testee.Intercept(null, null, null);

            actualReturnValue.Should().Be(expectedReturnValue);
        }
    }
}