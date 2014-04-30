namespace IntegrationTests.ProxyWithoutTarget
{
    [StaticProxy]
    internal interface IProxiedInterface
    {
        void Foo(int x, object y);

        int Bar();
    }
}