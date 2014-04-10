namespace StaticProxy.Interceptor.Tests
{
    using System.Collections.ObjectModel;

    public class FakeDynamicInterceptorCollection : Collection<IDynamicInterceptor>, IDynamicInterceptorCollection
    {
    }
}