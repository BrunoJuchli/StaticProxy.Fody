namespace StaticProxy.Interceptor
{
    using System.Reflection;

    internal class InvocationFactory : IInvocationFactory
    {
        public IInvocation Create(object target, MethodInfo decoratedMethod, MethodInfo implementationMethod, object[] arguments, IDynamicInterceptor[] interceptors)
        {
            return new Invocation(target, decoratedMethod, implementationMethod, arguments, interceptors);
        }
    }
}