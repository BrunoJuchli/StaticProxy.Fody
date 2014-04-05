namespace StaticProxy.Interceptor
{
    using System;

    public static class Exceptions
    {
        public static void EnsureDynamicInterceptorManagerNotNull(IDynamicInterceptorManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(typeof(IDynamicInterceptorManager).Name);
            }
        }
    }
}