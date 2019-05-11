using System;
using FluentAssertions;
using Xunit;

namespace MicroElements.Functional.Tests
{
    public class MessageTests
    {
        [Fact]
        public void EqualsTest()
        {
            var now = DateTimeOffset.Now;
            Message message1 = new Message("Hello", timestamp: now);
            Message message2 = new Message("Hello", timestamp: now);
            message2.Should().BeEquivalentTo(message1);
        }

        [Fact]
        public void Test2()
        {
            var message = new Message("User {Name} created in {Elapsed} ms.")
                .WithProperty("Name", "Alex")
                .WithProperty("Elapsed", 145);

            message.FormattedMessage.Should().Be("User Alex created in 145 ms.");
            message.GetProperty("Name").GetValueOrThrow().Should().Be("Alex");
            message.GetProperty("Elapsed").GetValueOrThrow().Should().Be(145);
        }


        [Fact]
        public void Test3()
        {
            var timestamp = new DateTimeOffset(2019, 05, 09, 10, 40, 55, TimeSpan.Zero);
            var message = new Message("{Timestamp:yyyy-MM-ddTHH:mm:ss.fff} | {Severity:upper():trim(4)} | User {Name} created in {Elapsed} ms.", MessageSeverity.Warning, timestamp)
                .WithProperty("Name", "Alex")
                .WithProperty("Elapsed", 145);

            message.FormattedMessage.Should().Be("2019-05-09T10:40:55.000 | WARN | User Alex created in 145 ms.");

        }
    }
}
