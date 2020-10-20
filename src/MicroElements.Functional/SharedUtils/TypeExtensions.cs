// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace MicroElements.Functional
{
    /// <summary>
    /// Reflection extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets default value for type.
        /// </summary>
        /// <param name="type">Source type.</param>
        /// <returns>Default value.</returns>
        public static object? GetDefaultValue(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Determines whether <paramref name="type"/> is assignable to <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to test assignability to.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <returns>True if type is assignable to references of type <typeparamref name="T" />; otherwise, False.</returns>
        public static bool IsAssignableTo<T>(this Type type)
        {
            type.AssertArgumentNotNull(nameof(type));

            return typeof(T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        /// <summary>
        /// Determines whether <paramref name="type"/> is concrete type: (not interface and not abstract).
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if type is concrete.</returns>
        public static bool IsConcreteType(this Type type)
        {
            type.AssertArgumentNotNull(nameof(type));
            return !type.IsInterface && !type.IsAbstract;
        }
    }
}
