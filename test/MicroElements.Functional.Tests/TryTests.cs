using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional.Tests
{
    public class TryTests
    {
        [Fact]
        public void TryTest()
        {
            Func<int> func1 = () => 1;
            Try(func1).Should().Be(SuccessResult(1));

            Func<int> func2 = () => throw new Exception();
            Result<int, Exception> result = Try(func2);
            result.IsFailed.Should().BeTrue();
            result.MatchError(error => error.Should().BeOfType<Exception>());

            Try(func1, exception => -1).Should().Be(SuccessResult(1));
            Try(func2, exception => -1).MatchError(error => error.Should().Be(-1));
        }

        [Fact]
        public async Task TryAsyncTest()
        {
            Func<Task<int>> func1 = () => 1.ToTask();
            (await TryAsync(func1)).Should().Be(SuccessResult(1));

            Func<Task<int>> func2 = () => throw new Exception();
            Result<int, Exception> result = await TryAsync(func2);
            result.IsFailed.Should().BeTrue();
            result.MatchError(error => error.Should().BeOfType<Exception>());

            (await TryAsync(func1, exception => -1)).Should().Be(SuccessResult(1));
            (await TryAsync(func2, exception => -1)).MatchError(error => error.Should().Be(-1));
        }
    }
}
