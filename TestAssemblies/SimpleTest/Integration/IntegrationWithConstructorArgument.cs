namespace SimpleTest.Integration
{
    using System;

    [StaticProxy]
    public class IntegrationWithConstructorArgument
    {
        private readonly IDependency dependency;

        private int internalNumber;

        public IntegrationWithConstructorArgument(IDependency dependency)
        {
            this.dependency = dependency;
        }

        public void InitializeInternalNumberFromDependency()
        {
            this.internalNumber = this.dependency.Number;
        }

        public void MultiplyInternalNumberByThree()
        {
            this.internalNumber *= 3;
        }

        public void MultiplyInternalNumber(int by)
        {
            this.internalNumber *= by;
        }

        public void AssertInternalNumberIs(int expectedValue)
        {
            if (this.internalNumber != expectedValue)
            {
                throw new Exception(string.Format("internalNumber was expected to be {0} but is {1}", expectedValue, this.internalNumber));
            }
        }
    }
}