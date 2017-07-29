namespace StaticProxy.Interceptor
{
    using StaticProxy.Interceptor.InterceptedMethod;
    using StaticProxy.Interceptor.Reflection;
    using StaticProxy.Interceptor.TargetInvocation;

    /// <summary>
    ///  For performance reasons the immutable / no data holding types are created only once
    /// </summary>
    internal static class SingletonHolder
    {
        internal static readonly IInterceptedMethodFactory InterceptedMethodFactory = new InterceptedMethodFactory(new TargetInvocationFactory());
        internal static readonly IInvocationFactory InvocationFactory = new InvocationFactory();
        internal static readonly ITypeInformation TypeInformation = new TypeInformation();
        internal static readonly IDynamicInterceptorManagerFactory DynamicInterceptorManagerFactory = new DynamicInterceptorManagerFactory();
    }
}
