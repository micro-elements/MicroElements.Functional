// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Contracts;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional
{
    /// <summary>
    /// Option operations.
    /// </summary>
    public static class OptionOperations
    {
        /// <summary>
        /// Evaluates a specified function based on the option state.
        /// </summary>
        /// <typeparam name="A">Value type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source option.</param>
        /// <param name="some">Function to evaluate on <see cref="OptionState.Some"/> state.</param>
        /// <param name="none">Function to evaluate on <see cref="OptionState.None"/> state.</param>
        /// <returns>NotNull evaluated result.</returns>
        /// <exception cref="ArgumentNullException">some is null.</exception>
        /// <exception cref="ArgumentNullException">none is null.</exception>
        /// <exception cref="ResultIsNullException">result is null.</exception>
        [Pure]
        public static B Match<A, B>(
            in Option<A> source,
            Func<A, B> some,
            Func<B> none)
        {
            some.AssertArgumentNotNull(nameof(some));
            none.AssertArgumentNotNull(nameof(none));

            var res = source.IsSome
                ? some(source.Value)
                : none();
            return res.AssertNotNullResult();
        }

        /// <summary>
        /// Evaluates a specified function based on the option state.
        /// </summary>
        /// <typeparam name="A">Value type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source option.</param>
        /// <param name="some">Function to evaluate on <see cref="OptionState.Some"/> state.</param>
        /// <param name="none">Result value in case of <see cref="OptionState.None"/> state.</param>
        /// <returns>NotNull evaluated result.</returns>
        /// <exception cref="ArgumentNullException">some is null.</exception>
        /// <exception cref="ArgumentNullException">none is null.</exception>
        /// <exception cref="ResultIsNullException">result is null.</exception>
        [Pure]
        public static B Match<A, B>(
            in Option<A> source,
            Func<A, B> some,
            B none)
        {
            some.AssertArgumentNotNull(nameof(some));
            none.AssertArgumentNotNull(nameof(none));

            var res = source.IsSome
                ? some(source.Value)
                : none;
            return res.AssertNotNullResult();
        }

        /// <summary>
        /// Executes a specified action based on the option state.
        /// </summary>
        /// <typeparam name="A">Value type.</typeparam>
        /// <param name="source">Source option.</param>
        /// <param name="some">Action to evaluate on <see cref="OptionState.Some"/> state.</param>
        /// <param name="none">Action to evaluate on <see cref="OptionState.None"/> state.</param>
        /// <returns><see cref="Unit"/>.</returns>
        /// <exception cref="ArgumentNullException">some is null.</exception>
        /// <exception cref="ArgumentNullException">none is null.</exception>
        [Pure]
        public static Unit Match<A>(
            in Option<A> source,
            Action<A> some,
            Action none)
        {
            some.AssertArgumentNotNull(nameof(some));
            none.AssertArgumentNotNull(nameof(none));

            if (source.IsSome)
                some(source.Value);
            else
                none();
            return Unit.Default;
        }

        /// <summary>
        /// Option bind operation.
        /// </summary>
        /// <typeparam name="A">Input value type.</typeparam>
        /// <typeparam name="B">Result value type.</typeparam>
        /// <param name="option">Source option.</param>
        /// <param name="bind">Bind func.</param>
        /// <returns>Option of type <typeparamref name="B"/>.</returns>
        public static Option<B> Bind<A, B>(
            this in Option<A> option,
            Func<A, Option<B>> bind)
        {
            bind.AssertArgumentNotNull(nameof(bind));

            return option.Match(value => bind(value), () => None);
        }
    }
}
