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

    class WithGenericMethods : IWithGenericMethods
    {
        public T ReturnsT<T>()
        {
            throw new NotImplementedException();
        }

        public void TakesT<T>(T t)
        {
            throw new NotImplementedException(typeof(T).ToString());
        }

        public T WithConstraint<T>(T t) where T : class
        {
            throw new NotImplementedException();
        }

        T IWithGenericMethods.WithConstraints<T>(T t)
        {
            throw new NotImplementedException();
        }

        public void Demo<X,Y,Z>()
        {
            var method = this.GetType().GetMethod("Demo");
            var genericMethod = method.MakeGenericMethod(typeof(X), typeof(Y), typeof(Z));
            throw new NotImplementedException(genericMethod.Name);
        }

        public void GenericMethod<X,Y,Z>()
        {
            throw new NotImplementedException();
        }
    }
}
