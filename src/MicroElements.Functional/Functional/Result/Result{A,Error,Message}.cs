// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents the result of an operation with additional messages.
    /// Result can be Success: {A, Messages} or Failed: {Error, Messages}.
    /// </summary>
    /// <typeparam name="A">Success result type.</typeparam>
    /// <typeparam name="Error">Error type.</typeparam>
    /// <typeparam name="Message">Message type.</typeparam>
    public readonly struct Result<A, Error, Message> :
        IResultWithMessages,
        IEquatable<Result<A, Error, Message>>,
        IEquatable<SuccessResult<A>>,
        IEquatable<A>
    {
        /// <summary>
        /// Empty result.
        /// </summary>
        public static readonly Result<A, Error, Message> Empty = default;

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

        /// <summary>
        /// Message list for Success or Error state.
        /// </summary>
        public readonly IMessageList<Message> Messages;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates Success result.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="messages">Message list.</param>
        internal Result(A value, IMessageList<Message> messages)
        {
            State = ResultState.Success;
            Value = value;
            ErrorValue = default;
            Messages = messages;
        }

        /// <summary>
        /// Creates failed result.
        /// </summary>
        /// <param name="error">Error value.</param>
        /// <param name="messages">Message list.</param>
        internal Result(Error error, IMessageList<Message> messages)
        {
            State = ResultState.Error;
            Value = default;
            ErrorValue = error;
            Messages = messages;
        }

        #endregion

        #region Conversions

        /// <summary>
        /// Implicit conversion operator from <typeparamref name="A"/> to Success result.
        /// </summary>
        /// <param name="value">Success Value.</param>
        [Pure]
        public static implicit operator Result<A, Error, Message>(A value) =>
            Result.Success<A, Error, Message>(value, MessageList<Message>.Empty);

        /// <summary>
        /// Implicit conversion from <see cref="SuccessResult{A}"/>.
        /// </summary>
        /// <param name="successResult">SuccessResult of type <typeparamref name="A"/>.</param>
        [Pure]
        public static implicit operator Result<A, Error, Message>(in SuccessResult<A> successResult) =>
            Result.Success<A, Error, Message>(successResult.Value, MessageList<Message>.Empty);

        /// <summary>
        /// Implicit conversion from <see cref="FailedResult{Error}"/>.
        /// </summary>
        /// <param name="failedResult">FailedResult of type <typeparamref name="Error"/>.</param>
        [Pure]
        public static implicit operator Result<A, Error, Message>(in FailedResult<Error> failedResult) =>
            Result.Fail<A, Error, Message>(failedResult.ErrorValue, MessageList<Message>.Empty);

        /// <summary>
        /// Implicit conversion from <see cref="ValueWithMessages{A,Message}"/>.
        /// </summary>
        /// <param name="valueWithMessages">Success result data.</param>
        [Pure]
        public static implicit operator Result<A, Error, Message>(in ValueWithMessages<A, Message> valueWithMessages) =>
            Result.Success<A, Error, Message>(valueWithMessages.Value, valueWithMessages.Messages);

        /// <summary>
        /// Implicit conversion from <see cref="ValueTuple{A,Message}"/>.
        /// </summary>
        /// <param name="valueWithMessages">Success result data.</param>
        [Pure]
        public static implicit operator Result<A, Error, Message>(in ValueTuple<A, IEnumerable<Message>> valueWithMessages) =>
            Result.Success<A, Error, Message>(valueWithMessages.Item1, valueWithMessages.Item2);

        /// <summary>
        /// Implicit conversion from <see cref="ValueWithMessages{Error,Message}"/>.
        /// </summary>
        /// <param name="valueWithMessages">Failed result data.</param>
        [Pure]
        public static implicit operator Result<A, Error, Message>(in ValueWithMessages<Error, Message> valueWithMessages) =>
            Result.Fail<A, Error, Message>(valueWithMessages.Value, valueWithMessages.Messages);

        /// <summary>
        /// Implicit conversion from <see cref="ValueTuple{Error,Message}"/>.
        /// </summary>
        /// <param name="valueWithMessages">Failed result data.</param>
        [Pure]
        public static implicit operator Result<A, Error, Message>(in ValueTuple<Error, IEnumerable<Message>> valueWithMessages) =>
            Result.Fail<A, Error, Message>(valueWithMessages.Item1, valueWithMessages.Item2);

        /// <summary>
        /// Implicit conversion operator from <typeparamref name="Error"/> to Failed result.
        /// </summary>
        /// <param name="error">Error value.</param>
        [Pure]
        public static implicit operator Result<A, Error, Message>(Error error) =>
            Result.Fail<A, Error, Message>(error, MessageList.Empty<Message>());

        /// <summary>
        /// Implicit conversion operator from <typeparamref name="Message"/> to Failed result.
        /// </summary>
        /// <param name="message">Error message.</param>
        [Pure]
        public static implicit operator Result<A, Error, Message>(Message message) =>
            Result.Fail<A, Error, Message>(default, message);

        /// <summary>
        /// Explicit unsafe conversion to underlying type.
        /// </summary>
        /// <param name="source">Source result.</param>
        [Pure]
        public static explicit operator A(Result<A, Error, Message> source) =>
            source.GetValueOrThrow();

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
        public Type GetMessageType() => typeof(Message);

        /// <inheritdoc />
        public Res MatchUntyped<Res>(Func<object, Res> success, Func<object, Res> error)
            => IsSuccess ? success(Value) : error(ErrorValue);

        /// <inheritdoc />
        public Res MatchUntyped<Res>(Func<object, IEnumerable, Res> success, Func<object, IEnumerable, Res> error)
            => ResultOperations.Match(this, (a, list) => success(a, list), (e, list) => error(e, list));

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
            SuccessFunc<A, Message, B> success,
            ErrorFunc<Error, Message, B> error)
            => ResultOperations.Match(this, success, error);

        /// <summary>
        /// Executes a specified action based on the result state.
        /// </summary>
        /// <param name="success">Action to execute on <see cref="ResultState.Success"/> state.</param>
        /// <param name="error">Action to execute on <see cref="ResultState.Error"/> state.</param>
        /// <returns>Unit.</returns>
        public Unit Match(
            SuccessAction<A, Message> success,
            ErrorAction<Error, Message> error)
            => Match(success.ToFunc(), error.ToFunc());

        /// <summary>
        /// Monad Bind operation.
        /// </summary>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="bind">Bind function.</param>
        /// <returns>New result of type <typeparamref name="B"/>.</returns>
        public Result<B, Error, Message> Bind<B>(
            Func<A, Result<B, Error, Message>> bind)
            => ResultOperations.Bind(this, bind);

        #endregion

        #region Equality

        /// <inheritdoc />
        public bool Equals(Result<A, Error, Message> other)
        {
            return Match(
                (value, list) => other.IsSuccess && EqualityComparer<A>.Default.Equals(value, other.Value),
                (error, list) => other.IsFailed && EqualityComparer<Error>.Default.Equals(error, other.ErrorValue));
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
            if (ReferenceEquals(null, obj))
                return false;
            if (obj is Result<A, Error, Message> other)
                return Equals(other);
            if (obj is SuccessResult<A> successResult)
                return Equals(successResult);
            if (obj is A a)
                return Equals(a);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                if (IsFailed)
                    return -1;

                var hashCode = (int) State;
                hashCode = (hashCode * 397) ^ EqualityComparer<A>.Default.GetHashCode(Value);
                hashCode = (hashCode * 397) ^ EqualityComparer<Error>.Default.GetHashCode(ErrorValue);
                hashCode = (hashCode * 397) ^ (Messages != null ? Messages.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion

        /// <inheritdoc />
        public override string ToString() => IsSuccess ? $"{State}({Value})" : $"Result{State}({ErrorValue})";
    }
}
