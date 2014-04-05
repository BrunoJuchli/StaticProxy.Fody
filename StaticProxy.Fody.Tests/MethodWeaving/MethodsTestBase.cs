namespace StaticProxy.Fody.Tests.MethodWeaving
{
    using System;
    using System.Globalization;
    using System.Reflection;

    using FluentAssertions;

    using Moq;

    public class MethodsTestBase : SimpleTestBase
    {
        protected readonly Type Clazz;
        protected readonly Mock<IDynamicInterceptorManager> InterceptorManager;

        protected readonly dynamic Instance;

        public MethodsTestBase()
        {
            this.Clazz = this.WovenSimpleTestAssembly.GetType("SimpleTest.Methods");

            this.InterceptorManager = new FakeDynamicInterceptorManager();
            
            this.Instance = Activator.CreateInstance(this.Clazz, this.InterceptorManager.Object);
        }

        protected void VerifyMethodCalled(string originalMethodName)
        {
            string expectedMethodName = string.Format(CultureInfo.InvariantCulture, "{0}<SP>", originalMethodName);
            this.TestMessages.Should().Contain(expectedMethodName);
        }

        private class FakeDynamicInterceptorManager : Mock<IDynamicInterceptorManager>
        {
            private object target;

            public FakeDynamicInterceptorManager()
            {
                this.Setup(x => x.Initialize(It.IsAny<object>()))
                    .Callback<object>(target => this.target = target);

                this.Setup(x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<object[]>()))
                    .Callback<MethodBase, MethodBase, object[]>((decoratedMethod, implementationMethod, arguments) => 
                        implementationMethod.Invoke(this.target, arguments));
            }
        }
    }
}