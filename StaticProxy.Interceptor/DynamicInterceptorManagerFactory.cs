namespace StaticProxy.Interceptor
{
    class DynamicInterceptorManagerFactory : IDynamicInterceptorManagerFactory
    {
        public IDynamicInterceptorManager Create(IDynamicInterceptor[] interceptors)
        {
            return new DynamicInterceptorManager(new DynamicInterceptorCollection(interceptors));
        }
    }
}
