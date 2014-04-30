namespace StaticProxy.Interceptor
{
    using System.Reflection;

    using StaticProxy.Interceptor.TargetInvocation;

    internal class InvocationFactory : IInvocationFactory
    {
        public IInvocation Create(
            ITargetInvocation targetInvocation,
            MethodInfo decoratedMethod,
            object[] arguments,
            IDynamicInterceptor[] interceptors)
        {
            return new Invocation(targetInvocation, decoratedMethod, arguments, interceptors);
        }
    }
}