namespace StaticProxy.Fody.Tests.ClassDecoration.ConstructorWeaving
{
    using System;
    using System.Linq;
    using System.Reflection;

    using FluentAssertions;

    using Moq;

    using Xunit;

    public class When_proxying_class_without_constructor_arguments : SimpleTestBase
    {
        private readonly Type clazz;

        public When_proxying_class_without_constructor_arguments()
        {
            this.clazz = this.WovenSimpleTestAssembly.GetType("SimpleTest.ConstructorWithoutArguments");
        }

        [Fact]
        public void It_should_not_add_a_constructor()
        {
            this.clazz
                .GetConstructors().Should().HaveCount(1);
        }

        [Fact]
        public void It_should_add_dynamic_interceptor_manager_to_constructor()
        {
            var parameters = this.clazz
                .GetConstructors().Single().GetParameters();

            parameters.Should()
                .HaveCount(1)
                .And.Contain(x => x.ParameterType == typeof(IDynamicInterceptorManager));
        }

        [Fact]
        public void Ctor_WhenDynamicInterceptorManagerIsNull_MustThrowArgumentException()
        {
            this.Invoking(x => Activator.CreateInstance(this.clazz, (IDynamicInterceptorManager)null))
                .ShouldThrow<TargetInvocationException>()
                .WithInnerException<ArgumentNullException>()
                .Where(x => ((ArgumentNullException)x.InnerException).ParamName == typeof(IDynamicInterceptorManager).Name);
        }

        [Fact]
        public void Ctor_should_initialize_manager()
        {
            var interceptorManager = new Mock<IDynamicInterceptorManager>();

            object instance = Activator.CreateInstance(this.clazz, interceptorManager.Object);

            interceptorManager.Verify(x => x.Initialize(instance));
        }
    }
}