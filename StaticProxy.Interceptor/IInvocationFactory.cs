namespace StaticProxy.Interceptor
{
    using System.Reflection;

    internal interface IInvocationFactory
    {
        IInvocation Create(object target, MethodInfo decoratedMethod, MethodInfo implementationMethod, object[] arguments, IDynamicInterceptor[] interceptors);
    }
}