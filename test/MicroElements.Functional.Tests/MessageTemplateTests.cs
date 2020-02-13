using System;
using FluentAssertions;
using Xunit;

namespace MicroElements.Functional.Tests
{
    public class MessageTemplateTests
    {
        [Theory]
        [InlineData("User {Name} created in {Elapsed} ms.", new object[]{ "Alex", 5 }, "User Alex created in 5 ms.")]
        [InlineData("User {Name, 5} created in {Elapsed:000} ms.", new object[] { "Alex", 5 }, "User  Alex created in 005 ms.")]
        [InlineData("User {Name,5:upper():trim(4)} created in {Elapsed, -4: 000} ms.", new object[] { "Alexander", 5 }, "User  ALEX created in 005  ms.")]
        [InlineData("User {0} created in {1} ms.", new object[] { "Alex", 5 }, "User Alex created in 5 ms.")]
        [InlineData("User {0} created in {1} ms. Hello {0}!", new object[] { "Alex", 5 }, "User Alex created in 5 ms. Hello Alex!")]
        [InlineData("User {Name} created in {Elapsed} ms. Hello {Name}!", new object[] { "Alex", 5 }, "User Alex created in 5 ms. Hello Alex!")]
        public void ParseAndRender(string messageTemplate, object[] args, string expected)
        {
            var template = MessageTemplateParser.Instance.Parse(messageTemplate);
            string render = MessageTemplateRenderer.Instance.RenderToString(template, args);
            render.Should().Be(expected);
        }

        [Fact]
        public void RenderDateTime()
        {
            ParseAndRender(
                "Date: {date:yyyy-MM-dd}, Time: {date:HH:mm:ss}",
                new object[] {new DateTime(2019, 05, 09, 10, 40, 55)},
                "Date: 2019-05-09, Time: 10:40:55");
        }
    }
}
