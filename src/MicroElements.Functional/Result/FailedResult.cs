using System;

namespace MicroElements.Functional
{
    /// <summary>
    /// Failed variant of <see cref="Result{A,Error}"/> or <see cref="Result{A,Error,Message}"/>.
    /// </summary>
    /// <typeparam name="Error">Error type.</typeparam>
    public readonly struct FailedResult<Error> :
        IResult
    {
        /// <summary>
        /// Checks that FailedResult is initialized.
        /// </summary>
        internal readonly bool IsInitialized;

        /// <summary>
        /// Value.
        /// </summary>
        private readonly Error errorValue;

        /// <summary>
        /// Wrapped value.
        /// </summary>
        public Error ErrorValue => IsInitialized ? errorValue : throw new NotInitializedException($"Can't get ErrorValue for not initialized value FailedResult<{typeof(Error)}>.");

        /// <summary>
        /// Creates new SuccessResult.
        /// </summary>
        /// <param name="errorValue">Error value.</param>
        internal FailedResult(Error errorValue)
        {
            IsInitialized = true;
            this.errorValue = errorValue;
        }

        #region IResult

        /// <inheritdoc />
        public bool IsSuccess => false;

        /// <inheritdoc />
        public bool IsFailed => true;

        /// <inheritdoc />
        public Type GetSuccessValueType() => typeof(Unit);

        /// <inheritdoc />
        public Type GetErrorValueType() => typeof(Error);

        /// <inheritdoc />
        public Res MatchUntyped<Res>(Func<object, Res> success, Func<object, Res> error) =>
            IsInitialized ? error(ErrorValue) : throw new NotInitializedException($"FailedResult<{typeof(Error)} is not initialized");

        #endregion
    }
}
