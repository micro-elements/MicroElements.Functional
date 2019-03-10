// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

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
        public static T AssertNotNullResult<T>(this T result)
            => result.IsNull()
                ? throw new ResultIsNullException()
                : result;

        /// <summary>
        /// Checks that argument of an operation is not null.
        /// </summary>
        /// <typeparam name="T">Argument type.</typeparam>
        /// <param name="arg">The argument.</param>
        /// <param name="name">The argument name.</param>
        /// <returns>NotNull arg or throws <see cref="ArgumentNullException"/>.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static T AssertArgumentNotNull<T>(this T arg, string name)
        {
            if (arg.IsNull())
                throw new ArgumentNullException(name);
            return arg;
        }
    }
}
