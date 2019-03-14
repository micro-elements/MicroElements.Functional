// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional
{
    /// <summary>
    /// Option extensions.
    /// TODO: Document and tests.
    /// </summary>
    public static class OptionExtensions
    {
        public static Some<T> ToSome<T>(this T value) => new Some<T>(value);

        /// <summary>
        /// Gets value or throws if option is in <see cref="OptionState.None"/> state.
        /// </summary>
        /// <typeparam name="T">Option value type.</typeparam>
        /// <param name="option">Source option.</param>
        /// <exception cref="ValueIsNoneException">Option is in <see cref="OptionState.None"/> state.</exception>
        /// <returns>Value.</returns>
        public static T GetValueOrThrow<T>(this Option<T> option)
            => option.Match((t) => t, () => throw new ValueIsNoneException());

        public static T GetValueOrDefault<T>(this Option<T> opt, T defaultValue)
            => opt.MatchUnsafe((t) => t, () => defaultValue);

        public static T GetValueOrDefault<T>(this Option<T> opt, Func<T> fallback)
            => opt.MatchUnsafe((t) => t, () => fallback());

        public static Option<T> OrElse<T>(this Option<T> left, Option<T> right)
            => left.Match((_) => left, () => right);

        public static Option<T> OrElse<T>(this Option<T> left, Func<Option<T>> right)
            => left.Match((_) => left, () => right());

        public static Option<R> Map<T, R>(this OptionNone _, Func<T, R> f)
            => None;

        public static Option<R> Map<T, R>(this Some<T> some, Func<T, R> f)
            => Some(f(some.Value));

        public static Option<R> Map<T, R>(this Option<T> option, Func<T, R> f)
            => option.Match(
                (t) => Some(f(t)),
                () => None);

        public static Option<Func<T2, R>> Map<T1, T2, R>(this Option<T1> option, Func<T1, T2, R> func)
            => option.Map(func.Curry());

        public static Option<Func<T2, T3, R>> Map<T1, T2, T3, R>(this Option<T1> option, Func<T1, T2, T3, R> func)
            => option.Map(func.CurryFirst());

        // LINQ

        public static Option<R> Select<T, R>(this Option<T> option, Func<T, R> func)
            => option.Map(func);

        public static Option<T> Where<T>(this Option<T> option, Func<T, bool> predicate)
            => option.Match(
                (t) => predicate(t) ? option : None,
                () => None);

        public static Option<RR> SelectMany<T, R, RR>(this Option<T> option, Func<T, Option<R>> bind, Func<T, R, RR> project)
            => option.Match(
                (t) => bind(t).Match(
                    (r) => Some(project(t, r)),
                    () => None),
                () => None);
    }
}
