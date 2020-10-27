// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;

namespace MicroElements.Functional
{
    /// <summary>
    /// Base interface for Results.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Gets value whether the result in a Success state.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Gets value whether the result in a Failed state.
        /// </summary>
        bool IsFailed { get; }

        /// <summary>
        /// Returns value type.
        /// </summary>
        /// <returns>Value type.</returns>
        Type GetSuccessValueType();

        /// <summary>
        /// Returns error type.
        /// </summary>
        /// <returns>Error type.</returns>
        Type GetErrorValueType();

        /// <summary>
        /// Match untyped for base matching.
        /// </summary>
        /// <typeparam name="Res">Result type.</typeparam>
        /// <param name="success">Success function: (Value,Messages)->TResult.</param>
        /// <param name="error">Error function: (Messages)->TResult.</param>
        /// <returns>TResult.</returns>
        Res MatchUntyped<Res>(Func<object, Res> success, Func<object, Res> error);
    }

    /// <summary>
    /// Result with additional messages.
    /// </summary>
    public interface IResultWithMessages : IResult
    {
        /// <summary>
        /// Returns message type.
        /// </summary>
        /// <returns>Message type.</returns>
        Type GetMessageType();

        /// <summary>
        /// Match untyped for base matching.
        /// </summary>
        /// <typeparam name="Res">Result type.</typeparam>
        /// <param name="success">Success function: (Value,Messages)->TResult.</param>
        /// <param name="error">Error function: (Messages)->TResult.</param>
        /// <returns>TResult.</returns>
        Res MatchUntyped<Res>(Func<object, IEnumerable, Res> success, Func<object, IEnumerable, Res> error);
    }
}
