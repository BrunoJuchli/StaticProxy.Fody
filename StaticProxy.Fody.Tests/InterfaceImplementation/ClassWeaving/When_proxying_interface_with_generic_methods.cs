namespace StaticProxy.Fody.Tests.InterfaceImplementation.ClassWeaving
{
    using FluentAssertions;
    using Moq;
    using StaticProxy.Fody.InterfaceImplementation;
    using System;
    using System.Linq;
    using System.Reflection;
    using Xunit;

    // see https://github.com/BrunoJuchli/StaticProxy.Fody/issues/6
    public class When_proxying_interface_with_generic_methods : SimpleTestBase
    {
        private const string InterfaceFullName = "SimpleTest.InterfaceImplementation.IWithGenericMethods";

        private readonly Type clazz;

        public When_proxying_interface_with_generic_methods()
        {
            this.clazz = this.WovenSimpleTestAssembly.GetType(InterfaceFullName + InterfaceImplementationWeaver.ClassNameSuffix);
            this.clazz.Should().NotBeNull();
        }

        public void Must_implement_interface()
        {
            this.clazz.GetInterfaces().Should()
                .HaveCount(1)
                .And.Contain(x => x.FullName == InterfaceFullName);
        }

        public void Must_Implement_all_methods()
        {
            this.clazz.GetMethods().Should()
                .HaveCount(4);
        }

        [Fact]
        public void Must_implement_method_taking_T()
        {
            var method = this.clazz.GetMethods().Single(x => x.Name == "TakesT");

            method.IsGenericMethodDefinition.Should().BeTrue();
            method.ContainsGenericParameters.Should().BeTrue();
            var parameters = method.GetParameters();
            parameters.Should().HaveCount(1);
            parameters[0].ParameterType.IsGenericParameter.Should().BeTrue();
            method.ReturnType.Should().Be(typeof(void));
        }

        [Fact]
        public void Must_implement_method_returning_T()
        {
            var method = this.clazz.GetMethods().Single(x => x.Name == "ReturnsT");

            method.IsGenericMethodDefinition.Should().BeTrue();
            method.ContainsGenericParameters.Should().BeTrue();
            method.ReturnType.IsGenericParameter.Should().BeTrue();
        }

        [Fact]
        public void Must_implement_method_with_single_generic_constraint()
        {
            var method = this.clazz.GetMethods().Single(x => x.Name == "WithConstraint");

            method.IsGenericMethodDefinition.Should().BeTrue();
            method.ContainsGenericParameters.Should().BeTrue();
            var parameters = method.GetParameters();
            parameters.Should().HaveCount(1);
            parameters[0].ParameterType.IsGenericParameter.Should().BeTrue();
            method.ReturnType.IsGenericParameter.Should().BeTrue();
        }

        [Fact]
        public void Must_implement_method_with_multiple_generic_constraints()
        {
            var method = this.clazz.GetMethods().Single(x => x.Name == "WithConstraints");

            method.IsGenericMethodDefinition.Should().BeTrue();
            method.ContainsGenericParameters.Should().BeTrue();
            var parameters = method.GetParameters();
            parameters.Should().HaveCount(1);
            parameters[0].ParameterType.IsGenericParameter.Should().BeTrue();
            method.ReturnType.IsGenericParameter.Should().BeTrue();
        }
        
        [Fact]
        public void Instanciating_should_not_throw()
        {
            Activator.CreateInstance(this.clazz, Mock.Of<IDynamicInterceptorManager>());
        }
    }
}