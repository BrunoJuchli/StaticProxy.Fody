namespace StaticProxy.Interceptor.IntegrationTests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class When_class_proxy_does_not_have_interceptor
    {
        private readonly DemoClassProxy demoClassProxy;

        private readonly DynamicInterceptorManager testee;

        public When_class_proxy_does_not_have_interceptor()
        {
            this.demoClassProxy = new DemoClassProxy();

            this.testee = new DynamicInterceptorManager(new FakeDynamicInterceptorCollection());
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
        public void Intercept_ShouldReturnOriginalImplementationValue()
        {
            const int initialValue = 2222;

            this.testee.Initialize(this.demoClassProxy, false);

            var returnValue = this.testee.Intercept(
                MethodInfos.ClassProxy.DecoratedMethodReturningValueType,
                MethodInfos.ClassProxy.ImplementedMethodReturningValueType,
                new Type[0],
                new object[] { initialValue });

            returnValue.Should().Be(2 * initialValue); //original implementation multiplies by 2
        }
    }
}