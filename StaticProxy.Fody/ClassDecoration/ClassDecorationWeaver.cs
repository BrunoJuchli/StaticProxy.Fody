namespace StaticProxy.Fody.ClassDecoration
{
    using System.Linq;

    using Mono.Cecil;

    public class ClassDecorationWeaver
    {
        private readonly ConstructorWeaver constructorWeaver;
        private readonly MethodWeaver methodWeaver;

        public ClassDecorationWeaver(ConstructorWeaver constructorWeaver)
        {
            this.constructorWeaver = constructorWeaver;
            this.methodWeaver = new MethodWeaver();
        }

        public void DecorateClass(TypeDefinition classToDecorate)
        {
            FieldDefinition interceptorRetriever =
                this.constructorWeaver.ExtendConstructorWithDynamicInterceptorRetriever(classToDecorate);

            this.DecorateClassProxyMethods(classToDecorate, interceptorRetriever);
        }
        
        private void DecorateClassProxyMethods(TypeDefinition classToDecorate, FieldDefinition interceptorRetriever)
        {
            var methodsToDecorate =
                classToDecorate.Methods
                    .Where(x => !x.IsConstructor)
                    .Where(x => !x.IsAbstract)
                    .Where(x => !x.IsPrivate)
                    .ToList();

            foreach (MethodDefinition method in methodsToDecorate)
            {
                this.methodWeaver.DecorateMethod(method, interceptorRetriever);
            }
        }
    }
}