namespace IntegrationTests.NotProxiedType
{
    using Ninject;

    using Xunit;

    public class NotProxiedTypeTest
    {
        [Fact]
        public void NotProxiedType()
        {
            using (IKernel kernel = new StandardKernel())
            {
                // this would throw an ActivationException in case it would be proxied
                kernel.Get<NotProxiedType>();
            }
        } 
    }
}