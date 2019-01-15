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
    }
}
