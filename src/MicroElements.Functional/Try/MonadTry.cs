using System;
using System.Diagnostics.Contracts;

// ReSharper disable CheckNamespace
namespace MicroElements.Functional
{
    /// <summary>
    /// The Try monad captures exceptions and uses them to cancel the
    /// computation.  Primarily useful for expression based processing
    /// of errors.
    /// </summary>
    /// <remarks>To invoke directly, call x.Try()</remarks>
    /// <returns>A value that represents the outcome of the computation, either
    /// Success or Failure</returns>
    public delegate Result<A, Exception> Try<A>();

    public static class TryExtensions
    {
        [Pure]
        public static Result<T, Exception> Try<T>(this Try<T> self)
        {
            try
            {
                if (self == null)
                {
                    throw new ArgumentNullException(nameof(self));
                }
                return self();
            }
            catch (Exception e)
            {
                //TryConfig.ErrorLogger(e);
                return e;
            }
        }
    }

    public static partial class Prelude
    {
        public static Result<A, Exception> Try<A>(Func<A> func)
        {
            try
            {
                return Result.Success(func());
            }
            catch (Exception exc)
            {
                return exc;
            }
        }

        public static Result<A, Error> Try<A, Error>(Func<A> func, Func<Exception, Error> mapError)
        {
            try
            {
                return Result.Success(func());
            }
            catch (Exception exc)
            {
                return mapError(exc);
            }
        }

        public static Result<A, Error> Try<A, Error>(Func<Result<A, Error>> func, Func<Exception, Error> mapError)
        {
            try
            {
                return func();
            }
            catch (Exception exc)
            {
                return mapError(exc);
            }
        }
    }
}
