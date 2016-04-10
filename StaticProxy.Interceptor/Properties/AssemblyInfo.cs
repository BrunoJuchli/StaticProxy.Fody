using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("StaticProxy.Interceptor")]
[assembly: AssemblyDescription("Interception functionality for the Fody AddIn StaticProxy")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Bruno Juchli")]
[assembly: AssemblyProduct("StaticProxy.Interceptor")]
[assembly: AssemblyCopyright("Copyright © Bruno Juchli 2016")]
[assembly: NeutralResourcesLanguage("en")]

// these values are here to be patched by AppVeyor
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]
[assembly: AssemblyInformationalVersion("0.1.0.0")]

[assembly: InternalsVisibleTo("StaticProxy.Interceptor.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
