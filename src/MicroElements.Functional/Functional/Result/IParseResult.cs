// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;

namespace MicroElements.Functional
{
    /// <summary>
    /// Base interface for ParseResult.
    /// </summary>
    public interface IParseResult
    {
        /// <summary>
        /// Gets value whether the result in Success state.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Returns value underlying type.
        /// </summary>
        /// <returns>Value underlying type.</returns>
        Type GetUnderlyingType();

        /// <summary>
        /// Returns message underlying type.
        /// </summary>
        /// <returns>Message underlying type.</returns>
        Type GetMessageUnderlyingType();

        /// <summary>
        /// Match untyped for base matching.
        /// </summary>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <param name="success">Success function: (Value,Messages)->TResult.</param>
        /// <param name="error">Error function: (Messages)->TResult.</param>
        /// <returns>TResult.</returns>
        TResult MatchUntyped<TResult>(Func<object, IEnumerable, TResult> success, Func<IEnumerable, TResult> error);
    }
}
