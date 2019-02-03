// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace MicroElements.Functional
{
    /// <summary>
    /// Result helpers.
    /// </summary>
    public static class Result
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

        /// <summary>
        /// Creates success result from value and messages.
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
        /// Creates success result from value.
        /// </summary>
        /// <typeparam name="A">Success value type.</typeparam>
        /// <typeparam name="Error">Error value type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <param name="value">Success result value.</param>
        /// <returns>Success result.</returns>
        public static Result<A, Error, Message> Success<A, Error, Message>(A value)
            => new Result<A, Error, Message>(value, MessageList<Message>.Empty);

        /// <summary>
        /// Creates success result from value.
        /// </summary>
        /// <typeparam name="A">Success value type.</typeparam>
        /// <param name="value">Success result value.</param>
        /// <returns>Specific SuccessResult that can be implicitly converted to concrete result..</returns>
        public static SuccessResult<A> Success<A>(A value)
            => new SuccessResult<A>(value);

        /// <summary>
        /// Creates failed result from error.
        /// </summary>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <param name="value">Success result value.</param>
        /// <returns>Specific FailedResult that can be implicitly converted to concrete result..</returns>
        public static FailedResult<Error> Fail<Error>(Error value)
            => new FailedResult<Error>(value);

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
}
