using System.Collections.Generic;

namespace MicroElements.Functional
{
    /// <summary>
    /// Message list.
    /// </summary>
    /// <typeparam name="TMessage">Message type.</typeparam>
    public interface IMessageList<TMessage> : IEnumerable<TMessage>
    {
        /// <summary>
        /// Gets messages count.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds message to list.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <returns>New instance of message list with added message.</returns>
        IMessageList<TMessage> Add(TMessage message);

        /// <summary>
        /// Appends other messages and returns new message list.
        /// </summary>
        /// <param name="other">Other messages.</param>
        /// <returns>New list that contains current messages and other messages.</returns>
        IMessageList<TMessage> AddRange(IEnumerable<TMessage> other);
    }
}
