namespace IntegrationTests.ProxyWithoutTarget
{
    [StaticProxy]
    internal interface ISomeProxiedInterface
    {
        void Foo(int x, object y);

        int Bar();
    }
}