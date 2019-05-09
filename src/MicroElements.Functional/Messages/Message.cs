// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents message.
    /// Can be used as simple log message, detailed or structured log message, validation message, diagnostic message or error object.
    /// It violates some SOLID principles but is very useful infrastructure layer citizen.
    /// </summary>
    public sealed class Message : IMessage, ICanBeError, IFormattableObject
    {
        private static readonly IReadOnlyList<KeyValuePair<string, object>> EmptyPropertyList = new List<KeyValuePair<string, object>>();

        /// <summary>
        /// Date and time of message created.
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Message severity.
        /// </summary>
        public MessageSeverity Severity { get; }

        /// <summary>
        /// Original message. Can be in form of MessageTemplates.org.
        /// </summary>
        public string OriginalMessage { get; }

        /// <summary>
        /// Event name.
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Message properties.
        /// </summary>
        public IReadOnlyList<KeyValuePair<string, object>> Properties { get; }

        /// <summary>
        /// Object is Error.
        /// </summary>
        public bool IsError => Severity == MessageSeverity.Error;

        /// <summary>
        /// <seealso cref="Functional.MessageTemplate"/> parsed from <seealso cref="OriginalMessage"/>.
        /// </summary>
        public Lazy<MessageTemplate> MessageTemplate { get; }

        /// <summary>
        /// Formatted message. This is a result of MessageTemplate rendered with <seealso cref="Properties"/>.
        /// </summary>
        public string FormattedMessage => TryRenderMessageTemplate();

        /// <summary>
        /// Creates new instance of <see cref="Message"/>.
        /// </summary>
        /// <param name="originalMessage">Original message. Can be in form of MessageTemplates.org.</param>
        /// <param name="severity">Message severity.</param>
        /// <param name="timestamp">Optional timestamp. Evaluates as <see cref="DateTimeOffset.Now"/> if not set.</param>
        /// <param name="eventName">Optional Event Name.</param>
        /// <param name="properties">Optional properties.</param>
        public Message(
            string originalMessage,
            MessageSeverity severity = MessageSeverity.Information,
            DateTimeOffset? timestamp = null,
            string eventName = null,
            IReadOnlyList<KeyValuePair<string, object>> properties = null)
        {
            OriginalMessage = originalMessage.AssertArgumentNotNull(nameof(originalMessage));
            Severity = severity;
            Timestamp = timestamp ?? DateTimeOffset.Now;
            EventName = eventName;
            Properties = properties ?? EmptyPropertyList;

            MessageTemplate = new Lazy<MessageTemplate>(TryParseMessageTemplate);
        }

        /// <summary>
        /// Implicit conversion from string.
        /// </summary>
        /// <param name="text">Text message.</param>
        public static implicit operator Message(string text) => new Message(text);

        /// <inheritdoc />
        public override string ToString() => $"{Timestamp:yyyy-MM-ddTHH:mm:ss.fff} | {Severity} | {EventName.AddIfNotNull()} {FormattedMessage}";

        #region IReadOnlyList

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => AllPropertiesCached.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public int Count => AllPropertiesCached.Count;

        /// <inheritdoc />
        public KeyValuePair<string, object> this[int index] => new KeyValuePair<string, object>(AllPropertiesCached.Keys[index], AllPropertiesCached.Values[index]);

        #endregion

        #region IReadOnlyDictionary

        private IEnumerable<KeyValuePair<string, object>> GetBaseProperties()
        {
            yield return new KeyValuePair<string, object>(nameof(Timestamp), Timestamp);
            yield return new KeyValuePair<string, object>(nameof(Severity), Severity);
            yield return new KeyValuePair<string, object>(nameof(OriginalMessage), OriginalMessage);
            yield return new KeyValuePair<string, object>(nameof(EventName), EventName);
        }

        private SortedList<string, object> GetAllProperties()
        {
            var dictionary = GetBaseProperties()
                .AddWithReplace(Properties)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            return new SortedList<string, object>(dictionary, StringComparer.InvariantCultureIgnoreCase);
        }

        private SortedList<string, object> AllPropertiesCached => Memoize(GetAllProperties)();

        /// <inheritdoc />
        public bool ContainsKey(string key) => AllPropertiesCached.ContainsKey(key);

        /// <inheritdoc />
        public bool TryGetValue(string key, out object value) => AllPropertiesCached.TryGetValue(key, out value);

        /// <inheritdoc />
        public object this[string key] => AllPropertiesCached[key];

        /// <inheritdoc />
        public IEnumerable<string> Keys => AllPropertiesCached.Keys;

        /// <inheritdoc />
        public IEnumerable<object> Values => AllPropertiesCached.Values;

        #endregion

        #region IFormattableObject

        /// <inheritdoc />
        public IEnumerable<(string Name, object Value)> GetNameValuePairs()
        {
            return AllPropertiesCached.Select(pair => (pair.Key, pair.Value));
        }

        #endregion

        #region MessageTemplate

        private MessageTemplate TryParseMessageTemplate()
        {
            return MessageTemplateParser.Instance.TryParse(OriginalMessage);
        }

        private string TryRenderMessageTemplate()
        {
            try
            {
                return MessageTemplateRenderer.Instance.RenderToString(MessageTemplate.Value, AllPropertiesCached);
            }
            catch
            {
                return OriginalMessage;
            }
        }

        #endregion
    }
}
