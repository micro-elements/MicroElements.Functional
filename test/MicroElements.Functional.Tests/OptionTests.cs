using System;
using System.Linq;
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
                some: i => (i == 123).Should().BeTrue(),
                none: () => throw new InvalidOperationException("Shouldn't get here"));

            int addOne = optional.Match(
                some: i => i + 1,
                none: () => 0);

            Assert.True(addOne == 124);
        }

        [Fact]
        public void Bind()
        {
            Some(3)
                .Bind(i => Some(i * 2))
                .GetValueOrThrow().Should().Be(6);
        }

        [Fact]
        public void NoneGeneratorTestsObject()
        {
            Option<int> optional = None;

            optional.Match(
                some: i => Assert.False(true, "Shouldn't get here"),
                none: () => Assert.True(true));

            int c = optional.Match(
                some: i => i + 1,
                none: () => 0);

            Assert.True(c == 0);
        }

        [Fact]
        public void SomeLinqTest()
        {
            var two = Some(2);
            var four = Some(4);
            var six = Some(6);

            var expr =
                from x in two
                from y in four
                from z in six
                select x + y + z;

            expr.Match(
                some: i => (i == 12).Should().BeTrue(),
                none: () => throw new InvalidOperationException("Shouldn't get here"));
        }

        [Fact]
        public void AssertNotInitialized()
        {
            Some<int> some = default;
            Static.GetValue(() => some.Value).Should().Throw<SomeNotInitializedException>();
        }

        [Fact]
        public void OptionalInterface()
        {
            var optional = Some("Hello");
            optional.IsSome.Should().BeTrue();
            optional.IsNone.Should().BeFalse();
            optional.GetUnderlyingType().Should().Be<string>();
            optional.MatchUntyped(
                value => value+" world",
                () => throw new InvalidOperationException())
                .Should().Be("Hello world");
            optional.AsEnumerable().Should().BeEquivalentTo("Hello");
            optional.ToList().Should().BeEquivalentTo("Hello");
            optional.Should().BeEquivalentTo(Some("Hello"));
            Equals(optional, Some("Hello")).Should().BeTrue();
            optional.ToString().Should().Be("Some(Hello)");
            optional.GetHashCode().Should().Be("Hello".GetHashCode());

            var notInitalizedOption = new Option<string>();
            Static.GetValue(() => (string)notInitalizedOption).Should().Throw<InvalidCastException>();
            notInitalizedOption.GetHashCode().Should().Be(0);
        }
    }
}
