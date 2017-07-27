namespace StaticProxy.Interceptor.InterceptedMethod
{
    using System.Reflection;

    using StaticProxy.Interceptor.TargetInvocation;
    using System;
    internal interface IInterceptedMethod
    {
        MethodInfo DecoratedMethod { get; }

        Type[] GenericArguments { get; }

        ITargetInvocation TargetInvocation { get; }
    }
}