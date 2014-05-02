namespace StaticProxy.Fody.Tests.ClassDecoration.MethodWeaving
{
    using Xunit;

    public class When_proxying_method_with_reference_arguments : MethodsTestBase
    {
        [Fact]
        public void Calling_method_must_use_interceptor_manager()
        {
            var arg1 = new object();
            var arg2 = new object();
            var expectedArguments = new[] { arg1, arg2 };

            this.Instance.ReferenceArguments(arg1, arg2);

            this.InterceptorManager.VerifyMethodCallIntercepted("ReferenceArguments", expectedArguments);
        }

        [Fact]
        public void CallingMethod_MustExecuteOriginalMethod()
        {
            var expectedArgument1 = new ToStringObject("Argument1");
            var expectedArgument2 = new ToStringObject("Argument2");

            this.Instance.ReferenceArguments(expectedArgument1, expectedArgument2);

            this.VerifyMethodCalled("ReferenceArguments", expectedArgument1, expectedArgument2);
        }
    }
}