namespace StaticProxy.Interceptor
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using StaticProxy.Interceptor.InterfaceProxy;

    public interface IProxyBuilder
    {
        T CreateClassProxy<T>(IDynamicInterceptor[] interceptors, object[] arguments);

        T CreateInterfaceProxy<T>(params IDynamicInterceptor[] interceptors)
            where T : class;

        T Create<T>(Expression<Func<T>> newExpression, params IDynamicInterceptor[] interceptors);
    }

    public class ProxyBuilder : IProxyBuilder
    {
        private readonly IDynamicInterceptorManagerFactory dynamicInterceptorManagerFactory;

        public ProxyBuilder()
            : this(SingletonHolder.DynamicInterceptorManagerFactory)
        { }

        internal ProxyBuilder(IDynamicInterceptorManagerFactory dynamicInterceptorManagerFactory)
        {
            this.dynamicInterceptorManagerFactory = dynamicInterceptorManagerFactory;
        }

        public T Create<T>(Expression<Func<T>> newExpression, params IDynamicInterceptor[] interceptors)
        {
            var ctorExpression = newExpression.Body as NewExpression;
            if(ctorExpression == null)
            {
                throw new ArgumentException($"{newExpression} is not a `NewExpression`", nameof(newExpression));
            }

            object[] allArguments = new object[ctorExpression.Arguments.Count + 1];
            ctorExpression.Arguments
                .Select(exp => Expression.Lambda(exp).Compile().DynamicInvoke())
                .ToArray()
                .CopyTo(allArguments, 0);
            allArguments[ctorExpression.Arguments.Count] = this.dynamicInterceptorManagerFactory.Create(interceptors);

            return (T)ctorExpression.Constructor.Invoke(allArguments);
        }

        public T CreateClassProxy<T>(IDynamicInterceptor[] interceptors, object[] arguments)
        {
            object[] allArguments = new object[arguments.Length + 1];
            arguments.CopyTo(allArguments, 0);
            allArguments[arguments.Length] = this.dynamicInterceptorManagerFactory.Create(interceptors);
            return (T)Activator.CreateInstance(typeof(T), arguments);
        }
 
        public T CreateInterfaceProxy<T>(params IDynamicInterceptor[] interceptors) where T : class
        {
            Type implementationType = InterfaceProxyHelpers.GetImplementationTypeOfInterface(typeof(T));
            var dynamicInterceptorManager = this.dynamicInterceptorManagerFactory.Create(interceptors);
            return (T)Activator.CreateInstance(implementationType, dynamicInterceptorManager);
        }
    }
}
