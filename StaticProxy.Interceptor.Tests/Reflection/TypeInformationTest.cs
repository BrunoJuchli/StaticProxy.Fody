namespace StaticProxy.Interceptor.Reflection
{
    using FluentAssertions;
    using Xunit;

    public class TypeInformationTest
    {
        private readonly TypeInformation testee;

        public TypeInformationTest()
        {
            this.testee = new TypeInformation();
        }
        
        [Fact]
        public void IsNullable_WhenIsReferenceType_MustReturnTrue()
        {
            this.testee.IsNullable(typeof(string))
                .Should().BeTrue();
        }

        [Fact]
        public void IsNullable_WhenIsNullableValueType_MustReturnTrue()
        {
            this.testee.IsNullable(typeof(int?))
                .Should().BeTrue();
        }

        [Fact]
        public void IsNullable_WhenIsNotNullableValueType_MustReturnFalse()
        {
            this.testee.IsNullable(typeof(int))
                .Should().BeFalse();
        }

        [Fact]
        public void IsNullable_WhenIsVoid_MustReturnTrue()
        {
            this.testee.IsNullable(typeof(void))
                .Should().BeTrue();
        }
    }
}