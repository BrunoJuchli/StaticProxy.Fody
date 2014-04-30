namespace StaticProxy.Interceptor.TargetInvocation
{
    internal class WithoutTargetInvocation : ITargetInvocation
    {
        public object InvokeMethodOnTarget(object[] arguments)
        {
            return null;
        }
    }
}