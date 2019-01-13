using System;
using System.Threading.Tasks;
using FunctionalSample.Domain;
using FunctionalSample.Domain.Commands;

namespace FunctionalSample.App
{
    /// <summary>
    /// ChangeCustomerAddressCommandHandler.
    /// </summary>
    public class ChangeCustomerAddressCommandHandler : ICommandHandler<IChangeCustomerAddressCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        /// <summary>
        /// Creates new instance of <see cref="ChangeCustomerAddressCommandHandler"/>.
        /// </summary>
        /// <param name="customerRepository">Customer repository.</param>
        public ChangeCustomerAddressCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        /// <inheritdoc />
        public async Task Handle(IChangeCustomerAddressCommand command)
        {
        }
    }
}
