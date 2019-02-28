using System;
using System.Collections.Generic;

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents message.
    /// Can be used as simple log message, detailed or structured log message, validation message, diagnostic message.
    /// </summary>
    public sealed class Message : IMessage, ICanBeError
    {
        private static readonly IReadOnlyDictionary<string, object> EmptyPropertySet = new Dictionary<string, object>();

        /// <summary>
        /// Date and time of message created.
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Message severity.
        /// </summary>
        public MessageSeverity Severity { get; }

        /// <summary>
        /// Message text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Event name.
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Optional state.
        /// </summary>
        public object State { get; }

        /// <summary>
        /// Message properties.
        /// </summary>
        public IReadOnlyDictionary<string, object> Properties { get; }

        /// <inheritdoc />
        public bool IsError => Severity == MessageSeverity.Error;

        /// <summary>
        /// Creates new instance of <see cref="Message"/>.
        /// </summary>
        /// <param name="text">Message.</param>
        /// <param name="severity">Message severity.</param>
        /// <param name="timestamp">Optional timestamp. Evaluates as <see cref="DateTimeOffset.Now"/> if not set.</param>
        /// <param name="eventName">Optional Event Name.</param>
        /// <param name="state">Optional state.</param>
        /// <param name="properties">Optional properties.</param>
        public Message(
            string text,
            MessageSeverity severity = MessageSeverity.Information,
            DateTimeOffset? timestamp = null,
            string eventName = null,
            object state = null,
            IReadOnlyDictionary<string, object> properties = null)
        {
            Text = text.AssertArgumentNotNull(nameof(text));
            Severity = severity;
            Timestamp = timestamp ?? DateTimeOffset.Now;
            EventName = eventName;
            State = state;
            Properties = properties ?? EmptyPropertySet;
        }

        /// <summary>
        /// Implicit conversion from string.
        /// </summary>
        /// <param name="text">Text message.</param>
        public static implicit operator Message(string text) => new Message(text);
    }
}
