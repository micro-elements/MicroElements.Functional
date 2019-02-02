using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional.Tests
{
    public class SampleParser
    {
        public Result<ParsedObject, Exception, string> Parse(string source)
        {
            ParsedObject parsedObject = new ParsedObject();

            IMessageList<string> errors = MessageList<string>.Empty;
            IMessageList<string> messageList = MessageList<string>.Empty;
            ParseA(source)
                .MatchSuccess((a, list) => parsedObject.A = a)
                .MatchError((e, list) => errors = errors.AddRange(list))
                .MatchMessages(list => messageList = messageList.AddRange(list));

            ParseB(source)
                .MatchSuccess((b, list) => parsedObject.B = b)
                .MatchError((e, list) => errors = errors.AddRange(list))
                .MatchMessages(list => messageList = messageList.AddRange(list));

            return errors.Count == 0
                ? parsedObject.ToSuccess(messageList) 
                : Result.FailFromMessages<ParsedObject>(messageList);
        }

        public Result<int, Exception, string> ParseA(string source)
        {
            var src = source.Split(";").FirstOrDefault();
            return ParseInt(src)
                .Match(
                    some: i => i.ToSuccess("A parsed"),
                    none:() => $"{src} can not be parsed to int");
        }

        public Result<string, Exception, string> ParseB(string source)
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

    public class ResultWithMessagesTests
    {
        [Fact]
        public void SomeGeneratorTestsObject()
        {
            var parseResult = Result.Success(123);

            parseResult.Match(
                success: (i, list) => (i == 123).Should().BeTrue(),
                error: (e, list) => throw new InvalidOperationException("Shouldn't get here"));

            int addOne = parseResult.Match(
                success: (i, list) => i + 1,
                error: (e, list) => 0);

            Assert.True(addOne == 124);
        }

        [Fact]
        public void Bind()
        {
            var parseResult =
                SuccessResult(2, "first")
                    .Bind(i => SuccessResult(i * 2, "second"));

            parseResult.Match(
                success: (i, list) => (i).Should().Be(4),
                error: (e, list) => throw new InvalidOperationException("Shouldn't get here"));

            int addOne = parseResult.Match(
                success: (i, list) => i + 1,
                error: (e, list) => 0);

            parseResult.MatchMessages(list => list.Should().BeEquivalentTo("first", "second"));

            addOne.Should().Be(5);
        }

        [Fact]
        public void SampleParserTest()
        {
            var parseResult = new SampleParser().Parse("5;Text");
            parseResult.IsSuccess.Should().BeTrue();
            parseResult.IsFailed.Should().BeFalse();
            parseResult.Messages.Should().BeEquivalentTo("A parsed", "B parsed");
            parseResult
                .MatchSuccess((parsed, list) => parsed.A.Should().Be(5))
                .MatchSuccess((parsed, list) => parsed.B.Should().Be("Text"));
        }

        [Fact]
        public void SampleParserErrorTest()
        {
            var parseResult = new SampleParser().Parse("AAA;Text");
            parseResult.IsSuccess.Should().BeFalse();
            parseResult.IsFailed.Should().BeTrue();
            parseResult.Messages.Should().BeEquivalentTo("AAA can not be parsed to int", "B parsed");
        }
    }
}
