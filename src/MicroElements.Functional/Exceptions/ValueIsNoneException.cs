using System;

namespace MicroElements.Functional
{
    /// <summary>
    /// Value is none.
    /// </summary>
    [Serializable]
    public class ValueIsNoneException : Exception
    {
        public static readonly ValueIsNoneException Default = new ValueIsNoneException();

        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNoneException()
            : base("Value is none.")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNoneException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNoneException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}