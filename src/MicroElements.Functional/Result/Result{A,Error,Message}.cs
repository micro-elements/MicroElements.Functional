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
    public struct Result<A, Error, Message> :
        IResultWithMessages
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
        /// Implicit conversion operator from A to Result.
        /// </summary>
        /// <param name="value">Value.</param>
        [Pure]
        public static implicit operator Result<A, Error, Message>(A value) =>
            Result.Success<A, Error, Message>(value, MessageList<Message>.Empty);

        public static implicit operator Result<A, Error, Message>(Message message) =>
            Result.Fail<A, Error, Message>(default, message);

        public static implicit operator Result<A, Error, Message>(Error error) =>
            Result.Fail<A, Error, Message>(error, MessageList.Empty<Message>());

        //[Pure]
        //public static implicit operator Result<A, Error>(Result<A, Error, Message> result) =>
        //    result.Match;

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
        public Res MatchUntyped<Res>(Func<object, Res> success, Func<Res> error)
            => IsSuccess ? success(Value) : error();

        /// <inheritdoc />
        public Res MatchUntyped<Res>(Func<object, IEnumerable, Res> success, Func<object, IEnumerable, Res> error)
            => ResultOperations.Match(ref this, (a, list) => success(a, list), (e, list) => error(e, list));

        #endregion

        #region Operations

        /// <summary>
        /// Evaluates a specified function based on the result state.
        /// </summary>
        /// <typeparam name="Res">Result type.</typeparam>
        /// <param name="success">Function to evaluate on <see cref="ResultState.Success"/> state.</param>
        /// <param name="error">Function to evaluate on <see cref="ResultState.Error"/> state.</param>
        /// <returns>Evaluated result.</returns>
        public Res Match<Res>(
            Func<A, IMessageList<Message>, Res> success,
            Func<Error, IMessageList<Message>, Res> error)
            => ResultOperations.Match(ref this, success, error);

        /// <summary>
        /// Executes a specified action based on the result state.
        /// </summary>
        /// <param name="success">Action to execute on <see cref="ResultState.Success"/> state.</param>
        /// <param name="error">Action to execute on <see cref="ResultState.Error"/> state.</param>
        /// <returns>Unit.</returns>
        public Unit Match(
            Action<A, IMessageList<Message>> success,
            Action<Error, IMessageList<Message>> error)
            => Match(success.ToFunc(), error.ToFunc());

        public Result<B, Error, Message> Bind<B>(
            Func<A, Result<B, Error, Message>> bind)
            => ResultOperations.Bind(ref this, bind);

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
        /// <typeparam name="Message">Message type.</typeparam>
        /// <param name="value">Success result value.</param>
        /// <param name="messages">Message list.</param>
        /// <returns>Success result.</returns>
        public static Result<A, Error, Message> Success<A, Error, Message>(A value, IEnumerable<Message> messages)
            => new Result<A, Error, Message>(value, messages.ToMessageList());

        public static Result<A, Error, Message> Success<A, Error, Message>(A value)
            => new Result<A, Error, Message>(value, MessageList<Message>.Empty);

        public static Result<A, Exception, string> Success<A>(A value)
            => new Result<A, Exception, string>(value, MessageList<string>.Empty);

        /// <summary>
        /// Creates failed result.
        /// </summary>
        /// <typeparam name="A">Success value type.</typeparam>
        /// <typeparam name="Error">Error value type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <param name="error">Error value.</param>
        /// <param name="messages">Message list.</param>
        /// <returns>Failed result.</returns>
        public static Result<A, Error, Message> Fail<A, Error, Message>(Error error, IEnumerable<Message> messages)
            => new Result<A, Error, Message>(error, messages.ToMessageList());

        public static Result<A, Error, Message> Fail<A, Error, Message>(Error error, Message message)
            => new Result<A, Error, Message>(error, new MessageList<Message>(message));

        [Obsolete("add sugar")]
        public static Result<A, Exception, string> FailFromMessages<A>(IEnumerable<string> messages)
            => new Result<A, Exception, string>(default(Exception), messages.ToMessageList());
    }

    /// <summary>
    /// Monadic operations.
    /// </summary>
    public static partial class ResultOperations
    {
        /// <summary>
        /// Evaluates a specified function based on the result state.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="success">Function to evaluate on <see cref="ResultState.Success"/> state.</param>
        /// <param name="error">Function to evaluate on <see cref="ResultState.Error"/> state.</param>
        /// <returns>Evaluated result.</returns>
        [Pure]
        public static B Match<A, Error, Message, B>(
            this ref Result<A, Error, Message> source,
            Func<A, IMessageList<Message>, B> success,
            Func<Error, IMessageList<Message>, B> error)
        {
            success.AssertArgumentNotNull(nameof(success));
            error.AssertArgumentNotNull(nameof(error));

            var result = source.IsSuccess
                ? success(source.Value, source.Messages)
                : error(source.ErrorValue, source.Messages);
            return result.AssertNotNullResult();
        }

        /// <summary>
        /// Creates new Result with new messages added to message list.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="messages">Messages to add.</param>
        /// <returns>New Result with added messages.</returns>
        [Pure]
        public static Result<A, Error, Message> AddMessages<A, Error, Message>(
            this ref Result<A, Error, Message> source,
            IEnumerable<Message> messages)
        {
            return source.Match(
                (value, list) => new Result<A, Error, Message>(value, list.AddRange(messages)),
                (error, list) => new Result<A, Error, Message>(error, list.AddRange(messages)));
        }

        /// <summary>
        /// New Result of type <typeparamref name="B"/> as a result of <paramref name="map"/> function.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="map">Map function.</param>
        /// <returns>New result of type <typeparamref name="B"/>.</returns>
        public static Result<B, Error, Message> Map<A, Error, Message, B>(
            this ref Result<A, Error, Message> source,
            Func<A, B> map)
            => source.Match(
                (value, list) => new Result<B, Error, Message>(map(value), list),
                (error, list) => new Result<B, Error, Message>(error, list));

        /// <summary>
        /// New Result of type <typeparamref name="B"/> as a result of <paramref name="map"/> function.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="selector">Map function.</param>
        /// <returns>New result of type <typeparamref name="B"/>.</returns>
        public static Result<B, Error, Message> Select<A, Error, Message, B>(
            this ref Result<A, Error, Message> source, Func<A, B> selector) =>
            source.Map(selector);

        /// <summary>
        /// Monad bind operation.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="bind">Bind function.</param>
        /// <returns>New result of type B.</returns>
        public static Result<B, Error, Message> Bind<A, Error, Message, B>(
            ref Result<A, Error, Message> source,
            Func<A, Result<B, Error, Message>> bind)
        {
            bind.AssertArgumentNotNull(nameof(bind));

            return source.Match(
                (value, list) =>
                {
                    Result<B, Error, Message> result = bind(value);
                    return new Result<B, Error, Message>(result.Value, list.AddRange(result.Messages));
                },
                (error, list) => new Result<B, Error, Message>(error, list));
        }

        public static Result<C, Error, Message> SelectMany<A, Error, Message, B, C>(
            this Result<A, Error, Message> source,
            Func<A, Result<B, Error, Message>> bind,
            Func<A, B, C> project)
            => source.Match(
                error: Result.Fail<C, Error, Message>,
                success: (a, list) =>
                    bind(source.Value).Match(
                        error: Result.Fail<C, Error, Message>,
                        success: (b, list2) => Result.Success<C, Error, Message>(project(a, b), list2)));

        //TODO Combine?
    }

    public static class ResultExt
    {
        public static Result<A, Exception, Message> ToSuccess<A, Message>(
            this A value, params Message[] messages)
            => Result.Success<A, Exception, Message>(value, MessageList<Message>.Empty.AddRange(messages));

        public static Result<A, Exception, Message> ToSuccess<A, Message>(
            this A value, IMessageList<Message> messages)
            => Result.Success<A, Exception, Message>(value, messages);

        public static Result<A, Error, Message> MatchSuccess<A, Error, Message>(
            this Result<A, Error, Message> result,
            Action<A, IMessageList<Message>> success)
        {
            success.AssertArgumentNotNull(nameof(success));

            if (result.IsSuccess)
                success(result.Value, result.Messages);
            return result;
        }

        public static Result<A, Error, Message> MatchError<A, Error, Message>(
            this Result<A, Error, Message> result,
            Action<Error, IMessageList<Message>> error)
        {
            error.AssertArgumentNotNull(nameof(error));

            if (result.IsFailed)
                error(result.ErrorValue, result.Messages);
            return result;
        }

        public static Result<A, Error, Message> MatchMessages<A, Error, Message>(
            this Result<A, Error, Message> result,
            Action<IMessageList<Message>> onMessages)
        {
            onMessages.AssertArgumentNotNull(nameof(onMessages));

            onMessages(result.Messages);
            return result;
        }
    }

    public static partial class Prelude
    {
        public static Result<A, Exception, Message> SuccessResult<A, Message>(A value, Message message)
        {
            return Result.Success<A, Exception, Message>(value, new MessageList<Message>(message));
        }

        public static Result<A, Exception> SuccessResult<A>(A value)
        {
            return Result.Success<A, Exception>(value);
        }
    }
}
