using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        public void formatted_message_should_be_equal_to_original()
        {
            var message = new Message("User Alex created.")
                .WithProperty("Name", "Alex")
                .WithProperty("Elapsed", 145);

            message.FormattedMessage.Should().Be("User Alex created.");
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

        [Fact]
        public void message_should_be_serialized()
        {
            var message = new Message("User {Name} created.")
                .WithProperty("Name", "Alex");

            var memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize(memoryStream, message);
            memoryStream.Position = 0;

            var messageRestored = (Message)new BinaryFormatter().Deserialize(memoryStream);
            messageRestored.OriginalMessage.Should().Be(message.OriginalMessage);
            messageRestored.FormattedMessage.Should().Be(message.FormattedMessage);

            ((object)message).Should().BeBinarySerializable();
            //((object)message).Should().BeDataContractSerializable();
            //((object)message).Should().BeXmlSerializable();
        }

        [Fact]
        public void message_memoization()
        {
            var message = new Message("User {Name} created.")
                .WithProperty("Name", "Alex");

            var keys = message.Keys.ToArray();
            var values = message.Values.ToArray();
            keys.Length.Should().Be(5);

            message.GetMessageTemplate().Should().BeSameAs(message.GetMessageTemplate());
        }
    }
}
