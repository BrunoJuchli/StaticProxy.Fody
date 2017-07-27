namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using System.Reflection;

    using Moq;
    using System;

    public static class DynamicInterceptorManagerMockExtensions
    {
        public static void VerifyImplementedMethodCallIntercepted(this Mock<IDynamicInterceptorManager> interceptorManager, string originalMethodName, params object[] expectedArguments)
        {
            interceptorManager.Verify(x => x.Intercept(
                It.Is<MethodBase>(m => m.Name == originalMethodName && m.GetParameters().Length == expectedArguments.Length),
                null,
                It.Is<Type[]>(genericArguments => genericArguments.Length == 0),
                expectedArguments));
        }

        public static void VerifyImplementedGenericMethodCallIntercepted(
            this Mock<IDynamicInterceptorManager> interceptorManager,
            string originalMethodName,
            Type[] genericArguments,
            params object[] expectedArguments)
        {
            interceptorManager.Verify(x => x.Intercept(
                It.Is<MethodBase>(m => m.Name == originalMethodName && m.IsGenericMethod && m.GetParameters().Length == expectedArguments.Length),
                null,
                genericArguments,
                expectedArguments));
        }
    }
}