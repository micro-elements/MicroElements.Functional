// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Contracts;

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents the result of an operation: {A | Error}.
    /// </summary>
    /// <typeparam name="A">Success result type.</typeparam>
    /// <typeparam name="Error">Error type.</typeparam>
    public readonly struct Result<A, Error> : IResult
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
        /// Implicit conversion operator from A to Result.
        /// </summary>
        /// <param name="value">Value.</param>
        [Pure]
        public static implicit operator Result<A, Error>(A value) =>
            Result.Success<A, Error>(value);



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
        public TResult MatchUntyped<TResult>(Func<object, TResult> success, Func<TResult> error)
            => IsSuccess ? success(Value) : error();

        #endregion
    }

    /// <summary>
    /// Result helpers.
    /// </summary>
    public static partial class Result
    {
        /// <summary>
        /// Creates success result from value.
        /// </summary>
        /// <typeparam name="A">Success value type.</typeparam>
        /// <typeparam name="Error">Error value type.</typeparam>
        /// <param name="value">Success result value.</param>
        /// <returns>Success result.</returns>
        public static Result<A, Error> Success<A, Error>(A value)
            => new Result<A, Error>(value);

        /// <summary>
        /// Creates failed result.
        /// </summary>
        /// <typeparam name="A">Success value type.</typeparam>
        /// <typeparam name="Error">Error value type.</typeparam>
        /// <param name="error">Error value.</param>
        /// <returns>Failed result.</returns>
        public static Result<A, Error> Fail<A, Error>(Error error)
            => new Result<A, Error>(error);
    }
}
