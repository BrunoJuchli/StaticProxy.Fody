namespace StaticProxy.Fody.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class SimpleTestBase : IDisposable
    {
        protected readonly Assembly WovenSimpleTestAssembly;
        protected readonly IList<string> TestMessages;

        protected SimpleTestBase()
        {
            var weaverHelper = new WeaverHelper(@"SimpleTest\SimpleTest.csproj");
            this.WovenSimpleTestAssembly = weaverHelper.Weave();

            dynamic staticInstance = this.WovenSimpleTestAssembly.GetStaticInstance("SimpleTest.TestMessages");
            dynamic list = staticInstance.Messages;
            this.TestMessages = (IList<string>)list;
        }

        public void Dispose()
        {
            this.TestMessages.Clear();
        }
    }
}