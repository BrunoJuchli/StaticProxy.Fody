namespace SimpleTest.Integration
{
    using System;

    [StaticProxy]
    public class IntegrationThrowException
    {
        public void ThrowException(string message)
        {
            throw new InvalidOperationException(message);
        }

        public void RethrowException()
        {
            object foo = null;
            try
            {
                Console.WriteLine(foo.ToString());
            }
            catch (NullReferenceException ex)
            {
                throw new Exception("null reference", ex);
            }
        }
    }
}