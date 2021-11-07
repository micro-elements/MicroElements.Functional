// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using MicroElements.CodeContracts;
using MicroElements.Reflection.TypeCheck;

namespace MicroElements.Functional
{
    /// <summary>
    /// <see cref="IEnumerable{T}"/> extensions.
    /// </summary>
    public static class EnumerableExtensions
    {
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

        /// <summary>
        /// Flattens <paramref name="enumerationOfEnumerations"/> instances into a single, new enumerable.
        /// </summary>
        /// <typeparam name="T">Enumerable type.</typeparam>
        /// <param name="enumerationOfEnumerations">Enumeration of enumerations.</param>
        /// <returns>Single enumeration.</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerationOfEnumerations)
        {
            return enumerationOfEnumerations.SelectMany(enumerable => enumerable);
        }

        /// <summary>
        /// Materializes <paramref name="source"/> as <see cref="IReadOnlyList{T}"/> and invokes <paramref name="action"/>.
        /// If <paramref name="action"/> is null then no materialization occurs.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="source">Source enumeration.</param>
        /// <param name="action">Optional action with source snapshot as argument.</param>
        /// <returns>The same enumeration if action is null or materialized enumeration.</returns>
        public static IEnumerable<T> Materialize<T>(this IEnumerable<T> source, Action<IReadOnlyList<T>>? action)
        {
            if (action == null)
                return source;

            var materializedItems = source.ToArray();
            action(materializedItems);
            return materializedItems;
        }
    }
}
