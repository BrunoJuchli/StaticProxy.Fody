// ReSharper disable InconsistentNaming

namespace SimpleTest.Integration
{
    using System;
    using System.Linq;
    using System.Reflection;

    using FluentAssertions;

    using Moq;

    using Ninject;

    using StaticProxyInterceptor.Fody;

    public class IntegrationTests
    {
        public IntegrationTests()
        {
            MethodInfo[] methods = this.GetType()
                .GetMethods()
                .Where(x => x.ReturnType == typeof(void))
                .ToArray();
            
            methods.AsParallel()
                .ForAll(
                    x =>
                        {
                            try
                            {
                                x.Invoke(this, new object[0]);
                            }
                            catch (TargetInvocationException ex)
                            {
                                throw ex.InnerException.PreserveStackTrace();
                            }
                        });
        }

        public void ConstructorArguments_Void_ValueTypeParameters_NoInterceptor()
        {
            const int DependencyNumber = 5;

            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();
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

        public void ConstructorArguments_Void_ValueTypeParameters_OneInterceptor()
        {
            var fakeInterceptor = new Mock<IDynamicInterceptor>();
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();
                kernel.Bind<IDependency>().ToConstant(Mock.Of<IDependency>());
                kernel.Bind<IDynamicInterceptor>().ToConstant(fakeInterceptor.Object);

                var instance = kernel.Get<IntegrationWithConstructorArgument>();

                instance.MultiplyInternalNumber(555);

                fakeInterceptor.Verify(x => x.Intercept(It.Is<IInvocation>(i => i.Method.Name == "MultiplyInternalNumber")));
                fakeInterceptor.Verify(x => x.Intercept(It.Is<IInvocation>(i => i.Arguments.Length == 1)));
                fakeInterceptor.Verify(x => x.Intercept(It.Is<IInvocation>(i => (int)i.Arguments[0] == 555)));
                fakeInterceptor.Verify(x => x.Intercept(It.Is<IInvocation>(i => i.ReturnValue == null)));
            }
        }

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
                kernel.Bind<IDynamicInterceptor>().ToConstant(fakeInterceptor.Object);

                var instance = kernel.Get<IntegrationWithConstructorArgument>();

                instance.InitializeInternalNumberFromDependency();

                // interceptor overrides 3 with 8
                instance.MultiplyInternalNumber(3);
                
                instance.AssertInternalNumberIs(40);
            }
        }

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
                kernel.Bind<IDynamicInterceptor>().ToConstant(fakeInterceptor.Object);

                var instance = kernel.Get<IntegrationWithoutConstructorArgument>();

                instance.DoSomething(null, null);

                instance.AssertDoSomethingArgumentsWere(ExpectedArg1, expectedArg2);
            }
        }

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
                kernel.Bind<IDynamicInterceptor>().ToConstant(fakeInterceptor.Object);

                var instance = kernel.Get<IntegrationThrowException>();

                instance.ThrowException("hello");
            }
        }

        public void ImplementationRethrowsException_MustThrow()
        {
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();

                var instance = kernel.Get<IntegrationThrowException>();

                instance.Invoking(x => x.RethrowException())
                    .ShouldThrow<Exception>()
                    .WithInnerException<NullReferenceException>();
            }
        }

        public void MethodWithReferenceTypeReturnValue_NoInterceptor_MustReturnOriginalMethodReturnValue()
        {
            const string ExpectedString = "HelloWorld";
            var argument1 = new OverrideToString("Hello");
            var argument2 = new OverrideToString("World");

            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();

                var instance = kernel.Get<IntegrationWithReturnValue>();

                instance.CombineToStrings(argument1, argument2)
                    .Should().Be(ExpectedString);
            }
        }

        public void MethodWithReferenTypeReturnValue_InterceptorMustBeAbleToChangeReturnValue()
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
                kernel.Bind<IDynamicInterceptor>().ToConstant(fakeInterceptor.Object);

                var instance = kernel.Get<IntegrationWithReturnValue>();

                instance.CombineToStrings(argument1, argument2)
                    .Should().Be(ExpectedString);
            }
        }

        public void MethodWithValueTypeReturnValue_NoInterceptor_MustReturnOriginalMethodReturnValue()
        {
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<IDynamicInterceptorManager>().To<DynamicInterceptorManager>();

                var instance = kernel.Get<IntegrationWithReturnValue>();

                instance.Multiply(3, 5).Should().Be(15);
            }
        }

        public void NotProxiedType()
        {
            using (IKernel kernel = new StandardKernel())
            {
                // this would throw an ActivationException in case it would be proxied
                kernel.Get<NotProxiedType>();
            }
        }
    }
}