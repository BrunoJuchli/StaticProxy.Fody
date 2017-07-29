namespace StaticProxy.Interceptor
{
    using System.Collections.ObjectModel;

    public class FakeDynamicInterceptorCollection : Collection<IDynamicInterceptor>, IDynamicInterceptorCollection
    {
    }
}