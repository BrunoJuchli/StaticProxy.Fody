namespace StaticProxy.Interceptor.InterceptedMethod
{
    using System.Reflection;

    using StaticProxy.Interceptor.TargetInvocation;
    using System;
    internal class InterceptedMethod : IInterceptedMethod
    {
        public InterceptedMethod(MethodInfo decoratedMethod, Type[] genericArguments, ITargetInvocation targetInvocation)
        {
            this.DecoratedMethod = decoratedMethod;
            this.GenericArguments = genericArguments;
            this.TargetInvocation = targetInvocation;
        }

        public MethodInfo DecoratedMethod { get; }

        public Type[] GenericArguments { get; }

        public ITargetInvocation TargetInvocation { get; }
    }
}