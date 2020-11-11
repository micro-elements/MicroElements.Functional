// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MicroElements.Functional
{
    /// <summary>
    /// Provides strong typed error code and message.
    /// </summary>
    /// <typeparam name="TErrorCode">ErrorCode type.</typeparam>
    public readonly struct Error<TErrorCode> : IError<TErrorCode>, IEquatable<Error<TErrorCode>>
        where TErrorCode : notnull
    {
        /// <summary>
        /// Gets known error code.
        /// </summary>
        [NotNull]
        public TErrorCode ErrorCode { get; }

        /// <summary>
        /// Gets error message.
        /// </summary>
        public Message Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Error{TErrorCode}"/> class.
        /// </summary>
        /// <param name="errorCode">Known error code.</param>
        /// <param name="message">Error message.</param>
        public Error([NotNull] TErrorCode errorCode, [DisallowNull] Message message)
        {
            errorCode.AssertArgumentNotNull(nameof(errorCode));
            message.AssertArgumentNotNull(nameof(message));

            ErrorCode = errorCode;
            Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Error{TErrorCode}"/> class.
        /// </summary>
        /// <param name="errorCode">Known error code.</param>
        /// <param name="errorMessage">Error message.</param>
        public Error([NotNull] TErrorCode errorCode, [DisallowNull] string errorMessage)
        {
            errorCode.AssertArgumentNotNull(nameof(errorCode));
            errorMessage.AssertArgumentNotNull(nameof(errorMessage));

            ErrorCode = errorCode;
            Message = new Message(
                originalMessage: errorMessage,
                severity: MessageSeverity.Error,
                eventName: errorCode.ToString());
        }

        /// <inheritdoc />
        public override string ToString() => $"{nameof(ErrorCode)}: {ErrorCode}, {nameof(Message)}: {Message.FormattedMessage}";

        /// <inheritdoc />
        public bool Equals(Error<TErrorCode> other)
        {
            return EqualityComparer<TErrorCode>.Default.Equals(ErrorCode, other.ErrorCode)
                   && Message.FormattedMessage.Equals(other.Message.FormattedMessage);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Error<TErrorCode> other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(ErrorCode, Message.FormattedMessage);

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="left">Left part.</param>
        /// <param name="right">Right part.</param>
        /// <returns>True for equality.</returns>
        public static bool operator ==(Error<TErrorCode> left, Error<TErrorCode> right) => left.Equals(right);

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="left">Left part.</param>
        /// <param name="right">Right part.</param>
        /// <returns>True for inequality.</returns>
        public static bool operator !=(Error<TErrorCode> left, Error<TErrorCode> right) => !left.Equals(right);
    }

    /// <summary>
    /// Static error helpers.
    /// </summary>
    public static class Error
    {
        private static class ErrorCache<TErrorCode>
            where TErrorCode : struct
        {
            internal static readonly Error<TErrorCode> Empty = new Error<TErrorCode>(default, new Message($"Error: {default}"));
        }

        /// <summary>
        /// Provides empty error.
        /// </summary>
        /// <typeparam name="TErrorCode">Error code.</typeparam>
        /// <returns>Singleton instance of empty error.</returns>
        public static Error<TErrorCode> Empty<TErrorCode>()
            where TErrorCode : struct
        {
            return ErrorCache<TErrorCode>.Empty;
        }

        /// <summary>
        /// Creates error.
        /// </summary>
        /// <typeparam name="TErrorCode">Error code type.</typeparam>
        /// <param name="errorCode">Error code.</param>
        /// <param name="messageTemplate">Message template. Can be in form of MessageTemplates.org.</param>
        /// <param name="args">Args for <paramref name="messageTemplate"/>.</param>
        /// <returns><see cref="Error{TErrorCode}"/> instance.</returns>
        public static Error<TErrorCode> CreateError<TErrorCode>(
            [NotNull] TErrorCode errorCode,
            string messageTemplate,
            params object[]? args)
            where TErrorCode : notnull
        {
            errorCode.AssertArgumentNotNull(nameof(errorCode));
            messageTemplate.AssertArgumentNotNull(nameof(messageTemplate));

            Message message = new Message(
                eventName: errorCode.ToString(),
                severity: MessageSeverity.Error,
                originalMessage: messageTemplate).WithArgs(args ?? Array.Empty<object>());

            return new Error<TErrorCode>(errorCode, message);
        }

        /// <summary>
        /// Creates error from exception.
        /// </summary>
        /// <typeparam name="TErrorCode">Error code type.</typeparam>
        /// <param name="e">Exception.</param>
        /// <param name="undefinedCode">ErrorCode that returns if error occurs but undefined.</param>
        /// <returns><see cref="Error{TErrorCode}"/> instance.</returns>
        public static Error<TErrorCode> CreateError<TErrorCode>(Exception e, TErrorCode undefinedCode)
            where TErrorCode : notnull
        {
            if (e is ExceptionWithError<TErrorCode> knownException)
            {
                return knownException.Error;
            }

            return CreateError(undefinedCode, e.Message);
        }

        /// <summary>
        /// Tries to execute action and returns optional error.
        /// </summary>
        /// <typeparam name="TErrorCode">ErrorCode type.</typeparam>
        /// <param name="action">Action to execute.</param>
        /// <param name="undefinedCode">ErrorCode that returns if error occurs but undefined.</param>
        /// <returns>Optional error.</returns>
        public static Error<TErrorCode>? Try<TErrorCode>(Action action, TErrorCode undefinedCode)
            where TErrorCode : notnull
        {
            try
            {
                action();
                return default;
            }
            catch (Exception e)
            {
                return CreateError(e, undefinedCode);
            }
        }

        /// <summary>
        /// Tries to execute action and returns optional error.
        /// </summary>
        /// <typeparam name="TErrorCode">ErrorCode type.</typeparam>
        /// <param name="action">Action to execute.</param>
        /// <param name="undefinedCode">ErrorCode that returns if error occurs but undefined.</param>
        /// <returns>Optional error.</returns>
        public static Error<TErrorCode>? Try<TErrorCode>(Func<Error<TErrorCode>?> action, TErrorCode undefinedCode)
            where TErrorCode : notnull
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                return CreateError(e, undefinedCode);
            }
        }

        /// <summary>
        /// Tries to execute action and returns optional error.
        /// </summary>
        /// <typeparam name="TErrorCode">ErrorCode type.</typeparam>
        /// <param name="action">Action to execute.</param>
        /// <returns>Optional error.</returns>
        public static Error<TErrorCode>? Try<TErrorCode>(Action action)
            where TErrorCode : struct
        {
            return Try<TErrorCode>(action, default);
        }

        /// <summary>
        /// Tries to execute action and returns optional error.
        /// </summary>
        /// <typeparam name="TErrorCode">ErrorCode type.</typeparam>
        /// <param name="action">Action to execute.</param>
        /// <returns>Optional error.</returns>
        public static Error<TErrorCode>? Try<TErrorCode>(Func<Error<TErrorCode>?> action)
            where TErrorCode : struct
        {
            return Try(action, default);
        }
    }
}
