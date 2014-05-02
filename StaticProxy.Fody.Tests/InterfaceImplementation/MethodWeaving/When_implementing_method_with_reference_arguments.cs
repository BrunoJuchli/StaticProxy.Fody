namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using Xunit;

    public class When_implementing_method_with_reference_arguments : InterfaceWithMethodsTestBase
    {
        [Fact]
        public void Calling_method_must_use_interceptor_manager()
        {
            var arg1 = new object();
            var arg2 = new object();
            
            this.Instance.ReferenceArguments(arg1, arg2);

            this.InterceptorManager.VerifyImplementedMethodCallIntercepted("ReferenceArguments", arg1, arg2);
        }
    }
}