namespace IntegrationTests.ProxyWithoutTarget
{
    using System;
    using System.Reflection;

    using FluentAssertions;

    using Moq;

    public static class DynamicInterceptorMockExtensions
    {
        public static void VerifyGenericIntercepted(this Mock<IDynamicInterceptor> interceptor, MethodInfo expectedMethod, params object[] expectedArguments)
        {
            Type[] expectedGenericArguments = expectedMethod.GetGenericArguments();
            interceptor.Verify(x => x.Intercept(It.Is<IInvocation>(i => i.GenericArguments.Length == expectedGenericArguments.Length)));
            for (int index = 0; index < expectedArguments.Length; index++)
            {
                interceptor.Verify(x => x.Intercept(It.Is<IInvocation>(i => i.GenericArguments[index] == expectedGenericArguments[index])));
            }

            interceptor.VerifyIntercepted(expectedMethod, expectedArguments);
        }

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