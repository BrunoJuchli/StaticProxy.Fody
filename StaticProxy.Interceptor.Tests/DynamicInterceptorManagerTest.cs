namespace StaticProxy.Interceptor
{
    using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using FluentAssertions;
    using Moq;
    using StaticProxy.Interceptor.InterceptedMethod;
    using StaticProxy.Interceptor.Reflection;
    using Xunit;

    public class DynamicInterceptorManagerTest
    {
        private readonly DynamicInterceptorCollection interceptorCollection;
        private readonly Mock<IInterceptedMethodFactory> interceptedMethodFactory;
        private readonly Mock<IInvocationFactory> invocationFactory;
        private readonly Mock<ITypeInformation> typeInformation;

        private readonly DynamicInterceptorManager testee;

        public DynamicInterceptorManagerTest()
        {
            this.interceptorCollection = new DynamicInterceptorCollection();
            this.interceptedMethodFactory = new Mock<IInterceptedMethodFactory> { DefaultValue = DefaultValue.Mock };
            this.invocationFactory = new Mock<IInvocationFactory> { DefaultValue = DefaultValue.Mock };
            this.typeInformation = new Mock<ITypeInformation>();

            this.testee = new DynamicInterceptorManager(
                this.interceptorCollection,
                this.interceptedMethodFactory.Object,
                this.invocationFactory.Object,
                this.typeInformation.Object);
        }

        [Fact]
        public void Initialize_WhenTargetIsNull_MustThrow()
        {
            this.testee.Invoking(x => x.Initialize(null, false))
                .ShouldThrow<ArgumentNullException>()
                .Where(ex => ex.ParamName == "implementationMethodTarget");
        }

        [Fact]
        public void Initialize_WhenTargetIsNotNull_MustNotThrow()
        {
            this.testee.Invoking(x => x.Initialize(new object(), false))
                .ShouldNotThrow();
        }

        [Fact]
        public void Initialize_WhenInterceptorsNotRequiredAndThereIsNoInterceptor_MustNotThrow()
        {
            this.testee.Invoking(x => x.Initialize(new object(), false))
                .ShouldNotThrow();
        }

        [Fact]
        public void Initialize_WhenInterceptorsNotRequiredAndThereIsOneInterceptor_MustNotThrow()
        {
            this.interceptorCollection.Add(Mock.Of<IDynamicInterceptor>());

            this.testee.Invoking(x => x.Initialize(new object(), false))
                .ShouldNotThrow();
        }

        [Fact]
        public void Initialize_WhenInterceptorsRequiredButThereIsNoInterceptor_MustThrow()
        {
            this.testee.Invoking(x => x.Initialize(new object(), true))
                .ShouldThrow<InvalidOperationException>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void Initialize_WhenInterceptorsRequiredAndThereIsOneInterceptor_MustNotThrow(int numberOfInterceptors)
        {
            for(int index=0;index<numberOfInterceptors;index++)
            {
                this.interceptorCollection.Add(Mock.Of<IDynamicInterceptor>());
            }

            this.testee.Invoking(x => x.Initialize(new object(), true))
                .ShouldNotThrow();
        }

        [Fact]
        public void Intercept_WhenNotInitialized_MustThrow()
        {
            this.testee.Invoking(x => x.Intercept(null, null, null, null))
                .ShouldThrow<InvalidOperationException>();
        }

        [Theory]
        [ClassData(typeof(InterceptTestCaseSource))]
        public void Intercept_ShouldCreateInterceptedMethod(InterceptExample example)
        {
            var expectedTarget = new object();
            this.testee.Initialize(expectedTarget, false);

            this.testee.Intercept(example.DecoratedMethod, example.ImplementationMethod, example.GenericArguments, example.Arguments);

            this.interceptedMethodFactory.Verify(x => x.Create(expectedTarget, example.DecoratedMethod, example.ImplementationMethod, example.GenericArguments));
        }


        [Fact]
        public void Intercept_ShouldCreateInvocation()
        {
            var interceptor1 = Mock.Of<IDynamicInterceptor>();
            var interceptor2 = Mock.Of<IDynamicInterceptor>();
            this.interceptorCollection.Add(interceptor1);
            this.interceptorCollection.Add(interceptor2);
            var expectedInterceptors = new[] { interceptor1, interceptor2 };

            var expectedInterceptedMethod = Mock.Of<IInterceptedMethod>();
            this.interceptedMethodFactory.Setup(x => x.Create(It.IsAny<object>(), It.IsAny<MethodInfo>(), It.IsAny<MethodInfo>(), It.IsAny<Type[]>()))
                .Returns(expectedInterceptedMethod);

            var expectedDecoratedMethod = new DynamicMethod("decorated", typeof(object), new Type[0]);
            var expectedGenericArguments = new[] { typeof(int), typeof(IDictionary) };
            var expectedArguments = new[] { new object(), new object() };

            this.testee.Initialize(new object(), false);

            this.testee.Intercept(expectedDecoratedMethod, null, expectedGenericArguments, expectedArguments);

            this.invocationFactory.Verify(x =>
                x.Create(expectedInterceptedMethod, expectedArguments, It.Is<IDynamicInterceptor[]>(i => i.SequenceEqual(expectedInterceptors))));
        }

        [Fact]
        public void Intercept_ShouldProceedCreatedInvocation()
        {
            var invocation = new Mock<IInvocation> { DefaultValue = DefaultValue.Mock };
            this.SetupInvocationFactory(invocation.Object);
            this.testee.Initialize(new object(), false);

            this.testee.Intercept(null, null, null, null);

            invocation.Verify(x => x.Proceed());
        }

        [Fact]
        public void Intercept_ShouldReturnReturnValueOfInvocation()
        {
            var expectedReturnValue = new object();
            var invocation = new Mock<IInvocation>();
            this.SetupInvocationFactory(invocation.Object);
            this.testee.Initialize(new object(), false);

            invocation.Setup(x => x.Proceed())
                .Callback(() => invocation.SetupGet(x => x.ReturnValue).Returns(expectedReturnValue));

            object actualReturnValue = this.testee.Intercept(null, null, null, null);

            actualReturnValue.Should().Be(expectedReturnValue);
        }

        [Fact]
        public void Intercept_WhenReturnValueIsNull_MustDetermineWhetherReturnTypeIsNullable()
        {
            Type expectedReturnType = typeof(int);
            var decoratedMethod = new DynamicMethod("anyName", expectedReturnType, new Type[0]);
            this.interceptedMethodFactory
                .Setup(x => x.Create(It.IsAny<object>(), It.IsAny<MethodInfo>(), It.IsAny<MethodInfo>(), It.IsAny<Type[]>()))
                .Returns(Mock.Of<IInterceptedMethod>(im => im.DecoratedMethod == decoratedMethod));

            this.SetupInvocationFactory(Mock.Of<IInvocation>(x => x.ReturnValue == null));
            
            this.typeInformation.Setup(x => x.IsNullable(It.IsAny<Type>())).Returns(true);
            this.testee.Initialize(new object(), false);

            this.testee.Intercept(decoratedMethod, null, null, null);

            this.typeInformation.Verify(x => x.IsNullable(expectedReturnType));
        }

        [Fact]
        public void Intercept_WhenReturnTypeIsNotNullableAndReturnValueIsNull_MustThrow()
        {
            this.SetupInvocationFactory(Mock.Of<IInvocation>(x => x.ReturnValue == null));
            this.typeInformation.Setup(x => x.IsNullable(It.IsAny<Type>())).Returns(false);
            this.testee.Initialize(new object(), false);

            this.testee.Invoking(x => x.Intercept(null, null, null, null))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void Intercept_WhenReturnTypeIsNullableTypeAndReturnValueIsNull_MustNotThrow()
        {
            this.SetupInvocationFactory(Mock.Of<IInvocation>(x => x.ReturnValue == null));
            this.testee.Initialize(new object(), false);
            var decoratedMethod = new DynamicMethod("anyName", null, new Type[0]);
            this.typeInformation.Setup(x => x.IsNullable(It.IsAny<Type>())).Returns(true);

            this.testee.Invoking(x => x.Intercept(decoratedMethod, null, null, null))
                .ShouldNotThrow();
        }

        private void SetupInvocationFactory(IInvocation invocation)
        {
            this.invocationFactory
                .Setup(x => x.Create(It.IsAny<IInterceptedMethod>(), It.IsAny<object[]>(), It.IsAny<IDynamicInterceptor[]>()))
                .Returns(invocation);
        }

        private class InterceptTestCaseSource : IEnumerable<object[]>
        {
            private static readonly InterceptExample NonGeneric = new InterceptExample(
                    new DynamicMethod("decorated", typeof(object), new Type[0]),
                    new DynamicMethod("implementation", typeof(object), new Type[0]),
                    new Type[0],
                    new[] { new object(), new object() });

            private static readonly InterceptExample Generic = new InterceptExample(
                    new DynamicMethod("decoratedGeneric", typeof(object), new Type[0]),
                    new DynamicMethod("implementationGeneric", typeof(object), new Type[0]),
                    new[] { typeof(string), typeof(IDisposable), typeof(IComparable) },
                    new[] { new object(), new object() });

            public IEnumerator<object[]> GetEnumerator()
            {
                return this.GetTestCases()
                    .Select(x => new object[] { x })
                    .GetEnumerator();
            }

            private IEnumerable<InterceptExample> GetTestCases()
            {
                yield return NonGeneric;
                yield return Generic;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        public class InterceptExample
        {
            public MethodInfo DecoratedMethod { get; }

            public MethodInfo ImplementationMethod { get; }

            public Type[] GenericArguments { get; }

            public Object[] Arguments { get; }

            internal InterceptExample(MethodInfo decoratedMethod, MethodInfo implementationMethod, Type[] genericArguments, object[] arguments)
            {
                this.DecoratedMethod = decoratedMethod;
                this.ImplementationMethod = implementationMethod;
                this.GenericArguments = genericArguments;
                this.Arguments = arguments;
            }
        }
    }
}