namespace StaticProxy.Fody
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Mono.Cecil;
    using Mono.Cecil.Rocks;

    using StaticProxy.Fody.ClassDecoration;
    using StaticProxy.Fody.InterfaceImplementation;

    public static class ProxyWeaver
    {
        public static void Execute()
        {
            IEnumerable<TypeDefinition> typesToProxy = WeavingInformation.ModuleDefinition.Types
                .Where(HasStaticProxyAttribute)
                .ToList();

            var constructorWeaver = new ConstructorWeaver();

            var interfaceImplementationWeaver = new InterfaceImplementationWeaver(constructorWeaver);
            var classDecorationWeaver = new ClassDecorationWeaver(constructorWeaver);

            ImplementInterfaces(
                typesToProxy.Where(x => x.IsInterface),
                interfaceImplementationWeaver);

            DecorateClasses(classDecorationWeaver, typesToProxy.Where(x => x.IsClass).ToList());
        }

        private static void ImplementInterfaces(
            IEnumerable<TypeDefinition> interfacesToImplement,
            InterfaceImplementationWeaver interfaceImplementationWeaver)
        {
            foreach (TypeDefinition interfaceToProxy in interfacesToImplement)
            {
                interfaceImplementationWeaver.CreateImplementationOf(interfaceToProxy);
            }
        }

        private static void DecorateClasses(ClassDecorationWeaver classDecorationWeaver, ICollection<TypeDefinition> classesToDecorate)
        {
            AssertDoNotHaveMultipleConstructors(classesToDecorate);

            foreach (TypeDefinition classToDecorate in classesToDecorate)
            {
                classDecorationWeaver.DecorateClass(classToDecorate);
            }
        }

        private static bool HasStaticProxyAttribute(TypeDefinition typeDefinition)
        {
            return typeDefinition.CustomAttributes.Any(WeavingInformation.IsStaticProxyAttribute);
        }
        
        private static void AssertDoNotHaveMultipleConstructors(IEnumerable<TypeDefinition> classesToDecorate)
        {
            IEnumerable<string> violationMessages = classesToDecorate
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