namespace IntegrationTests.ProxyWithTarget
{
    [StaticProxy]
    public class IntegrationWithReturnValue
    {
        public string CombineToStrings(object obj1, object obj2)
        {
            return string.Concat(obj1.ToString(), obj2.ToString());
        }

        public int Multiply(int i, int by)
        {
            return i * by;
        }
    }
}