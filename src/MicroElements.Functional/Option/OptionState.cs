// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents Option state: Some | None.
    /// </summary>
    public enum OptionState : byte
    {
        /// <summary>
        /// Option is None.
        /// </summary>
        None,

        /// <summary>
        /// Option is Some (has value).
        /// </summary>
        Some,
    }
}