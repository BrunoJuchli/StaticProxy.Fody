namespace StaticProxy.Interceptor.IntegrationTests
{
    using System;

    class DemoInterceptor : IDynamicInterceptor
    {
        public Action<IInvocation> InterceptionLogic { get; set; }

        public void Intercept(IInvocation invocation)
        {
            this.InterceptionLogic(invocation);
        }
    }
}
