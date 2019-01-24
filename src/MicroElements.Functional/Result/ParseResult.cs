using System;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents the result of an operation with additional messages.
    /// Result can be Success: Value | Failed: Error.
    /// Result has message list that can contain additional diagnostic information.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">Error type.</typeparam>
    /// <typeparam name="TMessage">Message type.</typeparam>
    public class ParseResult<TValue, TError, TMessage>
    {
        /// <summary>
        /// Value.
        /// </summary>
        internal TValue Value { get; }

        /// <summary>
        /// Error value.
        /// </summary>
        internal TError Error { get; }

        /// <summary>
        /// Message list.
        /// </summary>
        internal IMessageList<TMessage> Messages { get; }

        /// <summary>
        /// Gets value whether result in Success state.
        /// </summary>
        public bool IsSuccess { get; }

        public ParseResult(TValue value, IMessageList<TMessage> messages)
        {
            Value = value;
            Messages = messages;
            IsSuccess = true;
        }

        public ParseResult(TError error, IMessageList<TMessage> messages)
        {
            Error = error;
            Messages = messages;
            IsSuccess = false;
        }

        public TResult Match<TResult>(Func<TValue, IMessageList<TMessage>, TResult> success, Func<TError, IMessageList<TMessage>, TResult> failed)
            => IsSuccess ? success(Value, Messages) : failed(Error, Messages);

        public Unit Match<TResult>(Action<TValue, IMessageList<TMessage>> success, Action<TError, IMessageList<TMessage>> failed)
            => Match(success.ToFunc(), failed.ToFunc());
    }

    public static class ParseResultExt
    {
        public static ParseResult<TValue, TError, TMessage> WithMessages<TValue, TError, TMessage>(
            this ParseResult<TValue, TError, TMessage> @this, IMessageList<TMessage> messages)
        {
            return @this.Match<ParseResult<TValue, TError, TMessage>>(
                (value, list) => new ParseResult<TValue, TError, TMessage>(@this.Value, @this.Messages.AddRange(messages)),
                (error, list) => new ParseResult<TValue, TError, TMessage>(@this.Error, @this.Messages.AddRange(messages)));
        }

        //Monad<U> Map(Func<T, U> f);
        public static ParseResult<TValue2, TError, TMessage> Map<TValue, TError, TMessage, TValue2>
            (this ParseResult<TValue, TError, TMessage> @this, Func<TValue, TValue2> f)
            => @this.Match<ParseResult<TValue2, TError, TMessage>>(
                (value, list) => new ParseResult<TValue2, TError, TMessage>(f(value), list),
                (error, list) => new ParseResult<TValue2, TError, TMessage>(error, list));

        //Monad<U> Bind(Func<T, Monad<U>> f);
        public static ParseResult<TValue2, TError, TMessage> Bind<TValue, TError, TMessage, TValue2>
            (this ParseResult<TValue, TError, TMessage> @this, Func<TValue, ParseResult<TValue2, TError, TMessage>> f)
            => @this.Match<ParseResult<TValue2, TError, TMessage>>(
                (value, list) => f(value).WithMessages(list),
                (error, list) => new ParseResult<TValue2, TError, TMessage>(error, list));
    }

    //public static partial class Prelude
    //{
    //    public static ParseResult<TValue, TMessage> Success<TValue, TMessage>
    //        (TValue value, IMessageList<TMessage> messages)
    //        => new ParseResult<TValue, TMessage>();
    //}

    // Value | Error | Message
    public class ResultWithMessage
    {
    }

    // Value | Error | Messages
    public class ResultWithMessages
    {
    }

    // Value | Exception
    public class ResultWithException
    {
    }

    // Value | Error
    public class ResultWithError
    {
    }

    public class ValueWithMessages<TValue, TMessage>
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
        public ValueWithMessages(Some<TValue> value, IMessageList<TMessage> messages = null)
        {
            Value = value.Value;
            Messages = messages ?? EmptyMessageList<TMessage>();
        }

        public void Deconstruct(out TValue value, out IMessageList<TMessage> messages)
        {
            value = Value;
            messages = Messages;
        }
    }

    public class ParseResult2<TValue, TMessage>
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

        internal ParseResult2(ValueWithMessages<TValue, TMessage> valueWithMessages)
        {
            Value = valueWithMessages.Value;
            Messages = valueWithMessages.Messages;
            State = ResultState.Success;
        }

        internal ParseResult2(IMessageList<TMessage> messages)
        {
            Messages = messages ?? EmptyMessageList<TMessage>();
            State = ResultState.Error;
        }

        public TResult Match<TResult>(Func<TValue, IMessageList<TMessage>, TResult> success, Func<IMessageList<TMessage>, TResult> error)
            => IsSuccess? success(Value, Messages) : error(Messages);

        public Unit Match<TResult>(Action<TValue, IMessageList<TMessage>> success, Action<IMessageList<TMessage>> error)
            => Match(success.ToFunc(), error.ToFunc());
    }

    public static class ParseResult2
    {
        public static ParseResult2<TValue, TMessage> Success<TValue, TMessage>(TValue value, IMessageList<TMessage> messages)
            => new ParseResult2<TValue, TMessage>(new ValueWithMessages<TValue, TMessage>(value.ToSome(), messages));

        public static ParseResult2<TValue, TMessage> ToSuccess<TValue, TMessage>
            (this TValue value, IMessageList<TMessage> messages)
            => Success(value, messages);

        public static ParseResult2<TValue, TMessage> Error<TValue, TMessage>(IMessageList<TMessage> messages)
            => new ParseResult2<TValue, TMessage>(messages);


        public static ParseResult2<TValue, TMessage> ToError<TValue, TMessage>(this IMessageList<TMessage> messages)
            => Error<TValue, TMessage>(messages);
    }

    public static class ParseResultExt2
    {
        public static ParseResult2<TValue, TMessage> WithMessages<TValue, TMessage>(
            this ParseResult2<TValue, TMessage> @this, IMessageList<TMessage> messages)
        {
            return @this.Match(
                (value, list) => ParseResult2.Success(@this.Value, @this.Messages.AddRange(messages)),
                (list) => ParseResult2.Error<TValue, TMessage>(@this.Messages.AddRange(messages)));
        }

        //Monad<U> Map(Func<T, U> f);
        public static ParseResult2<TValue2, TMessage> Map<TValue, TMessage, TValue2>
            (this ParseResult2<TValue, TMessage> @this, Func<TValue, TValue2> f)
            => @this.Match(
                (value, list) => new ParseResult2<TValue2, TMessage>(new ValueWithMessages<TValue2, TMessage>(f(value).ToSome(), list)),
                (list) => new ParseResult2<TValue2, TMessage>(list));

        //Monad<U> Bind(Func<T, Monad<U>> f);
        public static ParseResult2<TValue2, TMessage> Bind<TValue, TMessage, TValue2>
            (this ParseResult2<TValue, TMessage> @this, Func<TValue, ParseResult2<TValue2, TMessage>> f)
            => @this.Match(
                (value, list) => f(value).WithMessages(list),
                (list) => list.ToError<TValue2, TMessage>());
    }
}
