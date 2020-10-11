// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MicroElements.Functional
{
    /// <summary>
    /// Result extension methods.
    /// </summary>
    public static class ResultExtensions
    {
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
        public static Result<A, Error, Message> WithMessages<A, Error, Message>(
            this in Result<A, Error, Message> source,
            IEnumerable<Message> messages)
        {
            return source.Match(
                (value, list) => new Result<A, Error, Message>(value, list.AddRange(messages)),
                (error, list) => new Result<A, Error, Message>(error, list.AddRange(messages)));
        }

        [Pure]
        public static Result<A, Error, Message> WithMessages<A, Error, Message>(
            this in Result<A, Error> source,
            IEnumerable<Message> messages)
        {
            return source.Match(
                (value) => new Result<A, Error, Message>(value, new MessageList<Message>(messages)),
                (error) => new Result<A, Error, Message>(error, new MessageList<Message>(messages)));
        }

        /// <summary>
        /// Creates new Result with new messages added to in the begin of message list.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="messages">Messages to add.</param>
        /// <returns>New Result with added messages.</returns>
        [Pure]
        public static Result<A, Error, Message> WithMessagesAtStart<A, Error, Message>(
            this in Result<A, Error, Message> source,
            IMessageList<Message> messages)
        {
            return source.Match(
                (value, list) => Result.Success(value).WithMessages(messages.AddRange(list)), // new Result<A, Error, Message>(value, messages.AddRange(list)),
                (error, list) => new Result<A, Error, Message>(error, messages.AddRange(list)));
        }

        [Pure]
        public static A GetValueOrThrow<A, Error, Message>(this in Result<A, Error, Message> source) =>
            source.Match((a, list) => a, (error, list) => throw new InvalidCastException($"Result in Failed state can not be cast to {typeof(A)}"));

        [Pure]
        public static A GetValueOrThrow<A, Error>(this in Result<A, Error> source) =>
            source.Match((a) => a, (error) => throw new InvalidCastException($"Result in Failed state can not be cast to {typeof(A)}"));

        [Pure]
        public static A GetValueOrDefault<A, Error, Message>(
            this in Result<A, Error, Message> source,
            Func<Error, IMessageList<Message>, A> factory) =>
            source.MatchUnsafe((a, list) => a, (error, list) => factory(error, list));

        [Pure]
        public static A GetValueOrDefault<A, Error>(
            this in Result<A, Error> source,
            Func<Error, A> factory) =>
            source.MatchUnsafe((a) => a, (error) => factory(error));

        public static Result<A, Exception, Message> ToSuccess<A, Message>(
            this A value, params Message[] messages)
            => Result.Success<A, Exception, Message>(value, MessageList<Message>.Empty.AddRange(messages));

        public static Result<A, Exception, Message> ToSuccess<A, Message>(
            this A value, IMessageList<Message> messages)
            => Result.Success<A, Exception, Message>(value, messages);

        public static Result<A, Error, Message> ToFail<A, Error, Message>(
            this IMessageList<Message> messages)
            => Result.Fail<A, Error, Message>(default, messages);

        public static Result<A, Error> MatchSuccess<A, Error>(
            this in Result<A, Error> result,
            SuccessAction<A> success)
        {
            success.AssertArgumentNotNull(nameof(success));

            if (result.IsSuccess)
                success(result.Value);
            return result;
        }

        public static Result<A, Error> MatchError<A, Error>(
            this in Result<A, Error> result,
            ErrorAction<Error> error)
        {
            error.AssertArgumentNotNull(nameof(error));

            if (result.IsFailed)
                error(result.ErrorValue);
            return result;
        }

        public static Result<A, Error, Message> MatchSuccess<A, Error, Message>(
            this in Result<A, Error, Message> result,
            SuccessAction<A, Message> success)
        {
            success.AssertArgumentNotNull(nameof(success));

            if (result.IsSuccess)
                success(result.Value, result.Messages);
            return result;
        }

        public static Result<A, Error, Message> MatchError<A, Error, Message>(
            this in Result<A, Error, Message> result,
            ErrorAction<Error, Message> error)
        {
            error.AssertArgumentNotNull(nameof(error));

            if (result.IsFailed)
                error(result.ErrorValue, result.Messages);
            return result;
        }

        public static Result<A, Error, Message> MatchMessages<A, Error, Message>(
            this in Result<A, Error, Message> result,
            Action<IMessageList<Message>> onMessages)
        {
            onMessages.AssertArgumentNotNull(nameof(onMessages));

            onMessages(result.Messages);
            return result;
        }

        public static Result<A, Error> Validate<A, Error>(
            this in Result<A, Error> result,
            Func<A, Error> validate)
        {
            validate.AssertArgumentNotNull(nameof(validate));

            if (result.IsSuccess)
            {
                var error = validate(result.Value);
                if (error.IsNotNull())
                    return error;
            }

            return result;
        }

        public static Result<A, Error, Message> Validate<A, Error, Message>(
            this in Result<A, Error, Message> source,
            Func<A, IMessageList<Message>, Result<A, Error, Message>> validate)
        {
            validate.AssertArgumentNotNull(nameof(validate));

            if (source.IsSuccess)
            {
                var result = validate(source.Value, source.Messages);
                if (result.IsFailed)
                {
                    return source.WithMessages(result.Messages);
                }
            }

            return source;
        }

        public static Result<A, Error, Message> Validate<A, Error, Message>(
            this Result<A, Error> source,
            Func<A, IEnumerable<Message>> validate,
            Func<Message, bool> isError)
        {
            validate.AssertArgumentNotNull(nameof(validate));
            isError.AssertArgumentNotNull(nameof(isError));

            return source.Match(
                value =>
                {
                    var messages = validate(value).ToList();
                    var hasErrors = messages.Any(isError);
                    if (hasErrors)
                        return Result.Fail<A, Error, Message>(default, messages);
                    return source.WithMessages(messages);
                },
                error => Result.Fail<A, Error, Message>(error, Array.Empty<Message>()));
        }

        public static Result<A, Error, Message> Validate<A, Error, Message>(
            this Result<A, Error> source,
            Func<A, IEnumerable<Message>> validate)
            where Message : ICanBeError
        {
            return source.Validate(validate, message => message.IsError);
        }
    }
}
