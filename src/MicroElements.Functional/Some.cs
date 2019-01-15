// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace MicroElements.Functional
{
    /// <summary>
    /// Some wrapper for value. Can not be null.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    public struct Some<T>
    {
        public T Value { get; }

        public Some(T value)
        {
            if (value.IsNull())
                throw new ArgumentNullException(nameof(value), "Cannot wrap a null value in a 'Some'; use 'None' instead");
            Value = value;
        }
    }
}
