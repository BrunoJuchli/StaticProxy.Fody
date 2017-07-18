namespace SimpleTest.ClassDecoration
{
    using System;
    using System.Globalization;

    [StaticProxy]
    public class ClassWithMethods
    {
        public void NoArguments()
        {
            TestMessages.RecordMethodBody(new object[0]);
        }

        public void ValueArguments(int arg1, float arg2)
        {
            TestMessages.RecordMethodBody(new object[] { arg1, arg2 });
            Console.WriteLine("ValueParameters {0} {1}", arg1, arg2);
        }

        public void ReferenceArguments(object arg1, object arg2)
        {
            TestMessages.RecordMethodBody(new object[] { arg1, arg2 } );
            Console.WriteLine("ObjectParameters", arg1, arg2);
        }

        public void MixedArguments(int arg1, object arg2, string arg3)
        {
            TestMessages.RecordMethodBody(new object[] { arg1, arg2, arg3 });
            Console.WriteLine("WithParameters {0} {1} {2}", arg1, arg2, arg3);
        }

        public void WithLocalVariables()
        {
            TestMessages.RecordMethodBody(new object[0]);
            int x = 5;
            string guid = Guid.NewGuid().ToString();
            const string Format = "hello world {0} this is your number times 3: {1} and a guid: {2}";
            TestMessages.Record(string.Format(CultureInfo.InvariantCulture, Format, x, x * 3, guid));
        }

        public int ReturnsInteger()
        {
            return default(int);
        }

        public object ReturnsObject()
        {
            return default(object);
        }

        public float? ReturnsNullableFloat()
        {
            return default(float?);
        }
    }
}