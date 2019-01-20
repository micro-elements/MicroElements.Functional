using System;
using System.Collections.Generic;

namespace MicroElements.Functional
{
    /// <summary>
    /// <see cref="IEnumerable{T}"/> extensions.
    /// </summary>
    public static class EnumerableExt
    {
        /// <summary>
        /// Returns not null <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="items"><see cref="IEnumerable{T}"/> or null.</param>
        /// <returns>The same items or empty enumeration.</returns>
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> items) =>
            items ?? Array.Empty<T>();

        /// <summary>
        /// Returns not null <see cref="IReadOnlyCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="items"><see cref="IReadOnlyCollection{T}"/> or null.</param>
        /// <returns>The same items or empty collection.</returns>
        public static IReadOnlyCollection<T> NotNull<T>(IReadOnlyCollection<T> items) =>
            items ?? Array.Empty<T>();

        /// <summary>
        /// Returns not null <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="items"><see cref="IReadOnlyList{T}"/> or null.</param>
        /// <returns>The same items or empty list.</returns>
        public static IReadOnlyList<T> NotNull<T>(IReadOnlyList<T> items) =>
            items ?? Array.Empty<T>();

        /// <summary>
        /// Returns not null Array of T.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="items">array or null.</param>
        /// <returns>The same items or empty array.</returns>
        public static T[] NotNull<T>(T[] items) =>
            items ?? Array.Empty<T>();
    }
}
