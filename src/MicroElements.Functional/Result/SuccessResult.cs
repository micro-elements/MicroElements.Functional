using System;

namespace MicroElements.Functional
{
    /// <summary>
    /// Success variant of <see cref="Result{A,Error}"/> or <see cref="Result{A,Error,Message}"/>.
    /// </summary>
    /// <typeparam name="A">Success value type.</typeparam>
    public readonly struct SuccessResult<A> :
        IResult
    {
        /// <summary>
        /// Result state.
        /// </summary>
        private readonly ResultState State;

        /// <summary>
        /// Value.
        /// </summary>
        private readonly A value;

        /// <summary>
        /// Wrapped value.
        /// </summary>
        public A Value => IsInitialized ? value : throw new NotInitializedException($"Can't get Value for not initialized value SuccessResult<{typeof(A)}>.");

        /// <summary>
        /// Creates new SuccessResult.
        /// </summary>
        /// <param name="value">Value.</param>
        internal SuccessResult(A value)
        {
            State = ResultState.Success;
            this.value = value;
        }

        /// <summary>
        /// Checks that Some is initialized.
        /// </summary>
        internal bool IsInitialized => State == ResultState.Success;

        #region IResult

        /// <inheritdoc />
        public bool IsSuccess => State == ResultState.Success;

        /// <inheritdoc />
        public bool IsFailed => State == ResultState.Error;

        /// <inheritdoc />
        public Type GetSuccessValueType() => typeof(A);

        /// <inheritdoc />
        public Type GetErrorValueType() => typeof(Unit);

        /// <inheritdoc />
        public Res MatchUntyped<Res>(Func<object, Res> success, Func<object, Res> error) =>
            IsSuccess ? success(Value) : error(default);

        #endregion
    }
}
