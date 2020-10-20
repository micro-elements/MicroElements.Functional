using System;
using FluentAssertions;
using Xunit;

namespace MicroElements.Functional.Tests
{
    public class ReflectionTests
    {
        [Fact]
        public void test_default_values()
        {
            typeof(int).GetDefaultValue().Should().BeEquivalentTo(0);
            typeof(object).GetDefaultValue().Should().BeNull();
            typeof(Nullable<int>).GetDefaultValue().Should().BeNull();
        }
    }
}
