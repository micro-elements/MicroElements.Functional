using System;
using System.Collections.Generic;

namespace MicroElements.Functional.Result
{
    public interface IMessage
    {
        /// <summary>
        /// Date and time of message created.
        /// </summary>
        DateTimeOffset CreatedTime { get; }

        /// <summary>
        /// Message severity.
        /// </summary>
        Severity Severity { get; }

        /// <summary>
        /// Event name.
        /// </summary>
        string EventName { get; }

        /// <summary>
        /// Message text.
        /// </summary>
        string Text { get; }

        bool IsError { get; }
    }

    /// <summary>
    /// Message severity.
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// Information message.
        /// </summary>
        Information,

        /// <summary>
        /// Warning.
        /// </summary>
        Warning,

        /// <summary>
        /// Error message.
        /// </summary>
        Error,
    }

    /// <summary>
    /// Message list.
    /// </summary>
    public interface IMessageList<TMessage> : IEnumerable<TMessage>
    {
        /// <summary>
        /// Adds message to list.
        /// </summary>
        /// <param name="message">Message.</param>
        IMessageList<TMessage> Add(TMessage message);
    }

    /// <summary>
    /// Represents the result of an operation with additional messages.
    /// 
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class ParseResult<TValue, TError, TMessage>
    {
        internal TValue Value { get; }
        internal TError Error { get; }
        internal IMessageList<TMessage> Messages { get; }

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
    }
}
