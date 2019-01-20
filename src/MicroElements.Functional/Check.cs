// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.Functional
{
    internal static class Check
    {
        internal static T NotNullResult<T>(this T value) =>
            value.IsNull()
                ? throw new ResultIsNullException()
                : value;
    }
}
