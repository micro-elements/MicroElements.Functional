namespace MicroElements.Functional
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides method for getting equality components.
    /// </summary>
    public interface IEquatableObject
    {
        /// <summary>
        /// Gets all components for equality comparison.
        /// </summary>
        /// <returns>Enumeration of equality components.</returns>
        IEnumerable<object> GetEqualityComponents();
    }
}
