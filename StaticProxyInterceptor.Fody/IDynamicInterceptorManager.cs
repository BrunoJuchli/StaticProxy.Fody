using System.Reflection;

public interface IDynamicInterceptorManager
{
    void Initialize(object target);

    object Intercept(MethodBase decoratedMethod, MethodBase implementationMethod, object[] arguments);
}
