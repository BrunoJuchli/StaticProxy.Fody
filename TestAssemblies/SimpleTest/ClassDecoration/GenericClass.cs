namespace SimpleTest.ClassDecoration
{
    using System;

    [StaticProxy]
    public class GenericClass<T1, T2, T3>
        where T1 : new()
        where T2 : class
        where T3 : Uri
    {
        public T3 DoSomething(T1 argument1)
        {
            var x = this.GetType().GetInterfaces()[0].TypeHandle;

            throw new NotImplementedException();
        }
    }
}
