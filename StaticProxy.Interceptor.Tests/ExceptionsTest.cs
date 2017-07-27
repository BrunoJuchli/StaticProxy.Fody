namespace StaticProxy.Interceptor
{
    using System;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class ExceptionsTest
    {
        [Fact]
        public void EnsureDynamicInterceptorManagerNotNull_WhenIsNull_MustThrow()
        {
            this.Invoking(x => Exceptions.EnsureDynamicInterceptorManagerNotNull(null))
                .ShouldThrow<ArgumentNullException>()
                .Where(x => x.ParamName == typeof(IDynamicInterceptorManager).Name);
        }

        [Fact]
        public void EnsureDynamicInterceptorManagerNotNull_WhenIsNotNull_MustNotThrow()
        {
            var manager = Mock.Of<IDynamicInterceptorManager>();

            this.Invoking(x => Exceptions.EnsureDynamicInterceptorManagerNotNull(manager))
                .ShouldNotThrow();
        }
    }
}