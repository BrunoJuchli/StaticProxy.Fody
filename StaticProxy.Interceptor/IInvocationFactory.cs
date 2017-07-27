namespace StaticProxy.Interceptor
{
    using StaticProxy.Interceptor.InterceptedMethod;

    internal interface IInvocationFactory
    {
        IInvocation Create(IInterceptedMethod interceptedMethod, object[] arguments, IDynamicInterceptor[] interceptors);
    }
}