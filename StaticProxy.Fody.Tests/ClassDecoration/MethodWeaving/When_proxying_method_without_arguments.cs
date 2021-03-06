﻿namespace StaticProxy.Fody.Tests.ClassDecoration.MethodWeaving
{
    using Xunit;

    public class When_proxying_method_without_arguments : ClassWithMethodsTestBase
    {
        [Fact]
        public void CallingMethod_MustUseInterceptorManager()
        {
            this.Instance.NoArguments();

            this.InterceptorManager.VerifyDecoratedMethodCallIntercepted("NoArguments");
        }

        [Fact]
        public void CallingMethod_MustExecuteOriginalMethod()
        {
            this.Instance.NoArguments();

            this.VerifyMethodCalled("NoArguments");
        }
    }
}