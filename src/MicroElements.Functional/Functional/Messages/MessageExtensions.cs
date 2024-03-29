﻿// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using MicroElements.CodeContracts;

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
            string originalMessage = null,
            MessageSeverity? severity = null,
            DateTimeOffset? timestamp = null,
            string eventName = null,
            IReadOnlyCollection<KeyValuePair<string, object>> properties = null,
            PropertyAddMode propertyAddMode = PropertyAddMode.Merge)
        {
            var resultPropertyList = GetResultPropertyList(message, properties, propertyAddMode);

            return new Message(
                timestamp: timestamp ?? message.Timestamp,
                severity: severity ?? message.Severity,
                originalMessage: originalMessage ?? message.OriginalMessage,
                eventName: eventName ?? message.EventName,
                properties: resultPropertyList);
        }

        private static IReadOnlyCollection<KeyValuePair<string, object>> GetResultPropertyList(
            IMessage message,
            IReadOnlyCollection<KeyValuePair<string, object>> properties,
            PropertyAddMode propertyAddMode)
        {
            IReadOnlyCollection<KeyValuePair<string, object>> resultPropertyList;

            var propertiesToAdd = properties ?? Array.Empty<KeyValuePair<string, object>>();

            if (propertyAddMode == PropertyAddMode.Set)
                resultPropertyList = propertiesToAdd;
            else if (propertyAddMode == PropertyAddMode.Merge)
                resultPropertyList = message.Properties.AddWithReplace(propertiesToAdd);
            else if (propertyAddMode == PropertyAddMode.AddIfNotExists)
                resultPropertyList = message.Properties.AddIfNotExists(propertiesToAdd);
            else
                resultPropertyList = Array.Empty<KeyValuePair<string, object>>();

            return resultPropertyList;
        }

        /// <summary>
        /// Creates new copy of <see cref="Message"/> with required <see cref="IMessage.OriginalMessage"/>.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="text">New text value.</param>
        /// <returns>New instance of <see cref="Message"/> with changed <see cref="IMessage.OriginalMessage"/>.</returns>
        public static Message WithText(this IMessage message, string text) => message.With(originalMessage: text);

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
        /// <param name="propertyAddMode">Property list add mode.</param>
        /// <returns>New instance of <see cref="Message"/> with changed <see cref="IMessage.Properties"/>.</returns>
        public static Message WithProperties(
            this IMessage message,
            IReadOnlyList<KeyValuePair<string, object>> properties,
            PropertyAddMode propertyAddMode = PropertyAddMode.Set)
            => message.With(properties: properties, propertyAddMode: propertyAddMode);

        /// <summary>
        /// Creates new copy of <see cref="Message"/> with required <see cref="IMessage.Properties"/>.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="properties">New state.</param>
        /// <param name="propertyAddMode">Property list add mode.</param>
        /// <returns>New instance of <see cref="Message"/> with changed <see cref="IMessage.Properties"/>.</returns>
        public static Message WithProperties(
            this IMessage message,
            IEnumerable<KeyValuePair<string, object>> properties,
            PropertyAddMode propertyAddMode = PropertyAddMode.Set)
            => message.With(properties: properties.ToList(), propertyAddMode: propertyAddMode);

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
            var properties = message.Properties.ToDictionary(pair => pair.Key, pair => pair.Value);
            properties[name] = value;
            return message.WithProperties(properties, PropertyAddMode.Merge);
        }

        /// <summary>
        /// Creates new copy of <see cref="Message"/> and adds captured properties from MessageTemplate.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="args">MessageTemplate args.</param>
        /// <returns>New instance of <see cref="Message"/> with added properties.</returns>
        public static Message WithArgs(this Message message, params object[] args)
        {
            var capturedProps = message.GetMessageTemplate().ArgsToDictionary(args);
            return message.With(properties: capturedProps, propertyAddMode: PropertyAddMode.Merge);
        }

        /// <summary>
        /// Gets optional property by name.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Optional value.</returns>
        public static Option<object> GetProperty(this IMessage message, string propertyName)
        {
            return message.GetValueAsOption(propertyName);
        }

        /// <summary>
        /// Adds KeyValue pairs to source list. Replaces value if exists in source list.
        /// </summary>
        /// <param name="source">Source list.</param>
        /// <param name="keyValuePairs">Items to add.</param>
        /// <param name="keyEqualityComparer">Key comparer. If not set <see cref="StringComparer.InvariantCultureIgnoreCase"/> will be used.</param>
        /// <returns>New list with added values.</returns>
        public static IReadOnlyCollection<KeyValuePair<string, object>> AddWithReplace(
            this IReadOnlyCollection<KeyValuePair<string, object>> source,
            IReadOnlyCollection<KeyValuePair<string, object>> keyValuePairs,
            IEqualityComparer<string> keyEqualityComparer = null)
        {
            if (keyValuePairs.Count == 0)
                return source;

            keyEqualityComparer = keyEqualityComparer ?? StringComparer.InvariantCultureIgnoreCase;
            var dict = source.ToDictionary(pair => pair.Key, pair => pair.Value, keyEqualityComparer);
            foreach (var valuePair in keyValuePairs)
            {
                // Add or replace values
                dict[valuePair.Key] = valuePair.Value;
            }
            return dict.ToList();
        }

        /// <summary>
        /// Adds KeysValue pair if not exists in old list.
        /// </summary>
        /// <param name="source">Source list.</param>
        /// <param name="keyValuePairs">Items to add.</param>
        /// <param name="keyEqualityComparer">Key comparer. If not set <see cref="StringComparer.InvariantCultureIgnoreCase"/> will be used.</param>
        /// <returns>New list with added values.</returns>
        public static IReadOnlyCollection<KeyValuePair<string, object>> AddIfNotExists(
            this IReadOnlyCollection<KeyValuePair<string, object>> source,
            IReadOnlyCollection<KeyValuePair<string, object>> keyValuePairs,
            IEqualityComparer<string>? keyEqualityComparer = null)
        {
            if (keyValuePairs.Count == 0)
                return source;

            keyEqualityComparer ??= StringComparer.InvariantCultureIgnoreCase;
            var dict = source.ToDictionary(pair => pair.Key, pair => pair.Value, keyEqualityComparer);
            foreach (var valuePair in keyValuePairs)
            {
                // Skip value is exists in old list.
                if (dict.ContainsKey(valuePair.Key))
                    continue;
                dict.Add(valuePair.Key, valuePair.Value);
            }
            return dict.ToList();
        }

        /// <summary>
        /// Gets Exception property.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="exceptionKey">Exception key in message properties.</param>
        /// <returns>Exception or null.</returns>
        public static Exception? GetException(this IMessage message, string exceptionKey = "Exception")
        {
            return (Exception?)message.GetProperty(exceptionKey).GetValueOrDefault(defaultValue: null);
        }

        /// <summary>
        /// Creates new copy of <see cref="Message"/> with Exception property set.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="exception">Exception.</param>
        /// <param name="exceptionKey">Exception key in message properties.</param>
        /// <returns>New instance of <see cref="Message"/> with Exception property set.</returns>
        public static Message WithException(this IMessage message, Exception exception, string exceptionKey = "Exception")
        {
            exception.AssertArgumentNotNull(nameof(exception));

            return message.WithProperty(exceptionKey, exception);
        }
    }
}
