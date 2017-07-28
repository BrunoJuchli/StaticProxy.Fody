namespace SimpleTest.InterfaceImplementation
{
    using System;

    [StaticProxy]
    internal interface IWithGenericMethods
    {
        void GenericMethod<X,Y,Z>();

        T ReturnsT<T>();

        void TakesT<T>(T t);

        T WithConstraint<T>(T t)
            where T : class;

        T WithConstraints<T>(T t)
            where T : class, IComparable, IDisposable;
    }
}
