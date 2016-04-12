namespace StaticProxy.Fody
{
    using Mono.Cecil;
    using System.IO;

    public static class WeavingInformation
    {
        public static ModuleDefinition ModuleDefinition { get; private set; }

        public static TypeReference StaticProxyAttribute { get; private set; }

        public static ModuleDefinition InterceptorModuleDefinition { get; private set; }

        public static TypeReference DynamicInterceptorManagerReference { get; private set; }

        public static ReferenceFinder ReferenceFinder { get; private set; }

        public static TypeReference ObjectTypeReference { get; private set; }

        public static void Initialize()
        {
            ModuleDefinition = ModuleWeaver.Instance.ModuleDefinition;
            ReferenceFinder = new ReferenceFinder(ModuleDefinition);

            StaticProxyAttribute = ModuleDefinition.GetTypeReference("StaticProxyAttribute");

            InterceptorModuleDefinition = ResolveInterceptorModuleDefinition();

            TypeDefinition dynamicInterceptorManagerTypeDefinition = InterceptorModuleDefinition.GetTypeDefinition("IDynamicInterceptorManager");
            DynamicInterceptorManagerReference = ModuleDefinition.ImportReference(dynamicInterceptorManagerTypeDefinition);

            ObjectTypeReference = ReferenceFinder.GetTypeReference(typeof(object));
        }

        public static bool IsStaticProxyAttribute(CustomAttribute attribute)
        {
            return attribute.AttributeType == StaticProxyAttribute;
        }

        private static ModuleDefinition ResolveInterceptorModuleDefinition()
        {
            const string InterceptorAssemblyName = "StaticProxy.Interceptor";

            AssemblyDefinition definition = ModuleWeaver.Instance.AssemblyResolver.Resolve(InterceptorAssemblyName);
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

                definition = assemblyResolver.Resolve(InterceptorAssemblyName);
                if (definition == null)
                {
                    throw new WeavingException("Can't find StaticProxy.Interceptor assembly. Make sure you've downloaded and installed the nuget package!");
                }
            }

            return definition.MainModule;
        }
    }
}