// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents not null value with message list.
    /// </summary>
    /// <typeparam name="A">A value type.</typeparam>
    /// <typeparam name="Message">Message type.</typeparam>
    public readonly struct ValueWithMessages<A, Message>
    {
        /// <summary>
        /// Value.
        /// </summary>
        public readonly A Value;

        /// <summary>
        /// Message list.
        /// </summary>
        public readonly IMessageList<Message> Messages;

        /// <summary>
        /// Creates value with messages.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="messages">Message list.</param>
        public ValueWithMessages(A value, IMessageList<Message> messages)
        {
            Value = value.AssertArgumentNotNull(nameof(value));
            Messages = messages.AssertArgumentNotNull(nameof(messages));
        }

        /// <summary>
        /// Deconstructs values.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="messages">Message list.</param>
        public void Deconstruct(out A value, out IMessageList<Message> messages)
        {
            value = Value;
            messages = Messages;
        }
    }
}
