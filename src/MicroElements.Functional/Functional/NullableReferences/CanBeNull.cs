// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents value that can be null in terms of nullable-references.
    /// See: https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    public readonly struct CanBeNull<T>
    {
        /// <summary>
        /// Gets value that can be null in terms of nullable-references.
        /// </summary>
        [MaybeNull]
        [AllowNull]
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanBeNull{T}"/> struct.
        /// </summary>
        /// <param name="value">Value that can be null.</param>
        public CanBeNull([AllowNull] T value) => Value = value;

        /// <summary>
        /// Implicit conversion to the base value type.
        /// </summary>
        /// <param name="canBeNull"><see cref="CanBeNull{T}"/> value.</param>
        [return: MaybeNull]
        public static implicit operator T(in CanBeNull<T> canBeNull) => canBeNull.Value;

        /// <summary>
        /// Implicit conversion to <see cref="CanBeNull{T}"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator CanBeNull<T>([AllowNull] T value) => new CanBeNull<T>(value);
    }
}
