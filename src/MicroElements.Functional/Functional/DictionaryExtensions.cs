// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using MicroElements.Functional.Errors;
using MicroElements.Reflection.ObjectExtensions;
using static MicroElements.Functional.Prelude;

namespace MicroElements.Functional
{
    /// <summary>
    /// Extension methods for <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets optional value from dictionary by key.
        /// Returns Some(value) on success and None if key is not found in the dictionary.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="dictionary">Dictionary to search.</param>
        /// <param name="key">Key value.</param>
        /// <returns><see cref="Option{A}"/>.</returns>
        public static Option<TValue> GetValueAsOption<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var value) && value.IsNotNull())
                return value;
            return None;
        }

        /// <summary>
        /// Gets optional value from dictionary by key.
        /// Returns Some(value) on success and None if key is not found in the dictionary.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="dictionary">Dictionary to search.</param>
        /// <param name="key">Key value.</param>
        /// <returns><see cref="Option{A}"/>.</returns>
        public static Option<TValue> GetValueAsOption<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var value) && value.IsNotNull())
                return value;
            return None;
        }

        /// <summary>
        /// Gets optional value from dictionary by key.
        /// Returns Some(value) on success and None if key is not found in the dictionary.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="dictionary">Dictionary to search.</param>
        /// <param name="key">Key value.</param>
        /// <returns><see cref="Option{A}"/>.</returns>
        public static Option<TValue> GetValueAsOption<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var value) && value.IsNotNull())
                return value;
            return None;
        }

        /// <summary>
        /// Gets value from dictionary by key.
        /// Returns Success(value) on success and Fail(message) if key is not found in the dictionary.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="dictionary">Dictionary to search.</param>
        /// <param name="key">Key value.</param>
        /// <returns><see cref="Result{TValue,Error}"/>.</returns>
        public static Result<TValue, KeyNotFound<TKey>> GetValueAsResult<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var value))
                return value;
            return Result.Fail(new KeyNotFound<TKey>(key));
        }

        /// <summary>
        /// Gets value from dictionary by key.
        /// Returns Success(value) on success and Fail(message) if key is not found in the dictionary.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="dictionary">Dictionary to search.</param>
        /// <param name="key">Key value.</param>
        /// <returns><see cref="Result{TValue,Error}"/>.</returns>
        public static Result<TValue, KeyNotFound<TKey>> GetValueAsResult<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var value))
                return value;
            return Result.Fail(new KeyNotFound<TKey>(key));
        }

        /// <summary>
        /// Gets value from dictionary by key.
        /// Returns Success(value) on success and Fail(message) if key is not found in the dictionary.
        /// </summary>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="dictionary">Dictionary to search.</param>
        /// <param name="key">Key value.</param>
        /// <returns><see cref="Result{TValue,Error}"/>.</returns>
        public static Result<TValue, KeyNotFound<TKey>> GetValueAsResult<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var value))
                return value;
            return Result.Fail(new KeyNotFound<TKey>(key));
        }
    }
}
