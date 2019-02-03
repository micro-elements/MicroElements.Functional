using System;
using System.Diagnostics.Contracts;

namespace MicroElements.Functional
{
    /// <summary>
    /// Monadic operations.
    /// </summary>
    public static class ResultOperations
    {
        /// <summary>
        /// Evaluates a specified function based on the result state.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="success">Function to evaluate on <see cref="ResultState.Success"/> state.</param>
        /// <param name="error">Function to evaluate on <see cref="ResultState.Error"/> state.</param>
        /// <returns>NotNull evaluated result.</returns>
        /// <exception cref="ArgumentNullException">success is null.</exception>
        /// <exception cref="ArgumentNullException">error is null.</exception>
        /// <exception cref="ResultIsNullException">result is null.</exception>
        [Pure]
        public static B Match<A, Error, Message, B>(
            this in Result<A, Error, Message> source,
            SuccessFunc<A, Message, B> success,
            ErrorFunc<Error, Message, B> error)
        {
            success.AssertArgumentNotNull(nameof(success));
            error.AssertArgumentNotNull(nameof(error));

            var result = source.IsSuccess
                ? success(source.Value, source.Messages)
                : error(source.ErrorValue, source.Messages);
            return result.AssertNotNullResult();
        }

        /// <summary>
        /// Evaluates a specified function based on the result state.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="success">Function to evaluate on <see cref="ResultState.Success"/> state.</param>
        /// <param name="error">Function to evaluate on <see cref="ResultState.Error"/> state.</param>
        /// <returns>NotNull evaluated result.</returns>
        /// <exception cref="ArgumentNullException">success is null.</exception>
        /// <exception cref="ArgumentNullException">error is null.</exception>
        /// <exception cref="ResultIsNullException">result is null.</exception>
        [Pure]
        public static B Match<A, Error, B>(
            this in Result<A, Error> source,
            SuccessFunc<A, B> success,
            ErrorFunc<Error, B> error)
        {
            success.AssertArgumentNotNull(nameof(success));
            error.AssertArgumentNotNull(nameof(error));

            var result = source.IsSuccess
                ? success(source.Value)
                : error(source.ErrorValue);
            return result.AssertNotNullResult();
        }

        /// <summary>
        /// New Result of type <typeparamref name="B"/> as a result of <paramref name="map"/> function.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="map">Map function.</param>
        /// <returns>New result of type <typeparamref name="B"/>.</returns>
        /// <exception cref="ArgumentNullException">success is null.</exception>
        public static Result<B, Error, Message> Map<A, Error, Message, B>(
            this in Result<A, Error, Message> source,
            Func<A, B> map)
        {
            map.AssertArgumentNotNull(nameof(map));

            return source.Match(
                (value, list) => new Result<B, Error, Message>(map(value), list),
                (error, list) => new Result<B, Error, Message>(error, list));
        }

        /// <summary>
        /// New Result of type <typeparamref name="B"/> as a result of <paramref name="map"/> function.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="selector">Map function.</param>
        /// <returns>New result of type <typeparamref name="B"/>.</returns>
        /// <exception cref="ArgumentNullException">selector is null.</exception>
        public static Result<B, Error, Message> Select<A, Error, Message, B>(
            this in Result<A, Error, Message> source,
            Func<A, B> selector)
        {
            selector.AssertArgumentNotNull(nameof(selector));

            return source.Map(selector);
        }

        /// <summary>
        /// Monad bind operation.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="bind">Bind function.</param>
        /// <returns>New result of type B.</returns>
        /// <exception cref="ArgumentNullException">bind is null.</exception>
        public static Result<B, Error, Message> Bind<A, Error, Message, B>(
            in Result<A, Error, Message> source,
            Func<A, Result<B, Error, Message>> bind)
        {
            bind.AssertArgumentNotNull(nameof(bind));

            return source.Match(
                (value, list) =>
                {
                    var resultB = bind(value);
                    return resultB.Match(
                        (b, messages) => new Result<B, Error, Message>(b, list.AddRange(resultB.Messages)),
                        (error, messages) => new Result<B, Error, Message>(error, list.AddRange(resultB.Messages)));
                },
                (error, list) => new Result<B, Error, Message>(error, list));
        }

        /// <summary>
        /// Monad bind operation.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="bind">Bind function.</param>
        /// <returns>New result of type B.</returns>
        /// <exception cref="ArgumentNullException">bind is null.</exception>
        public static Result<B, Error> Bind<A, Error, B>(
            in Result<A, Error> source,
            Func<A, Result<B, Error>> bind)
        {
            bind.AssertArgumentNotNull(nameof(bind));

            return source.Match(
                (value) => bind(value),
                (error) => new Result<B, Error>(error));
        }

        public static Result<C, Error, Message> SelectMany<A, Error, Message, B, C>(
            this in Result<A, Error, Message> source,
            Func<A, Result<B, Error, Message>> bind,
            Func<A, B, C> project)
        {
            bind.AssertArgumentNotNull(nameof(bind));
            project.AssertArgumentNotNull(nameof(project));

            return source.Match(
                error: Result.Fail<C, Error, Message>,
                success: (a, list) =>
                {
                    Result<B, Error, Message> resultB = bind(a).AddMessagesToStart(list);
                    return resultB.Match(
                        error: Result.Fail<C, Error, Message>,
                        success: (b, list2) => Result.Success<C, Error, Message>(project(a, b), list2));
                });
        }

        public static Result<C, Error> SelectMany<A, Error, B, C>(
            this in Result<A, Error> source,
            Func<A, Result<B, Error>> bind,
            Func<A, B, C> project)
        {
            bind.AssertArgumentNotNull(nameof(bind));
            project.AssertArgumentNotNull(nameof(project));

            return source.Match(
                error: Result.Fail<C, Error>,
                success: (a) =>
                    bind(a).Match(
                        error: Result.Fail<C, Error>,
                        success: (b) => Result.Success<C, Error>(project(a, b))));
        }

        //TODO Combine?
    }
}
