namespace StaticProxy.Interceptor
{
    using System.Reflection;

    internal interface IInvocationFactory
    {
        IInvocation Create(object target, MethodBase decoratedMethod, MethodBase implementationMethod, object[] arguments, IDynamicInterceptor[] interceptors);
    }
}