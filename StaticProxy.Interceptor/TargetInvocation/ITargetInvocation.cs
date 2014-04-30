namespace StaticProxy.Interceptor.TargetInvocation
{
    internal interface ITargetInvocation
    {
        object InvokeMethodOnTarget(object[] arguments);
    }
}