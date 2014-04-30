namespace IntegrationTests.ProxyWithoutTarget
{
    using System;

    using FluentAssertions;
    using Moq;
    using Ninject;

    using Xunit;

    public class ProxyWithoutTargetIntegrationTests : IDisposable
    {
        private static readonly string ImplementationName = typeof(ISomeProxiedInterface).FullName + "Implementation";

        private readonly Type implementationType;
        private readonly IKernel kernel;

        public ProxyWithoutTargetIntegrationTests()
        {
            this.implementationType = typeof(ISomeProxiedInterface).Assembly.GetType(ImplementationName);
            this.implementationType.Should().NotBeNull("StaticProxy.Fody should weave the interface proxy class");

            this.kernel = new StandardKernel();

            this.kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();
            this.kernel.Bind<ISomeProxiedInterface>().To(this.implementationType);
        }

        [Fact(Skip = "not yet implemented")]
        public void ExecutingMethod_MustUseInterceptors()
        {
            const int ExpectedArgument1 = 4820;
            var expectedArgument2 = new object();

            IInvocation interceptedInvocation = null;
            var fakeInterceptor = new Mock<IDynamicInterceptor>();
            fakeInterceptor.Setup(x => x.Intercept(It.IsAny<IInvocation>()))
                .Callback<IInvocation>(invocation => interceptedInvocation = invocation);

            this.kernel.Bind<IDynamicInterceptorCollection>().ToConstant(new FakeDynamicInterceptorCollection(fakeInterceptor.Object));
            var instance = this.kernel.Get<ISomeProxiedInterface>();

            instance.Foo(ExpectedArgument1, expectedArgument2);


            interceptedInvocation.Should().NotBeNull();
            interceptedInvocation.Arguments.Should().ContainInOrder(ExpectedArgument1, expectedArgument2);
            interceptedInvocation.Method.Should().BeSameAs(typeof(ISomeProxiedInterface).GetMethod("Foo"));
        }

        [Fact(Skip = "not yet implemented")]
        public void ExecutingMethod_WithReturnValue_MustReturnReturnValueOfInterceptor()
        {
            const int ExpectedResult = 4820;

            var fakeInterceptor = new Mock<IDynamicInterceptor>();
            fakeInterceptor.Setup(x => x.Intercept(It.IsAny<IInvocation>()))
                .Callback<IInvocation>(invocation => invocation.ReturnValue = ExpectedResult);

            this.kernel.Bind<IDynamicInterceptorCollection>().ToConstant(new FakeDynamicInterceptorCollection(fakeInterceptor.Object));
            var instance = this.kernel.Get<ISomeProxiedInterface>();

            instance.Bar().Should().Be(ExpectedResult);
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }
}