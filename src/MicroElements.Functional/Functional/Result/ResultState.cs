// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents Result state: Success | Error.
    /// </summary>
    public enum ResultState : byte
    {
        /// <summary>
        /// Result is in Error state.
        /// </summary>
        Error,

        /// <summary>
        /// Result is in Success state.
        /// </summary>
        Success,
    }
}
