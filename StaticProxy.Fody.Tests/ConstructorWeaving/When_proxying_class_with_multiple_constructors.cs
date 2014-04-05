namespace StaticProxy.Fody.Tests.ConstructorWeaving
{
    using System.Globalization;

    using FluentAssertions;

    using Xunit;

    public class When_proxying_class_with_multiple_constructors
    {
        [Fact]
        public void It_should_throw_weaving_exception()
        {
            string expectedMessage = string.Format(CultureInfo.InvariantCulture, ExceptionMessages.MultipleConstructors, "FooNameSpace.MultipleConstructors");

            new WeaverHelper(@"MultipleConstructors\MultipleConstructors.csproj")
               .Invoking(x => x.Weave())
               .ShouldThrow<WeavingException>()
                .Where(ex => ex.Message == expectedMessage);
        } 
    }
}