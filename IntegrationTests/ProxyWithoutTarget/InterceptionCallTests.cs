namespace IntegrationTests.ProxyWithoutTarget
{
    using Moq;

    using Ninject;

    using Xunit;

    public class InterceptionCallTests : ProxyWithoutTargetIntegrationTestBase
    {
        private readonly Mock<IDynamicInterceptor> interceptor;

        private readonly IProxy testee;

        public InterceptionCallTests()
        {
            this.interceptor = new Mock<IDynamicInterceptor>();
            this.BindInterceptorCollection(this.interceptor.Object);

            this.testee = this.Kernel.Get<IProxy>();
        }

        [Fact]
        public void WhenInterceptingMethodWithoutArguments_MustUseInterceptors()
        {
            this.testee.NoArguments();

            this.interceptor.VerifyIntercepted(Reflector<IProxy>.GetMethod(x => x.NoArguments()));
        }

        [Fact]
        public void WhenInterceptingMethodWithValueTypeArguments_MustUseInterceptors()
        {
            const int Argument1 = 4820;
            const float Argument2 = 340.4f;

            this.testee.ValueArguments(Argument1, Argument2);

            this.interceptor.VerifyIntercepted(
                Reflector<IProxy>.GetMethod(x => x.ValueArguments(0, 0.0f)),
                Argument1,
                Argument2);
        }

        [Fact]
        public void WhenInterceptingMethodWithReferenceTypeArguments_MustUseInterceptors()
        {
            var argument1 = new object();
            var argument2 = new object();

            this.testee.ReferenceArguments(argument1, argument2);

            this.interceptor.VerifyIntercepted(
                Reflector<IProxy>.GetMethod(x => x.ReferenceArguments(null, null)),
                argument1,
                argument2);
        }

        [Fact]
        public void WhenInterceptingMethodWithValueAndReferenceTypeArguments_MustUseInterceptors()
        {
            const int Argument1 = 438393;
            var argument2 = new object();
            const string Argument3 = "hello world";

            this.testee.MixedArguments(Argument1, argument2, Argument3);

            this.interceptor.VerifyIntercepted(
                Reflector<IProxy>.GetMethod(x => x.MixedArguments(0, null, null)),
                Argument1,
                argument2,
                Argument3);
        }
    }
}