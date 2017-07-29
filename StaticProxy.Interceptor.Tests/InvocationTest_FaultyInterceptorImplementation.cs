namespace StaticProxy.Interceptor
{
    using System;
    using FluentAssertions;
    using Moq;
    using StaticProxy.Interceptor.TargetInvocation;
    using Xunit;
    using StaticProxy.Interceptor.InterceptedMethod;

    public class InvocationTest_FaultyInterceptorImplementation
    {
        private readonly Mock<IInterceptedMethod> interceptedMethod;

        private readonly Invocation testee;

        public InvocationTest_FaultyInterceptorImplementation()
        {
            this.interceptedMethod = new Mock<IInterceptedMethod>();

            this.testee = new Invocation(
                this.interceptedMethod.Object,
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