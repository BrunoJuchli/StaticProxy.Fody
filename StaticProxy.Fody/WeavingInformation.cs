namespace StaticProxy.Fody
{
    using Mono.Cecil;
    using System;
    using System.IO;

    public static class WeavingInformation
    {
        public static ModuleDefinition ModuleDefinition { get; private set; }

        public static TypeReference StaticProxyAttribute { get; private set; }

        public static ModuleDefinition InterceptorModuleDefinition { get; private set; }

        public static TypeReference DynamicInterceptorManagerReference { get; private set; }

        public static ReferenceFinder ReferenceFinder { get; private set; }

        public static TypeReference ObjectTypeReference { get; private set; }

        public static TypeReference TypeTypeReference { get; private set; }

        public static MethodReference GetTypeFromHandleMethodReference { get; private set; }

        public static void Initialize()
        {
            ModuleDefinition = ModuleWeaver.Instance.ModuleDefinition;
            ReferenceFinder = new ReferenceFinder(ModuleDefinition);

            StaticProxyAttribute = RetrieveStaticProxyAttributeReference();

            InterceptorModuleDefinition = ResolveInterceptorModuleDefinition();

            TypeDefinition dynamicInterceptorManagerTypeDefinition = InterceptorModuleDefinition.GetTypeDefinition("IDynamicInterceptorManager");
            DynamicInterceptorManagerReference = ModuleDefinition.ImportReference(dynamicInterceptorManagerTypeDefinition);

            ObjectTypeReference = ReferenceFinder.GetTypeReference(typeof(object));
            TypeTypeReference = ReferenceFinder.GetTypeReference(typeof(Type));

            GetTypeFromHandleMethodReference = ReferenceFinder.GetMethodReference(TypeTypeReference, md => md.Name == "GetTypeFromHandle");
        }

        private static TypeReference RetrieveStaticProxyAttributeReference()
        {
            try
            {
                return ModuleDefinition.GetTypeReference("StaticProxyAttribute");
            }
            catch (WeavingException ex)
            {
                throw new WeavingException(
                    "It seems there's no type decorated with [StaticProxyAttribute]. Decorate one or remove the StaticProxy.Fody nuget package from the project.",
                    ex);
            }
        }

        public static bool IsStaticProxyAttribute(CustomAttribute attribute)
        {
            return attribute.AttributeType == StaticProxyAttribute;
        }

        private static ModuleDefinition ResolveInterceptorModuleDefinition()
        {
            var interceptorAssemblyReference = new AssemblyNameReference("StaticProxy.Interceptor", null);

            AssemblyDefinition definition = ModuleWeaver.Instance.AssemblyResolver.Resolve(
                interceptorAssemblyReference,
                new ReaderParameters { InMemory = true } );
            if (definition == null)
            {
                // todo use an integration test to test this!
                DirectoryInfo nugetPackagesDirectory = Directory.GetParent(ModuleWeaver.Instance.AddinDirectoryPath);
                var assemblyResolver = new DefaultAssemblyResolver();

                DirectoryInfo[] packageDirectores = nugetPackagesDirectory.GetDirectories();
                foreach (DirectoryInfo packageDirectory in packageDirectores)
                {
                    assemblyResolver.AddSearchDirectory(packageDirectory.FullName);
                }

                definition = assemblyResolver.Resolve(interceptorAssemblyReference);
                if (definition == null)
                {
                    throw new WeavingException("Can't find StaticProxy.Interceptor assembly. Make sure you've downloaded and installed the nuget package!");
                }
            }

            return definition.MainModule;
        }
    }
}