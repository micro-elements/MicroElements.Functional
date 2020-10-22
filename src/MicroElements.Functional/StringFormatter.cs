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
    public static class StringFormatter
    {
        /// <summary>
        /// Invariant format info. Uses '.' as decimal separator for floating point numbers.
        /// </summary>
        public static readonly NumberFormatInfo DefaultNumberFormatInfo = NumberFormatInfo.ReadOnly(
            new NumberFormatInfo
            {
                NumberDecimalSeparator = ".",
            });

        /// <summary>
        /// Default string formatting for most used types.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <returns>Formatted string.</returns>
        public static string DefaultFormatValue(this object? value)
        {
            if (value == null)
                return "null";

            if (value is string stringValue)
                return stringValue;

            if (value is double doubleNumber)
                return doubleNumber.ToString(DefaultNumberFormatInfo);

            if (value is float floatNumber)
                return floatNumber.ToString(DefaultNumberFormatInfo);

            if (value is decimal decimalNumber)
                return decimalNumber.ToString(DefaultNumberFormatInfo);

            if (value is DateTime dateTime)
                return dateTime == dateTime.Date ? $"{dateTime:yyyy-MM-dd}" : $"{dateTime:yyyy-MM-ddTHH:mm:ss}";

            string typeFullName = value.GetType().FullName;

            if (typeFullName == "NodaTime.LocalDate" && value is IFormattable localDate)
                return localDate.ToString("yyyy-MM-dd", null);

            if (typeFullName == "NodaTime.LocalDateTime" && value is IFormattable localDateTime)
                return localDateTime.ToString("yyyy-MM-ddTHH:mm:ss", null);

            return $"{value}";
        }

        /// <summary>
        /// Formats enumeration of value as tuple: (value1, value2, ...).
        /// </summary>
        /// <param name="values">Values enumeration.</param>
        /// <param name="fieldSeparator">Optional field separator.</param>
        /// <param name="nullPlaceholder">Optional null placeholder.</param>
        /// <param name="startSymbol">Start symbol. DefaultValue='('.</param>
        /// <param name="endSymbol">End symbol. DefaultValue=')'.</param>
        /// <param name="formatValue">Func to format object value to string representation.</param>
        /// <returns>Formatted string.</returns>
        public static string FormatAsTuple(
            this IEnumerable values,
            string fieldSeparator = ", ",
            string nullPlaceholder = "null",
            string startSymbol = "(",
            string endSymbol = ")",
            Func<object, string>? formatValue = null)
        {
            values.AssertArgumentNotNull(nameof(values));
            fieldSeparator.AssertArgumentNotNull(nameof(fieldSeparator));
            nullPlaceholder.AssertArgumentNotNull(nameof(nullPlaceholder));
            startSymbol.AssertArgumentNotNull(nameof(startSymbol));
            endSymbol.AssertArgumentNotNull(nameof(endSymbol));

            formatValue ??= DefaultFormatValue;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(startSymbol);
            foreach (var value in values)
            {
                string text = value != null ? formatValue(value) : nullPlaceholder;
                stringBuilder.Append($"{text}{fieldSeparator}");
            }

            if (stringBuilder.Length > fieldSeparator.Length)
                stringBuilder.Length -= fieldSeparator.Length;
            stringBuilder.Append(endSymbol);
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

            formatValue ??= DefaultFormatValue;

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

        public static string AddIfNotNull(this string? text, string separator) => text != null ? $"{text}{separator}" : string.Empty;
    }
}
