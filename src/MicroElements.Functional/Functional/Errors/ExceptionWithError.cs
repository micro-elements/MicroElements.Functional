// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
        public Error<TErrorCode> Error { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionWithError{TErrorCode}"/> class.
        /// </summary>
        /// <param name="error">Error associated with exception.</param>
        public ExceptionWithError(Error<TErrorCode> error)
            : base(error.Message.FormattedMessage)
        {
            Error = error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionWithError{TErrorCode}"/> class.
        /// </summary>
        protected ExceptionWithError()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionWithError{TErrorCode}"/> class.
        /// </summary>
        /// <param name="info">SerializationInfo.</param>
        /// <param name="context">StreamingContext.</param>
        protected ExceptionWithError(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
