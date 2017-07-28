using System;

namespace IntegrationTests.ProxyWithoutTarget.Generic
{
    [StaticProxy]
    interface IGenericProxy<T,Y>
        where T : IDisposable
        where Y : class
    {
        T Foo(Y parameter);
    }
}
