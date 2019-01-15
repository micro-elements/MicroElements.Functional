// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Contracts;

namespace MicroElements.Functional
{
    /// <summary>
    /// Common functions that acts like a functional language's 'prelude'.
    /// </summary>
    public static partial class Prelude
    {
        /// <summary>
        /// Returns Unit result (void).
        /// </summary>
        /// <returns>Unit.</returns>
        public static Unit Unit() => default(Unit);

        /// <summary>
        /// Curry the function 'f' provided.
        /// You can then partially apply by calling:
        ///
        ///     var curried = curry(f);
        ///     var r = curried(a)(b);
        ///
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, R>> Curry<T1, T2, R>(this Func<T1, T2, R> f)
            => a => b => f(a, b);

        /// <summary>
        /// Curry the function 'f' provided.
        /// You can then partially apply by calling:
        ///
        ///     var curried = curry(f);
        ///     var r = curried(a)(b)(c);
        ///
        /// </summary>
        [Pure]
        public static Func<T1, Func<T2, Func<T3, R>>> Curry<T1, T2, T3, R>(this Func<T1, T2, T3, R> f)
            => a => b => c => f(a, b, c);


        public static Func<T1, Func<T2, T3, R>> CurryFirst<T1, T2, T3, R>(this Func<T1, T2, T3, R> f)
            => a => (b, c) => f(a, b, c);
    }
}
