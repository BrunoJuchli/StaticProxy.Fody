namespace IntegrationTests.ProxyWithTarget
{
    internal class Dependency : IDependency
    {
        public Dependency(int number)
        {
            this.Number = number;
        }

        public int Number { get; private set; }
    }
}