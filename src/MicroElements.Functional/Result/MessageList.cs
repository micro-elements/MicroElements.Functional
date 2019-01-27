using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MicroElements.Functional
{
    /// <summary>
    /// Default implementation of <see cref="IMessageList{TMessage}"/>.
    /// </summary>
    /// <typeparam name="TMessage">Message type.</typeparam>
    public class MessageList<TMessage> : IMessageList<TMessage>
    {
        /// <summary>
        /// Empty message list.
        /// </summary>
        public static readonly MessageList<TMessage> Empty = new MessageList<TMessage>();

        // Container for value.
        private readonly LinkedListNode<TMessage> _node;

        /// <summary>
        /// Creates message list with one message.
        /// </summary>
        /// <param name="message">Message.</param>
        public MessageList(TMessage message)
        {
            ThrowIfMessageIsNull(message);
            _node = new LinkedList<TMessage>().AddLast(message);
        }

        /// <summary>
        /// Creates empty message list.
        /// </summary>
        internal MessageList()
        {
            _node = null;
        }

        /// <summary>
        /// Creates message list by node.
        /// </summary>
        private MessageList(LinkedListNode<TMessage> node)
        {
            _node = node;
        }

        /// <summary>
        /// Implicit conversion to message list.
        /// </summary>
        /// <param name="message">Single message.</param>
        public static implicit operator MessageList<TMessage>(TMessage message) => new MessageList<TMessage>(message);

        /// <inheritdoc />
        public IEnumerator<TMessage> GetEnumerator()
        {
            if (_node != null)
                return _node.List.GetEnumerator();
            return Enumerable.Empty<TMessage>().GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public int Count => _node?.List.Count ?? 0;

        /// <inheritdoc />
        public IMessageList<TMessage> Add(TMessage message)
        {
            ThrowIfMessageIsNull(message);

            if (_node == null)
                return new MessageList<TMessage>(message);

            var newNode = _node.List.AddAfter(_node, message);
            return new MessageList<TMessage>(newNode);
        }

        /// <inheritdoc />
        public IMessageList<TMessage> AddRange(IEnumerable<TMessage> other)
        {
            IEnumerable<T> CheckItemsNotNull<T>(IEnumerable<T> items)
            {
                foreach (var message in items)
                {
                    ThrowIfMessageIsNull(message);
                    yield return message;
                }
            }

            var otherChecked = CheckItemsNotNull(other);
            if (_node == null)
            {
                var list = new LinkedList<TMessage>(otherChecked);
                return new MessageList<TMessage>(list.Last);
            }

            LinkedListNode<TMessage> newNode = _node;
            foreach (var message in otherChecked)
            {
                newNode = newNode.List.AddAfter(newNode, message);
            }

            return new MessageList<TMessage>(newNode);
        }

        private static void ThrowIfMessageIsNull<T>(T message)
        {
            if (message.IsNull())
                throw new ArgumentNullException(nameof(message), "Message can not be null");
        }
    }

    public static partial class Prelude
    {
        public static readonly IMessageList<string> EmptyMessageList = MessageList<string>.Empty;
    }
}
