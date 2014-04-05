namespace StaticProxy.Fody
{
    using System;
    using System.Collections.Generic;

    using Mono.Cecil;

    public static class MethodDecoratorDefinitions
    {
        static MethodDecoratorDefinitions()
        {
            Actions = new[]
                               {
                                   Import(typeof(Action)),
                                   Import(typeof(Action<>)),
                                   Import(typeof(Action<,>)),
                                   Import(typeof(Action<,,>)),
                                   Import(typeof(Action<,,,>)),
                                   Import(typeof(Action<,,,,>)),
                                   Import(typeof(Action<,,,,,>)),
                                   Import(typeof(Action<,,,,,,>)),
                                   Import(typeof(Action<,,,,,,,>)),
                                   Import(typeof(Action<,,,,,,,,>)),
                                   Import(typeof(Action<,,,,,,,,,>)),
                                   Import(typeof(Action<,,,,,,,,,,>)),
                                   Import(typeof(Action<,,,,,,,,,,,>)),
                                   Import(typeof(Action<,,,,,,,,,,,,>)),
                                   Import(typeof(Action<,,,,,,,,,,,,,>)),
                                   Import(typeof(Action<,,,,,,,,,,,,,,>)),
                                   Import(typeof(Action<,,,,,,,,,,,,,,,>)),
                               };

            Funcs = new[]
                             {
                                 Import(typeof(Func<>)),
                                 Import(typeof(Func<,>)),
                                 Import(typeof(Func<,,>)),
                                 Import(typeof(Func<,,,>)),
                                 Import(typeof(Func<,,,,>)),
                                 Import(typeof(Func<,,,,,>)),
                                 Import(typeof(Func<,,,,,,>)),
                                 Import(typeof(Func<,,,,,,,>)),
                                 Import(typeof(Func<,,,,,,,,>)),
                                 Import(typeof(Func<,,,,,,,,,>)),
                                 Import(typeof(Func<,,,,,,,,,,>)),
                                 Import(typeof(Func<,,,,,,,,,,,>)),
                                 Import(typeof(Func<,,,,,,,,,,,,>)),
                                 Import(typeof(Func<,,,,,,,,,,,,,>)),
                                 Import(typeof(Func<,,,,,,,,,,,,,,>)),
                                 Import(typeof(Func<,,,,,,,,,,,,,,,>)),
                                 Import(typeof(Func<,,,,,,,,,,,,,,,,>)),
                             };
        }

        public static IList<TypeReference> Actions { get; private set; }

        public static IList<TypeReference> Funcs { get; private set; }

        private static TypeReference Import(Type t)
        {
            return WeavingInformation.ReferenceFinder.GetTypeReference(t);
        }
    }
}