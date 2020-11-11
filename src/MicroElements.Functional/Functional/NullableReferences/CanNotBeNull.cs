// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents value that can not be null in terms of nullable-references.
    /// See: https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references.
    /// Note: CanNotBeNull can not be created with null value so you can use it safely in your code.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    public readonly struct CanNotBeNull<T>
    {
        private readonly bool _isInitialized;
        private readonly T _value;

        /// <summary>
        /// Gets value that can not be null in terms of nullable-references.
        /// </summary>
        [NotNull]
        [DisallowNull]
        public T Value
        {
            get
            {
                if (!_isInitialized)
                    throw new InvalidOperationException("default(CanNotBeNull) can not be used uninitialized.");
                return _value!;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanBeNull{T}"/> struct.
        /// </summary>
        /// <param name="value">Value that can not be null.</param>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        public CanNotBeNull([DisallowNull] T value)
        {
            // AssertArgumentNotNull is from MicroElements.Functional
            _value = value.AssertArgumentNotNull(nameof(value));
            _isInitialized = true;
        }

        /// <summary>
        /// Implicit conversion to the base value type.
        /// </summary>
        /// <param name="canBeNull"><see cref="CanBeNull{T}"/> value.</param>
        [return: NotNull]
        public static implicit operator T(in CanNotBeNull<T> canBeNull) => canBeNull.Value;

        /// <summary>
        /// Explicit conversion to <see cref="CanNotBeNull{T}"/>.
        /// </summary>
        /// <param name="value">Value.</param>
        public static explicit operator CanNotBeNull<T>([DisallowNull] T value) => new CanNotBeNull<T>(value);
    }
}
