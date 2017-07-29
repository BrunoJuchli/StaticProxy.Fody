namespace StaticProxy.Interceptor.InterceptedMethod
{
    using System.Reflection;
    using System;

    internal interface IInterceptedMethodFactory
    {

        IInterceptedMethod Create(object target, MethodInfo decoratedMethod, MethodInfo implementationMethod, Type[] genericArguments);
    }
}