using MicroElements.Functional;

namespace FunctionalSample.Domain
{
    /// <summary>
    /// <see cref="Customer"/> repository.
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Gets customer by id.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <returns>Optional customer.</returns>
        Option<Customer> FindById(string id);
    }
}
