namespace StaticProxy.Fody.Tests.ConstructorWeaving
{
    using System;
    using System.Linq;
    using System.Reflection;

    using FluentAssertions;

    using Moq;

    using Xunit;

    public class When_proxying_class_with_constructor_arguments : SimpleTestBase
    {
        private readonly Type clazz;

        public When_proxying_class_with_constructor_arguments()
        {
            this.clazz = this.WovenSimpleTestAssembly.GetType("SimpleTest.ConstructorWithArguments");
        }

        [Fact]
        public void It_should_not_add_a_constructor()
        {
            this.clazz
                .GetConstructors().Should().HaveCount(1);
        }

        [Fact]
        public void It_should_add_interceptor_retriever_to_constructor()
        {
            var parameters = this.clazz
                .GetConstructors().Single().GetParameters();

            parameters.Should()
                .HaveCount(3)
                .And.Contain(x => x.ParameterType == typeof(int))
                .And.Contain(x => x.ParameterType == typeof(object))
                .And.Contain(x => x.ParameterType == typeof(IDynamicInterceptorManager));
        }

        [Fact]
        public void Ctor_WhenDynamicInterceptorManagerIsNull_MustThrowArgumentException()
        {
            this.Invoking(x => 
                Activator.CreateInstance(
                    this.clazz,
                    0,
                    new object(),
                    (IDynamicInterceptorManager)null))
                .ShouldThrow<TargetInvocationException>()
                .WithInnerException<ArgumentNullException>()
                .Where(x => ((ArgumentNullException)x.InnerException).ParamName == typeof(IDynamicInterceptorManager).Name);
        }
        
        [Fact]
        public void Ctor_should_initialize_manager()
        {
            var interceptorManager = new Mock<IDynamicInterceptorManager>();

            var instance = (object)this.CreateInstance(interceptorManager);

            interceptorManager.Verify(x => x.Initialize(instance));
        }

        private dynamic CreateInstance(Mock<IDynamicInterceptorManager> interceptorManager)
        {
            return Activator.CreateInstance(this.clazz, 0, new object(), interceptorManager.Object);
        }
    }
}