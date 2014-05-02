namespace StaticProxy.Fody.Tests.ClassDecoration.MethodWeaving
{
    using System.Globalization;
    using System.Reflection;

    using Moq;

    public static class DynamicInterceptorManagerMockExtensions
    {
        public static void VerifyDecoratedMethodCallIntercepted(this Mock<IDynamicInterceptorManager> interceptorManager, string originalMethodName, params object[] expectedArguments)
        {
            string implementationMethodName = string.Format(CultureInfo.InvariantCulture, "{0}<SP>", originalMethodName);

            interceptorManager.Verify(x => x.Intercept(
                It.Is<MethodBase>(m => m.Name == originalMethodName && m.GetParameters().Length == expectedArguments.Length),
                It.Is<MethodBase>(m => m.Name == implementationMethodName && m.GetParameters().Length == expectedArguments.Length),
                expectedArguments));
        }
    }
}