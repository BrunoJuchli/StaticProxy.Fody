namespace StaticProxy.Interceptor.IntegrationTests
{
    using System;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class When_interface_proxy_does_not_have_interceptor
    {
        private readonly object demoInterfaceProxy;

        private readonly DynamicInterceptorManager testee;

        public When_interface_proxy_does_not_have_interceptor()
        {
            this.demoInterfaceProxy = new object();

            this.testee = new DynamicInterceptorManager(new DynamicInterceptorCollection());
        }

        [Fact]
        public void Initialize_WhenTargetIsNull_MustThrow()
        {
            this.testee.Invoking(x => x.Initialize(null, true))
                .ShouldThrow<ArgumentNullException>()
                .Where(ex => ex.ParamName == "implementationMethodTarget");
        }

        [Fact]
        public void Initialize_WhenTargetIsNotNull_MustThrow()
        {
            this.testee.Invoking(x => x.Initialize(new object(), true))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void Intercept_MustThrow()
        {
            this.testee.Invoking(x => x.Intercept(
                MethodInfos.InterfaceProxy.DecoratedMethodReturningReferenceType,
                null,
                new Type[0],
                new object[] { Mock.Of<IDisposable>() }))
                .ShouldThrow<InvalidOperationException>();
        }
    }
}