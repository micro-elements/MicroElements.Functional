// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    public struct Result<A, Error, Message>
    {
        /// <summary>
        /// Empty result.
        /// </summary>
        public static readonly Result<A, Error, Message> Empty = default(Result<A, Error, Message>);

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
        internal IMessageList<Message> Messages;

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

        //[Pure]
        //public static implicit operator Result<A, Error>(Result<A, Error, Message> result) =>
        //    result.Match;

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
    }
}
