using System.Diagnostics.CodeAnalysis;

namespace MicroElements.Functional
{
    /// <summary>
    /// Extension methods for errors.
    /// </summary>
    public static class ErrorExtensions
    {
        /// <summary>
        /// Creates <see cref="ExceptionWithError{TErrorCode}"/> instance by provided <see cref="IError{TErrorCode}"/>.
        /// </summary>
        /// <typeparam name="TErrorCode">ErrorCode type.</typeparam>
        /// <param name="error">Source error.</param>
        /// <returns>New <see cref="ExceptionWithError{TErrorCode}"/> instance.</returns>
        public static ExceptionWithError<TErrorCode> ToException<TErrorCode>(this IError<TErrorCode> error)
            where TErrorCode : notnull
        {
            return new ExceptionWithError<TErrorCode>(new Error<TErrorCode>(error.ErrorCode, error.Message));
        }

        /// <summary>
        /// Creates <see cref="ExceptionWithError{TErrorCode}"/> instance by provided <see cref="Error{TErrorCode}"/>.
        /// </summary>
        /// <typeparam name="TErrorCode">ErrorCode type.</typeparam>
        /// <param name="error">Source error.</param>
        /// <returns>New <see cref="ExceptionWithError{TErrorCode}"/> instance.</returns>
        public static ExceptionWithError<TErrorCode> ToException<TErrorCode>(this in Error<TErrorCode> error)
            where TErrorCode : notnull
        {
            return new ExceptionWithError<TErrorCode>(error);
        }

        /// <summary>
        /// Throws <see cref="ExceptionWithError{TErrorCode}"/> created by error.
        /// </summary>
        /// <typeparam name="TErrorCode">ErrorCode type.</typeparam>
        /// <param name="error">Source error.</param>
        /// <returns>No return value.</returns>
        [DoesNotReturn]
        public static Unit Throw<TErrorCode>(this in Error<TErrorCode> error)
            where TErrorCode : notnull
        {
            throw error.ToException();
        }

        /// <summary>
        /// Throws <see cref="ExceptionWithError{TErrorCode}"/> created by error.
        /// </summary>
        /// <typeparam name="TErrorCode">ErrorCode type.</typeparam>
        /// <param name="error">Source error.</param>
        /// <returns>No return value.</returns>
        [DoesNotReturn]
        public static Unit Throw<TErrorCode>(this IError<TErrorCode> error)
            where TErrorCode : notnull
        {
            throw error.ToException();
        }
    }
}
