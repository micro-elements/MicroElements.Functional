// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using MicroElements.CodeContracts;

namespace MicroElements.Functional
{
    /// <summary>
    /// Monadic operations.
    /// </summary>
    public static partial class ResultOperations
    {
        /// <summary>
        /// Executes Bind operation on <c>try</c> block.
        /// Uses <paramref name="mapError"/> to convert exception to <typeparamref name="Error"/> type.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="bind">Bind function.</param>
        /// <param name="mapError">Convert <see cref="Exception"/> to result <typeparamref name="Error"/> type.</param>
        /// <returns>New result of type B.</returns>
        /// <exception cref="ArgumentNullException">bind is null.</exception>
        /// <exception cref="ArgumentNullException">mapError is null.</exception>
        public static Result<B, Error> TryBind<A, Error, B>(
            this in Result<A, Error> source,
            Func<A, Result<B, Error>> bind,
            Func<Exception, Error> mapError)
        {
            bind.AssertArgumentNotNull(nameof(bind));
            mapError.AssertArgumentNotNull(nameof(mapError));

            try
            {
                return source.Bind(bind);
            }
            catch (Exception e)
            {
                return mapError(e);
            }
        }

        /// <summary>
        /// Executes Bind operation on <c>try</c> block.
        /// Uses <paramref name="mapError"/> to convert exception to <typeparamref name="Error"/> type.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="bind">Bind function.</param>
        /// <param name="mapError">Convert <see cref="Exception"/> to result <typeparamref name="Error"/> type.</param>
        /// <returns>New result of type B.</returns>
        /// <exception cref="ArgumentNullException">bind is null.</exception>
        /// <exception cref="ArgumentNullException">mapError is null.</exception>
        public static Result<B, Error> TryBind<A, Error, B>(
            this in Result<A, Error> source,
            Func<A, B> bind,
            Func<Exception, Error> mapError)
        {
            bind.AssertArgumentNotNull(nameof(bind));
            mapError.AssertArgumentNotNull(nameof(mapError));

            try
            {
                return source.Bind(a => Result.Success<B, Error>(bind(a)));
            }
            catch (Exception e)
            {
                return mapError(e);
            }
        }

        /// <summary>
        /// Executes BindAsync operation on <c>try</c> block.
        /// Uses <paramref name="mapError"/> to convert exception to <typeparamref name="Error"/> type.
        /// </summary>
        /// <typeparam name="A">Success result type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <typeparam name="B">Result type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="bindAsync">Async Bind function.</param>
        /// <param name="mapError">Convert <see cref="Exception"/> to result <typeparamref name="Error"/> type.</param>
        /// <returns>New result of type B.</returns>
        /// <exception cref="ArgumentNullException">bindAsync is null.</exception>
        /// <exception cref="ArgumentNullException">mapError is null.</exception>
        public static async Task<Result<B, Error>> TryBindAsync<A, Error, B>(
            this Result<A, Error> source,
            Func<A, Task<B>> bindAsync,
            Func<Exception, Error> mapError)
        {
            bindAsync.AssertArgumentNotNull(nameof(bindAsync));
            mapError.AssertArgumentNotNull(nameof(mapError));

            try
            {
                return await source.BindAsync(async a => Result.Success<B, Error>(await bindAsync(a)));
            }
            catch (Exception e)
            {
                return mapError(e);
            }
        }
    }
}
