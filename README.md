[![Build status](https://ci.appveyor.com/api/projects/status/29hg7caftwtfb0jo)](https://ci.appveyor.com/project/BrunoJuchli/staticproxy)

## this is a work in progress which is not working - yet ##

## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

![Icon](https://raw.github.com/BrunoJuchli/StaticProxy/master/Icons/package_icon.png)

Compile time static proxy via IL rewriting.
Interception is dynamic with Interceptors as known from Castle Core DynamicProxy.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

## Nuget

Nuget package http://nuget.org/packages/StaticProxy.Fody 

To Install from the Nuget Package Manager Console 
    
    PM> Install-Package StaticProxy.Fody
    
### Your Code

	public interface IMethodDecorator
	{
	    void OnEntry(MethodBase method);
	    void OnExit(MethodBase method);
	    void OnException(MethodBase method, Exception exception);
	}

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
	public class InterceptorAttribute : Attribute, IMethodDecorator
	{
	    public void OnEntry(MethodBase method)
	    {
	        TestMessages.Record(string.Format("OnEntry: {0}", method.DeclaringType.FullName + "." + method.Name));
	    }
	
	    public void OnExit(MethodBase method)
	    {
	        TestMessages.Record(string.Format("OnExit: {0}", method.DeclaringType.FullName + "." + method.Name));
	    }
	
	    public void OnException(MethodBase method, Exception exception)
	    {
	        TestMessages.Record(string.Format("OnException: {0} - {1}: {2}", method.DeclaringType.FullName + "." + method.Name, exception.GetType(), exception.Message));
	    }
	}
	
	public class Sample
	{
		[Interceptor]
		public void Method()
		{
		    Debug.WriteLine("Your Code");
		}
	}

### What gets compiled
	
	public class Sample
	{
		public void Method()
		{
		    MethodBase method = methodof(Sample.Method, Sample);
		    InterceptorAttribute attribute = (InterceptorAttribute) method.GetCustomAttributes(typeof(InterceptorAttribute), false)[0];
		    attribute.OnEntry(method);
		    try
		    {
		        Debug.WriteLine("Your Code");
		        attribute.OnExit(method);
		    }
		    catch (Exception exception)
		    {
		        attribute.OnException(method, exception);
		        throw;
		    }
		}
	}

## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)



