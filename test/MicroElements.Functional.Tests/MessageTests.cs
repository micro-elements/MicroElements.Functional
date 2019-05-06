using System;
using System.IO;
using System.Text;
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
    }

    public class MessageTemplateTests
    {
        [Theory]
        [InlineData("User {Name} created in {Elapsed} ms.", new object[]{ "Alex", 5 }, "User Alex created in 5 ms.")]
        [InlineData("User {Name, 5} created in {Elapsed:000} ms.", new object[] { "Alex", 5 }, "User Alex created in 005 ms.")]
        public void ParseTemplate(string messageTemplate, object[] args, string expected)
        {
            var template = new MessageTemplateParser().Parse(messageTemplate);
            string render = new MessageTemplateRenderer().RenderToString(template, args);
            render.Should().Be(expected);
        }
    }
}
