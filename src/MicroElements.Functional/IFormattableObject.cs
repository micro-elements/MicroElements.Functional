using System.Collections.Generic;

namespace MicroElements.Functional
{
    /// <summary>
    /// Provides method for getting object state as string key-value pairs.
    /// </summary>
    public interface IFormattableObject
    {
        /// <summary>
        /// Gets object state as string key-value pairs.
        /// </summary>
        /// <returns>Enumeration of key-value pairs.</returns>
        IEnumerable<(string Name, string Value)> GetNameValuePairs();
    }
}
