﻿using System;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents the result of an operation with additional messages.
    /// Result can be Success: {Value, Messages} | Failed: {Messages}.
    /// Result has message list that can contain additional diagnostic information.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TMessage">Message type.</typeparam>
    public class ParseResult<TValue, TMessage>
    {
        /// <summary>
        /// Value.
        /// </summary>
        internal TValue Value { get; }

        /// <summary>
        /// Result state.
        /// </summary>
        internal ResultState State { get; }

        /// <summary>
        /// Message list.
        /// </summary>
        public IMessageList<TMessage> Messages { get; }

        /// <summary>
        /// Gets value whether the result in Success state.
        /// </summary>
        public bool IsSuccess => State == ResultState.Success;

        /// <summary>
        /// Gets value whether the result in Error state.
        /// </summary>
        public bool IsError => State == ResultState.Error;

        internal ParseResult(SuccessData<TValue, TMessage> successData)
        {
            Value = successData.Value;
            Messages = successData.Messages;
            State = ResultState.Success;
        }

        internal ParseResult(IMessageList<TMessage> messages)
        {
            Messages = messages ?? MessageList<TMessage>.Empty;
            State = ResultState.Error;
        }

        public static implicit operator ParseResult<TValue, TMessage>(TMessage message) =>
            ParseResult.Error<TValue, TMessage>(message);

        public static implicit operator ParseResult<TValue, TMessage>(MessageList<TMessage> messages) =>
            ParseResult.Error<TValue, TMessage>(messages);

        public TResult Match<TResult>(Func<TValue, IMessageList<TMessage>, TResult> success, Func<IMessageList<TMessage>, TResult> error)
            => IsSuccess ? success(Value, Messages) : error(Messages);

        public Unit Match(Action<TValue, IMessageList<TMessage>> success, Action<IMessageList<TMessage>> error)
            => Match(success.ToFunc(), error.ToFunc());

        public ParseResult<TValue, TMessage> MatchSuccess(Action<TValue, IMessageList<TMessage>> success)
        {
            if (IsSuccess)
                success(Value, Messages);
            return this;
        }

        public ParseResult<TValue, TMessage> MatchError(Action<IMessageList<TMessage>> error)
        {
            if (IsError)
                error(Messages);
            return this;
        }

        public ParseResult<TValue, TMessage> MatchMessages(Action<IMessageList<TMessage>> onMessages)
        {
            onMessages(Messages);
            return this;
        }
    }

    internal class SuccessData<TValue, TMessage>
    {
        /// <summary>
        /// Value.
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// Message list.
        /// </summary>
        public IMessageList<TMessage> Messages { get; }

        /// <summary>
        /// Creates value with messages.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="messages">Message list.</param>
        public SuccessData(TValue value, IMessageList<TMessage> messages = null)
        {
            if (value.IsNull())
                throw new ArgumentNullException(nameof(value), "Cannot use null for SuccessData");
            Value = value;
            Messages = messages ?? MessageList<TMessage>.Empty;
        }
    }

    public static class ParseResult
    {
        public static ParseResult<TValue, TMessage> Success<TValue, TMessage>(TValue value, IMessageList<TMessage> messages)
            => new ParseResult<TValue, TMessage>(new SuccessData<TValue, TMessage>(value, messages));

        public static ParseResult<TValue, TMessage> Error<TValue, TMessage>(IMessageList<TMessage> messages)
            => new ParseResult<TValue, TMessage>(messages);

        public static ParseResult<TValue, TMessage> Error<TValue, TMessage>(MessageList<TMessage> messages)
            => new ParseResult<TValue, TMessage>(messages);
    }

    public static class ParseResultExt
    {
        public static ParseResult<TValue, string> ToSuccess<TValue>(this TValue value)
            => ParseResult.Success(value, EmptyMessageList);

        public static ParseResult<TValue, TMessage> ToSuccess<TValue, TMessage>(this TValue value, params TMessage[] messages)
            => ParseResult.Success(value, MessageList<TMessage>.Empty.AddRange(messages));

        public static ParseResult<TValue, TMessage> ToSuccess<TValue, TMessage>(this TValue value, IMessageList<TMessage> messages)
            => ParseResult.Success(value, messages);

        public static ParseResult<TValue, TMessage> ToError<TValue, TMessage>(this IMessageList<TMessage> messages)
            => ParseResult.Error<TValue, TMessage>(messages);

        public static ParseResult<TValue, TMessage> ToError<TValue, TMessage>(this TMessage message)
            => ParseResult.Error<TValue, TMessage>(new MessageList<TMessage>(message));

        public static ParseResult<TValue, TMessage> WithMessages<TValue, TMessage>(
            this ParseResult<TValue, TMessage> @this, IMessageList<TMessage> messages)
        {
            return @this.Match(
                (value, list) => ParseResult.Success(@this.Value, @this.Messages.AddRange(messages)),
                (list) => ParseResult.Error<TValue, TMessage>(@this.Messages.AddRange(messages)));
        }

        //Monad<U> Map(Func<T, U> f);
        public static ParseResult<TValue2, TMessage> Map<TValue, TMessage, TValue2>
            (this ParseResult<TValue, TMessage> @this, Func<TValue, TValue2> f)
            => @this.Match(
                (value, list) => new ParseResult<TValue2, TMessage>(new SuccessData<TValue2, TMessage>(f(value), list)),
                (list) => new ParseResult<TValue2, TMessage>(list));

        //Monad<U> Bind(Func<T, Monad<U>> f);
        public static ParseResult<TValue2, TMessage> Bind<TValue, TMessage, TValue2>
            (this ParseResult<TValue, TMessage> @this, Func<TValue, ParseResult<TValue2, TMessage>> f)
            => @this.Match(
                (value, list) => f(value).WithMessages(list),
                (list) => list.ToError<TValue2, TMessage>());
    }

    public static partial class Prelude
    {
        public static ParseResult<TValue, TMessage> SuccessParseResult<TValue, TMessage>(TValue value, TMessage message)
        {
            return ParseResult.Success(value, new MessageList<TMessage>(message));
        }
    }
}
