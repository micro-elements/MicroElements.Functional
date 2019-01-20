// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using static MicroElements.Functional.Prelude;

// ReSharper disable CheckNamespace
namespace MicroElements.Functional
{
    /// <summary>
    /// Either L R
    /// Holds one of two values 'Left' or 'Right'.  Usually 'Left' is considered 'wrong' or 'in error', and
    /// 'Right' is, well, right.  So when the Either is in a Left state, it cancels computations like bind
    /// or map, etc.  So you can see Left as an 'early out, with a message'.  Unlike Option that has None
    /// as its alternative value (i.e. it has an 'early out, but no message').
    /// </summary>
    public struct Either<L, R> :
        IEither
        /*// todo: IEquatable<Either<L, R>>,
        IEquatable<Either.Right<R>>,
        IEquatable<R>,*/
    {
        internal L Left { get; }
        internal R Right { get; }

        /// <summary>
        /// Is the Either in a Right state?
        /// </summary>
        public bool IsRight { get; }

        /// <summary>
        /// Is the Either in a Left state?
        /// </summary>
        public bool IsLeft => !IsRight;

        /// <inheritdoc />
        public Type GetUnderlyingRightType() => typeof(R);

        /// <inheritdoc />
        public Type GetUnderlyingLeftType() => typeof(L);

        internal Either(L left)
        {
            IsRight = false;
            Left = left;
            Right = default(R);
        }

        internal Either(R right)
        {
            IsRight = true;
            Right = right;
            Left = default(L);
        }

        public static implicit operator Either<L, R>(L left) => new Either<L, R>(left);
        public static implicit operator Either<L, R>(R right) => new Either<L, R>(right);

        public static implicit operator Either<L, R>(Either.Left<L> left) => new Either<L, R>(left.Value);
        public static implicit operator Either<L, R>(Either.Right<R> right) => new Either<L, R>(right.Value);

        public TR Match<TR>(Func<L, TR> Left, Func<R, TR> Right)
            => IsLeft ? Left(this.Left) : Right(this.Right);

        public Unit Match(Action<L> Left, Action<R> Right)
            => Match(Left.ToFunc(), Right.ToFunc());

        public IEnumerator<R> AsEnumerable()
        {
            if (IsRight)
                yield return Right;
        }

        public override string ToString() => Match(l => $"Left({l})", r => $"Right({r})");
    }

    public static class Either
    {
        public struct Left<L>
        {
            internal L Value { get; }
            internal Left(L value) { Value = value; }

            public override string ToString() => $"Left({Value})";
        }

        public struct Right<R>
        {
            internal R Value { get; }
            internal Right(R value) { Value = value; }

            public override string ToString() => $"Right({Value})";

            public Right<RR> Map<L, RR>(Func<R, RR> f) => Right(f(Value));
            public Either<L, RR> Bind<L, RR>(Func<R, Either<L, RR>> f) => f(Value);
        }
    }

    public static class EitherExt
    {
        public static Either<L, RR> Map<L, R, RR>
            (this Either<L, R> @this, Func<R, RR> f)
            => @this.Match<Either<L, RR>>(
                l => Left(l),
                r => Right(f(r)));

        public static Either<LL, RR> Map<L, LL, R, RR>
            (this Either<L, R> @this, Func<L, LL> left, Func<R, RR> right)
            => @this.Match<Either<LL, RR>>(
                l => Left(left(l)),
                r => Right(right(r)));

        public static Either<L, Unit> ForEach<L, R>
            (this Either<L, R> @this, Action<R> act)
            => Map(@this, act.ToFunc());

        public static Either<L, RR> Bind<L, R, RR>
            (this Either<L, R> @this, Func<R, Either<L, RR>> f)
            => @this.Match(
                l => Left(l),
                r => f(r));


        // LINQ

        public static Either<L, R> Select<L, T, R>(this Either<L, T> @this
            , Func<T, R> map) => @this.Map(map);


        public static Either<L, RR> SelectMany<L, T, R, RR>(this Either<L, T> @this
            , Func<T, Either<L, R>> bind, Func<T, R, RR> project)
            => @this.Match(
                Left: l => Left(l),
                Right: t =>
                    bind(@this.Right).Match<Either<L, RR>>(
                        Left: l => Left(l),
                        Right: r => project(t, r)));
    }

    public interface IEither
    {
        bool IsRight { get; }

        bool IsLeft { get; }

        Type GetUnderlyingRightType();
        Type GetUnderlyingLeftType();
    }

    public static partial class Prelude
    {
        public static Either.Left<L> Left<L>(L l) => new Either.Left<L>(l);
        public static Either.Right<R> Right<R>(R r) => new Either.Right<R>(r);
    }
}
