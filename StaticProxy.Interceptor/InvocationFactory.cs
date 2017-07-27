namespace StaticProxy.Interceptor
{
    using StaticProxy.Interceptor.InterceptedMethod;

    internal class InvocationFactory : IInvocationFactory
    {
        public IInvocation Create(IInterceptedMethod interceptedMethod, object[] arguments, IDynamicInterceptor[] interceptors)
        {
            return new Invocation(interceptedMethod, arguments, interceptors);
        }
    }
}