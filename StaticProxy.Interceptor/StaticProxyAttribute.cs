using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class StaticProxyAttribute : Attribute
{
}