// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace MicroElements.Functional
{
    /// <summary>
    /// Some wrapper for value. Can not be null.
    /// Implicitly converts to <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    public struct Some<T>
    {
        /// <summary>
        /// Wrapped not null value.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Some{T}"/> struct.
        /// </summary>
        /// <param name="value">Value to wrap.</param>
        public Some(T value)
        {
            if (value.IsNull())
                throw new ArgumentNullException(nameof(value), "Cannot wrap a null value in a 'Some'; use 'None' instead");
            Value = value;
        }
    }
}
