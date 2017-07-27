using System;
using System.Reflection;

public interface IDynamicInterceptorManager
{
    void Initialize(object target, bool requireInterceptor);

    object Intercept(MethodBase decoratedMethod, MethodBase implementationMethod, Type[] genericArguments, object[] arguments);
}
