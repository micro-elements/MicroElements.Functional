using System.Collections.Generic;
using MicroElements.Design.Annotations;
using MicroElements.Functional;

namespace FunctionalSample.Domain
{
    /// <summary>
    /// Address for delivery.
    /// </summary>
    [Model(Convention = ModelConvention.ValueObject)]
    public class Address : ValueObject
    {
        public string Country { get; }
        public string City { get; }
        public string State { get; }
        public string ZipCode { get; }

        public string StreetLine1 { get; }
        public string StreetLine2 { get; }

        public string FullName { get; }
        public string PhoneNumber { get; }

        public Address(string country, string city, string state, string zipCode, string streetLine1, string streetLine2, string fullName, string phoneNumber)
        {
            //TODO: Domain checks
            Country = country;
            City = city;
            State = state;
            ZipCode = zipCode;
            StreetLine1 = streetLine1;
            StreetLine2 = streetLine2;
            FullName = fullName;
            PhoneNumber = phoneNumber;
        }

        /// <inheritdoc />
        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Country;
            yield return City;
            yield return State;
            yield return ZipCode;
            yield return StreetLine1;
            yield return StreetLine2;
            yield return FullName;
            yield return PhoneNumber;
        }
    }
}
