namespace IntegrationTests.ProxyWithoutTarget.Generic
{
    using System;

    using FluentAssertions;
    using Ninject;

    public abstract class GenericProxyWithoutTargetIntegrationTestBase : IDisposable
    {
        protected readonly IKernel Kernel;

        private static readonly string ImplementationName = string.Join("Implementation`", typeof(IGenericProxy<,>).FullName.Split('`'));
        private readonly Type implementationType;

        protected GenericProxyWithoutTargetIntegrationTestBase()
        {
            this.implementationType = typeof(IProxy).Assembly.GetType(ImplementationName);
            this.implementationType.Should().NotBeNull("StaticProxy.Fody should weave the interface proxy class");

            this.Kernel = new StandardKernel();

            this.Kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();
            this.Kernel.Bind(typeof(IGenericProxy<,>)).To(this.implementationType);
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