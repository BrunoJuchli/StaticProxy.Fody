namespace StaticProxy.Fody.Tests
{
    using System.Reflection;

    public class DecoratedSimpleTest
    {
        public DecoratedSimpleTest()
        {
            var weaverHelper = new WeaverHelper(@"SimpleTest\SimpleTest.csproj");
            this.Assembly = weaverHelper.Weave();
        }

        public Assembly Assembly { get; private set; }
    }
}