// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static MicroElements.Functional.Prelude;

#pragma warning disable SA1629 // Documentation text should end with a period
// ReSharper disable CheckNamespace
namespace MicroElements.Functional
{
    /// <summary>
    /// Represents the result of an operation:
    ///
    ///     A | Exception
    ///
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct Result<A> : IEquatable<Result<A>>
    {
        public static readonly Result<A> Bottom = default(Result<A>);

        internal readonly ResultState State;
        internal readonly A Value;
        internal readonly Exception Exception;

        /// <summary>
        /// Constructor of a concrete value
        /// </summary>
        /// <param name="value"></param>
        [Pure]
        public Result(A value)
        {
            State = ResultState.Success;
            Value = value;
            Exception = null;
        }

        /// <summary>
        /// Constructor of an error value
        /// </summary>
        /// <param name="e"></param>
        [Pure]
        public Result(Exception e)
        {
            State = ResultState.Faulted;
            Exception = e;
            Value = default(A);
        }

        /// <summary>
        /// Implicit conversion operator from A to Result<A>
        /// </summary>
        /// <param name="value">Value</param>
        [Pure]
        public static implicit operator Result<A>(A value) =>
            new Result<A>(value);

        /// <summary>
        /// True if the result is faulted
        /// </summary>
        [Pure]
        public bool IsFaulted =>
            State == ResultState.Faulted;

        /// <summary>
        /// True if the struct is in an success
        /// </summary>
        [Pure]
        public bool IsSuccess =>
            State == ResultState.Success;

        /// <summary>
        /// True if the struct is in an invalid state
        /// </summary>
        [Pure]
        public bool IsBottom =>
            State == ResultState.Faulted && (Exception == null || Exception is BottomException);

        /// <inheritdoc />
        public bool Equals(Result<A> other)
        {
            return State == other.State && EqualityComparer<A>.Default.Equals(Value, other.Value) && Equals(Exception, other.Exception);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Result<A> other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                if (IsFaulted) return -1;

                var hashCode = (int) State;
                hashCode = (hashCode * 397) ^ EqualityComparer<A>.Default.GetHashCode(Value);
                hashCode = (hashCode * 397) ^ (Exception != null ? Exception.GetHashCode() : 0);
                return hashCode;
            }
        }

        [Pure]
        public A IfFail(A defaultValue) =>
            IsFaulted
                ? defaultValue
                : Value;

        [Pure]
        public A IfFail(Func<Exception, A> f) =>
            IsFaulted
                ? f(Exception ?? BottomException.Default)
                : Value;

        public Unit IfFail(Action<Exception> f)
        {
            if (IsFaulted)
                f(Exception ?? BottomException.Default);
            return unit;
        }

        public Unit IfSucc(Action<A> f)
        {
            if (IsSuccess)
                f(Value);
            return unit;
        }

        [Pure]
        public R Match<R>(Func<A, R> Succ, Func<Exception, R> Fail) =>
            IsBottom
                ? Fail(BottomException.Default)
                : IsFaulted
                    ? Fail(Exception)
                    : Succ(Value);

        //[Pure]
        //internal OptionalResult<A> ToOptional() =>
        //    IsFaulted
        //        ? new OptionalResult<A>(Exception)
        //        : new OptionalResult<A>(Optional(Value));

        [Pure]
        public Result<B> Map<B>(Func<A, B> f) =>
            IsBottom
                ? Result<B>.Bottom
                : IsFaulted
                    ? new Result<B>(Exception)
                    : new Result<B>(f(Value));

        [Pure]
        public async Task<Result<B>> MapAsync<B>(Func<A, Task<B>> f) =>
            IsBottom
                ? Result<B>.Bottom
                : IsFaulted
                    ? new Result<B>(Exception)
                    : new Result<B>(await f(Value));
    }
}
