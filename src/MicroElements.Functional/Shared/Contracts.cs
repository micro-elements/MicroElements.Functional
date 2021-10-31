// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using MicroElements.Reflection.ObjectExtensions;

// ReSharper disable once CheckNamespace
namespace MicroElements.Functional
{
    /// <summary>
    /// Contract check methods.
    /// </summary>
    public static class Contracts
    {
        /// <summary>
        /// Checks that result is not null.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="result">Result of an operation.</param>
        /// <returns>Result or throws <see cref="ResultIsNullException"/>.</returns>
        /// <exception cref="ResultIsNullException">Result is null.</exception>
        [return: NotNull]
        public static T AssertNotNullResult<T>([AllowNull] this T result)
            => result.IsNull()
                ? throw new ResultIsNullException()
                : result;
    }
}
