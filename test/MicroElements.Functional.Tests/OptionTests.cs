using System;
using FluentAssertions;
using Xunit;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional.Tests
{
    public class OptionTests
    {
        [Fact]
        public void SomeGeneratorTestsObject()
        {
            var optional = Some(123);

            optional.Match(
                Some: i => (i == 123).Should().BeTrue(),
                None: () => throw new InvalidOperationException("Shouldn't get here"));

            int addOne = optional.Match(
                Some: i => i + 1,
                None: () => 0);

            Assert.True(addOne == 124);
        }

        [Fact]
        public void NoneGeneratorTestsObject()
        {
            Option<int> optional = None;

            optional.Match(
                Some: i => Assert.False(true, "Shouldn't get here"),
                None: () => Assert.True(true));

            int c = optional.Match(
                Some: i => i + 1,
                None: () => 0);

            Assert.True(c == 0);
        }
    }
}
