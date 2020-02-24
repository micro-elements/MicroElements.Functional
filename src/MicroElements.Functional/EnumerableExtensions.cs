using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroElements.Functional
{
    /// <summary>
    /// <see cref="IEnumerable{T}"/> extensions.
    /// </summary>
    public static class EnumerableExtensions
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

        /// <summary>
        /// Returns the first element of the sequence that satisfies a condition or None if no such element is found.
        /// It's like FirstOrDefault but returns Option.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>First element that satisfies <paramref name="predicate"/> or None.</returns>
        public static Option<T> FirstOrNone<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            source.AssertArgumentNotNull(nameof(source));
            predicate.AssertArgumentNotNull(nameof(predicate));

            if (TypeCheck<T>.IsReferenceType)
                return source.FirstOrDefault(predicate);

            foreach (T element in source)
            {
                if (predicate(element))
                {
                    return element;
                }
            }

            return Option<T>.None;
        }
    }
}
