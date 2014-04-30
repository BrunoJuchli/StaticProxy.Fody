namespace StaticProxy.Interceptor.TargetInvocation
{
    using System.Reflection;

    internal interface ITargetInvocationFactory
    {
        ITargetInvocation Create(object target, MethodBase implementationMethod);
    }
}