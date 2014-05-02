namespace StaticProxy.Fody.Tests.InterfaceProxy.ConstructorWeaving
{
    using System;
    using System.Linq;
    using System.Reflection;

    using FluentAssertions;

    using Moq;

    using Xunit;

    public class When_proxying_interface : SimpleTestBase
    {
        private const string InterfaceFullName = "SimpleTest.IProxy";

        private readonly Type clazz;

        public When_proxying_interface()
        {
            this.clazz = this.WovenSimpleTestAssembly.GetType(InterfaceFullName + InterfaceImplementationWeaver.ClassNameSuffix);
            this.clazz.Should().NotBeNull();
        }

        [Fact(Skip = "implement")]
        public void Must_Implement_Interface()
        {
            this.clazz.GetInterfaces().Should()
                .HaveCount(1)
                .And.Contain(x => x.FullName == InterfaceFullName);
        }

        [Fact]
        public void Must_add_constructor()
        {
            this.clazz.GetConstructors().Should().HaveCount(1);
        }

        [Fact]
        public void Must_add_dynamic_interceptor_manager_to_constructor()
        {
            this.clazz.GetConstructors().Single().GetParameters().Should()
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

        [Fact]
        public void Instanciating_should_not_throw()
        {
            Activator.CreateInstance(this.clazz, Mock.Of<IDynamicInterceptorManager>());
        }
    }
}