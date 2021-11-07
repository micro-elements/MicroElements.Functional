// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using MicroElements.CodeContracts;
using MicroElements.Formatting.StringFormatter;

namespace MicroElements.Functional
{
    /// <summary>
    /// StringFormat helpers.
    /// </summary>
    public static class StringFormatter
    {
        /// <summary>
        /// Formats key-value pairs as Json.
        /// </summary>
        /// <param name="values">Values enumeration.</param>
        /// <param name="fieldSeparator">Optional field separator.</param>
        /// <param name="nullPlaceholder">Optional null placeholder.</param>
        /// <param name="formatValue">Func to format object value to string representation.</param>
        /// <returns>Formatted string.</returns>
        public static string FormatAsJson(
            this IEnumerable<(string Name, object? Value)> values,
            string fieldSeparator = ", ",
            string nullPlaceholder = "null",
            Func<object?, string>? formatValue = null)
        {
            values.AssertArgumentNotNull(nameof(values));
            fieldSeparator.AssertArgumentNotNull(nameof(fieldSeparator));
            nullPlaceholder.AssertArgumentNotNull(nameof(nullPlaceholder));

            formatValue ??= FormatAsJsonDefault;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            foreach ((string Name, object? Value) formatComponent in values)
            {
                var formatted = formatValue(formatComponent.Value) ?? nullPlaceholder;
                stringBuilder.Append($"{formatComponent.Name}: \"{formatted}\"{fieldSeparator}");
            }

            if (stringBuilder.Length > fieldSeparator.Length)
                stringBuilder.Length -= fieldSeparator.Length;
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        private static string FormatAsJsonDefault(object? value)
        {
            if (value is IFormattableObject collection)
                return collection.GetNameValuePairs().FormatAsJson();
            return value.FormatValue();
        }
    }
}
