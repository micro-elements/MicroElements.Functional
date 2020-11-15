// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

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
        [LinqTunnel]
        public static IEnumerable<T> NotNull<T>([NoEnumeration] this IEnumerable<T>? items) =>
            items ?? Array.Empty<T>();

        /// <summary>
        /// Returns not null <see cref="IReadOnlyCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="items"><see cref="IReadOnlyCollection{T}"/> or null.</param>
        /// <returns>The same items or empty collection.</returns>
        public static IReadOnlyCollection<T> NotNull<T>(IReadOnlyCollection<T>? items) =>
            items ?? Array.Empty<T>();

        /// <summary>
        /// Returns not null <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="items"><see cref="IReadOnlyList{T}"/> or null.</param>
        /// <returns>The same items or empty list.</returns>
        public static IReadOnlyList<T> NotNull<T>(IReadOnlyList<T>? items) =>
            items ?? Array.Empty<T>();

        /// <summary>
        /// Returns not null Array of T.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="items">array or null.</param>
        /// <returns>The same items or empty array.</returns>
        public static T[] NotNull<T>(T[]? items) =>
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

        /// <summary>
        /// Iterates values.
        /// Can be used to replace <see cref="Enumerable.ToArray{TSource}"/> if no need to create array but only iterate.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="values">Enumeration.</param>
        public static void Iterate<T>(this IEnumerable<T> values)
        {
            values.Iterate(DoNothing);
        }

        /// <summary>
        /// Iterates values and executes <paramref name="action"/> for each value.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="values">Enumeration.</param>
        /// <param name="action">Action to execute.</param>
        public static void Iterate<T>(this IEnumerable<T> values, Action<T> action)
        {
            values.AssertArgumentNotNull(nameof(values));
            action.AssertArgumentNotNull(nameof(action));

            foreach (T value in values)
            {
                action(value);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void DoNothing<T>(T value) { }
    }
}
