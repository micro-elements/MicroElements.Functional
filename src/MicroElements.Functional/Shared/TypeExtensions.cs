// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using MicroElements.Shared;

// ReSharper disable once CheckNamespace
namespace MicroElements.Functional
{
    /// <summary>
    /// Reflection extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns a value indicating whether the type is a reference type.
        /// </summary>
        /// <param name="type">Source type.</param>
        /// <returns>True if argument is a reference type.</returns>
        public static bool IsReferenceType(this Type type)
        {
            type.AssertArgumentNotNull(nameof(type));

            return !type.GetTypeInfo().IsValueType;
        }

        /// <summary>
        /// Returns a value indicating whether the type is a nullable struct.
        /// </summary>
        /// <param name="type">Source type.</param>
        /// <returns>True if argument is a nullable struct.</returns>
        public static bool IsNullableStruct(this Type type)
        {
            type.AssertArgumentNotNull(nameof(type));

            return type.GetTypeInfo().IsValueType && Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Returns a value indicating whether the type is a numeric type.
        /// </summary>
        /// <param name="type">Source type.</param>
        /// <returns>True if argument is a numeric type.</returns>
        public static bool IsNumericType(this Type type)
        {
            type.AssertArgumentNotNull(nameof(type));

            return TypeCache.NumericTypes.Contains(type);
        }

        /// <summary>
        /// Returns a value indicating whether the type is a nullable numeric type.
        /// </summary>
        /// <param name="type">Source type.</param>
        /// <returns>True if argument is a nullable numeric type.</returns>
        public static bool IsNullableNumericType(this Type type)
        {
            type.AssertArgumentNotNull(nameof(type));

            if (!type.GetTypeInfo().IsValueType)
                return false;

            Type? underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType is null)
                return false;

            return underlyingType.IsNumericType();
        }

        /// <summary>
        /// Returns <c>true</c> if <c>null</c> can be assigned to type.
        /// </summary>
        /// <param name="targetType">Target type.</param>
        /// <returns><c>true</c> if <c>null</c> can be assigned to type.</returns>
        public static bool CanAcceptNull(this Type targetType)
        {
            targetType.AssertArgumentNotNull(nameof(targetType));

            return targetType.IsReferenceType() || targetType.IsNullableStruct();
        }

        /// <summary>
        /// Returns <c>true</c> if <c>null</c> can not be assigned to type.
        /// </summary>
        /// <param name="targetType">Target type.</param>
        /// <returns><c>true</c> if <c>null</c> can not be assigned to type.</returns>
        public static bool CanNotAcceptNull(this Type targetType)
        {
            targetType.AssertArgumentNotNull(nameof(targetType));

            return !targetType.CanAcceptNull();
        }

        /// <summary>
        /// Returns <c>true</c> if <paramref name="value"/> is assignable to <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">Type.</param>
        /// <param name="value">Value to check.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is assignable to <paramref name="targetType"/>.</returns>
        public static bool IsAssignableTo(this Type targetType, object value)
        {
            targetType.AssertArgumentNotNull(nameof(targetType));
            value.AssertArgumentNotNull(nameof(value));

            return value.GetType().IsAssignableTo(targetType);
        }

        /// <summary>
        /// Returns <c>true</c> if <paramref name="value"/> is assignable to <paramref name="targetType"/>.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="targetType">Type to check.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is assignable to <paramref name="targetType"/>.</returns>
        public static bool IsAssignableTo(this object value, Type targetType)
        {
            value.AssertArgumentNotNull(nameof(value));
            targetType.AssertArgumentNotNull(nameof(targetType));

            return value.GetType().IsAssignableTo(targetType);
        }

        /// <summary>
        /// Returns <c>true</c> if <paramref name="sourceType"/> is assignable to <paramref name="targetType"/>.
        /// </summary>
        /// <param name="sourceType">Source type.</param>
        /// <param name="targetType">Type to check.</param>
        /// <returns><c>true</c> if <paramref name="sourceType"/> is assignable to <paramref name="targetType"/>.</returns>
        public static bool IsAssignableTo(this Type sourceType, Type targetType)
        {
            sourceType.AssertArgumentNotNull(nameof(sourceType));
            targetType.AssertArgumentNotNull(nameof(targetType));

            return targetType.GetTypeInfo().IsAssignableFrom(sourceType.GetTypeInfo());
        }

        /// <summary>
        /// Gets default value for type.
        /// </summary>
        /// <param name="type">Source type.</param>
        /// <returns>Default value.</returns>
        public static object? GetDefaultValue(this Type type)
        {
            type.AssertArgumentNotNull(nameof(type));

            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Determines whether <paramref name="sourceType"/> is assignable to <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to test assignability to.</typeparam>
        /// <param name="sourceType">Source type to check.</param>
        /// <returns>True if type is assignable to references of type <typeparamref name="T" />; otherwise, False.</returns>
        public static bool IsAssignableTo<T>(this Type sourceType)
        {
            sourceType.AssertArgumentNotNull(nameof(sourceType));

            return typeof(T).GetTypeInfo().IsAssignableFrom(sourceType.GetTypeInfo());
        }

        /// <summary>
        /// Determines whether <paramref name="sourceType"/> is concrete type: (not interface and not abstract).
        /// </summary>
        /// <param name="sourceType">Source type to check.</param>
        /// <returns>True if type is concrete.</returns>
        public static bool IsConcreteType(this Type sourceType)
        {
            sourceType.AssertArgumentNotNull(nameof(sourceType));

            return !sourceType.IsInterface && !sourceType.IsAbstract;
        }

        /// <summary>
        /// Determines whether <paramref name="sourceType"/> is concrete class and assignable to <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">Target type to test assignability to.</typeparam>
        /// <param name="sourceType">Source type to check.</param>
        /// <returns>True if type is assignable to references of type <typeparamref name="T" />; otherwise, False.</returns>
        public static bool IsConcreteAndAssignableTo<T>(this Type sourceType)
        {
            sourceType.AssertArgumentNotNull(nameof(sourceType));

            return sourceType.IsConcreteType() && sourceType.IsAssignableTo<T>();
        }
    }
}
