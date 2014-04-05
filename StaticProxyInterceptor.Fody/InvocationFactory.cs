namespace StaticProxyInterceptor.Fody
{
    using System.Reflection;

    internal class InvocationFactory : IInvocationFactory
    {
        public IInvocation Create(object target, MethodBase decoratedMethod, MethodBase implementationMethod, object[] arguments, IDynamicInterceptor[] interceptors)
        {
            return new Invocation(target, decoratedMethod, implementationMethod, arguments, interceptors);
        }
    }
}