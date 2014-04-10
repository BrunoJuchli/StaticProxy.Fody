using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

using StaticProxy.Interceptor;

public class DynamicInterceptorManager : IDynamicInterceptorManager
{
    private readonly IDynamicInterceptor[] interceptors;
    private readonly IInvocationFactory invocationFactory;

    private object target;

    public DynamicInterceptorManager(IDynamicInterceptorCollection interceptors)
        : this(interceptors, new InvocationFactory())
    {
    }

    internal DynamicInterceptorManager(IDynamicInterceptorCollection interceptors, IInvocationFactory invocationFactory)
    {
        this.invocationFactory = invocationFactory;
        this.interceptors = interceptors.ToArray();
    }

    public void Initialize(object target)
    {
        if (target == null)
        {
            throw new ArgumentNullException("target");
        }

        this.target = target;
    }

    public object Intercept(MethodBase decoratedMethod, MethodBase implementationMethod, object[] arguments)
    {
        // since we only support methods, not constructors, these are actually MethodInfo's
        var decoratedMethodInfo = (MethodInfo)decoratedMethod;
        var implementationMethodInfo = (MethodInfo)implementationMethod;
        
        if (this.target == null)
        {
            throw new InvalidOperationException("Something has gone seriously wrong with StaticProxy.Fody." + 
                ".Initialize(target) must be called once before any .Intercept(..)");
        }

        IInvocation invocation = this.invocationFactory
            .Create(this.target, decoratedMethodInfo, implementationMethodInfo, arguments, this.interceptors);

        invocation.Proceed();

        if (invocation.ReturnValue == null && decoratedMethodInfo.ReturnType.IsValueType)
        {
            string message = string.Format(
                CultureInfo.InvariantCulture,
                "Method {0}.{1} has return type {2} which is a value type. After the invocation the invocation the return value was null. Please ensure that your interceptors call IInvocation.Proceed() or sets a valid IInvocation.ReturnValue.",
                this.target.GetType().FullName,
                decoratedMethodInfo.ToString(),
                decoratedMethodInfo.ReturnType.Name);
            throw new InvalidOperationException(message);
        }

        return invocation.ReturnValue;
    }
}