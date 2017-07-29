namespace StaticProxy.Fody.Tests.ClassDecoration.MethodWeaving
{
    using System.Globalization;
    using System.Reflection;

    using Moq;
    using System;

    public static class DynamicInterceptorManagerMockExtensions
    {
        public static void VerifyGenericDecoratedMethodCallIntercepted(
            this Mock<IDynamicInterceptorManager> interceptorManager, 
            string originalMethodName, 
            Type[] genericArguments,
            params object[] expectedArguments)
        {
            string implementationMethodName = ComputeImplementationMethodName(originalMethodName);

            interceptorManager.Verify(x => x.Intercept(
                It.Is<MethodBase>(m => m.Name == originalMethodName && m.GetParameters().Length == expectedArguments.Length),
                It.Is<MethodBase>(m => m.Name == implementationMethodName && m.GetParameters().Length == expectedArguments.Length),
                genericArguments,
                expectedArguments));
        }

        public static void VerifyDecoratedMethodCallIntercepted(this Mock<IDynamicInterceptorManager> interceptorManager, string originalMethodName, params object[] expectedArguments)
        {
            string implementationMethodName = ComputeImplementationMethodName(originalMethodName);

            interceptorManager.Verify(x => x.Intercept(
                It.Is<MethodBase>(m => m.Name == originalMethodName && m.GetParameters().Length == expectedArguments.Length),
                It.Is<MethodBase>(m => m.Name == implementationMethodName && m.GetParameters().Length == expectedArguments.Length),
                It.Is<Type[]>(genericArguments => genericArguments.Length == 0),
                expectedArguments));
        }

        private static string ComputeImplementationMethodName(string originalMethodName)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}<SP>", originalMethodName);
        }
    }
}