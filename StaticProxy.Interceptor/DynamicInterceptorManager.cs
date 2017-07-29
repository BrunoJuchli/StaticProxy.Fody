using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

using StaticProxy.Interceptor;
using StaticProxy.Interceptor.Reflection;
using StaticProxy.Interceptor.TargetInvocation;
using StaticProxy.Interceptor.InterceptedMethod;

public class DynamicInterceptorManager : IDynamicInterceptorManager
{
    private readonly IDynamicInterceptorCollection interceptors;
    private readonly IInterceptedMethodFactory interceptedMethodFactory;
    private readonly IInvocationFactory invocationFactory;
    private readonly ITypeInformation typeInformation;

    private object implementationMethodTarget;

    public DynamicInterceptorManager(IDynamicInterceptorCollection interceptors)
        : this(interceptors, SingletonHolder.InterceptedMethodFactory, SingletonHolder.InvocationFactory, SingletonHolder.TypeInformation)
    {
    }

    internal DynamicInterceptorManager(
        IDynamicInterceptorCollection interceptors, 
        IInterceptedMethodFactory interceptedMethodFactory,
        IInvocationFactory invocationFactory,
        ITypeInformation typeInformation)
    {
        this.interceptedMethodFactory = interceptedMethodFactory;
        this.invocationFactory = invocationFactory;
        this.typeInformation = typeInformation;
        this.interceptors = interceptors;

        this.implementationMethodTarget = null;
    }

    /// <summary>
    /// Tell the manager whether it is a class or interface proxy and initialize it with the generated interface proxy object or class-proxy object.
    /// </summary>
    /// <param name="implementationMethodTarget">The proxy itself in case of an interface-proxy or the proxied-class in case of a class proxy.</param>
    /// <param name="requireInterceptor">Should be <c>True</c> in case of an interface-proxy and <c>False</c> in case of a class proxy.</param>
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
                "There is no interceptor for '{0}', but the proxy requires at least one to work.",
                implementationMethodTarget.GetType().FullName);

            throw new InvalidOperationException(message);
        }
    }

    public object Intercept(MethodBase decoratedMethod, MethodBase implementationMethod, Type[] genericArguments, object[] arguments)
    {
        if (this.implementationMethodTarget == null)
        {
            throw new InvalidOperationException("Something has gone seriously wrong with StaticProxy.Fody." +
                ".Initialize(implementationMethodTarget) must be called once before any .Intercept(..)");
        }

        var interceptedMethod = this.interceptedMethodFactory.Create(
            this.implementationMethodTarget, 
            (MethodInfo)decoratedMethod, // since we only support methods, not constructors, this is actually a MethodInfo
            (MethodInfo)implementationMethod, // since we only support methods, not constructors, this is actually a MethodInfo
            genericArguments);
        
        IInvocation invocation = this.invocationFactory
            .Create(interceptedMethod, arguments, this.interceptors.ToArray());

        invocation.Proceed();

        if (invocation.ReturnValue == null && !this.typeInformation.IsNullable(interceptedMethod.DecoratedMethod.ReturnType))
        {
            string message = string.Format(
                CultureInfo.InvariantCulture,
                "Method {0}.{1} has return type {2} which is a value type. After the invocation the invocation the return value was null. Please ensure that your interceptor(s) call IInvocation.Proceed() or set a valid IInvocation.ReturnValue.",
                this.implementationMethodTarget.GetType().FullName,
                interceptedMethod.DecoratedMethod,
                interceptedMethod.DecoratedMethod.ReturnType.FullName);
            throw new InvalidOperationException(message);
        }

        return invocation.ReturnValue;
    }
}