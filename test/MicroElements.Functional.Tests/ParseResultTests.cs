using System;
using System.Threading.Tasks;
using FluentAssertions;
using MicroElements.Functional.Tests.Domain;
using Xunit;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional.Tests
{
    public class ResultTests
    {
        [Fact]
        public void SomeGeneratorTestsObject()
        {
            Result<int, Unit, string> parseResult = Result.Success(123);

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
        public async Task MatchAsync()
        {
            (await SuccessResult(2, "two")
                .ToTask()
                .MatchAsync((value, messages) => "success".ToTask(), (error, messages) => "fail".ToTask()))
                .Should().Be("success");
        }

        [Fact]
        public async Task BindAndMatchAsync()
        {
            (await SuccessResult(2, "two")
                    .ToTask()
                    .BindAsync(i => SuccessResult(i*2, "doubled").ToTask())
                    .MatchAsync((value, messages) => value.ToTask(), (error, messages) => 0.ToTask()))
                .Should().Be(4);
        }

        [Fact]
        public async Task BindAsync()
        {
            (await SuccessResult(2, "two")
                    .ToTask()
                    .BindAsync(i => SuccessResult(i * 2, "doubled").ToTask()))
                .Should().BeEquivalentTo((Result<int, Exception, string>)Result.Success(4));
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

        [Fact]
        public async Task SampleParserAsyncTest()
        {
            int result = await new SampleParser()
                .ParseBAsync("5;Text")
                .MatchAsync(
                    (value, messages) => Task.FromResult(1),
                    (error, messages) => Task.FromResult(2));
            result.Should().Be(1);
        }

        [Fact]
        public void LinqTest()
        {
            Result<int, string> Two = 2;
            Result<int, string> Four = 4;
            Result<int, string> Six = 6;
            Result<int, string> Error = "Error";

            var result =
                from x in Two
                from y in Four
                from z in Six
                select x + y + z;
            result.GetValueOrThrow().Should().Be(12);

            result =
                from x in Two
                from y in Four
                from _ in Error
                from z in Six
                select x + y + z;
            result.MatchError(error => error.Should().Be("Error"));
        }

        [Fact]
        public void LinqTest2()
        {
            Result<int, Exception, string> Two = 2.ToSuccess("Two");
            Result<int, Exception, string> Four = 4.ToSuccess("Four");
            Result<int, Exception, string> Six = 6.ToSuccess("Six");
            Result<int, Exception, string> Error = Result.Fail(new Exception("Error"));

            var result =
                from x in Two
                from y in Four
                from z in Six
                select x + y + z;
            result.GetValueOrThrow().Should().Be(12);

            result =
                from x in Two
                from y in Four
                from _ in Error
                from z in Six
                select x + y + z;
            result.MatchError((error, messages) => error.Should().BeEquivalentTo(new Exception("Error")));
        }
    }
}
