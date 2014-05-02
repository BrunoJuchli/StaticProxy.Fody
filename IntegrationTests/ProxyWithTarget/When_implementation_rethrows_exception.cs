namespace IntegrationTests.ProxyWithTarget
{
    using System;

    using FluentAssertions;

    using Ninject;

    using Xunit;

    public class When_implementation_rethrows_exception
    {
        [Fact]
        public void MustThrowRethrownException()
        {
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();
                kernel.Bind<IDynamicInterceptorCollection>().ToConstant(new FakeDynamicInterceptorCollection());

                var instance = kernel.Get<IntegrationThrowException>();

                instance.Invoking(x => x.RethrowException())
                    .ShouldThrow<Exception>()
                    .WithInnerException<NullReferenceException>();
            }
        }    
    }
}