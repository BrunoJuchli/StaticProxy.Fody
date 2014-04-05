namespace StaticProxy.Fody.Tests
{
    using System;

    using FluentAssertions;

    using Moq;

    using StaticProxyInterceptor.Fody;

    using Xunit;

    public class InvocationTest_FaultyInterceptorImplementation
    {
        private readonly Invocation testee;

        public InvocationTest_FaultyInterceptorImplementation()
        {
            this.testee = new Invocation(
                new Mock<IFakeTarget>().Object,
                typeof(IFakeTarget).GetMethod("Decorated"),
                typeof(IFakeTarget).GetMethod("Implementation"),
                new object[0], 
                new IDynamicInterceptor[] { new FaultyInterceptor() });
        }
        
        [Fact(Skip = "Interceptors need to be able to be execute multiple times, e.g. for retry. So how to detect whether it's faulty? I don't think we can, except maybe with checking whether there was an exception,...")]
        public void Proceed_WhenInterceptorProceedsMoreThanOnce()
        {          
            this.testee.Invoking(x => x.Proceed())
                .ShouldThrow<InvalidOperationException>();
        }

        public interface IFakeTarget
        {
            void Decorated();

            void Implementation();
        }

        public class FaultyInterceptor : IDynamicInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                invocation.Proceed();
                invocation.Proceed();
            }
        }
    }
}