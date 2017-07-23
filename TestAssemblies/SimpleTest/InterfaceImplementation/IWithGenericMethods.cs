using System;

namespace SimpleTest.InterfaceImplementation
{
    [StaticProxy]
    internal interface IWithGenericMethods
    {
        T ReturnsT<T>();

        void TakesT<T>(T t);

       //T WithConstraint<T>(T t)
       //    where T : class;

       // T WithConstraints<T>(T t)
       //     where T : class, IComparable, IDisposable;
    }

    public class WithGenericMethodsRemoveAfterILSpy : IWithGenericMethods
    {
        public T ReturnsT<T>()
        {
            throw new NotImplementedException();
        }

        public void TakesT<T>(T t)
        {
            throw new NotImplementedException();
        }

        public T WithConstraint<T>(T t) where T : class
        {
            throw new NotImplementedException();
        }

        public T WithConstraints<T>(T t)
        {
            throw new NotImplementedException();
        }
    }
}
