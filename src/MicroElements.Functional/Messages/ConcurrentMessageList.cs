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
        public IEnumerator<TMessage> GetEnumerator()
        {
            return _messages.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_messages).GetEnumerator();
        }

        /// <inheritdoc />
        public int Count => _messages.Count;

        /// <inheritdoc />
        public void Add(TMessage message)
        {
            _messages.Enqueue(message);
        }

        /// <inheritdoc />
        public void AddRange(IEnumerable<TMessage> other)
        {
            foreach (var message in other)
            {
                _messages.Enqueue(message);
            }
        }

        /// <inheritdoc />
        IMessageList<TMessage> IMessageList<TMessage>.Add(TMessage message)
        {
            _messages.Enqueue(message);
            return this;
        }

        /// <inheritdoc />
        IMessageList<TMessage> IMessageList<TMessage>.AddRange(IEnumerable<TMessage> other)
        {
            AddRange(other);
            return this;
        }
    }
}
