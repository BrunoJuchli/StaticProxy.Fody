using StaticProxy.Interceptor;
using System;
using System.Reflection;

namespace SimpleTest.InterfaceImplementation
{
    [StaticProxy]
    internal interface IWithGenericMethods
    {
        T ReturnsT<T>();

        void TakesT<T>(T t);

        T WithConstraint<T>(T t)
            where T : class;

        T WithConstraints<T>(T t)
            where T : class, IComparable, IDisposable;
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
            return (T)this.IDynamicInterceptorManager.Intercept(methodFromHandle, implementationMethod, arguments);
        }

        public void TakesT<T>(T t)
        {
            MethodBase methodFromHandle = typeof(IWithGenericMethods).GetMethod("TakesT");
            /*MethodBase.GetMethodFromHandle(methodof(IWithGenericMethods.ReturnsT()).MethodHandle, typeof(IWithGenericMethods).TypeHandle)*/
            MethodBase implementationMethod = null;
            object[] arguments = new object[1];
            arguments[1] = t;
            this.IDynamicInterceptorManager.Intercept(methodFromHandle, implementationMethod, arguments);
        }

        public T WithConstraint<T>(T t) where T : class
        {
            MethodBase methodFromHandle = typeof(IWithGenericMethods).GetMethod("WithConstraint");
            /*MethodBase.GetMethodFromHandle(methodof(IWithGenericMethods.ReturnsT()).MethodHandle, typeof(IWithGenericMethods).TypeHandle)*/
            MethodBase implementationMethod = null;

            object[] arguments = new object[1];
            arguments[1] = t;
            
            return (T)this.IDynamicInterceptorManager.Intercept(methodFromHandle, implementationMethod, arguments);
        }

        public T WithConstraints<T>(T t)
            where T : class, IComparable, IDisposable
        {
            MethodBase methodFromHandle = typeof(IWithGenericMethods).GetMethod("WithConstraints");
            /*MethodBase.GetMethodFromHandle(methodof(IWithGenericMethods.ReturnsT()).MethodHandle, typeof(IWithGenericMethods).TypeHandle)*/
            MethodBase implementationMethod = null;

            object[] arguments = new object[1];
            arguments[1] = t;

            return (T)this.IDynamicInterceptorManager.Intercept(methodFromHandle, implementationMethod, arguments);
        }
    }
}
