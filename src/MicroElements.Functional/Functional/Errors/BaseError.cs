// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace MicroElements.Functional.Errors
{
    /// <summary>
    /// Base class for common errors.
    /// </summary>
    public abstract class BaseError : ValueObject
    {
        private readonly object[] _args;

        /// <summary>
        /// Gets ErrorCode.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Gets string representing error format.
        /// </summary>
        public string ErrorFormat { get; }

        /// <summary>
        /// Gets args for <see cref="ErrorFormat"/> to construct <see cref="ErrorMessage"/>.
        /// </summary>
        public IReadOnlyList<object> Args => _args;

        /// <summary>
        /// Gets final formatted error message.
        /// </summary>
        public string ErrorMessage => Args != null ? string.Format(ErrorFormat, (object[])Args) : ErrorFormat;

        /// <summary>
        /// Creates new instance of <see cref="BaseError"/>.
        /// </summary>
        /// <param name="errorCode">Optional error code. If not set that type name uses.</param>
        /// <param name="errorFormat">Error format. Not null.</param>
        /// <param name="args">Optional args.</param>
        protected BaseError(string errorCode = null, string errorFormat = null, params object[] args)
        {
            ErrorCode = errorCode ?? GetType().Name;
            ErrorFormat = errorFormat ?? $"{ErrorCode} occured.";
            _args = args;
        }

        /// <inheritdoc />
        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return ErrorCode;
            yield return ErrorMessage;
        }

        /// <inheritdoc />
        public override string ToString() => ErrorMessage;
    }
}
