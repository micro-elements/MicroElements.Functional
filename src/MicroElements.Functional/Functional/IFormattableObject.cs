// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace MicroElements.Functional
{
    /// <summary>
    /// Provides method for getting object state as string key-value pairs.
    /// </summary>
    public interface IFormattableObject
    {
        /// <summary>
        /// Gets object state as key-value pairs.
        /// </summary>
        /// <returns>Enumeration of key-value pairs.</returns>
        IEnumerable<(string Name, object Value)> GetNameValuePairs();
    }
}
