using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

using StaticProxy.Interceptor;
using StaticProxy.Interceptor.Reflection;
using StaticProxy.Interceptor.TargetInvocation;

public class DynamicInterceptorManager : IDynamicInterceptorManager
{
    private readonly IDynamicInterceptorCollection interceptors;
    private readonly ITargetInvocationFactory targetInvocationFactory;
    private readonly IInvocationFactory invocationFactory;
    private readonly ITypeInformation typeInformation;

    private object implementationMethodTarget;

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
        this.interceptors = interceptors;

        this.implementationMethodTarget = null;
    }

    public void Initialize(object implementationMethodTarget, bool requireInterceptor)
    {
        if (implementationMethodTarget == null)
        {
            throw new ArgumentNullException("implementationMethodTarget");
        }

        this.implementationMethodTarget = implementationMethodTarget;

        if (requireInterceptor && !this.interceptors.Any())
        {
            string message = string.Format(
                CultureInfo.InvariantCulture,
                "There is no interceptor for '{0}', but the proxy requires one to work.",
                implementationMethodTarget.GetType().FullName);

            throw new InvalidOperationException(message);
        }
    }

    public object Intercept(MethodBase decoratedMethod, MethodBase implementationMethod, object[] arguments)
    {
        if (this.implementationMethodTarget == null)
        {
            throw new InvalidOperationException("Something has gone seriously wrong with StaticProxy.Fody." +
                ".Initialize(implementationMethodTarget) must be called once before any .Intercept(..)");
        }

        // since we only support methods, not constructors, this is actually a MethodInfo
        var decoratedMethodInfo = (MethodInfo)decoratedMethod;
        
        ITargetInvocation targetInvocation = this.targetInvocationFactory.Create(this.implementationMethodTarget, implementationMethod);
        
        IInvocation invocation = this.invocationFactory
            .Create(targetInvocation, decoratedMethodInfo, arguments, this.interceptors.ToArray());

        invocation.Proceed();

        if (invocation.ReturnValue == null && !this.typeInformation.IsNullable(decoratedMethodInfo.ReturnType))
        {
            string message = string.Format(
                CultureInfo.InvariantCulture,
                "Method {0}.{1} has return type {2} which is a value type. After the invocation the invocation the return value was null. Please ensure that your interceptors call IInvocation.Proceed() or sets a valid IInvocation.ReturnValue.",
                this.implementationMethodTarget.GetType().FullName,
                decoratedMethodInfo,
                decoratedMethodInfo.ReturnType.FullName);
            throw new InvalidOperationException(message);
        }

        return invocation.ReturnValue;
    }
}