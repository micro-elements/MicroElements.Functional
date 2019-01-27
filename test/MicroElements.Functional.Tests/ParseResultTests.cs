using System;
using System.Linq;
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
            var parseResult = ParseResult.Success(123, EmptyMessageList);

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
            var parseResult =
                SuccessParseResult(2, "first")
                    .Bind(i => SuccessParseResult(i * 2, "second"));

            parseResult.Match(
                success: (i, list) => (i).Should().Be(4),
                error: (list) => throw new InvalidOperationException("Shouldn't get here"));

            int addOne = parseResult.Match(
                success: (i, list) => i + 1,
                error: (list) => 0);

            addOne.Should().Be(5);
        }

        [Fact]
        public void SampleParserTest()
        {
            ParseResult<ParsedObject, string> parseResult = new SampleParser().Parse("5;Text");
            parseResult.IsSuccess.Should().BeTrue();
            parseResult.IsError.Should().BeFalse();
            parseResult.Messages.Should().BeEquivalentTo("A parsed", "B parsed");
            parseResult
                .MatchSuccess((parsed, list) => parsed.A.Should().Be(5))
                .MatchSuccess((parsed, list) => parsed.B.Should().Be("Text"));
        }

        [Fact]
        public void SampleParserErrorTest()
        {
            ParseResult<ParsedObject, string> parseResult = new SampleParser().Parse("AAA;Text");
            parseResult.IsSuccess.Should().BeFalse();
            parseResult.IsError.Should().BeTrue();
            parseResult.Messages.Should().BeEquivalentTo("AAA can not be parsed to int", "B parsed");
        }
    }

    public class SampleParser
    {
        public ParseResult<ParsedObject, string> Parse(string source)
        {
            ParsedObject parsedObject = new ParsedObject();

            IMessageList<string> errors = MessageList<string>.Empty;
            IMessageList<string> messageList = MessageList<string>.Empty;
            ParseA(source)
                .MatchSuccess((a, list) => parsedObject.A = a)
                .MatchError(list => errors = errors.AddRange(list))
                .MatchMessages(list => messageList = messageList.AddRange(list));

            ParseB(source)
                .MatchSuccess((b, list) => parsedObject.B = b)
                .MatchError(list => errors = errors.AddRange(list))
                .MatchMessages(list => messageList = messageList.AddRange(list));

            return errors.Count == 0? parsedObject.ToSuccess(messageList) : (MessageList<string>)messageList;
        }

        public ParseResult<int, string> ParseA(string source)
        {
            var src = source.Split(";").FirstOrDefault();
            return ParseInt(src)
                .Match(
                    some: i => i.ToSuccess("A parsed"),
                    none:() => $"{src} can not be parsed to int");
        }

        public ParseResult<string, string> ParseB(string source)
        {
            var src = source.Split(";").LastOrDefault();
            return src.ToSuccess("B parsed");
        }
    }

    public class ParsedObject
    {
        public int A { get; set; }
        public string B { get; set; }
    }
}
