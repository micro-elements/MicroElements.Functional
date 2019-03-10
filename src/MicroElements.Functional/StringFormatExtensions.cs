// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Text;

namespace MicroElements.Functional
{
    /// <summary>
    /// StringFormat helpers.
    /// </summary>
    public static class StringFormatExtensions
    {
        /// <summary>
        /// Formats enumeration of value as tuple: (value1, value2, ...).
        /// </summary>
        /// <param name="values">Values enumeration.</param>
        /// <param name="fieldSeparator">Optional field separator.</param>
        /// <param name="nullPlaceholder">Optional null placeholder.</param>
        /// <returns>Formatted string.</returns>
        public static string FormatAsTuple(this IEnumerable<object> values, string fieldSeparator = ", ", string nullPlaceholder = "null")
        {
            values.AssertArgumentNotNull(nameof(values));
            fieldSeparator.AssertArgumentNotNull(nameof(fieldSeparator));
            nullPlaceholder.AssertArgumentNotNull(nameof(nullPlaceholder));

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("(");
            foreach (var value in values)
                stringBuilder.Append($"{value ?? nullPlaceholder}{fieldSeparator}");
            if (stringBuilder.Length > fieldSeparator.Length)
                stringBuilder.Length -= fieldSeparator.Length;
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Formats key-value pairs as Json.
        /// </summary>
        /// <param name="values">Values enumeration.</param>
        /// <param name="fieldSeparator">Optional field separator.</param>
        /// <param name="nullPlaceholder">Optional null placeholder.</param>
        /// <returns>Formatted string.</returns>
        public static string FormatAsJson(this IEnumerable<(string Name, string Value)> values, string fieldSeparator = ", ", string nullPlaceholder = "null")
        {
            values.AssertArgumentNotNull(nameof(values));
            fieldSeparator.AssertArgumentNotNull(nameof(fieldSeparator));
            nullPlaceholder.AssertArgumentNotNull(nameof(nullPlaceholder));

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            foreach ((string Name, string Value) formatComponent in values)
            {
                if (formatComponent.Value != null)
                    stringBuilder.Append($"{formatComponent.Name}: \"{formatComponent.Value}\"{fieldSeparator}");
                else
                    stringBuilder.Append($"{formatComponent.Name}: {nullPlaceholder}{fieldSeparator}");
            }
            if (stringBuilder.Length > fieldSeparator.Length)
                stringBuilder.Length -= fieldSeparator.Length;
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }
    }
}
