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
}
