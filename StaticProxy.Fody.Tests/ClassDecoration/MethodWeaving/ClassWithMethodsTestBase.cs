namespace StaticProxy.Fody.Tests.ClassDecoration.MethodWeaving
{
    using System;
    using System.Reflection;
    using System.Text;

    using FluentAssertions;

    using Moq;

    public class ClassWithMethodsTestBase : SimpleTestBase
    {
        protected readonly Type Clazz;
        protected readonly Mock<IDynamicInterceptorManager> InterceptorManager;

        protected readonly dynamic Instance;

        public ClassWithMethodsTestBase()
        {
            this.Clazz = this.WovenSimpleTestAssembly.GetType("SimpleTest.ClassDecoration.ClassWithMethods");

            this.InterceptorManager = new FakeDynamicInterceptorManager();
            
            this.Instance = Activator.CreateInstance(this.Clazz, this.InterceptorManager.Object);
        }

        protected void VerifyMethodCalled(string originalMethodName, params object[] arguments)
        {
            string expectedMessage = new StringBuilder()
                .Append(originalMethodName)
                .Append("<SP>")
                .Append("(")
                    .Append(string.Join(", ", arguments))
                .Append(")")
                .ToString();

            this.TestMessages.Should().Contain(expectedMessage);
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