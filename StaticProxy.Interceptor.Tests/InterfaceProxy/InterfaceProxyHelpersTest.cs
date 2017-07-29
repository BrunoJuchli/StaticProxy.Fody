namespace StaticProxy.Interceptor.InterfaceProxy
{
    using System;

    using FluentAssertions;

    using Xunit;
    using Xunit.Extensions;

    public class InterfaceProxyHelpersTest
    {
        [Fact]
        public void GetImplementationTypeOfInterface_MustReturnImplementationType()
        {
            InterfaceProxyHelpers.GetImplementationTypeOfInterface(typeof(IWithImplementation))
                .Should().Be(typeof(IWithImplementationImplementation));
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(int))]
        public void GetImplementationTypeOfInterface_WhenTypeIsNotInterface_MustThrow(Type interfaceType)
        {
            this.Invoking(x => InterfaceProxyHelpers.GetImplementationTypeOfInterface(interfaceType))
                .ShouldThrow<ArgumentOutOfRangeException>()
                .Where(ex => ex.ParamName == "interfaceType");
        }

        [Fact]
        public void GetImplementationTypeOfInterface_WhenThereIsNoImplementation_MustThrow()
        {
            this.Invoking(x => InterfaceProxyHelpers.GetImplementationTypeOfInterface(typeof(IWithoutImplementation)))
                .ShouldThrow<InvalidOperationException>()
                .Where(ex => ex.Message.Contains("There is no auto-generated implementation"));
        }

        public interface IWithImplementation { }

        public class IWithImplementationImplementation { }

        public interface IWithoutImplementation { }
    }
}