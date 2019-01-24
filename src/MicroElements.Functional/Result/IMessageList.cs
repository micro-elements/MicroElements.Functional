using System.Collections;
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

    public static partial class Prelude
    {
        public static IMessageList<TMessage> EmptyMessageList<TMessage>() => Empty.MessageList<TMessage>.Instance;
    }

    namespace Empty
    {
        public class MessageList<TMessage> : IMessageList<TMessage>
        {
            public static readonly MessageList<TMessage> Instance = new MessageList<TMessage>();

            private MessageList()
            {
            }

            public static implicit operator MessageList<TMessage>(Unit empty) => Instance;

            /// <inheritdoc />
            public IEnumerator<TMessage> GetEnumerator()
            {
                yield break;
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            /// <inheritdoc />
            public IMessageList<TMessage> Add(TMessage message) => this;

            /// <inheritdoc />
            public IMessageList<TMessage> AddRange(IEnumerable<TMessage> other) => this;
        }
    }

    public class MessageList<TMessage> : IMessageList<TMessage>
    {
        private LinkedListNode<TMessage> _node;

        /// <inheritdoc />
        public MessageList(TMessage message)
        {
            _node = new LinkedListNode<TMessage>(message);
            if (_node.List == null)
            {
                _node = new LinkedList<TMessage>().AddLast(message);
            }
        }

        /// <inheritdoc />
        private MessageList(LinkedListNode<TMessage> node)
        {
            _node = node;
        }

        /// <inheritdoc />
        public IEnumerator<TMessage> GetEnumerator()
        {
            return _node.List.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public IMessageList<TMessage> Add(TMessage message)
        {
            var newNode = _node.List.AddAfter(_node, message);
            return new MessageList<TMessage>(newNode);
        }

        /// <inheritdoc />
        public IMessageList<TMessage> AddRange(IEnumerable<TMessage> other)
        {
            LinkedListNode<TMessage> node = _node;
            foreach (var message in other)
            {
                node = node.List.AddAfter(node, message);
            }

            return new MessageList<TMessage>(node);
        }
    }
}
