namespace StaticProxy.Interceptor.Tests
{
    using System;
    using System.Reflection;

    using FluentAssertions;

    using Moq;

    using StaticProxy.Interceptor;
    using StaticProxy.Interceptor.TargetInvocation;

    using Xunit;

    public class InvocationTest_GenericMethod
    {
        private const string OriginalArgument1 = "OriginalArgument1";
        private const int OriginalArgument2 = 492;

        private readonly Mock<ITargetInvocation> targetInvocation;
        private readonly MethodInfo decoratedMethod;
        private readonly object[] arguments = new object[] { OriginalArgument1, OriginalArgument2 };
        private readonly Mock<IDynamicInterceptor> interceptor1;
        private readonly Mock<IDynamicInterceptor> interceptor2;

        private readonly Invocation testee;

        public InvocationTest_GenericMethod()
        {
            this.targetInvocation = new Mock<ITargetInvocation>();
            this.decoratedMethod = typeof(IFakeTarget).GetMethod("Foo").MakeGenericMethod(typeof(string));
            this.interceptor1 = new Mock<IDynamicInterceptor>();
            this.interceptor2 = new Mock<IDynamicInterceptor>();

            this.testee = new Invocation(
                this.targetInvocation.Object,
                this.decoratedMethod,
                this.arguments, 
                new[] { this.interceptor1.Object, this.interceptor2.Object });
        }

        [Fact]
        public void Ctor_MustSetGenericArguments()
        {
            this.testee.GenericArguments.Should()
                .HaveCount(1)
                .And.Contain(typeof(string));
        }

        public interface IFakeTarget
        {
            Tuple<T, int> Foo<T>(T t, int i);
        }
    }
}