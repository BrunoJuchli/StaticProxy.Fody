using System;
using System.Reflection;

namespace SimpleTest.InterfaceImplementation
{
    [StaticProxy]
    public interface IGenericProxy<T1, T2, T3>
        where T1 : new()
        where T2 : class
        where T3 : Uri
    {
        T3 DoSomething(T1 argument1);
    }
}