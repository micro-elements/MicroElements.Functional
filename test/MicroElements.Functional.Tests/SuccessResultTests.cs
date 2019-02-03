using System;
using FluentAssertions;
using Xunit;

namespace MicroElements.Functional.Tests
{
    public class SuccessResultTests
    {
        [Fact]
        public void FactoryTest()
        {
            var successResult = Result.Success(123);
            successResult.Should().BeOfType<SuccessResult<int>>();
            successResult.IsSuccess.Should().BeTrue();
            successResult.IsFailed.Should().BeFalse();
            successResult.Value.Should().Be(123);
            successResult.GetSuccessValueType().Should().Be(typeof(int));
            successResult.GetErrorValueType().Should().Be(typeof(Unit));
        }

        [Fact]
        public void Uninitialized()
        {
            SuccessResult<int> successResult = new SuccessResult<int>();
            successResult.Should().BeOfType<SuccessResult<int>>();
            successResult.IsSuccess.Should().BeFalse();
            successResult.IsFailed.Should().BeTrue();
            Static.GetValue(() => successResult.Value).Should().Throw<NotInitializedException>();
        }

        [Fact]
        public void ImplicitConversion()
        {
            Result<int, Unit, string> result = Result.Success(123);
            result.IsSuccess.Should().BeTrue();
            result.GetValueOrThrow().Should().Be(123);
            result.GetSuccessValueType().Should().Be(typeof(int));
            result.GetErrorValueType().Should().Be(typeof(Unit));
            result.GetMessageType().Should().Be(typeof(string));

            Result<long, Exception, IMessage> result2 = Result.Success(123L);
            result2.IsSuccess.Should().BeTrue();
            result2.GetValueOrThrow().Should().Be(123);
            result2.GetSuccessValueType().Should().Be(typeof(long));
            result2.GetErrorValueType().Should().Be(typeof(Exception));
            result2.GetMessageType().Should().Be(typeof(IMessage));

            Result<int, Exception> result3 = Result.Success(123);
            result3.IsSuccess.Should().BeTrue();
            result3.GetValueOrThrow().Should().Be(123);
            result3.GetSuccessValueType().Should().Be(typeof(int));
            result3.GetErrorValueType().Should().Be(typeof(Exception));
        }
    }
}
