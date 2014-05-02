namespace StaticProxy.Fody.Tests.ClassDecoration.MethodWeaving
{
    using Xunit;

    public class When_proxying_method_with_mixed_arguments : ClassWithMethodsTestBase
    {
        [Fact]
        public void Calling_method_must_use_interceptor_manager()
        {
            const int Arg1 = 5;
            var arg2 = new object();
            const string Arg3 = "blub";

            var expectedArguments = new object[] { Arg1, arg2, Arg3 };

            this.Instance.MixedArguments(Arg1, arg2, Arg3);

            this.InterceptorManager.VerifyMethodCallIntercepted("MixedArguments", expectedArguments);
        }

        [Fact]
        public void CallingMethod_MustExecuteOriginalMethod()
        {
            const int Argument1 = 38203;
            object argument2 = new ToStringObject("AnyToString");
            const string Argument3 = "AnyString";

            this.Instance.MixedArguments(Argument1, argument2, Argument3);

            this.VerifyMethodCalled("MixedArguments", Argument1, argument2, Argument3);
        }
    }
}