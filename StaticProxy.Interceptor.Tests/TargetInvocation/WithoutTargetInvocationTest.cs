namespace StaticProxy.Interceptor.TargetInvocation
{
    using FluentAssertions;
    using Xunit;

    public class WithoutTargetInvocationTest
    {
        private readonly WithoutTargetInvocation testee;

        public WithoutTargetInvocationTest()
        {
            this.testee = new WithoutTargetInvocation();
        }

        [Fact]
        public void InvokeMethodOnTarget_MustReturnNull()
        {
            object result = this.testee.InvokeMethodOnTarget(null);

            result.Should().BeNull();
        }
    }
}