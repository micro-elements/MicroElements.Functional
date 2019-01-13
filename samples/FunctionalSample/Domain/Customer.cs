using System;
using System.Collections.Generic;
using MicroElements.Design.Annotations;
using MicroElements.Functional;
using NodaTime;

namespace FunctionalSample.Domain
{
    /// <summary>
    /// Customer.
    /// </summary>
    [Model(Convention = ModelConvention.DomainModel)]
    public class Customer : IEntity<CustomerId>
    {
        public CustomerId Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public LocalDate BirthDate { get; }
        public Address Address { get; }

        /// <summary>
        /// Creates new instance of Customer.
        /// </summary>
        public Customer(CustomerId id, string firstName, string lastName, LocalDate birthDate, Address address)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            Address = address;
        }
    }

    [Model(Convention = ModelConvention.DTO)]
    public class CustomerState
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public LocalDate BirthDate { get; set; }
        public Address Address { get; set; }
    }

    public class CustomerId : ValueObject
    {
        public string Email { get; }

        /// <inheritdoc />
        public CustomerId(string email)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }

        /// <inheritdoc />
        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Email;
        }
    }

    public class Order
    {
        public string Id { get; }
    }

    public class OrderLine
    {

    }
}
