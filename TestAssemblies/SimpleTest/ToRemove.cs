namespace SimpleTest
{
    using System;
    using System.Reflection;

    using StaticProxyInterceptor.Fody;

    public class ToRemove
    {
        private readonly IDynamicInterceptorManager dynamicInterceptorManager;

        private readonly FooInterceptor foo;

        public ToRemove(int i1, int i2, int i3, int i4, int i5, int i6, FooInterceptor foo)
        {
            this.foo = foo;
        }

        public ToRemove(IDynamicInterceptorManager dynamicInterceptorManager)
        {
            Exceptions.EnsureDynamicInterceptorManagerNotNull(dynamicInterceptorManager);
            
            this.dynamicInterceptorManager = dynamicInterceptorManager;
        }

        public void Undecorated()
        {
            Console.WriteLine("bar");
        }

        public void Decorated()
        {
            //var currentMethod = MethodBase.GetCurrentMethod();

            this.foo.InterceptTest(this.Undecorated);

            // this.foo.InterceptNoReturnValue(null, new object[0], parameters => this.Undecorated());
        }

        public void Intermediary()
        {
            this.Undecorated();
        }
    }

    public class FooInterceptor
    {
        public object Intercept(MethodBase method, object[] parameters, Func<object[], object> implementation)
        {
            // add interception logic
            return implementation(null);
        }

        public void InterceptNoReturnValue(MethodBase method, object[] parameters, Action<object[]> implementation)
        {
            this.Intercept(
                method,
                parameters,
                p =>
                    {
                        implementation(p);
                        return null;
                    });
        }

        public void InterceptTest(Action action)
        {
            action();
        }
    }
}