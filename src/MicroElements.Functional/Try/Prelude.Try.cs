﻿using System;

namespace MicroElements.Functional
{
    public static partial class Prelude
    {
        /// <summary>
        /// Executes <paramref name="func"/> in <c>try</c> block.
        /// Returns SuccessResult or FailedResult with <see cref="Exception"/> as error.
        /// </summary>
        /// <typeparam name="A">A type.</typeparam>
        /// <param name="func">Function to execute.</param>
        /// <returns>SuccessResult or FailedResult with <see cref="Exception"/> as error.</returns>
        public static Result<A, Exception> Try<A>(Func<A> func)
        {
            try
            {
                return Result.Success(func());
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /// <summary>
        /// Executes <paramref name="func"/> in <c>try</c> block.
        /// Uses <paramref name="mapError"/> for converting <see cref="Exception"/> to error type.
        /// </summary>
        /// <typeparam name="A">A type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <param name="func">Function to execute.</param>
        /// <param name="mapError">Function to convert exception to error type.</param>
        /// <returns>SuccessResult or FailedResult with <typeparamref name="Error"/> as error.</returns>
        public static Result<A, Error> Try<A, Error>(Func<A> func, Func<Exception, Error> mapError)
        {
            try
            {
                return Result.Success(func());
            }
            catch (Exception e)
            {
                return mapError(e);
            }
        }

        /// <summary>
        /// Executes <paramref name="func"/> in <c>try</c> block.
        /// Uses <paramref name="mapError"/> for converting <see cref="Exception"/> to error type.
        /// </summary>
        /// <typeparam name="A">A type.</typeparam>
        /// <typeparam name="Error">Error type.</typeparam>
        /// <param name="func">Function to execute.</param>
        /// <param name="mapError">Function to convert exception to error type.</param>
        /// <returns>SuccessResult or FailedResult with <typeparamref name="Error"/> as error.</returns>
        public static Result<A, Error> Try<A, Error>(Func<Result<A, Error>> func, Func<Exception, Error> mapError)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                return mapError(e);
            }
        }
    }
}