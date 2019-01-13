using FunctionalSample.App;

namespace FunctionalSample.Domain.Commands
{
    /// <summary>
    /// ChangeCustomerAddressCommand.
    /// </summary>
    public interface IChangeCustomerAddressCommand : ICommand
    {
        /// <summary>
        /// The customer id.
        /// </summary>
        string CustomerId { get; }

        /// <summary>
        /// New customer address.
        /// </summary>
        Address NewAddress { get; }
    }


    public class ChangeCustomerAddressCommand : IChangeCustomerAddressCommand
    {
        /// <inheritdoc />
        public string CustomerId { get; set; }

        /// <inheritdoc />
        public Address NewAddress { get; set; }
    }
}
