// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace MicroElements.Functional
{
    /// <summary>
    /// Represents cached type checks.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    [SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = "Ok")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Ok")]
    public static class TypeCheck<T>
    {
        /// <summary>
        /// Gets a value indicating whether the type specified by the generic argument is a reference type.
        /// </summary>
        public static readonly bool IsReferenceType;

        /// <summary>
        /// Gets a value indicating whether the type specified by the generic argument is a nullable struct.
        /// </summary>
        public static readonly bool IsNullableStruct;

        /// <summary>
        /// Gets a default equality comparer for the type specified by the generic argument.
        /// </summary>
        public static readonly EqualityComparer<T> DefaultEqualityComparer;

        static TypeCheck()
        {
            IsNullableStruct = typeof(T).IsNullableStruct();
            IsReferenceType = typeof(T).IsReferenceType();
            DefaultEqualityComparer = EqualityComparer<T>.Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsDefault([AllowNull] T value) => DefaultEqualityComparer.Equals(value, default(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsNull([AllowNull] T value) => value is null;
    }
}
