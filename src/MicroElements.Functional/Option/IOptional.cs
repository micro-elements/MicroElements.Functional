using System;

namespace MicroElements.Functional
{
    /// <summary>
    /// Common interface for optional types.
    /// </summary>
    public interface IOptional
    {
        /// <summary>
        /// Is the option in a Some state.
        /// </summary>
        bool IsSome { get; }

        /// <summary>
        /// Is the option in a None state.
        /// </summary>
        bool IsNone { get; }

        /// <summary>
        /// Returns option underlying type.
        /// </summary>
        /// <returns>Option underlying type.</returns>
        Type GetUnderlyingType();

        /// <summary>
        /// Evaluates one of the specified function based on the Option state.
        /// </summary>
        /// <typeparam name="R">Result type.</typeparam>
        /// <param name="some">Function to evaluate on <see cref="OptionState.Some"/> state.</param>
        /// <param name="none">Function to evaluate on <see cref="OptionState.None"/> state.</param>
        /// <returns>Evaluated NotNull result.</returns>
        R MatchUntyped<R>(Func<object, R> some, Func<R> none);
    }
}
