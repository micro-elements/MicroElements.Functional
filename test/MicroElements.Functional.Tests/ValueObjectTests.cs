using System.Collections.Generic;
using FluentAssertions;
using MicroElements.CodeContracts;
using Xunit;

namespace MicroElements.Functional.Tests
{
    /// <summary>
    /// ValueObjectTests.
    /// </summary>
    public class ValueObjectTests
    {
        [Fact]
        public void ValueObjectToString()
        {
            Address address = new Address("USA", "New York", "NY", 10118, "350 5th Ave", null);
            address.ToString().Should().Be("(USA, NEW YORK, NY, 10118, 350 5TH AVE, null)");
        }

        [Fact]
        public void ValueObjectToStringFormattable()
        {
            var name = new FormattableName("Bill", "Gates");
            name.ToString().Should().Be("{FirstName: \"Bill\", LastName: \"Gates\"}");
        }

        [Fact]
        public void TwoValueObjectsShouldBeEquivalent()
        {
            Address address1 = new Address("USA", "New York", "NY", 10118, "350 5th Ave", null);
            Address address2 = new Address("USA", "New York", "NY", 10118, "350 5th Ave", null);
            
            address2.Should().NotBeSameAs(address1);
            address2.Should().Be(address1);
            address2.Should().BeEquivalentTo(address1);

            (address2 == address1).Should().BeTrue();
            (address2 != address1).Should().BeFalse();

            address2.GetHashCode().Should().Be(address1.GetHashCode(), "GetHashCode should be tha same");
        }

        [Fact]
        public void TwoValueObjectsWithDifferentCasingShouldBeEquivalent()
        {
            Address address1 = new Address("USA", "New York", "NY", 10118, "350 5th Ave", null);
            Address address2 = new Address("usa", "new york", "ny", 10118, "350 5th ave", null);

            address2.Should().NotBeSameAs(address1);
            address2.Should().Be(address1);
            address2.Should().BeEquivalentTo(address1);

            (address2 == address1).Should().BeTrue();
            (address2 != address1).Should().BeFalse();

            address2.GetHashCode().Should().Be(address1.GetHashCode());
        }

        [Fact]
        public void TwoDifferentValueObjectsShouldNotBeEquivalent()
        {
            Address address1 = new Address("USA", "New York", "NY", 10118, "350 5th Ave", null);
            Address address2 = new Address("USA", "New York", "NY", 10118, "351 5th Ave", null);

            address2.Should().NotBeSameAs(address1);
            address2.Should().NotBe(address1);

            (address2 == address1).Should().BeFalse();
            (address2 != address1).Should().BeTrue();

            address2.GetHashCode().Should().NotBe(address1.GetHashCode());
        }
    }

    /// <summary>
    /// Address for delivery.
    /// </summary>
    public class Address : ValueObject
    {
        public string Country { get; }
        public string City { get; }
        public string State { get; }
        public int ZipCode { get; }

        public string StreetLine1 { get; }
        public string StreetLine2 { get; }

        public Address(string country, string city, string state, int zipCode, string streetLine1, string streetLine2)
        {
            //TODO: Domain checks
            Country = country;
            City = city;
            State = state;
            ZipCode = zipCode;
            StreetLine1 = streetLine1;
            StreetLine2 = streetLine2;
        }

        /// <inheritdoc />
        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Country?.ToUpperInvariant();
            yield return City?.ToUpperInvariant();
            yield return State?.ToUpperInvariant();
            yield return ZipCode;
            yield return StreetLine1?.ToUpperInvariant();
            yield return StreetLine2?.ToUpperInvariant();
        }
    }

    public class FormattableName : ValueObject, IFormattableObject
    {
        public string FirstName { get; }
        public string LastName { get; }

        /// <inheritdoc />
        public FormattableName(string firstName, string lastName)
        {
            FirstName = firstName.AssertArgumentNotNull(nameof(firstName));
            LastName = lastName.AssertArgumentNotNull(nameof(lastName));
        }

        /// <inheritdoc />
        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName.ToUpperInvariant();
            yield return LastName.ToUpperInvariant();
        }

        /// <inheritdoc />
        public IEnumerable<(string Name, object Value)> GetNameValuePairs()
        {
            yield return ("FirstName", FirstName);
            yield return ("LastName", LastName);
        }
    }
}
