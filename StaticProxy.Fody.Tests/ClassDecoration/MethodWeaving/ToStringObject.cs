namespace StaticProxy.Fody.Tests.ClassDecoration.MethodWeaving
{
    public class ToStringObject
    {
        private readonly string toString;

        public ToStringObject(string toString)
        {
            this.toString = toString;
        }

        public override string ToString()
        {
            return this.toString;
        }
    }
}