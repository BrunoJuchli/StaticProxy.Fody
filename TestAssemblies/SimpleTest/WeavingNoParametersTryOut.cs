namespace SimpleTest
{
    using System;

    public class WeavingNoParametersTryOut
    {
        public void Decorated(int foo, string bar, SomeClass some)
        {
            var parameters = new[]
                                 {
                                     new InterceptingValue(typeof(int), foo), 
                                     new InterceptingValue(typeof(string), bar), 
                                     new InterceptingValue(typeof(SomeClass), some), 
                                 };
            this.Intercept(
                parameters,
                () => this.Undecorated((int)parameters[0].Value, (string)parameters[1].Value, (SomeClass)parameters[2].Value));
        }

        public void Undecorated(int foo, string bar, SomeClass some)
        {
            Console.WriteLine("foo = {0}, bar = {1}, some = {2}", foo, bar, some);
        }

        public void Intercept(InterceptingValue[] parameters, Action decorated)
        {
            parameters[0].Value = ((int)parameters[0].Value) * 4;
            parameters[1].Value = ((string)parameters[1].Value) + "Modified";
            parameters[2].Value = new SomeClass { SomeValue = "FooBalicious" };
            decorated();
        }
    }

    public class SomeClass
    {
        public SomeClass()
        {
            this.SomeValue = string.Empty;
        }

        public string SomeValue { get; set; }

        public override string ToString()
        {
            return this.SomeValue;
        }
    }
}