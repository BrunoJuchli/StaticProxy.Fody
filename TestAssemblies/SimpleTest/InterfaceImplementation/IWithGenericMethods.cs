using System;

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
}
