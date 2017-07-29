namespace StaticProxy.Interceptor.IntegrationTests
{
    using System;

    public class DemoClassProxy
    {
        public int DecoratedMethodReturningValueType(int i)
        {
            throw new NotSupportedException("should never be called in test scenario");
        }

        public int ImplementedMethodReturningValueType(int i)
        {
            return i*2;
        }

        public IDisposable DecoratedMethodReturningReferenceType(IDisposable disposable)
        {
            throw new NotSupportedException("should never be called in test scenario");
        }

        public IDisposable ImplementedMethodReturningReferenceType(IDisposable disposable)
        {
            return disposable;
        }

        public Y DecoratedGenericMethod<X,Y>(X x, Y y)
        {
            throw new NotSupportedException("should never be called in test scenario");
        }

        public Y ImplementedGenericMethod<X,Y>(X x, Y y)
        {
            return y;
        }
    }
}
