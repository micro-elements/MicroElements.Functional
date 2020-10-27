// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MicroElements.Functional
{
    /// <summary>
    /// ObjectExt from LanguageExt.
    /// </summary>
    public static class ObjectExt
    {
        /// <summary>
        /// Returns true if the value is equal to this type's default value.
        /// </summary>
        /// <example>
        ///     0.IsDefault() == true
        ///     1.IsDefault() == false
        /// </example>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">Value to check.</param>
        /// <returns>True if the value is equal to this type's default value.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefault<T>([AllowNull] this T value) =>
            TypeCheck<T>.IsDefault(value);

        /// <summary>
        /// Returns true if the value is null, and does so without
        /// boxing of any value-types.  Value-types will always
        /// return false.
        /// </summary>
        /// <example>
        ///     int x = 0;
        ///     string y = null;
        ///
        ///     x.IsNull()  // false
        ///     y.IsNull()  // true
        /// </example>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">Value to check.</param>
        /// <returns>True if the value is null, and does so without
        /// boxing of any value-types.  Value-types will always
        /// return false.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>([AllowNull][NotNullWhen(false)] this T value) =>
            TypeCheck<T>.IsNull(value);

        /// <summary>
        /// Returns true if value is not null. Value-types will always
        /// return true.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">Value to check.</param>
        /// <returns>True if value is not null.Value-types will always
        /// return true.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull<T>([AllowNull][NotNullWhen(true)] this T value) =>
            !value.IsNull();
    }
}
