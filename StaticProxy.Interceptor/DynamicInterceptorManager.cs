using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

using StaticProxy.Interceptor;
using StaticProxy.Interceptor.Reflection;
using StaticProxy.Interceptor.TargetInvocation;

public class DynamicInterceptorManager : IDynamicInterceptorManager
{
    private readonly IDynamicInterceptor[] interceptors;
    private readonly ITargetInvocationFactory targetInvocationFactory;
    private readonly IInvocationFactory invocationFactory;
    private readonly ITypeInformation typeInformation;

    private object target;

    public DynamicInterceptorManager(IDynamicInterceptorCollection interceptors)
        : this(interceptors, new TargetInvocationFactory(), new InvocationFactory(), new TypeInformation())
    {
    }

    internal DynamicInterceptorManager(
        IDynamicInterceptorCollection interceptors, 
        ITargetInvocationFactory targetInvocationFactory,
        IInvocationFactory invocationFactory,
        ITypeInformation typeInformation)
    {
        this.targetInvocationFactory = targetInvocationFactory;
        this.invocationFactory = invocationFactory;
        this.typeInformation = typeInformation;
        this.interceptors = interceptors.ToArray();

        this.target = null;
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
        if (this.target == null)
        {
            throw new InvalidOperationException("Something has gone seriously wrong with StaticProxy.Fody." +
                ".Initialize(target) must be called once before any .Intercept(..)");
        }

        // since we only support methods, not constructors, this is actually a MethodInfo
        var decoratedMethodInfo = (MethodInfo)decoratedMethod;
        
        ITargetInvocation targetInvocation = this.targetInvocationFactory.Create(this.target, implementationMethod);
        
        IInvocation invocation = this.invocationFactory
            .Create(targetInvocation, decoratedMethodInfo, arguments, this.interceptors);

        invocation.Proceed();

        if (invocation.ReturnValue == null && !this.typeInformation.IsNullable(decoratedMethodInfo.ReturnType))
        {
            string message = string.Format(
                CultureInfo.InvariantCulture,
                "Method {0}.{1} has return type {2} which is a value type. After the invocation the invocation the return value was null. Please ensure that your interceptors call IInvocation.Proceed() or sets a valid IInvocation.ReturnValue.",
                this.target.GetType().FullName,
                decoratedMethodInfo,
                decoratedMethodInfo.ReturnType.FullName);
            throw new InvalidOperationException(message);
        }

        return invocation.ReturnValue;
    }
}