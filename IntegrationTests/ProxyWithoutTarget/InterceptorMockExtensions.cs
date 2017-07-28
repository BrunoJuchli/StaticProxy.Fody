namespace IntegrationTests.ProxyWithoutTarget
{
    using Moq;

    internal static class InterceptorMockExtensions
    {
        public static void SetupReturnValue(this Mock<IDynamicInterceptor> interceptor, object returnValue)
        {
            interceptor.Setup(x => x.Intercept(It.IsAny<IInvocation>()))
                .Callback<IInvocation>(invocation => invocation.ReturnValue = returnValue);
        }
    }
}
