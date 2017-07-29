namespace StaticProxy.Fody.Tests.ClassDecoration.MethodWeaving
{
    using System;
    using System.Reflection;
    using System.Text;

    using FluentAssertions;

    using Moq;

    public class ClassWithGenericMethodsTestBase : SimpleTestBase
    {
        protected readonly Type Clazz;
        protected readonly Mock<IDynamicInterceptorManager> InterceptorManager;

        protected readonly dynamic Instance;

        public ClassWithGenericMethodsTestBase()
        {
            this.Clazz = this.WovenSimpleTestAssembly.GetType("SimpleTest.ClassDecoration.ClassWithGenericMethods");

            this.InterceptorManager = new FakeDynamicInterceptorManager();
            
            this.Instance = Activator.CreateInstance(this.Clazz, this.InterceptorManager.Object);
        }

        protected void VerifyMethodCalled(string originalMethodName, params object[] arguments)
        {
            string expectedMessage = new StringBuilder()
                .Append(originalMethodName)
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
                this.Setup(x => x.Initialize(It.IsAny<object>(), It.IsAny<bool>()))
                    .Callback<object, bool>((target, requiresInterceptor) => this.target = target);

                this.Setup(x => x.Intercept(It.IsAny<MethodBase>(), It.IsAny<MethodBase>(), It.IsAny<Type[]>(), It.IsAny<object[]>()))
                    .Callback<MethodBase, MethodBase, Type[], object[]>((decoratedMethod, implementationMethod, genericArguments, arguments) =>
                    {
                        if(genericArguments.Length > 0)
                        {
                            ((MethodInfo)implementationMethod)
                                .MakeGenericMethod(genericArguments)
                                .Invoke(this.target, arguments);
                        }
                        else
                        {
                            implementationMethod.Invoke(this.target, arguments);
                        }
                    });
                        
            }
        }
    }
}