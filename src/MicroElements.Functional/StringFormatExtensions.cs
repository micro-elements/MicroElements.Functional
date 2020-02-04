// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MicroElements.Functional
{
    /// <summary>
    /// StringFormat helpers.
    /// </summary>
    public static class StringFormatExtensions
    {
        public static readonly NumberFormatInfo DefaultNumberFormatInfo = NumberFormatInfo.ReadOnly(
            new NumberFormatInfo
            {
                NumberDecimalSeparator = ".",
            });

        public static Func<object, string> DefaultFormatValue = value =>
        {
            switch (value)
            {
                case double num:
                    return num.ToString(DefaultNumberFormatInfo);
                case float num:
                    return num.ToString(DefaultNumberFormatInfo);
                case decimal num:
                    return num.ToString(DefaultNumberFormatInfo);
                case DateTime dateTime:
                    return dateTime == dateTime.Date ? $"{dateTime:yyyy-MM-dd}" : $"{dateTime:yyyy-MM-ddTHH:mm:ss}";
                default:
                    return $"{value}";
            }
        };

        /// <summary>
        /// Formats enumeration of value as tuple: (value1, value2, ...).
        /// </summary>
        /// <param name="values">Values enumeration.</param>
        /// <param name="fieldSeparator">Optional field separator.</param>
        /// <param name="nullPlaceholder">Optional null placeholder.</param>
        /// <param name="formatValue">Func to format object value to string representation.</param>
        /// <returns>Formatted string.</returns>
        public static string FormatAsTuple(
            this IEnumerable values,
            string fieldSeparator = ", ",
            string nullPlaceholder = "null",
            Func<object, string> formatValue = null)
        {
            values.AssertArgumentNotNull(nameof(values));
            fieldSeparator.AssertArgumentNotNull(nameof(fieldSeparator));
            nullPlaceholder.AssertArgumentNotNull(nameof(nullPlaceholder));

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("(");
            foreach (var value in values)
            {
                string text = value != null ? formatValue != null ? formatValue(value) : value.ToString() : nullPlaceholder;
                stringBuilder.Append($"{text}{fieldSeparator}");
            }
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
        /// <param name="formatValue">Func to format object value to string representation.</param>
        /// <returns>Formatted string.</returns>
        public static string FormatAsJson(
            this IEnumerable<(string Name, object Value)> values,
            string fieldSeparator = ", ",
            string nullPlaceholder = "null",
            Func<object, string> formatValue = null)
        {
            values.AssertArgumentNotNull(nameof(values));
            fieldSeparator.AssertArgumentNotNull(nameof(fieldSeparator));
            nullPlaceholder.AssertArgumentNotNull(nameof(nullPlaceholder));

            formatValue = formatValue ?? DefaultFormatValue;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            foreach ((string Name, object Value) formatComponent in values)
            {
                var formatted = formatValue(formatComponent.Value) ?? nullPlaceholder;
                stringBuilder.Append($"{formatComponent.Name}: \"{formatted}\"{fieldSeparator}");
            }

            if (stringBuilder.Length > fieldSeparator.Length)
                stringBuilder.Length -= fieldSeparator.Length;
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }
    }
}
