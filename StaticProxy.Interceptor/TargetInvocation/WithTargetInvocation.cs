namespace StaticProxy.Interceptor.TargetInvocation
{
    using System;
    using System.Reflection;

    internal class WithTargetInvocation : ITargetInvocation
    {
        private readonly object target;
        private readonly MethodBase implementationMethod;

        public WithTargetInvocation(object target, MethodBase implementationMethod)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (implementationMethod == null)
            {
                throw new ArgumentNullException("implementationMethod");
            }

            this.target = target;
            this.implementationMethod = implementationMethod;
        }

        public object InvokeMethodOnTarget(object[] arguments)
        {
            try
            {
                return this.implementationMethod.Invoke(this.target, arguments);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException.PreserveStackTrace();
                }

                throw;
            }
        }
    }
}