namespace SimpleTest.InterfaceImplementation
{
    using System;
    using System.Reflection;

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

    // todo remove this, Only created to have a look at the generated IL
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

        public void Demo2<X,Y,Z>()
        {
            var method = this.GetType().GetMethod("Demo");
            Type[] genericArguments = new Type[3];
            genericArguments[0] = typeof(X);
            genericArguments[1] = typeof(Y);
            genericArguments[2] = typeof(Z);
            FooBar(method, genericArguments);

            Type[] ldargS = new Type[128];

            Type[] ldarg = new Type[12483];
        }

        public void GenericMethod<X,Y,Z>()
        {
            throw new NotImplementedException();
        }

        public void FooBar(MethodInfo m, Type[] genericArguments)
        {

        }
    }
}
