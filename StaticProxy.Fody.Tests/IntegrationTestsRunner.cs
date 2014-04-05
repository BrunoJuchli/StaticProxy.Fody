namespace StaticProxy.Fody.Tests
{
    using System;
    using Xunit;

    public class IntegrationTestsRunner : SimpleTestBase
    {
        protected readonly Type Clazz;

        public IntegrationTestsRunner()
        {
            this.Clazz = this.WovenSimpleTestAssembly.GetType("SimpleTest.Integration.IntegrationTests");
        }

        [Fact]
        public void Run_Integration_Tests()
        {
            // the constructor of the type is executing the tests.
            Activator.CreateInstance(this.Clazz);
        }
    }
}