namespace StaticProxy.Interceptor
{
    using System.Reflection;

    using StaticProxy.Interceptor.TargetInvocation;

    internal interface IInvocationFactory
    {
        IInvocation Create(ITargetInvocation targetInvocation, MethodInfo decoratedMethod, object[] arguments, IDynamicInterceptor[] interceptors);
    }
}