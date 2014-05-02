namespace IntegrationTests.ProxyWithoutTarget
{
    using System.Reflection;

    using FluentAssertions;

    using Moq;

    public static class DynamicInterceptorMockExtensions
    {
        public static void VerifyIntercepted(this Mock<IDynamicInterceptor> interceptor, MethodInfo expectedMethod, params object[] expectedArguments)
        {
            interceptor.Verify(x => x.Intercept(It.Is<IInvocation>(i => ThrowIfInvocationDoesNotMatch(i, expectedMethod, expectedArguments))));
        }

        private static bool ThrowIfInvocationDoesNotMatch(IInvocation invocation, MethodInfo expectedMethod, params object[] expectedArguments)
        {
            invocation.Method.Should().BeSameAs(expectedMethod);

            invocation.Arguments.Should()
                .HaveSameCount(expectedArguments)
                .And.ContainInOrder(expectedArguments);

            return true;
        }
    }
}