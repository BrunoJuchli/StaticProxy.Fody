namespace StaticProxy.Interceptor.Tests
{
    using System;
    using FluentAssertions;
    using Moq;
    using StaticProxy.Interceptor;
    using StaticProxy.Interceptor.TargetInvocation;
    using Xunit;

    public class InvocationTest_FaultyInterceptorImplementation
    {
        private readonly Mock<ITargetInvocation> targetInvocation;

        private readonly Invocation testee;

        public InvocationTest_FaultyInterceptorImplementation()
        {
            this.targetInvocation = new Mock<ITargetInvocation>();

            this.testee = new Invocation(
                this.targetInvocation.Object,
                typeof(IFakeTarget).GetMethod("Foo"),
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
            void Foo();
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