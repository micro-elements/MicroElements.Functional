// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Contracts;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional
{
    /// <summary>
    /// ActionExtensions.
    /// </summary>
    public static class ActionExtensions
    {
        /// <summary>
        /// Converts <see cref="Action"/> to <see cref="Func{Unit}"/>.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <returns>Func that returns Unit.</returns>
        [Pure]
        public static Func<Unit> ToFunc(this Action action)
            => () => { action(); return Unit(); };

        /// <summary>
        /// Converts <see cref="Action{T}"/> to <see cref="Func{T, Unit}"/>.
        /// </summary>
        /// <typeparam name="T">Arg type.</typeparam>
        /// <param name="action">Action.</param>
        /// <returns>Func that returns Unit.</returns>
        [Pure]
        public static Func<T, Unit> ToFunc<T>(this Action<T> action)
            => t => { action(t); return Unit(); };

        /// <summary>
        /// Converts <see cref="Action{T1, T2}"/> to <see cref="Func{T1, T2, Unit}"/>.
        /// </summary>
        /// <typeparam name="T1">Arg1 type.</typeparam>
        /// <typeparam name="T2">Arg2 type.</typeparam>
        /// <param name="action">Action with two args.</param>
        /// <returns>Func that returns Unit.</returns>
        [Pure]
        public static Func<T1, T2, Unit> ToFunc<T1, T2>(this Action<T1, T2> action)
            => (t1, t2) => { action(t1, t2); return Unit(); };

        /// <summary>
        /// Combines two actions.
        /// </summary>
        /// <typeparam name="T">Action arg type.</typeparam>
        /// <param name="first">Optional action1.</param>
        /// <param name="second">Optional action2.</param>
        /// <returns>Action that is a combination of both actions.</returns>
        [Pure]
        public static Action<T> Combine<T>(this Action<T>? first, Action<T>? second)
        {
            return arg =>
            {
                first?.Invoke(arg);
                second?.Invoke(arg);
            };
        }
    }
}
