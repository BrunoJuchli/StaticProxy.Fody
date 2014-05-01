namespace SimpleTest
{
    // todo add type constraints
    [StaticProxy]
    public interface IGenericProxy<T1, T2, T3>
    {
        void Foo(int x, object y);

        int Bar();
    }
}