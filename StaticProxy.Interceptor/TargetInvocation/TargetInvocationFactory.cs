namespace StaticProxy.Interceptor.TargetInvocation
{
    using System.Reflection;

    internal class TargetInvocationFactory : ITargetInvocationFactory
    {
        private static readonly WithoutTargetInvocation WithoutTargetInvocation = new WithoutTargetInvocation();

        public ITargetInvocation Create(object target, MethodBase implementationMethod)
        {
            if (implementationMethod == null)
            {
                return WithoutTargetInvocation;
            }

            return new WithTargetInvocation(target, implementationMethod);
        }
    }
}