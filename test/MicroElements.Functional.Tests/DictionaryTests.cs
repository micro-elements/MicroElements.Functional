
using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace MicroElements.Functional.Tests
{
    public class DictionaryTests
    {
        [Fact]
        public void GetValueAsOptionTests()
        {
            IDictionary<int, string> numberNames = new Dictionary<int, string>()
            {
                {1, "one"},
                {2, "two"}
            };
            var one = numberNames.GetValueAsOption(1);
            one.IsSome.Should().BeTrue();
            one.Should().BeEquivalentTo("one");

            numberNames.GetValueAsOption(2)
                .Match(two => two.Should().Be("two"),
                    () => throw new InvalidOperationException("Should never be here!"));

            var three = numberNames.GetValueAsOption(3);
            three.IsSome.Should().BeFalse();
        }

        [Fact]
        public void GetValueAsResultTests()
        {
            IDictionary<int, string> numberNames = new Dictionary<int, string>()
            {
                {1, "one"},
                {2, "two"}
            };
            var one = numberNames.GetValueAsResult(1);
            one.IsSuccess.Should().BeTrue();
            one.Should().Be("one");

            numberNames.GetValueAsResult(2)
                .Match(two => two.Should().Be("two"),
                    error => throw new InvalidOperationException("Should never be here!"));

            var three = numberNames.GetValueAsResult(3);
            three.IsSuccess.Should().BeFalse();
            three.MatchError(error => error.ErrorMessage.Should().Be("Key 3 is not exists in dictionary."));
        }
    }
}
