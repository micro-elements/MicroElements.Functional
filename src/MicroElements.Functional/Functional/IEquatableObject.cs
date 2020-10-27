// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace MicroElements.Functional
{
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
