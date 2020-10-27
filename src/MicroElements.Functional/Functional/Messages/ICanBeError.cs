// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents object that can be treated as Error.
    /// </summary>
    public interface ICanBeError
    {
        /// <summary>
        /// Object is Error.
        /// </summary>
        bool IsError { get; }
    }
}
