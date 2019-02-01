namespace MicroElements.Functional
{
    /// <summary>
    /// Represents the result of an operation: {A | Error}.
    /// </summary>
    /// <typeparam name="A">Success result type.</typeparam>
    /// <typeparam name="Error">Error type.</typeparam>
    public readonly struct Result<A, Error>
    {
        /// <summary>
        /// Empty result.
        /// </summary>
        public static readonly Result<A, Error> Empty = default(Result<A, Error>);

        /// <summary>
        /// Result state.
        /// </summary>
        internal readonly ResultState State;

        /// <summary>
        /// Success value.
        /// </summary>
        internal readonly A Value;

        /// <summary>
        /// Error value.
        /// </summary>
        internal readonly Error ErrorValue;

        /// <summary>
        /// Creates success result.
        /// </summary>
        /// <param name="value">Value.</param>
        internal Result(A value)
        {
            State = ResultState.Success;
            Value = value;
            ErrorValue = default(Error);
        }

        /// <summary>
        /// Creates faulted result.
        /// </summary>
        /// <param name="error">Error.</param>
        internal Result(Error error)
        {
            State = ResultState.Error;
            Value = default(A);
            ErrorValue = error;
        }
    }

    /// <summary>
    /// Result helpers.
    /// </summary>
    public static partial class Result
    {
        /// <summary>
        /// Creates success result from value.
        /// </summary>
        /// <typeparam name="A">Success value type.</typeparam>
        /// <typeparam name="Error">Error value type.</typeparam>
        /// <param name="value">Success result value.</param>
        /// <returns>Success result.</returns>
        public static Result<A, Error> Success<A, Error>(A value)
            => new Result<A, Error>(value);

        /// <summary>
        /// Creates failed result.
        /// </summary>
        /// <typeparam name="A">Success value type.</typeparam>
        /// <typeparam name="Error">Error value type.</typeparam>
        /// <param name="error">Error value.</param>
        /// <returns>Failed result.</returns>
        public static Result<A, Error> Fail<A, Error>(Error error)
            => new Result<A, Error>(error);
    }
}
