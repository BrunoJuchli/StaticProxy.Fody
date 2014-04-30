namespace StaticProxy.Interceptor.Reflection
{
    using System;

    internal interface ITypeInformation
    {
        bool IsNullable(Type t);
    }
}