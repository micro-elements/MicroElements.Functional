using System.Threading.Tasks;
using FunctionalSample.App;

namespace FunctionalSample.Domain
{
    /// <summary>
    /// Command handler.
    /// </summary>
    /// <typeparam name="T">Command.</typeparam>
    public interface ICommandHandler<in T> where T : ICommand
    {
        /// <summary>
        /// Handle command.
        /// </summary>
        /// <param name="command">Command.</param>
        /// <returns>Task.</returns>
        Task Handle(T command);
    }
}
