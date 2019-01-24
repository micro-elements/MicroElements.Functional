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
    /// Some T not initialized.
    /// </summary>
    [Serializable]
    public class SomeNotInitializedException : Exception
    {
        public SomeNotInitializedException(Type type)
            : base($"Unitialized Some<{type.Name}>.")
        {
        }
    }

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

    /// <summary>
    /// Value is bottom.
    /// </summary>
    [Serializable]
    public class BottomException : Exception
    {
        public static readonly BottomException Default = new BottomException();

        public BottomException(string type = "Value")
            : base($"{type} is in a bottom state and therefore not valid.  This can happen when the value was filtered and the predicate " +
                 "returned false and there was no valid state the value could be in.  If you are going to use the type in a filter " +
                 "you should check if the IsBottom flag is set before use.  This can also happen if the struct wasn't initialised properly and then used.")
        {
        }

        public BottomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
