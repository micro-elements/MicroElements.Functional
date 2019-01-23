using System;

namespace MicroElements.Functional.Result
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
                (value, list) =>
                    new ParseResult<TValue, TError, TMessage>(@this.Value, @this.Messages.Concat(messages)),
                (error, list) =>
                    new ParseResult<TValue, TError, TMessage>(@this.Error, @this.Messages.Concat(messages)));
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
}
