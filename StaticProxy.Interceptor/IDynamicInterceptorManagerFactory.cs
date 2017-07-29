namespace StaticProxy.Interceptor
{
    interface IDynamicInterceptorManagerFactory
    {
        IDynamicInterceptorManager Create(IDynamicInterceptor[] interceptors);
    }
}
