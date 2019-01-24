using System;
using FluentAssertions;
using Xunit;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional.Tests
{
    public class ParseResultTests
    {
        [Fact]
        public void SomeGeneratorTestsObject()
        {
            var parseResult = ParseResult2.Success<int, string>(123, EmptyMessageList<string>());

            parseResult.Match(
                success: (i, list) => (i == 123).Should().BeTrue(),
                error: (list) => throw new InvalidOperationException("Shouldn't get here"));

            int addOne = parseResult.Match(
                success: (i, list) => i + 1,
                error: (list) => 0);

            Assert.True(addOne == 124);
        }

        [Fact]
        public void Bind()
        {
            var parseResult = ParseResult2.Success<int, string>(2, new MessageList<string>("first"))
            .Bind(i => ParseResult2.Success(i*2, new MessageList<string>("second")));

            parseResult.Match(
                success: (i, list) => (i == 4).Should().BeTrue(),
                error: (list) => throw new InvalidOperationException("Shouldn't get here"));

            int addOne = parseResult.Match(
                success: (i, list) => i + 1,
                error: (list) => 0);

            Assert.True(addOne == 124);
        }
    }
}
