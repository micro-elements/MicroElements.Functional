// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.Functional
{
    /// <summary>
    /// Mode for adding list of properties to <see cref="Message"/>.
    /// </summary>
    public enum PropertyAddMode
    {
        /// <summary>
        /// Sets new property list (replaces old list).
        /// </summary>
        Set,

        /// <summary>
        /// Merges new list with old list. New values replaces old if property name equals.
        /// </summary>
        Merge,

        /// <summary>
        /// Adds property if not exists on old list.
        /// </summary>
        AddIfNotExists
    }
}
