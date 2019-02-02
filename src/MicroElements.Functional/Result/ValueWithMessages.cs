using System;

namespace MicroElements.Functional
{
    public class ValueWithMessages<TValue, TMessage>
    {
        /// <summary>
        /// Value.
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// Message list.
        /// </summary>
        public IMessageList<TMessage> Messages { get; }

        /// <summary>
        /// Creates value with messages.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="messages">Message list.</param>
        public ValueWithMessages(TValue value, IMessageList<TMessage> messages = null)
        {
            if (value.IsNull())
                throw new ArgumentNullException(nameof(value), "Cannot use null for ValueWithMessages");
            Value = value;
            Messages = messages ?? MessageList<TMessage>.Empty;
        }
    }
}