namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using System.Reflection;

    using Moq;

    public static class DynamicInterceptorManagerMockExtensions
    {
        public static void VerifyImplementedMethodCallIntercepted(this Mock<IDynamicInterceptorManager> interceptorManager, string originalMethodName, params object[] expectedArguments)
        {
            interceptorManager.Verify(x => x.Intercept(
                It.Is<MethodBase>(m => m.Name == originalMethodName && m.GetParameters().Length == expectedArguments.Length),
                null,
                expectedArguments));
        }
    }
}