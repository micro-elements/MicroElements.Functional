// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents the result of an operation: {A | Error}.
    /// </summary>
    /// <typeparam name="A">Success result type.</typeparam>
    /// <typeparam name="Error">Error type.</typeparam>
    public readonly struct Result<A, Error> :
        IResult,
        IEquatable<Result<A, Error>>,
        IEquatable<SuccessResult<A>>,
        IEquatable<A>
    {
        /// <summary>
        /// Empty result.
        /// </summary>
        public static readonly Result<A, Error> Empty = default(Result<A, Error>);

        #region Fields

        /// <summary>
        /// Result state.
        /// </summary>
        internal readonly ResultState State;

        /// <summary>
        /// Success value.
        /// </summary>
        internal readonly A Value;

        /// <summary>
        /// Error value.
        /// </summary>
        internal readonly Error ErrorValue;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates success result.
        /// </summary>
        /// <param name="value">Value.</param>
        internal Result(A value)
        {
            State = ResultState.Success;
            Value = value;
            ErrorValue = default;
        }

        /// <summary>
        /// Creates failed result.
        /// </summary>
        /// <param name="error">Error value.</param>
        internal Result(Error error)
        {
            State = ResultState.Error;
            Value = default;
            ErrorValue = error;
        }

        #endregion

        #region Conversions

        /// <summary>
        /// Implicit conversion operator from <typeparamref name="A"/> to Result.
        /// </summary>
        /// <param name="value">Success Value.</param>
        [Pure]
        public static implicit operator Result<A, Error>(A value) =>
            Result.Success<A, Error>(value);

        /// <summary>
        /// Implicit conversion from <see cref="SuccessResult{A}"/>.
        /// </summary>
        /// <param name="successResult">SuccessResult of type <typeparamref name="A"/>.</param>
        [Pure]
        public static implicit operator Result<A, Error>(SuccessResult<A> successResult) =>
            Result.Success<A, Error>(successResult.Value);

        /// <summary>
        /// Implicit conversion from <see cref="FailedResult{Error}"/>.
        /// </summary>
        /// <param name="failedResult">FailedResult of type <typeparamref name="Error"/>.</param>
        [Pure]
        public static implicit operator Result<A, Error>(FailedResult<Error> failedResult) =>
            Result.Fail<A, Error>(failedResult.ErrorValue);

        /// <summary>
        /// Implicit conversion from <typeparamref name="Error"/> to Result.
        /// </summary>
        /// <param name="error">Error value.</param>
        [Pure]
        public static implicit operator Result<A, Error>(Error error) =>
            Result.Fail<A, Error>(error);

        #endregion

        #region IResult

        /// <inheritdoc />
        public bool IsSuccess => State == ResultState.Success;

        /// <inheritdoc />
        public bool IsFailed => State == ResultState.Error;

        /// <inheritdoc />
        public Type GetSuccessValueType() => typeof(A);

        /// <inheritdoc />
        public Type GetErrorValueType() => typeof(Error);

        /// <inheritdoc />
        public TResult MatchUntyped<TResult>(Func<object, TResult> success, Func<object, TResult> error)
            => IsSuccess ? success(Value) : error(ErrorValue);

        #endregion

        #region Operations

        /// <summary>
        /// Evaluates a specified function based on the result state.
        /// </summary>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="success">Function to evaluate on <see cref="ResultState.Success"/> state.</param>
        /// <param name="error">Function to evaluate on <see cref="ResultState.Error"/> state.</param>
        /// <returns>Evaluated result.</returns>
        public B Match<B>(
            SuccessFunc<A, B> success,
            ErrorFunc<Error, B> error)
            => ResultOperations.Match(this, success, error);

        /// <summary>
        /// Executes a specified action based on the result state.
        /// </summary>
        /// <param name="success">Action to execute on <see cref="ResultState.Success"/> state.</param>
        /// <param name="error">Action to execute on <see cref="ResultState.Error"/> state.</param>
        /// <returns>Unit.</returns>
        public Unit Match(
            SuccessAction<A> success,
            ErrorAction<Error> error)
            => Match(success.ToFunc(), error.ToFunc());

        /// <summary>
        /// Monad Bind operation.
        /// </summary>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="bind">Bind function.</param>
        /// <returns>New result of type <typeparamref name="B"/>.</returns>
        public Result<B, Error> Bind<B>(
            Func<A, Result<B, Error>> bind)
            => ResultOperations.Bind(this, bind);

        #endregion

        #region Equality

        /// <inheritdoc />
        public bool Equals(Result<A, Error> other)
        {
            return Match(
                (value) => other.IsSuccess && EqualityComparer<A>.Default.Equals(value, other.Value),
                (error) => other.IsFailed && EqualityComparer<Error>.Default.Equals(error, other.ErrorValue));
        }

        /// <inheritdoc />
        public bool Equals(SuccessResult<A> other)
        {
            return IsSuccess == other.IsSuccess && EqualityComparer<A>.Default.Equals(Value, other.Value);
        }

        /// <inheritdoc />
        public bool Equals(A value)
        {
            return IsSuccess && EqualityComparer<A>.Default.Equals(Value, value);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Result<A, Error> other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                if (IsFailed)
                    return -1;

                int hashCode = (int)State;
                hashCode = (hashCode * 397) ^ EqualityComparer<A>.Default.GetHashCode(Value);
                hashCode = (hashCode * 397) ^ EqualityComparer<Error>.Default.GetHashCode(ErrorValue);
                return hashCode;
            }
        }

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return IsSuccess ? $"Result: {State}({Value})" : $"Result: {State}({ErrorValue})";
        }
    }
}
