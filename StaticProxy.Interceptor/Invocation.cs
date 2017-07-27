// portions of this code have been taken from the Castle project (http://www.castleproject.org/), a big thanks to the authors.

namespace StaticProxy.Interceptor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using StaticProxy.Interceptor.TargetInvocation;
    using StaticProxy.Interceptor.InterceptedMethod;

    internal class Invocation : IInvocation
    {
        private readonly ITargetInvocation targetInvocation;
        private readonly IDynamicInterceptor[] interceptors;
        private readonly Lazy<Type[]> parameterTypes;

        private int currentInterceptorIndex = -1;

        public Invocation(IInterceptedMethod interceptedMethod, object[] arguments, IDynamicInterceptor[] interceptors)
        {
            this.targetInvocation = interceptedMethod.TargetInvocation;
            this.Method = interceptedMethod.DecoratedMethod;
            this.GenericArguments = interceptedMethod.GenericArguments;
            this.Arguments = arguments;
            this.interceptors = interceptors;

            this.parameterTypes = new Lazy<Type[]>(() => this.Method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public object[] Arguments { get; }

        public Type[] GenericArguments { get; }

        public MethodInfo Method { get; }

        public object ReturnValue { get; set; }

        public object GetArgumentValue(int index)
        {
            return this.Arguments[index];
        }

        public void Proceed()
        {
            this.currentInterceptorIndex++;
            try
            {
                if (this.currentInterceptorIndex == this.interceptors.Length)
                {
                    this.ReturnValue = this.targetInvocation.InvokeMethodOnTarget(this.Arguments);
                }
                else if (this.currentInterceptorIndex > this.interceptors.Length)
                {
                    throw this.CreateExceptionForInvalidCurrentInterceptorIndex();
                }
                else
                {
                    this.interceptors[this.currentInterceptorIndex].Intercept(this);
                }
            }
            finally
            {
                this.currentInterceptorIndex--;
            }
        }

        public void SetArgumentValue(int index, object value)
        {
            var expectedParameterType = this.parameterTypes.Value[index];
            if (value == null)
            {
                if (expectedParameterType.GetTypeInfo().IsValueType)
                {
                    throw new ArgumentNullException(
                        index.ToString(),
                        string.Format(CultureInfo.InvariantCulture, "Cannot set value-type ({0}) parameter to null", expectedParameterType));
                }
            }
            else
            {
                var actualParameterType = value.GetType();
                if (!expectedParameterType.GetTypeInfo().IsAssignableFrom(actualParameterType.GetTypeInfo()))
                {
                    throw new ArgumentOutOfRangeException(
                        index.ToString(),
                        string.Format(CultureInfo.InvariantCulture, "Cannot set {0} parameter to value of type {1}", expectedParameterType, actualParameterType));
                }
            }
            
            this.Arguments[index] = value;
        }

        private InvalidOperationException CreateExceptionForInvalidCurrentInterceptorIndex()
        {
            string interceptorsCount;
            if (this.interceptors.Length > 1)
            {
                interceptorsCount = " each one of " + this.interceptors.Length + " interceptors";
            }
            else
            {
                interceptorsCount = " interceptor";
            }

            var message = "This is a StaticProxy.Fody error: invocation.Proceed() has been called more times than expected."
                          + "This usually signifies a bug in the calling code. Make sure that" + interceptorsCount
                          + " selected for the method '" + this.Method + "'" + "calls invocation.Proceed() at most once.";
            return new InvalidOperationException(message);
        }
    }
}