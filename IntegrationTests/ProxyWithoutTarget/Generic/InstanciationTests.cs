namespace IntegrationTests.ProxyWithoutTarget.Generic
{
    using System;
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Xunit;

    public class InstanciationTests : GenericProxyWithoutTargetIntegrationTestBase
    {
        [Fact]
        public void WhenThereIsNoInterceptor_InstanciationMustThrow()
        {
            this.BindInterceptorCollection();

            this.Kernel.Invoking(x => x.Get<IGenericProxy<IDisposable, string>>())
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void WhenThereIsAnInterceptor_InstanciationMustNotThrow()
        {
            this.BindInterceptorCollection(Mock.Of<IDynamicInterceptor>());

            this.Kernel.Invoking(x => x.Get<IGenericProxy<IDisposable, string>>())
                .ShouldNotThrow();
        }
    }
}