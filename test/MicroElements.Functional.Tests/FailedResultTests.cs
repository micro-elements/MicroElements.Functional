using FluentAssertions;
using MicroElements.Functional.Tests.Domain;
using Xunit;

namespace MicroElements.Functional.Tests
{
    public class FailedResultTests
    {
        [Fact]
        public void FactoryTest()
        {
            var failedResult = Result.Fail(DomainError.ClientNotFound);
            failedResult.Should().BeOfType<FailedResult<DomainError>>();
            failedResult.IsSuccess.Should().BeFalse();
            failedResult.IsFailed.Should().BeTrue();
            failedResult.ErrorValue.Should().Be(DomainError.ClientNotFound);
        }

        [Fact]
        public void Uninitialized()
        {
            FailedResult<DomainError> failedResult = new FailedResult<DomainError>();
            failedResult.Should().BeOfType<FailedResult<DomainError>>();
            failedResult.IsSuccess.Should().BeFalse();
            failedResult.IsFailed.Should().BeTrue();
            Static.GetValue(() => failedResult.ErrorValue).Should().Throw<NotInitializedException>();
        }

        [Fact]
        public void ImplicitConversion()
        {
            Result<int, DomainError, string> result = Result.Fail(DomainError.ClientNotFound);
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.MatchError((error, list) => error.Should().Be(DomainError.ClientNotFound));
            result.GetSuccessValueType().Should().Be(typeof(int));
            result.GetErrorValueType().Should().Be(typeof(DomainError));
            result.GetMessageType().Should().Be(typeof(string));

            Result<long, DomainError, IMessage> result2 = Result.Fail(DomainError.ClientNotFound);
            result2.IsSuccess.Should().BeFalse();
            result2.IsFailed.Should().BeTrue();
            result2.MatchError((error, list) => error.Should().Be(DomainError.ClientNotFound));
            result2.GetSuccessValueType().Should().Be(typeof(long));
            result2.GetErrorValueType().Should().Be(typeof(DomainError));
            result2.GetMessageType().Should().Be(typeof(IMessage));

            Result<int, DomainError> result3 = Result.Fail(DomainError.ClientNotFound);
            result3.IsSuccess.Should().BeFalse();
            result3.IsFailed.Should().BeTrue();
            result3.MatchError((error) => error.Should().Be(DomainError.ClientNotFound));
            result3.GetSuccessValueType().Should().Be(typeof(int));
            result3.GetErrorValueType().Should().Be(typeof(DomainError));
        }
    }

    public interface IMessage
    {
    }
}
