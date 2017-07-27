namespace StaticProxy.Interceptor.IntegrationTests
{
    using System;
    public interface IDemoInterfaceProxy
    {
        int DecoratedMethodReturningValueType(int i);

        IDisposable DecoratedMethodReturningReferenceType(IDisposable disposable);

        Y DecoratedGenericMethod<X, Y>(X x, Y y);
    }
}
