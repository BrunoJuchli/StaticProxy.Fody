namespace StaticProxy.Interceptor.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using FluentAssertions;

    using Moq;

    using StaticProxy.Interceptor;
    using StaticProxy.Interceptor.TargetInvocation;

    using Xunit;

    public class InvocationTest
    {
        private const int OriginalArgument1 = 482;
        private static readonly object OriginalArgument2 = new object();

        private readonly Mock<ITargetInvocation> targetInvocation;
        private readonly MethodInfo decoratedMethod;
        private readonly object[] arguments = new object[] { OriginalArgument1, OriginalArgument2 };
        private readonly Mock<IDynamicInterceptor> interceptor1;
        private readonly Mock<IDynamicInterceptor> interceptor2;

        private readonly Invocation testee;

        public InvocationTest()
        {
            this.targetInvocation = new Mock<ITargetInvocation>();
            this.decoratedMethod = typeof(IFakeTarget).GetMethod("Foo");
            this.interceptor1 = new Mock<IDynamicInterceptor>();
            this.interceptor2 = new Mock<IDynamicInterceptor>();

            this.testee = new Invocation(
                this.targetInvocation.Object,
                this.decoratedMethod,
                this.arguments, 
                new[] { this.interceptor1.Object, this.interceptor2.Object });
        }

        [Fact]
        public void Ctor_MustSetReturnValueToNull()
        {
            this.testee.ReturnValue.Should().BeNull();
        }

        [Fact]
        public void SetReturnValue_MustSetReturnValue()
        {
            var expectedValue = new object();

            this.testee.ReturnValue = expectedValue;

            this.testee.ReturnValue.Should().Be(expectedValue);
        }

        [Fact]
        public void Method_MustReturnDecoratedMethod()
        {
            ((object)this.testee.Method).Should().Be(this.decoratedMethod);
        }

        [Fact]
        public void Arguments_MustReturnArguments()
        {
            ((object)this.testee.Arguments).Should().Be(this.arguments);
        }

        [Fact]
        public void GetArgumentValue_MustReturnArgumentValue()
        {
            this.testee.GetArgumentValue(1).Should().Be(OriginalArgument2);
        }

        [Fact]
        public void SetArgumentValue_MustSetArgumentValue()
        {
            var newValue = new object();

            this.testee.SetArgumentValue(1, newValue);

            this.arguments[1].Should().Be(newValue);
        }

        [Fact]
        public void SetArgumentValueToNull_WhenArgumentTypeIsValueType_MustThrow()
        {
            this.testee
                .Invoking(x => x.SetArgumentValue(0, null))
                .ShouldThrow<ArgumentNullException>()
                .Where(ex => ex.ParamName == "0");
        }

        [Fact]
        public void SetArgumentValueToNull_WhenArgumentTypeIsReferenceType_MustSetArgumentValue()
        {
            this.testee.SetArgumentValue(1, null);

            this.arguments[1].Should().BeNull();
        }

        [Fact]
        public void SetArgumentValue_WhenArgumentTypeIsIncompatible_MustThrow()
        {
            this.testee.Invoking(x => x.SetArgumentValue(0, "hello world"))
                .ShouldThrow<ArgumentOutOfRangeException>()
                .Where(ex => ex.ParamName == "0");
        }

        [Fact]
        public void Proceed()
        {
            var sequence = new List<object>();

            this.interceptor1
                .Setup(x => x.Intercept(It.IsAny<IInvocation>()))
                .Callback<IInvocation>(
                    x =>
                        {
                            sequence.Add(this.interceptor1);
                            x.Proceed();
                        });

            this.interceptor2
                .Setup(x => x.Intercept(It.IsAny<IInvocation>()))
                .Callback<IInvocation>(
                    x =>
                    {
                        sequence.Add(this.interceptor2);
                        x.Proceed();
                    });

            this.targetInvocation.Setup(x => x.InvokeMethodOnTarget(It.IsAny<object[]>()))
                .Callback(() => sequence.Add(this.targetInvocation));

            this.testee.Proceed();

            this.interceptor1.Verify(x => x.Intercept(this.testee));
            this.interceptor2.Verify(x => x.Intercept(this.testee));
            this.targetInvocation.Verify(x => x.InvokeMethodOnTarget(new[] { OriginalArgument1, OriginalArgument2 }));

            sequence.Should()
                .HaveCount(3)
                .And.ContainInOrder(this.interceptor1, this.interceptor2, this.targetInvocation);
        }

        [Fact]
        public void Proceed_When_OriginalImplementationThrows_MustRethrowOriginalException()
        {
            this.targetInvocation
                .Setup(x => x.InvokeMethodOnTarget(It.IsAny<object[]>()))
                .Throws<ArgumentOutOfRangeException>();

            this.interceptor1.Setup(x => x.Intercept(It.IsAny<IInvocation>())).Callback<IInvocation>(x => x.Proceed());
            this.interceptor2.Setup(x => x.Intercept(It.IsAny<IInvocation>())).Callback<IInvocation>(x => x.Proceed());

            this.testee.Invoking(x => x.Proceed())
                .ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Proceed_MustStoreReturnValueOfOriginalMethod()
        {
            var expectedReturnValue = new object();
            this.targetInvocation
                .Setup(x => x.InvokeMethodOnTarget(It.IsAny<object[]>()))
                .Returns(expectedReturnValue);

            this.interceptor1.Setup(x => x.Intercept(It.IsAny<IInvocation>())).Callback<IInvocation>(x => x.Proceed());
            this.interceptor2.Setup(x => x.Intercept(It.IsAny<IInvocation>())).Callback<IInvocation>(x => x.Proceed());

            this.testee.Proceed();

            this.testee.ReturnValue.Should().Be(expectedReturnValue);
        }

        public interface IFakeTarget
        {
            object Foo(int i, object o);
        }
    }
}