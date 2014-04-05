using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using StaticProxy.Interceptor;

public class DynamicInterceptorManager : IDynamicInterceptorManager
{
    private readonly IDynamicInterceptor[] interceptors;
    private readonly IInvocationFactory invocationFactory;

    private object target;

    public DynamicInterceptorManager(IEnumerable<IDynamicInterceptor> interceptors)
        : this(interceptors, new InvocationFactory())
    {
    }

    internal DynamicInterceptorManager(IEnumerable<IDynamicInterceptor> interceptors, IInvocationFactory invocationFactory)
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
        if (this.target == null)
        {
            throw new InvalidOperationException("Something has gone seriously wrong with StaticProxy.Fody." + 
                ".Initialize(target) must be called once before any .Intercept(..)");
        }

        IInvocation invocation = this.invocationFactory
            .Create(this.target, decoratedMethod, implementationMethod, arguments, this.interceptors);

        
        invocation.Proceed();

        return invocation.ReturnValue;
    }
}