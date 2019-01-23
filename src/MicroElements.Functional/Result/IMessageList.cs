using System.Collections.Generic;

namespace MicroElements.Functional.Result
{
    /// <summary>
    /// Message list.
    /// </summary>
    /// <typeparam name="TMessage">Message type.</typeparam>
    public interface IMessageList<TMessage> : IEnumerable<TMessage>
    {
        /// <summary>
        /// Adds message to list.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <returns>New instance of message list with added message.</returns>
        IMessageList<TMessage> Add(TMessage message);

        IMessageList<TMessage> Concat(IEnumerable<TMessage> other);
    }
}
