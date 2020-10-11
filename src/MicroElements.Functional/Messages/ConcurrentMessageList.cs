// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MicroElements.Functional
{
    /// <summary>
    /// Concurrent mutable message list.
    /// </summary>
    /// <typeparam name="TMessage">Message type.</typeparam>
    public class ConcurrentMessageList<TMessage> : IMutableMessageList<TMessage>
    {
        private readonly ConcurrentQueue<TMessage> _messages = new ConcurrentQueue<TMessage>();

        /// <inheritdoc />
        public IEnumerator<TMessage> GetEnumerator() => _messages.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_messages).GetEnumerator();

        /// <inheritdoc />
        public int Count => _messages.Count;

        /// <inheritdoc />
        public IMutableMessageList<TMessage> Add(TMessage message)
        {
            _messages.Enqueue(message);
            return this;
        }

        /// <inheritdoc />
        public IMutableMessageList<TMessage> AddRange(IEnumerable<TMessage> messages)
        {
            foreach (var message in messages)
            {
                _messages.Enqueue(message);
            }

            return this;
        }

        /// <inheritdoc />
        IMessageList<TMessage> IMessageList<TMessage>.Add(TMessage message) => Add(message);

        /// <inheritdoc />
        IMessageList<TMessage> IMessageList<TMessage>.AddRange(IEnumerable<TMessage> messages) => AddRange(messages);
    }
}
