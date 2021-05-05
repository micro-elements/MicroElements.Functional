// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using MicroElements.CodeContracts;

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
        public ValueWithMessages(A value, IEnumerable<Message> messages)
        {
            value.AssertArgumentNotNull(nameof(value));
            messages.AssertArgumentNotNull(nameof(messages));

            Value = value;
            Messages = new MessageList<Message>(messages);
        }

        /// <summary>
        /// Creates value with messages.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="message">Single Message.</param>
        public ValueWithMessages(A value, Message message)
        {
            value.AssertArgumentNotNull(nameof(value));
            message.AssertArgumentNotNull(nameof(message));

            Value = value;
            Messages = new MessageList<Message>(message);
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

    public static class ValueWithMessagesExtensions
    {
        /// <summary>
        /// Creates new <see cref="ValueWithMessages{A,Message}"/> by value and messages.
        /// </summary>
        /// <typeparam name="A">Value type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <param name="value">Source value.</param>
        /// <param name="messages">Messages.</param>
        /// <returns><see cref="ValueWithMessages{A,Message}"/>.</returns>
        public static ValueWithMessages<A, Message> ValueWithMessages<A, Message>(this A value, params Message[] messages) =>
            new ValueWithMessages<A, Message>(value, messages);
    }
}
