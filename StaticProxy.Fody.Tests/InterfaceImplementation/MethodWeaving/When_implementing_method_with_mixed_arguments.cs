namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using Xunit;

    public class When_implementing_method_with_mixed_arguments : InterfaceWithMethodsTestBase
    {
        [Fact]
        public void Calling_method_must_use_interceptor_manager()
        {
            const int Arg1 = 5;
            var arg2 = new object();
            const string Arg3 = "blub";

            this.Instance.MixedArguments(Arg1, arg2, Arg3);

            this.InterceptorManager.VerifyImplementedMethodCallIntercepted("MixedArguments", Arg1, arg2, Arg3);
        }
    }
}