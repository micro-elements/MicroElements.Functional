using System.Collections;
using System.Collections.Generic;

namespace MicroElements.Functional
{
    /// <summary>
    /// SingleThreaded mutable message list.
    /// </summary>
    /// <typeparam name="TMessage">Message type.</typeparam>
    public class MutableMessageList<TMessage> : IMutableMessageList<TMessage>
    {
        private readonly List<TMessage> _messages = new List<TMessage>();

        /// <inheritdoc />
        public IEnumerator<TMessage> GetEnumerator() => _messages.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_messages).GetEnumerator();

        /// <inheritdoc />
        public int Count => _messages.Count;

        /// <inheritdoc />
        public IMutableMessageList<TMessage> Add(TMessage message)
        {
            _messages.Add(message);
            return this;
        }

        /// <inheritdoc />
        public IMutableMessageList<TMessage> AddRange(IEnumerable<TMessage> messages)
        {
            _messages.AddRange(messages);
            return this;
        }

        /// <inheritdoc />
        IMessageList<TMessage> IMessageList<TMessage>.Add(TMessage message) => Add(message);

        /// <inheritdoc />
        IMessageList<TMessage> IMessageList<TMessage>.AddRange(IEnumerable<TMessage> messages) => AddRange(messages);
    }
}
