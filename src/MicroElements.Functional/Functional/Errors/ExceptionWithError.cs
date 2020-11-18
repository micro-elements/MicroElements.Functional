// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace MicroElements.Functional
{
    /// <summary>
    /// Base exception that provides <see cref="IError{TErrorCode}"/>.
    /// </summary>
    /// <typeparam name="TErrorCode">Error code type.</typeparam>
    [Serializable]
    public class ExceptionWithError<TErrorCode> : Exception
        where TErrorCode : notnull
    {
        /// <summary>
        /// Gets Error associated with exception.
        /// </summary>
        [NotNull]
        public Error<TErrorCode> Error { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionWithError{TErrorCode}"/> class.
        /// </summary>
        /// <param name="error">Error associated with exception.</param>
        public ExceptionWithError(IError<TErrorCode> error)
            : base(GetExceptionMessage(error))
        {
            error.AssertArgumentNotNull(nameof(error));

            Error = error is Error<TErrorCode> e ? e : new Error<TErrorCode>(error.ErrorCode, error.Message);
        }

        private static string GetExceptionMessage(IError<TErrorCode>? error) => error?.Message.FormattedMessage ?? $"Unknown error {typeof(TErrorCode)}";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionWithError{TErrorCode}"/> class.
        /// </summary>
        protected ExceptionWithError()
        {
            // Can be used by some serializers
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionWithError{TErrorCode}"/> class.
        /// </summary>
        /// <param name="info">SerializationInfo.</param>
        /// <param name="context">StreamingContext.</param>
        protected ExceptionWithError(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Error = (Error<TErrorCode>)info.GetValue(nameof(Error), typeof(Error<TErrorCode>));
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Error), Error, typeof(Error<TErrorCode>));
        }
    }
}
