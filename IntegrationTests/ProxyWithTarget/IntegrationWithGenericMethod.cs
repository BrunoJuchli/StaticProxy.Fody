namespace IntegrationTests.ProxyWithTarget
{
    using FluentAssertions;

    [StaticProxy]
    public class IntegrationWithGenericMethod   
    {
        public Z ImplementedGenericMethod<X,Y,Z>(X arg1, Y arg2, Z arg3)
        {
            return arg3;
        }
    }
}