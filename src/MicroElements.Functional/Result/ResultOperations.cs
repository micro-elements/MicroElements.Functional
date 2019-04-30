using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace MicroElements.Functional
{
    /// <summary>
    /// Monadic operations.
    /// </summary>
    public static partial class ResultOperations
    {
        /// <summary>
        /// Evaluates a specified function based on the result state.
        /// Can not return null result. If null result intended use <see cref="MatchUnsafe{A,Error,Message,B}"/>.
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
        /// Can return null result.
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
        [Pure]
        public static B MatchUnsafe<A, Error, Message, B>(
            this in Result<A, Error, Message> source,
            SuccessFunc<A, Message, B> success,
            ErrorFunc<Error, Message, B> error)
        {
            success.AssertArgumentNotNull(nameof(success));
            error.AssertArgumentNotNull(nameof(error));

            var result = source.IsSuccess
                ? success(source.Value, source.Messages)
                : error(source.ErrorValue, source.Messages);
            return result;
        }

        /// <summary>
        /// Evaluates a specified async function based on the result state.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="success">Function to evaluate on <see cref="ResultState.Success"/> state.</param>
        /// <param name="error">Function to evaluate on <see cref="ResultState.Error"/> state.</param>
        /// <returns>ECompleted task with not null evaluated result.</returns>
        /// <exception cref="ArgumentNullException">success is null.</exception>
        /// <exception cref="ArgumentNullException">error is null.</exception>
        /// <exception cref="ResultIsNullException">result is null.</exception>
        [Pure]
        public static async Task<B> MatchAsync<A, Error, Message, B>(
            this Result<A, Error, Message> source,
            SuccessFunc<A, Message, Task<B>> success,
            ErrorFunc<Error, Message, Task<B>> error)
        {
            success.AssertArgumentNotNull(nameof(success));
            error.AssertArgumentNotNull(nameof(error));

            var resultBTask = source.IsSuccess
                ? success(source.Value, source.Messages)
                : error(source.ErrorValue, source.Messages);
            var resultB = await resultBTask;
            return resultB.AssertNotNullResult();
        }

        /// <summary>
        /// Evaluates a specified async function based on the result state.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="success">Function to evaluate on <see cref="ResultState.Success"/> state.</param>
        /// <param name="error">Function to evaluate on <see cref="ResultState.Error"/> state.</param>
        /// <returns>Completed task with not null evaluated result.</returns>
        /// <exception cref="ArgumentNullException">success is null.</exception>
        /// <exception cref="ArgumentNullException">error is null.</exception>
        /// <exception cref="ResultIsNullException">result is null.</exception>
        [Pure]
        public static async Task<B> MatchAsync<A, Error, Message, B>(
            this Task<Result<A, Error, Message>> source,
            SuccessFunc<A, Message, Task<B>> success,
            ErrorFunc<Error, Message, Task<B>> error)
        {
            success.AssertArgumentNotNull(nameof(success));
            error.AssertArgumentNotNull(nameof(error));

            var sourceResult = await source;
            return await MatchAsync(sourceResult, success, error);
        }

        /// <summary>
        /// Evaluates a specified function based on the result state.
        /// Can not return null result. If null result intended then use <see cref="MatchUnsafe{A,Error,B}"/>.
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
        /// Evaluates a specified function based on the result state.
        /// Can return null result.
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
        [Pure]
        public static B MatchUnsafe<A, Error, B>(
            this in Result<A, Error> source,
            SuccessFunc<A, B> success,
            ErrorFunc<Error, B> error)
        {
            success.AssertArgumentNotNull(nameof(success));
            error.AssertArgumentNotNull(nameof(error));

            var result = source.IsSuccess
                ? success(source.Value)
                : error(source.ErrorValue);
            return result;
        }

        /// <summary>
        /// Evaluates a specified async function based on the result state.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="success">Function to evaluate on <see cref="ResultState.Success"/> state.</param>
        /// <param name="error">Function to evaluate on <see cref="ResultState.Error"/> state.</param>
        /// <returns>Completed task with not null evaluated result.</returns>
        /// <exception cref="ArgumentNullException">success is null.</exception>
        /// <exception cref="ArgumentNullException">error is null.</exception>
        /// <exception cref="ResultIsNullException">result is null.</exception>
        [Pure]
        public static async Task<B> MatchAsync<A, Error, B>(
            this Result<A, Error> source,
            SuccessFunc<A, Task<B>> success,
            ErrorFunc<Error, Task<B>> error)
        {
            success.AssertArgumentNotNull(nameof(success));
            error.AssertArgumentNotNull(nameof(error));

            var resultBTask = source.IsSuccess
                ? success(source.Value)
                : error(source.ErrorValue);
            var result = await resultBTask;
            return result.AssertNotNullResult();
        }

        /// <summary>
        /// Evaluates a specified async function based on the result state.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="success">Function to evaluate on <see cref="ResultState.Success"/> state.</param>
        /// <param name="error">Function to evaluate on <see cref="ResultState.Error"/> state.</param>
        /// <returns>Completed task with not null evaluated result.</returns>
        /// <exception cref="ArgumentNullException">success is null.</exception>
        /// <exception cref="ArgumentNullException">error is null.</exception>
        /// <exception cref="ResultIsNullException">result is null.</exception>
        [Pure]
        public static async Task<B> MatchAsync<A, Error, B>(
            this Task<Result<A, Error>> source,
            SuccessFunc<A, Task<B>> success,
            ErrorFunc<Error, Task<B>> error)
        {
            success.AssertArgumentNotNull(nameof(success));
            error.AssertArgumentNotNull(nameof(error));

            var sourceResult = await source;
            return await MatchAsync(sourceResult, success, error);
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
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="map">Map function.</param>
        /// <returns>New result of type <typeparamref name="B"/>.</returns>
        /// <exception cref="ArgumentNullException">success is null.</exception>
        public static Result<B, Error> Map<A, Error, B>(
            this in Result<A, Error> source,
            Func<A, B> map)
        {
            map.AssertArgumentNotNull(nameof(map));

            return source.Match(
                (value) => new Result<B, Error>(map(value)),
                (error) => new Result<B, Error>(error));
        }

        /// <summary>
        /// Linq select operation.
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
        /// Linq select operation.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="selector">Map function.</param>
        /// <returns>New result of type <typeparamref name="B"/>.</returns>
        /// <exception cref="ArgumentNullException">selector is null.</exception>
        public static Result<B, Error> Select<A, Error, B>(
            this in Result<A, Error> source,
            Func<A, B> selector)
        {
            selector.AssertArgumentNotNull(nameof(selector));

            return source.Map(selector);
        }

        /// <summary>
        /// Monad bind operation.
        /// Returns new monad Result of type <typeparamref name="B"/>.
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
                    return resultB.WithMessagesAtStart(list);
                },
                (error, list) => new Result<B, Error, Message>(error, list));
        }

        /// <summary>
        /// Monad bind operation (async version).
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="bindAsync">Async bind function.</param>
        /// <returns>Completed task with new result of type B.</returns>
        /// <exception cref="ArgumentNullException">bind is null.</exception>
        public static async Task<Result<B, Error, Message>> BindAsync<A, Error, Message, B>(
            this Result<A, Error, Message> source,
            Func<A, Task<Result<B, Error, Message>>> bindAsync)
        {
            bindAsync.AssertArgumentNotNull(nameof(bindAsync));

            return await source.Match(
                async (value, list) =>
                {
                    var resultB = await bindAsync(value);
                    return resultB.WithMessagesAtStart(list);
                },
                (error, list) => new Result<B, Error, Message>(error, list).ToTask());
        }

        /// <summary>
        /// Monad bind operation (async version).
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="Message">Message type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="sourceAsync">Task for get source object.</param>
        /// <param name="bindAsync">Async bind function.</param>
        /// <returns>Completed task with new result of type B.</returns>
        /// <exception cref="ArgumentNullException">bind is null.</exception>
        public static async Task<Result<B, Error, Message>> BindAsync<A, Error, Message, B>(
            this Task<Result<A, Error, Message>> sourceAsync,
            Func<A, Task<Result<B, Error, Message>>> bindAsync)
        {
            bindAsync.AssertArgumentNotNull(nameof(bindAsync));

            var source = await sourceAsync;
            return await source.BindAsync(bindAsync);
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
            this in Result<A, Error> source,
            Func<A, Result<B, Error>> bind)
        {
            bind.AssertArgumentNotNull(nameof(bind));

            return source.Match(
                (value) => bind(value),
                (error) => new Result<B, Error>(error));
        }

        /// <summary>
        /// Monad bind operation (async version).
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="bindAsync">Async bind function.</param>
        /// <returns>Completed task with new result of type B.</returns>
        /// <exception cref="ArgumentNullException">bind is null.</exception>
        public static async Task<Result<B, Error>> BindAsync<A, Error, B>(
            this Result<A, Error> source,
            Func<A, Task<Result<B, Error>>> bindAsync)
        {
            bindAsync.AssertArgumentNotNull(nameof(bindAsync));

            return await source.Match(
                async (value) => await bindAsync(value),
                (error) => new Result<B, Error>(error).ToTask());
        }

        /// <summary>
        /// Monad bind operation (async version).
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="sourceAsync">Task for get source object.</param>
        /// <param name="bindAsync">Async bind function.</param>
        /// <returns>Completed task with new result of type B.</returns>
        /// <exception cref="ArgumentNullException">bind is null.</exception>
        public static async Task<Result<B, Error>> BindAsync<A, Error, B>(
            this Task<Result<A, Error>> sourceAsync,
            Func<A, Task<Result<B, Error>>> bindAsync)
        {
            bindAsync.AssertArgumentNotNull(nameof(bindAsync));

            var source = await sourceAsync;
            return await BindAsync(source, bindAsync);
        }

        /// <summary>
        /// LINQ SelectMany.
        /// </summary>
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
                    Result<B, Error, Message> resultB = bind(a).WithMessagesAtStart(list);
                    return resultB.Match(
                        error: Result.Fail<C, Error, Message>,
                        success: (b, list2) => Result.Success<C, Error, Message>(project(a, b), list2));
                });
        }

        /// <summary>
        /// LINQ SelectMany.
        /// </summary>
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
    }
}
