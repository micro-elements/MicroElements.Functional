using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroElements.Functional
{
    /// <summary>
    /// Message extensions.
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Copy method.
        /// </summary>
        internal static Message With(
            this IMessage message,
            string text = null,
            MessageSeverity? severity = null,
            DateTimeOffset? timestamp = null,
            string eventName = null,
            object state = null,
            IReadOnlyDictionary<string, object> properties = null)
        {
            return new Message(
                timestamp: timestamp ?? message.Timestamp,
                severity: severity ?? message.Severity,
                text: text ?? message.Text,
                eventName: eventName ?? message.EventName,
                state: state ?? message.State,
                properties: properties ?? message.Properties);
        }

        /// <summary>
        /// Creates new copy of <see cref="Message"/> with required <see cref="IMessage.Text"/>.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="text">New text value.</param>
        /// <returns>New instance of <see cref="Message"/> with changed <see cref="IMessage.Text"/>.</returns>
        public static Message WithText(this IMessage message, string text) => message.With(text: text);

        /// <summary>
        /// Creates new copy of <see cref="Message"/> with required <see cref="IMessage.State"/>.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="state">New state.</param>
        /// <returns>New instance of <see cref="Message"/> with changed <see cref="IMessage.State"/>.</returns>
        public static Message WithState(this IMessage message, object state) => message.With(state: state);

        /// <summary>
        /// Creates new copy of <see cref="Message"/> with required <see cref="IMessage.Severity"/>.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="severity">New severity.</param>
        /// <returns>New instance of <see cref="Message"/> with changed <see cref="IMessage.Severity"/>.</returns>
        public static Message WithSeverity(this IMessage message, MessageSeverity severity) => message.With(severity: severity);

        /// <summary>
        /// Creates new copy of <see cref="Message"/> with required <see cref="IMessage.EventName"/>.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="eventName">New EventName.</param>
        /// <returns>New instance of <see cref="Message"/> with changed <see cref="IMessage.EventName"/>.</returns>
        public static Message WithEventName(this IMessage message, string eventName) => message.With(eventName: eventName);

        /// <summary>
        /// Creates new copy of <see cref="Message"/> with required <see cref="IMessage.Timestamp"/>.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="timestamp">New Timestamp.</param>
        /// <returns>New instance of <see cref="Message"/> with changed <see cref="IMessage.Timestamp"/>.</returns>
        public static Message WithTimestamp(this IMessage message, DateTimeOffset timestamp) => message.With(timestamp: timestamp);

        /// <summary>
        /// Creates new copy of <see cref="Message"/> with required <see cref="IMessage.Properties"/>.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="properties">New state.</param>
        /// <returns>New instance of <see cref="Message"/> with changed <see cref="IMessage.Properties"/>.</returns>
        public static Message WithProperties(this IMessage message, IReadOnlyDictionary<string, object> properties) => message.With(properties: properties);

        /// <summary>
        /// Creates new copy of <see cref="Message"/> with new property added.
        /// Replaces property if it already exists.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        /// <returns>New instance of <see cref="Message"/> with changed <see cref="IMessage.Properties"/>.</returns>
        public static Message WithProperty(this IMessage message, string name, object value)
        {
            if (message.Properties == null)
                return message.WithProperties(new Dictionary<string, object> { { name, value } });

            var properties = message.Properties.ToDictionary(pair => pair.Key, pair => pair.Value);
            properties[name] = value;
            return message.WithProperties(properties);
        }

        /// <summary>
        /// Gets optional property by name.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Optional value.</returns>
        public static Option<object> GetProperty(this IMessage message, string propertyName)
        {
            return message.Properties.GetValueAsOption(propertyName);
        }
    }
}
