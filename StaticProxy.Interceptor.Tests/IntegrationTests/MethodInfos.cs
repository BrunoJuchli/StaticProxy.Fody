namespace StaticProxy.Interceptor.IntegrationTests
{
    using System.Reflection;
    public static class MethodInfos
    {
        public static class ClassProxy
        {
            public static readonly MethodInfo DecoratedMethodReturningValueType = typeof(DemoClassProxy).GetMethod("DecoratedMethodReturningValueType");
            public static readonly MethodInfo ImplementedMethodReturningValueType = typeof(DemoClassProxy).GetMethod("ImplementedMethodReturningValueType");
            public static readonly MethodInfo DecoratedMethodReturningReferenceType = typeof(DemoClassProxy).GetMethod("DecoratedMethodReturningReferenceType");
            public static readonly MethodInfo ImplementedMethodReturningReferenceType = typeof(DemoClassProxy).GetMethod("ImplementedMethodReturningReferenceType");
            public static readonly MethodInfo DecoratedGenericMethod = typeof(DemoClassProxy).GetMethod("DecoratedGenericMethod");
            public static readonly MethodInfo ImplementedGenericMethod = typeof(DemoClassProxy).GetMethod("ImplementedGenericMethod");
        }

        public static class InterfaceProxy
        {
            public static readonly MethodInfo DecoratedMethodReturningValueType = typeof(IDemoInterfaceProxy).GetMethod("DecoratedMethodReturningValueType");
            public static readonly MethodInfo DecoratedMethodReturningReferenceType = typeof(IDemoInterfaceProxy).GetMethod("DecoratedMethodReturningReferenceType");
            public static readonly MethodInfo DecoratedGenericMethod = typeof(IDemoInterfaceProxy).GetMethod("DecoratedGenericMethod");
        }

    }
}
