namespace IntegrationTests.ProxyWithTarget
{
    using System;

    [StaticProxy]
    public class IntegrationWithGenericClass<X, Y, Z>
        where X : class
        where Y : new()
        where Z : IDisposable
    {
        public Z ImplementedGenericMethod<X,Y,Z>(X arg1, Y arg2, Z arg3)
        {
            return arg3;
        }
    }
}