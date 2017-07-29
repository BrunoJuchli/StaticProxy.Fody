namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using System;
    using Moq;

    using StaticProxy.Fody.InterfaceImplementation;

    public class InterfaceWithGenericMethodsTestBase : SimpleTestBase
    {
        protected readonly Type Clazz;
        protected readonly Mock<IDynamicInterceptorManager> InterceptorManager;

        protected readonly dynamic Instance;

        public InterfaceWithGenericMethodsTestBase()
        {
            this.Clazz = this.WovenSimpleTestAssembly.GetType("SimpleTest.InterfaceImplementation.IWithGenericMethods" + InterfaceImplementationWeaver.ClassNameSuffix);

            this.InterceptorManager = new Mock<IDynamicInterceptorManager>();

            this.Instance = Activator.CreateInstance(this.Clazz, this.InterceptorManager.Object);
        }
    }
}