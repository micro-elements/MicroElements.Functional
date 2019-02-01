namespace MicroElements.Functional
{
    /// <summary>
    /// Represents the result of an operation with additional messages.
    /// Result can be Success: {A, Messages} | Failed: {Error, Messages}.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="Error"></typeparam>
    /// <typeparam name="Message"></typeparam>
    public struct Result<A, Error, Message>
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
    }
}
