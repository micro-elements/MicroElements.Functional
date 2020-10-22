// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents message.
    /// Can be used as simple log message, detailed or structured log message, validation message, diagnostic message or error object.
    /// It violates some SOLID principles but is very useful infrastructure layer citizen.
    /// </summary>
    [Serializable]
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
        /// Original message.
        /// Can be in form of MessageTemplates.org.
        /// </summary>
        public string OriginalMessage { get; }

        /// <summary>
        /// Event name.
        /// </summary>
        public string? EventName { get; }

        /// <summary>
        /// Message properties.
        /// </summary>
        public IReadOnlyCollection<KeyValuePair<string, object>> Properties { get; }

        /// <summary>
        /// Formatted message. This is a result of MessageTemplate rendered with <seealso cref="Properties"/>.
        /// </summary>
        public string FormattedMessage
        {
            get
            {
                var messageTemplate = GetLazyContext().MessageTemplate;
                var messageTemplateRenderer = GetLazyContext().MessageTemplateRenderer;
                return messageTemplateRenderer.TryRenderToString(messageTemplate, GetLazyContext().AllProperties, OriginalMessage);
            }
        }

        /// <summary>
        /// Message text. For backward compatibility returns <see cref="OriginalMessage"/>.
        /// Use <see cref="FormattedMessage"/> for rendered message.
        /// </summary>
        public string Text => OriginalMessage;

        /// <summary>
        /// Object is Error.
        /// </summary>
        bool ICanBeError.IsError => Severity == MessageSeverity.Error;

        /// <summary>
        /// Creates new instance of <see cref="Message"/>.
        /// </summary>
        /// <param name="originalMessage">Original message. Can be in form of MessageTemplates.org.</param>
        /// <param name="severity">Message severity.</param>
        /// <param name="timestamp">Optional timestamp. Evaluates as <see cref="DateTimeOffset.Now"/> if not set.</param>
        /// <param name="eventName">Optional Event Name.</param>
        /// <param name="properties">Optional properties.</param>
        /// <param name="messageTemplateParser">Optional MessageTemplateParser.</param>
        /// <param name="messageTemplateRenderer">Optional MessageTemplateRenderer.</param>
        public Message(
            string originalMessage,
            MessageSeverity severity = MessageSeverity.Information,
            DateTimeOffset? timestamp = null,
            string? eventName = null,
            IReadOnlyCollection<KeyValuePair<string, object>>? properties = null,
            IMessageTemplateParser? messageTemplateParser = null,
            IMessageTemplateRenderer? messageTemplateRenderer = null)
        {
            // Required
            OriginalMessage = originalMessage.AssertArgumentNotNull(nameof(originalMessage));

            // Optional
            Severity = severity;
            Timestamp = timestamp ?? DateTimeOffset.Now;
            EventName = eventName;
            Properties = properties ?? EmptyPropertyList;

            // Optional temporary context
            _lazyContext = new Lazy<MessageContext>(() => new MessageContext(this, messageTemplateParser, messageTemplateRenderer));
        }

        /// <summary>
        /// Gets <see cref="MessageTemplate"/> parsed from <see cref="OriginalMessage"/>.
        /// </summary>
        /// <returns><see cref="MessageTemplate"/>.</returns>
        public MessageTemplate GetMessageTemplate() => GetLazyContext().MessageTemplate;

        /// <summary>
        /// Implicit conversion from string.
        /// </summary>
        /// <param name="text">Text message.</param>
        public static implicit operator Message(string text) => new Message(text);

        /// <inheritdoc />
        public override string ToString() => $"{Timestamp:yyyy-MM-ddTHH:mm:ss.fff} | {Severity} | {EventName.AddIfNotNull(" |")} {FormattedMessage}";

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

        private KeyValuePair<string, object>[] GetBaseProperties()
        {
            return new[]
            {
                new KeyValuePair<string, object>(nameof(Timestamp), Timestamp),
                new KeyValuePair<string, object>(nameof(Severity), Severity),
                new KeyValuePair<string, object>(nameof(OriginalMessage), OriginalMessage),
                new KeyValuePair<string, object>(nameof(EventName), EventName),
            };
        }

        private SortedList<string, object> GetAllProperties()
        {
            var dictionary = GetBaseProperties()
                .AddWithReplace(Properties)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            return new SortedList<string, object>(dictionary, StringComparer.InvariantCultureIgnoreCase);
        }

        private SortedList<string, object> AllPropertiesCached => GetLazyContext().AllProperties;

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

        #region MessageContext

        /// <summary>
        /// Optional temporary context.
        /// </summary>
        [NonSerialized]
        private Lazy<MessageContext> _lazyContext;

        private MessageContext GetLazyContext()
        {
            // After deserialization context will be lost
            if (_lazyContext == null)
                _lazyContext = new Lazy<MessageContext>(() => new MessageContext(this, null, null));
            return _lazyContext.Value;
        }

        /// <summary>
        /// Optional temporary context for caches.
        /// </summary>
        private class MessageContext
        {
            private readonly Message _message;
            private readonly IMessageTemplateParser? _messageTemplateParser;
            private readonly IMessageTemplateRenderer? _messageTemplateRenderer;

            private readonly Lazy<MessageTemplate> _lazyMessageTemplate;
            private readonly Lazy<SortedList<string, object>> _lazyAllProperties;

            public IMessageTemplateParser MessageTemplateParser => _messageTemplateParser ?? Functional.MessageTemplateParser.Instance;

            public IMessageTemplateRenderer MessageTemplateRenderer => _messageTemplateRenderer ?? Functional.MessageTemplateRenderer.Instance;

            public MessageTemplate MessageTemplate => _lazyMessageTemplate.Value;

            public SortedList<string, object> AllProperties => _lazyAllProperties.Value;

            public MessageContext(
                Message message,
                IMessageTemplateParser? messageTemplateParser,
                IMessageTemplateRenderer? messageTemplateRenderer)
            {
                _message = message;
                _messageTemplateParser = messageTemplateParser;
                _messageTemplateRenderer = messageTemplateRenderer;

                _lazyMessageTemplate = new Lazy<MessageTemplate>(GetMessageTemplate);
                _lazyAllProperties = new Lazy<SortedList<string, object>>(message.GetAllProperties);
            }

            private MessageTemplate GetMessageTemplate() => MessageTemplateParser.TryParse(_message.OriginalMessage);
        }

        #endregion
    }
}
