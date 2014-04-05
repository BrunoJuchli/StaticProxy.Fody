namespace StaticProxy.Fody
{
    using System;
    using Mono.Cecil;

    public static class WeavingInformation
    {
        public static ModuleDefinition ModuleDefinition { get; private set; }

        public static IAssemblyResolver AssemblyResolver { get; private set; }

        public static TypeReference StaticProxyAttribute { get; private set; }

        public static ModuleDefinition InterceptorModuleDefinition { get; private set; }

        public static TypeReference DynamicInterceptorManagerReference { get; private set; }

        public static ReferenceFinder ReferenceFinder { get; private set; }

        public static TypeReference ObjectTypeReference { get; private set; }

        public static void Initialize(ModuleDefinition moduleDefinition, IAssemblyResolver assemblyResolver, Action<string> logInfo, Action<string> logWarning)
        {
            ModuleDefinition = moduleDefinition;
            AssemblyResolver = assemblyResolver;
            ReferenceFinder = new ReferenceFinder(ModuleDefinition);

            InterceptorModuleDefinition = AssemblyResolver.Resolve("StaticProxyInterceptor.Fody").MainModule;

            StaticProxyAttribute = ModuleDefinition.GetTypeReference<StaticProxyAttribute>();

            TypeDefinition interceptorManagerDefinition = InterceptorModuleDefinition.GetTypeDefinition<IDynamicInterceptorManager>();
            DynamicInterceptorManagerReference = ModuleDefinition.Import(interceptorManagerDefinition);

            ObjectTypeReference = ReferenceFinder.GetTypeReference(typeof(object));
        }

        public static bool IsStaticProxyAttribute(CustomAttribute attribute)
        {
            return attribute.AttributeType == StaticProxyAttribute;
        }
    }
}