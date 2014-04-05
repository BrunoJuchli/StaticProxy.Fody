namespace StaticProxy.Fody
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Mono.Cecil;
    using Mono.Cecil.Rocks;

    public static class ProxyWeaver
    {
        public static void Execute()
        {
            IEnumerable<TypeDefinition> typesToProxy = WeavingInformation.ModuleDefinition.Types
                .Where(HasStaticProxyAttribute)
                .ToList();

            AssertDoNotHaveMultipleConstructors(typesToProxy);

            var constructorDecorator = new ConstructorWeaver();
            var methodDecorator = new MethodWeaver();

            foreach (TypeDefinition typeToProxy in typesToProxy)
            {
                FieldDefinition interceptorRetriever = constructorDecorator.ExtendConstructorWithDynamicInterceptorRetriever(typeToProxy);

                DecorateMethods(typeToProxy, methodDecorator, interceptorRetriever);
            }
        }

        private static void DecorateMethods(
            TypeDefinition typeToProxy,
            MethodWeaver methodWeaver,
            FieldDefinition interceptorRetriever)
        {
            var methodsToDecorate =
                typeToProxy.Methods
                    .Where(x => !x.IsConstructor)
                    .Where(x => !x.IsAbstract)
                    .Where(x => !x.IsPrivate)
                    .ToList();

            foreach (MethodDefinition method in methodsToDecorate)
            {
                methodWeaver.DecorateMethod(method, interceptorRetriever);
            }
        }

        private static bool HasStaticProxyAttribute(TypeDefinition typeDefinition)
        {
            return typeDefinition.CustomAttributes.Any(WeavingInformation.IsStaticProxyAttribute);
        }
        
        private static void AssertDoNotHaveMultipleConstructors(IEnumerable<TypeDefinition> typesToProxy)
        {
            IEnumerable<string> violationMessages = typesToProxy
                .Where(HasMultipleConstructors)
                .Select(BuildMultipleConstructorsExceptionMessage)
                .ToList();

            if (violationMessages.Any())
            {
                var message = string.Join(Environment.NewLine, violationMessages);
                throw new WeavingException(message);
            }
        }

        private static bool HasMultipleConstructors(TypeDefinition typeDefinition)
        {
            return typeDefinition.GetConstructors().Count() > 1;
        }

        private static string BuildMultipleConstructorsExceptionMessage(TypeDefinition typeDefinition)
        {
            return string.Format(CultureInfo.InvariantCulture, ExceptionMessages.MultipleConstructors, typeDefinition.FullName);
        }
    }
}