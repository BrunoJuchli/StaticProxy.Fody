namespace SimpleTest.InterfaceImplementation
{
    [StaticProxy]
    public interface IWithMethods
    {
        void NoArguments();

        void ValueArguments(int arg1, float arg2);

        void ReferenceArguments(object arg1, object arg2);

        void MixedArguments(int arg1, object arg2, string arg3);

        void WithLocalVariables();

        int ReturnsInteger();

        object ReturnsObject();

        float? ReturnsNullableFloat();
    }
}