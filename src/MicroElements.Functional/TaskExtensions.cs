// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace MicroElements.Functional
{
    /// <summary>
    /// Task extension methods.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Converts value to <see cref="Task{T}"/>.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="value">Value.</param>
        /// <returns>Completed task.</returns>
        public static Task<T> ToTask<T>(this T value) => Task.FromResult(value);
    }
}
