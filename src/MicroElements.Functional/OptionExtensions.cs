// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional
{
    /// <summary>
    /// Option extensions.
    /// </summary>
    public static class OptionExtensions
    {
        internal static bool IsSome<T>(this Option<T> option)
            => option.Match((_) => true, () => false);

        internal static T ValueUnsafe<T>(this Option<T> option)
            => option.Match((t) => t, () => { throw new InvalidOperationException(); });

        public static T GetOrElse<T>(this Option<T> opt, T defaultValue)
            => opt.Match((t) => t, () => defaultValue);

        public static T GetOrElse<T>(this Option<T> opt, Func<T> fallback)
            => opt.Match((t) => t, () => fallback());

        public static Option<T> OrElse<T>(this Option<T> left, Option<T> right)
            => left.Match((_) => left, () => right);

        public static Option<T> OrElse<T>(this Option<T> left, Func<Option<T>> right)
            => left.Match((_) => left, () => right());

        public static Option<Result> Bind<T, Result>(this Option<T> option, Func<T, Option<Result>> f)
            => option.Match(t => f(t), () => None);

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
