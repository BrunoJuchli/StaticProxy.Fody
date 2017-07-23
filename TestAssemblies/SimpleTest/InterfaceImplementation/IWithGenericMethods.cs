using StaticProxy.Interceptor;
using System;
using System.Reflection;

namespace SimpleTest.InterfaceImplementation
{
    [StaticProxy]
    internal interface IWithGenericMethods
    {
        T ReturnsT<T>();

        //void TakesT<T>(T t);

        //T WithConstraint<T>(T t)
        //    where T : class;

        // T WithConstraints<T>(T t)
        //     where T : class, IComparable, IDisposable;
    }

    public sealed class WithGenericMethodsRemoveAfterILSpy : IWithGenericMethods
    {
        private readonly IDynamicInterceptorManager IDynamicInterceptorManager;

        public WithGenericMethodsRemoveAfterILSpy(IDynamicInterceptorManager IDynamicInterceptorManager)
        {
            Exceptions.EnsureDynamicInterceptorManagerNotNull(IDynamicInterceptorManager);
            this.IDynamicInterceptorManager = IDynamicInterceptorManager;
            this.IDynamicInterceptorManager.Initialize(this, true);
        }

        public T ReturnsT<T>()
        {
            MethodBase methodFromHandle = typeof(IWithGenericMethods).GetMethod("ReturnsT");
                /*MethodBase.GetMethodFromHandle(methodof(IWithGenericMethods.ReturnsT()).MethodHandle, typeof(IWithGenericMethods).TypeHandle)*/;
            object[] arguments = new object[0];
            MethodBase implementationMethod = null;
            return (T)((object)this.IDynamicInterceptorManager.Intercept(methodFromHandle, implementationMethod, arguments));
        }
    }
}
