// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional
{
    /// <summary>
    /// Option extensions.
    /// TODO: Document and tests.
    /// </summary>
    public static class OptionExtensions
    {
        /// <summary>
        /// Converts value to Some(value).
        /// Throws if value is null.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">Not null value.</param>
        /// <returns>Option in Some state.</returns>
        public static Some<T> ToSome<T>([DisallowNull]this T value) => new Some<T>(value);

        /// <summary>
        /// Converts value to optional value (Some or None).
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">Value or null.</param>
        /// <returns>Option.</returns>
        public static Option<T> ToOption<T>([AllowNull] this T value) => value;

        /// <summary>
        /// Gets value or throws if option is in <see cref="OptionState.None"/> state.
        /// </summary>
        /// <typeparam name="T">Option value type.</typeparam>
        /// <param name="option">Source option.</param>
        /// <exception cref="ValueIsNoneException">Option is in <see cref="OptionState.None"/> state.</exception>
        /// <returns>Value.</returns>
        public static T GetValueOrThrow<T>(this in Option<T> option)
            => option.Match((t) => t, () => throw new ValueIsNoneException());

        /// <summary>
        /// Gets value or returns default value.
        /// </summary>
        /// <typeparam name="T">Option type.</typeparam>
        /// <param name="source">Source option.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>Option value or default value.</returns>
        [return: MaybeNull]
        public static T GetValueOrDefault<T>(this in Option<T> source, [AllowNull] T defaultValue = default)
            => source.MatchUnsafe((t) => t, defaultValue);

        /// <summary>
        /// Gets value or returns default value.
        /// </summary>
        /// <typeparam name="T">Option type.</typeparam>
        /// <param name="source">Source option.</param>
        /// <param name="fallback">Function that returns default value.</param>
        /// <returns>Option value or default value.</returns>
        [return: MaybeNull]
        public static T GetValueOrDefault<T>(this in Option<T> source, Func<T> fallback)
            => source.MatchUnsafe((t) => t, fallback);

        public static Option<T> OrElse<T>(this Option<T> left, Option<T> right)
            => left.Match((_) => left, () => right);

        public static Option<T> OrElse<T>(this Option<T> left, Func<Option<T>> right)
            => left.Match((_) => left, () => right());

        public static Option<B> Map<A, B>(this OptionNone _, Func<A, B> map)
            => None;

        public static Option<B> Map<A, B>(this in Some<A> some, Func<A, B> map)
            => Some(map(some.Value));

        public static Option<B> Map<A, B>(this in Option<A> option, Func<A, B> map)
            => option.Match(a => Some(map(a)), None);

        public static Option<Func<T2, R>> Map<T1, T2, R>(this in Option<T1> option, Func<T1, T2, R> func)
            => option.Map(func.Curry());

        public static Option<Func<T2, T3, R>> Map<T1, T2, T3, R>(this in Option<T1> option, Func<T1, T2, T3, R> func)
            => option.Map(func.CurryFirst());

        // LINQ

        public static Option<R> Select<T, R>(this in Option<T> option, Func<T, R> func)
            => option.Map(func);

        public static Option<T> Where<T>(this Option<T> option, Func<T, bool> predicate)
            => option.Match(
                (t) => predicate(t) ? option : None,
                () => None);

        public static Option<RR> SelectMany<T, R, RR>(this in Option<T> option, Func<T, Option<R>> bind, Func<T, R, RR> project)
            => option.Match(
                (t) => bind(t).Match(
                    (r) => Some(project(t, r)),
                    () => None),
                () => None);

        public static Option<B> TryMap<A, B>(this in Option<A> option, Func<A, B> map)
            => option.Match(
                some: a =>
                {
                    try
                    {
                        B b = map(a);
                        return Some(b);
                    }
                    catch
                    {
                        return None;
                    }
                },
                none: None);
    }
}
