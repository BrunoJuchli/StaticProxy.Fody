namespace IntegrationTests
{
    public class OverrideToString
    {
        private readonly string myString;

        public OverrideToString(string myString)
        {
            this.myString = myString;
        }

        public override string ToString()
        {
            return this.myString;
        }
    }
}