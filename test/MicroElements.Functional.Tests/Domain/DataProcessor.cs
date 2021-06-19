
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional.Tests.Domain
{
    public class DataProcessor
    {
        public void Process(string fileName)
        {
            ReadFile(fileName)
                .Bind(TryParseCustomerDto)
                .Validate(ValidateCustomer)
                .Match(
                    (value, messages) => Console.WriteLine("Ok"),
                    (error, messages) => Console.WriteLine("Error"));
        }

        public Result<string, ErrorMessage> ReadFile(string fileName)
        {
            return Try(() => File.ReadAllText(fileName), exception => new ErrorMessage(exception.Message));
        }

        public Result<CustomerDto, ErrorMessage> TryParseCustomerDto(string source)
        {
            return Try(() => ParseCustomerDto(source), exception => new ErrorMessage(exception.Message));
        }

        public CustomerDto ParseCustomerDto(string source)
        {
            return JsonConvert.DeserializeObject<CustomerDto>(source);
        }

        public IEnumerable<ErrorMessage> ValidateCustomer(CustomerDto customer)
        {
            if (String.IsNullOrWhiteSpace(customer.Name))
                yield return ("Customer name is empty");
            if (String.IsNullOrWhiteSpace(customer.Email))
                yield return ("Customer Email is empty");
        }
    }

    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class Customer
    {
        public int Id { get; }
        public string Name { get; }
        public string Email { get; }
    }

    public class ErrorMessage : ICanBeError
    {
        public string Text { get; }

        public ErrorMessage(string text)
        {
            Text = text;
        }

        public static implicit operator ErrorMessage(string value) => new ErrorMessage(value);

        /// <inheritdoc />
        public bool IsError => true;
    }
}
