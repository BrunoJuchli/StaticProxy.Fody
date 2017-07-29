namespace StaticProxy.Interceptor.TargetInvocation
{
    using System;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class WithTargetInvocationTest
    {
        private readonly Mock<IFakeTarget> target;

        private readonly WithTargetInvocation testee;

        public WithTargetInvocationTest()
        {
            this.target = new Mock<IFakeTarget>();

            this.testee = new WithTargetInvocation(this.target.Object, typeof(IFakeTarget).GetMethod("Foo"));
        }

        [Fact]
        public void InvokeMethodOnTarget_MustInvokeMethodOnTarget()
        {
            const double ExpectedParameterX = 30.5;
            const int ExpectedParemterY = 38203820;

            this.testee.InvokeMethodOnTarget(new object[] { ExpectedParameterX, ExpectedParemterY });

            this.target.Verify(x => x.Foo(ExpectedParameterX, ExpectedParemterY));
        }

        [Fact]
        public void InvokeMethodOnTarget_MustReturnMethodResult()
        {
            const double ExpectedResult = 34028.32;
            this.target
                .Setup(x => x.Foo(It.IsAny<double>(), It.IsAny<int>()))
                .Returns(ExpectedResult);

            object actualResult = this.testee.InvokeMethodOnTarget(new object[] { 0.0, 1 });

            actualResult.Should().Be(ExpectedResult);
        }

        [Fact]
        public void InvokeMethodOnTarget_WhenTargetThrows_MustThrowUnwrappedException()
        {
            var expectedException = new Exception();
            this.target
                .Setup(x => x.Foo(It.IsAny<double>(), It.IsAny<int>()))
                .Throws(expectedException);

            this.testee
                .Invoking(x => x.InvokeMethodOnTarget(new object[] { 0.0, 1 }))
                .ShouldThrow<Exception>()
                .Where(ex => ex == expectedException);
        }

        public interface IFakeTarget
        {
            double Foo(double x, int y);
        }
    }
}