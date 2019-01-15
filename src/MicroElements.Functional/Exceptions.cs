// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1649 // File name should match first type name

using System;

namespace MicroElements.Functional
{
    /// <summary>
    /// Result is null.
    /// </summary>
    [Serializable]
    public class ResultIsNullException : Exception
    {
        public static readonly ResultIsNullException Default = new ResultIsNullException();

        public ResultIsNullException()
            : base("Result is null.")
        {
        }

        public ResultIsNullException(string message)
            : base(message)
        {
        }

        public ResultIsNullException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Value is none.
    /// </summary>
    [Serializable]
    public class ValueIsNoneException : Exception
    {
        public static readonly ValueIsNoneException Default = new ValueIsNoneException();

        public ValueIsNoneException()
            : base("Value is none.")
        {
        }

        public ValueIsNoneException(string message)
            : base(message)
        {
        }

        public ValueIsNoneException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
