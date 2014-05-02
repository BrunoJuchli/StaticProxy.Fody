namespace StaticProxy.Fody.Tests.InterfaceImplementation.MethodWeaving
{
    using System;
    using Moq;

    using StaticProxy.Fody.InterfaceImplementation;

    public class InterfaceWithMethodsTestBase : SimpleTestBase
    {
        protected readonly Type Clazz;
        protected readonly Mock<IDynamicInterceptorManager> InterceptorManager;

        protected readonly dynamic Instance;

        public InterfaceWithMethodsTestBase()
        {
            this.Clazz = this.WovenSimpleTestAssembly.GetType("SimpleTest.InterfaceImplementation.IWithMethods" + InterfaceImplementationWeaver.ClassNameSuffix);

            this.InterceptorManager = new Mock<IDynamicInterceptorManager>();
            
            this.Instance = Activator.CreateInstance(this.Clazz, this.InterceptorManager.Object);
        }
    }
}