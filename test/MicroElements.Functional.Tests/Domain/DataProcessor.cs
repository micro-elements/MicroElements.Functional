
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

        public Result<string, string> ReadFile(string fileName)
        {
            return Try(() => File.ReadAllText(fileName), exception => exception.Message);
        }

        public Result<CustomerDto, string> TryParseCustomerDto(string source)
        {
            return Try(() => ParseCustomerDto(source), exception => exception.Message);
        }

        public CustomerDto ParseCustomerDto(string source)
        {
            return JsonConvert.DeserializeObject<CustomerDto>(source);
        }

        public IEnumerable<Message> ValidateCustomer(CustomerDto customer)
        {
            if (String.IsNullOrWhiteSpace(customer.Name))
                yield return ErrorMessage("Customer name is empty");
            if (String.IsNullOrWhiteSpace(customer.Email))
                yield return WarningMessage("Customer Email is empty");
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
}
