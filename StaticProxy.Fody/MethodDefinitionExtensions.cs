namespace StaticProxy.Fody
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    public static class MethodDefinitionExtensions
    {
        public static VariableDefinition AddVariableDefinition(this MethodDefinition method, string variableName, TypeReference variableType)
        {
            var variableDefinition = new VariableDefinition(variableName, variableType);
            method.Body.Variables.Add(variableDefinition);
            return variableDefinition;
        }
    }
}