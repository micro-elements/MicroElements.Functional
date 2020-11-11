// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace MicroElements.Functional
{
    /// <summary>
    /// Provides error code and message.
    /// </summary>
    public interface IError
    {
        /// <summary>
        /// Gets error message.
        /// </summary>
        [NotNull]
        public Message Message { get; }
    }

    /// <summary>
    /// Provides strong typed error code and message.
    /// </summary>
    /// <typeparam name="TErrorCode">ErrorCode type.</typeparam>
    public interface IError<out TErrorCode> : IError
        where TErrorCode : notnull
    {
        /// <summary>
        /// Gets known error code.
        /// </summary>
        [NotNull]
        public TErrorCode ErrorCode { get; }
    }
}
