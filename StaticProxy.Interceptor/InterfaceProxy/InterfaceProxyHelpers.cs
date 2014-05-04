namespace StaticProxy.Interceptor.InterfaceProxy
{
    using System;
    using System.Globalization;

    public class InterfaceProxyHelpers
    {
        public const string InterfaceImplementationSuffix = "Implementation";

        public static Type GetImplementationTypeOfInterface(Type interfaceType)
        {
            EnsureTypeIsInterface(interfaceType);

            return RetrieveProxyType(interfaceType);
        }

        private static void EnsureTypeIsInterface(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
@"`{0}` Is not an interface. StaticProxy.Fody creates an implementation for every interface where you put the [StaticProxy] on.
For class proxies, use the appropriate binding. See your IoC extension documentation.",
                    interfaceType.FullName);
                throw new ArgumentOutOfRangeException("interfaceType", message);
            }
        }

        private static Type RetrieveProxyType(Type interfaceType)
        {
            string proxyTypeName = string.Concat(interfaceType.FullName, InterfaceImplementationSuffix);
            Type proxyType = interfaceType.Assembly.GetType(proxyTypeName);
            if (proxyType == null)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
@"There is no auto-generated implementation for interface `{0}`.
Verify the following:
 - Put the attribute [StaticProxy] on the interface '{0}'. The attribute can be found in the nuget package `StaticProxy.Interceptor`.
 - Add the nuget packages `Fody` and `StaticProxy.Fody` to the assembly containing the interface `{0}`
As a result there should be a class named `{1}` in the same assembly as the interface",
                    interfaceType.FullName,
                    proxyTypeName);

                throw new InvalidOperationException(message);
            }

            return proxyType;
        }
    }
}