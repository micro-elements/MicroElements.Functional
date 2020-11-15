// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Contracts;

namespace MicroElements.Functional
{
    /// <summary>
    /// FuncExtensions.
    /// </summary>
    public static class FuncExtensions
    {
        /// <summary>
        /// Combines two functions.
        /// </summary>
        /// <typeparam name="A">Input arg type.</typeparam>
        /// <typeparam name="B">Temp arg type.</typeparam>
        /// <typeparam name="C">Result arg type.</typeparam>
        /// <param name="first">First function.</param>
        /// <param name="second">Second function.</param>
        /// <returns>Function that is a combination of both functions.</returns>
        [Pure]
        public static Func<A, C> Combine<A, B, C>(this Func<A, B> first, Func<B, C> second)
        {
            return a =>
            {
                var b = first(a);
                var c = second(b);
                return c;
            };
        }
    }
}
