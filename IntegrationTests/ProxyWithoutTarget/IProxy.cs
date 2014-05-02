namespace IntegrationTests.ProxyWithoutTarget
{
    [StaticProxy]
    internal interface IProxy
    {
        void NoArguments();

        void ValueArguments(int arg1, float arg2);

        void ReferenceArguments(object arg1, object arg2);

        void MixedArguments(int arg1, object arg2, string arg3);

        int ReturnsInteger();

        object ReturnsObject();

        float? ReturnsNullableFloat();
    }
}