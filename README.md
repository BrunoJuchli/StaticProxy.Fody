StaticProxy.Fody: [![Build status](https://ci.appveyor.com/api/projects/status/j6tubf9q9deyngu4)](https://ci.appveyor.com/project/BrunoJuchli/staticproxy-fody)
StaticProxy.Interceptor: [![Build status](https://ci.appveyor.com/api/projects/status/bpji3ka4pmwd54wm)](https://ci.appveyor.com/project/BrunoJuchli/staticproxy-fody-951)

## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

![Icon](https://raw.github.com/BrunoJuchli/StaticProxy/master/Icons/package_icon.png)

Very cool utilities have been created by the help of proxying by dynamic code emitting:
Moq, FakeItEasy, Castle Dynamic Proxy, LinFu Proxy,.. and many more.

Sadly enough, some platforms do not support dynamic code emitting. These include WinRT, Windows Phone and IPhone.
You are also most likely affected if you are creating a PCL.

Here comes StaticProxy.Fody to the rescue!
Instead of dynamically creating proxies, it is weaving them at compile time by means of IL rewritting
(see: [Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)).

It is meant to be used in conjunction with dependency injection (IoC), since it is adding arguments to the constructor - and thus breaks all `new Foo(...)` calls.


## Nuget

Nuget package http://nuget.org/packages/StaticProxy.Fody 

To Install the static proxy weaver from the Nuget Package Manager Console 
    
    PM> Install-Package StaticProxy.Fody
    
    
## Usage
 - Add the StaticProxy.Fody nuget package to any project where you wish to add static proxy weaving.
 - Put an `[StaticProxy]` attribute on any class you wish to be proxied.
 - Write interceptors (`class SomeProxy : IDynamicInterceptor`)
 

Then, use one of the existing StaticProxy IoC container integrations:
  - Ninject (PCL): [ninject.extensions.staticproxy](https://github.com/BrunoJuchli/ninject.extensions.staticproxy)
 
or roll your own:
  - Configure your Inversion of Control (IoC) container to be able to resolve `IDynamicInterceptorManager`. The implementation is provided by the `StaticProxy.Interceptor` nuget package.
  - Configure your IoC container to be able to resolve `IDynamicInterceptorCollection`. It needs to contain the interceptor for the proxied type.
 

### Your Code

    [StaticProxy]
    public class Foo
    {
        private readonly IBar bar;
    
        public Foo(IBar bar)
        {
            this.bar = bar;
        }
    
        public int Multiply(int multiplicand , int multiplier)
        {
            return multiplicand * multiplier;
        }
        
        public void NoReturnValue(string value)
        {
            Console.WriteLine(value);
        }
    }

### What gets compiled
	
    public class Foo
    {
        private readonly IBar bar;
        private readonly IDynamicInterceptorManager dynamicInterceptorManager;
    
        public Foo(IBar bar, IDynamicInterceptorManager IDynamicInterceptorManager)
        {
            this.bar = bar;
            this.dynamicInterceptorManager = dynamicInterceptorManager;
            this.dynamicInterceptorManager.Initialize(this);
        }
    
        public int Multiply(int multiplicand , int multiplier)
        {
            object[] arguments = new object[] { multiplicand, multiplier };
            MethodInfo decoratedMethod = methodOf(Multiply);
            MethodInfo implementationMethod = methodOf(Multiply<SP>);
        
            return this.dynamicInterceptorManager.Intercept(decoratedMethod, implementationMethod, arguments);
        }
    
        public int Multiply<SP>(int multiplicand , int multiplier)
        {
            return multiplicand * multiplier;
        }
        
        public void NoReturnValue(string value)
        {
            object[] arguments = new object[] { value };
            MethodInfo decoratedMethod = methodOf(NoReturnValue);
            MethodInfo implementationMethod = methodOf(NoReturnValue<SP>);
        
            this.dynamicInterceptorManager.Intercept(decoratedMethod, implementationMethod, arguments);
        }
        
        public void NoReturnValue<SP>(string value)
        {
            Console.WriteLine(value);
        }
    }
    
#### Explanation ####

 - `IDynamicInterceptorManager` argument is added to the constructor
 - the constructor passes a reference to the newly created object to the `IDynamicInterceptorManager` by way of `.Initialize(this);`
 - all public methods are renamed from `Orig(..)` to `(Orig<SP>(...)`. For each a decorating method with the original signature is created.
 - The decorating method is passing every call to the `IDynamicInterceptorManager` by `.Intercept(..)`.
 - The `IDynamicInterceptorManager` is calling the `IDynamicInterceptor`s in sequence and lastly the implementation.

## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)
