namespace StaticProxy.Interceptor.InterceptedMethod
{
    using System.Reflection;

    using StaticProxy.Interceptor.TargetInvocation;
    using System;
    internal class InterceptedMethodFactory : IInterceptedMethodFactory
    {
        private readonly ITargetInvocationFactory targetInvocationFactory;

        public InterceptedMethodFactory(ITargetInvocationFactory targetInvocationFactory)
        {
            this.targetInvocationFactory = targetInvocationFactory;
        }

        public IInterceptedMethod Create(object target, MethodInfo decoratedMethod, MethodInfo implementationMethod, Type[] genericArguments)
        {
            if(genericArguments.Length > 0)
            {
                decoratedMethod = decoratedMethod.MakeGenericMethod(genericArguments);
                if(implementationMethod != null)
                {
                    implementationMethod = implementationMethod.MakeGenericMethod(genericArguments);
                }
            }

            return new InterceptedMethod(decoratedMethod, genericArguments, this.targetInvocationFactory.Create(target, implementationMethod));
        }
    }
}