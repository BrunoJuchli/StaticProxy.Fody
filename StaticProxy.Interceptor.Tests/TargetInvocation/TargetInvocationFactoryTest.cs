namespace StaticProxy.Interceptor.Tests.TargetInvocation
{
    using System;

    using FluentAssertions;

    using Moq;

    using StaticProxy.Interceptor.TargetInvocation;

    using Xunit;

    public class TargetInvocationFactoryTest
    {
        private readonly TargetInvocationFactory testee;

        public TargetInvocationFactoryTest()
        {
            this.testee = new TargetInvocationFactory();
        }

        [Fact]
        public void Create_WhenImplementationMethodIsNull_MustReturnWithoutTargetInvocation()
        {
            ITargetInvocation targetInvocation = this.testee.Create(null, null);

            targetInvocation.Should().BeOfType<WithoutTargetInvocation>();
        }

        [Fact]
        public void Create_WhenImplementationMethodNotIsNull_MustReturnWithTargetInvocation()
        {
            var target = new Mock<IFakeTarget>();

            ITargetInvocation targetInvocation = this.testee.Create(target.Object, typeof(IFakeTarget).GetMethod("Foo"));

            targetInvocation.Should().BeOfType<WithTargetInvocation>();

            targetInvocation.InvokeMethodOnTarget(new object[0]);
            target.Verify(x => x.Foo());
        }

        public interface IFakeTarget
        {
            void Foo();
        }
    }
}