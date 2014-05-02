namespace IntegrationTests.ProxyWithoutTarget
{
    using System;

    using FluentAssertions;
    using Ninject;

    public abstract class ProxyWithoutTargetIntegrationTestBase : IDisposable
    {
        protected readonly IKernel Kernel;

        private static readonly string ImplementationName = typeof(IProxy).FullName + "Implementation";
        private readonly Type implementationType;

        protected ProxyWithoutTargetIntegrationTestBase()
        {
            this.implementationType = typeof(IProxy).Assembly.GetType(ImplementationName);
            this.implementationType.Should().NotBeNull("StaticProxy.Fody should weave the interface proxy class");

            this.Kernel = new StandardKernel();

            this.Kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();
            this.Kernel.Bind<IProxy>().To(this.implementationType);
        }

        public void Dispose()
        {
            this.Kernel.Dispose();
        }

        protected void BindInterceptorCollection(params IDynamicInterceptor[] interceptors)
        {
            this.Kernel
                .Bind<IDynamicInterceptorCollection>()
                .ToConstant(new FakeDynamicInterceptorCollection(interceptors));
        }
    }
}