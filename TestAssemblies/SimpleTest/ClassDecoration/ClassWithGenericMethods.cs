namespace SimpleTest.ClassDecoration
{
    using System;

    [StaticProxy]
    public class ClassWithGenericMethods
    {
        public void GenericMethod<X, Y, Z>() { }

        public T ReturnsT<T>()
        {
            return default(T);
        }

        public void TakesT<T>(T t) { }

        public T WithConstraint<T>(T t)
            where T : class
        {
            return t;
        }

        public T WithConstraints<T>(T t)
            where T : class, IComparable, IDisposable
        {
            return t;
        }
    }
}
