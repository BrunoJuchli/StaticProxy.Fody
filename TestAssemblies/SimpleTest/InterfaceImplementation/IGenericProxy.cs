using System;

namespace SimpleTest.InterfaceImplementation
{
    // todo implement
    // todo add type constraints
    // [StaticProxy]
    public interface IGenericProxy<T1, T2, T3>
        where T1 : new()
        where T2 : class
        where T3 : Uri
    {
    }
}