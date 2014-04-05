using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("StaticProxyInterceptor.Fody")]
[assembly: AssemblyDescription("Interception functionality for the Fody AddIn StaticProxy")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Bruno Juchli")]
[assembly: AssemblyProduct("StaticProxyInterceptor.Fody")]
[assembly: AssemblyCopyright("Copyright © Bruno Juchli 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

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

[assembly: InternalsVisibleTo("StaticProxy.Fody.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
