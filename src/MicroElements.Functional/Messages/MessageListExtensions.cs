using System.Collections.Generic;

namespace MicroElements.Functional
{
    /// <summary>
    /// MessageListExtensions.
    /// </summary>
    public static class MessageListExtensions
    {
        /// <summary>
        /// Creates MessageList from <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <param name="messages">Messages.</param>
        /// <returns>MessageList.</returns>
        public static MessageList<Message> ToMessageList<Message>(this IEnumerable<Message> messages)
            => MessageList.FromEnumerable(messages);

        /// <summary>
        /// Creates MessageList with one message.
        /// </summary>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <param name="message">Single message.</param>
        /// <returns>MessageList.</returns>
        public static MessageList<Message> ToMessageList<Message>(this Message message)
            => new MessageList<Message>(message);
    }
}