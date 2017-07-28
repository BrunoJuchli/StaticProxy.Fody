StaticProxy.Fody: [![Build status](https://ci.appveyor.com/api/projects/status/j6tubf9q9deyngu4)](https://ci.appveyor.com/project/BrunoJuchli/staticproxy-fody)
StaticProxy.Interceptor: [![Build status](https://ci.appveyor.com/api/projects/status/bpji3ka4pmwd54wm)](https://ci.appveyor.com/project/BrunoJuchli/staticproxy-fody-951)

## ![Icon](https://raw.githubusercontent.com/BrunoJuchli/StaticProxy.Fody/master/Icons/package_icon.png) This is an add-in for [Fody](https://github.com/Fody/Fody/) 

StaticProxy weaves proxies at compile time. This is similar to tools like Castle Dynamic Proxy and LinFu Proxy, but it works during compilation (also see: [Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)). But why? Because some platforms, notably WinRT, Windows Phone and iOS do not support dynamic code emitting. So "dynamic proxy" cannot be used on these platforms. StaticProxy to the rescue!

It is meant to be used in conjunction with dependency injection containers since it is adding arguments to the constructor - and thus breaks `new Foo(...)` calls of all proxied classes.

## Nuget

Nuget package http://nuget.org/packages/StaticProxy.Fody 

To Install the static proxy weaver and the interceptor infrastructure from the Nuget Package Manager Console 
    
    PM> Install-Package StaticProxy.Fody
    PM> Install-Package StaticProxy.Interceptor
    
    
## Usage
 - Add the `StaticProxy.Fody` and `StaticProxy.Interceptor` nuget packages to any project where you wish to add static proxy weaving.
 - Put an `[StaticProxy]` attribute on any class or interface you wish to be proxied.
 - Write interceptors (`class SomeProxy : IDynamicInterceptor`)
 
Then, use one of the existing StaticProxy IoC container integrations:
  - Ninject (PCL): [ninject.extensions.staticproxy](https://github.com/BrunoJuchli/ninject.extensions.staticproxy)
  - Unity [Unity.StaticProxyExtension](https://github.com/BrunoJuchli/Unity.StaticProxyExtension)
 
or roll your own:
  - Configure your Inversion of Control (IoC) container to be able to resolve `IDynamicInterceptorManager`. The implementation is provided by the `StaticProxy.Interceptor` nuget package.
  - Configure your IoC container to be able to resolve `IDynamicInterceptorCollection`. It needs to contain the interceptor(s) for the proxied type.
 
### Class Proxy
Is created by putting the `[StaticProxy]` attribute on a class.
This is similar to castle dynamic proxy "class proxy" and "interface proxy with target".
The class will be decorated, so that all method calls can be intercepted.

#### Your Code

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

#### What gets compiled
	
    public class Foo
    {
        private readonly IBar bar;
        private readonly IDynamicInterceptorManager dynamicInterceptorManager;
    
        public Foo(IBar bar, IDynamicInterceptorManager IDynamicInterceptorManager)
        {
            this.bar = bar;
            this.dynamicInterceptorManager = dynamicInterceptorManager;
            this.dynamicInterceptorManager.Initialize(this, false);
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
    
##### Explanation

 - `IDynamicInterceptorManager` argument is added to the constructor
 - the constructor passes a reference to the newly created object to the `IDynamicInterceptorManager` by way of `.Initialize(this, false);`
 - all public methods are renamed from `Orig(..)` to `(Orig<SP>(...)`. For each a decorating method with the original signature is created.
 - The decorating method is passing every call to the `IDynamicInterceptorManager` by `.Intercept(..)`.
 - The `IDynamicInterceptorManager` is calling the `IDynamicInterceptor`s in sequence and lastly the implementation.

### Interface Proxy
Is created by putting the `[StaticProxy]` attribute on an interface.
This is similar to castle dynamic proxy "interface proxy without target".
An implementation of the interface is created. This implementation will call the interceptor(s). The interceptors will need to provide the actual "business" implementation of the method. Subsequently, this type of proxy does only work if there are 1+ interceptors.

#### Your Code

    [StaticProxy]
    public interface IBar
    {
        int Multiply(int multiplicand , int multiplier);
        
        void NoReturnValue(string value);
    }

#### What gets compiled
	
    public class IBarImplementation
    {
        private readonly IDynamicInterceptorManager dynamicInterceptorManager;
    
        public Foo(IDynamicInterceptorManager IDynamicInterceptorManager)
        {
            this.dynamicInterceptorManager = dynamicInterceptorManager;
            this.dynamicInterceptorManager.Initialize(this, true);
        }
    
        public int Multiply(int multiplicand , int multiplier)
        {
            object[] arguments = new object[] { multiplicand, multiplier };
            MethodInfo decoratedMethod = methodOf(Multiply);
            MethodInfo implementationMethod = null;
        
            return this.dynamicInterceptorManager.Intercept(decoratedMethod, implementationMethod, arguments);
        }
        
        public void NoReturnValue(string value)
        {
            object[] arguments = new object[] { value };
            MethodInfo decoratedMethod = methodOf(NoReturnValue);
            MethodInfo implementationMethod = null;
        
            this.dynamicInterceptorManager.Intercept(decoratedMethod, implementationMethod, arguments);
        }
    }
    
##### Explanation

 - a class is added to the assembly where the interface resided. The class is named "InterfaceName" + "Implementation". The class implements the interface.
 - The class contains a constructor with `IDynamicInterceptorManager` argument
 - the constructor passes a reference to the newly created object to the `IDynamicInterceptorManager` by way of `.Initialize(this, true);`
 - all interface methods are implemented. They make a call to `IDynamicInterceptorManager.Intercept(..)`.
 - The `IDynamicInterceptorManager` is calling the `IDynamicInterceptor`s in sequence.


## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)
