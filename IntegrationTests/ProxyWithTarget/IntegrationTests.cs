// ReSharper disable InconsistentNaming

namespace IntegrationTests.ProxyWithTarget
{
    using System;

    using FluentAssertions;
    using Moq;
    using Ninject;
    using Xunit;

    public class IntegrationTests
    {
        [Fact]
        public void ConstructorArguments_Void_ValueTypeParameters_NoInterceptor()
        {
            const int DependencyNumber = 5;

            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();
                kernel.Bind<IDynamicInterceptorCollection>().ToConstant(new FakeDynamicInterceptorCollection());
                kernel.Bind<IDependency>().To<Dependency>()
                    .WithConstructorArgument("number", DependencyNumber);

                var instance = kernel.Get<IntegrationWithConstructorArgument>();

                instance.AssertInternalNumberIs(0);

                instance.InitializeInternalNumberFromDependency();
                instance.AssertInternalNumberIs(5);

                instance.MultiplyInternalNumberByThree();
                instance.AssertInternalNumberIs(15);

                instance.MultiplyInternalNumber(4);
                instance.AssertInternalNumberIs(60);
            }
        }

        [Fact]
        public void ConstructorArguments_Void_ValueTypeParameters_OneInterceptor()
        {
            var fakeInterceptor = new Mock<IDynamicInterceptor>();
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();
                kernel.Bind<IDependency>().ToConstant(Mock.Of<IDependency>());
                kernel.Bind<IDynamicInterceptorCollection>()
                    .ToConstant(new FakeDynamicInterceptorCollection(fakeInterceptor.Object));

                var instance = kernel.Get<IntegrationWithConstructorArgument>();

                instance.MultiplyInternalNumber(555);

                fakeInterceptor.Verify(x => x.Intercept(It.Is<IInvocation>(i => i.Method.Name == "MultiplyInternalNumber")));
                fakeInterceptor.Verify(x => x.Intercept(It.Is<IInvocation>(i => i.Arguments.Length == 1)));
                fakeInterceptor.Verify(x => x.Intercept(It.Is<IInvocation>(i => (int)i.Arguments[0] == 555)));
                fakeInterceptor.Verify(x => x.Intercept(It.Is<IInvocation>(i => i.ReturnValue == null)));
            }
        }

        [Fact]
        public void ConstructorArguments_Void_ValueTypeParameters_InterceptorReplacingArgument()
        {
            const int DependencyNumber = 5;
            var fakeInterceptor = new Mock<IDynamicInterceptor>();
            fakeInterceptor
                .Setup(x => x.Intercept(It.IsAny<IInvocation>()))
                .Callback<IInvocation>(
                    invocation =>
                        {
                            if (invocation.Method.Name == "MultiplyInternalNumber")
                            {
                                invocation.Arguments[0] = 8;
                            }

                            invocation.Proceed(); 
                        });

            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();
                kernel.Bind<IDependency>().To<Dependency>()
                    .WithConstructorArgument("number", DependencyNumber);
                kernel.Bind<IDynamicInterceptorCollection>()
                    .ToConstant(new FakeDynamicInterceptorCollection(fakeInterceptor.Object));

                var instance = kernel.Get<IntegrationWithConstructorArgument>();

                instance.InitializeInternalNumberFromDependency();

                // interceptor overrides 3 with 8
                instance.MultiplyInternalNumber(3);
                
                instance.AssertInternalNumberIs(40);
            }
        }

        [Fact]
        public void NoConstructorArguments_Void_ReferenceParameters_InterceptorReplacingArgument()
        {
            const string ExpectedArg1 = "Hello world";
            var expectedArg2 = new object();
            
            var fakeInterceptor = new Mock<IDynamicInterceptor>();
            fakeInterceptor
                .Setup(x => x.Intercept(It.IsAny<IInvocation>()))
                .Callback<IInvocation>(
                    invocation =>
                    {
                        if (invocation.Method.Name == "DoSomething")
                        {
                            invocation.Arguments[0] = ExpectedArg1;
                            invocation.Arguments[1] = expectedArg2;
                        }

                        invocation.Proceed();
                    });

            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();
                kernel.Bind<IDynamicInterceptorCollection>()
                    .ToConstant(new FakeDynamicInterceptorCollection(fakeInterceptor.Object));

                var instance = kernel.Get<IntegrationWithoutConstructorArgument>();

                instance.DoSomething(null, null);

                instance.AssertDoSomethingArgumentsWere(ExpectedArg1, expectedArg2);
            }
        }

        [Fact]
        public void ProceedThrowsException_InterceptorMustBeAbleToHandleException()
        {
            var fakeInterceptor = new Mock<IDynamicInterceptor>();
            fakeInterceptor
                .Setup(x => x.Intercept(It.IsAny<IInvocation>()))
                .Callback<IInvocation>(
                    invocation =>
                    {
                        try
                        {
                            invocation.Proceed();
                        }
                        catch (InvalidOperationException)
                        {   
                        }
                    });

            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();
                kernel.Bind<IDynamicInterceptorCollection>()
                    .ToConstant(new FakeDynamicInterceptorCollection(fakeInterceptor.Object));

                var instance = kernel.Get<IntegrationThrowException>();

                instance.ThrowException("hello");
            }
        }

        [Fact]
        public void MethodWithReferenceTypeReturnValue_NoInterceptor_MustReturnOriginalMethodReturnValue()
        {
            const string ExpectedString = "HelloWorld";
            var argument1 = new OverrideToString("Hello");
            var argument2 = new OverrideToString("World");

            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();
                kernel.Bind<IDynamicInterceptorCollection>().ToConstant(new FakeDynamicInterceptorCollection());

                var instance = kernel.Get<IntegrationWithReturnValue>();

                instance.CombineToStrings(argument1, argument2)
                    .Should().Be(ExpectedString);
            }
        }

        [Fact]
        public void MethodWithReferenceTypeReturnValue_InterceptorMustBeAbleToChangeReturnValue()
        {
            const string ExpectedString = "HelloWorldIntercepted";
            const string InterceptorAppends = "Intercepted";
            var argument1 = new OverrideToString("Hello");
            var argument2 = new OverrideToString("World");

            var fakeInterceptor = new Mock<IDynamicInterceptor>();
            fakeInterceptor
                .Setup(x => x.Intercept(It.IsAny<IInvocation>()))
                .Callback<IInvocation>(
                    invocation =>
                    {
                        invocation.Proceed();
                        invocation.ReturnValue = ((string)invocation.ReturnValue) + InterceptorAppends;
                    });

            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();
                kernel.Bind<IDynamicInterceptorCollection>().ToConstant(new FakeDynamicInterceptorCollection(fakeInterceptor.Object));

                var instance = kernel.Get<IntegrationWithReturnValue>();

                instance.CombineToStrings(argument1, argument2)
                    .Should().Be(ExpectedString);
            }
        }

        [Fact]
        public void MethodWithValueTypeReturnValue_NoInterceptor_MustReturnOriginalMethodReturnValue()
        {
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();
                kernel.Bind<IDynamicInterceptorCollection>().ToConstant(new FakeDynamicInterceptorCollection());

                var instance = kernel.Get<IntegrationWithReturnValue>();

                instance.Multiply(3, 5).Should().Be(15);
            }
        }
    }
}