// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MicroElements.Functional
{
    internal static class TypeCheck<T>
    {
        internal static readonly bool IsReferenceType;
        internal static readonly bool IsNullableStruct;
        internal static readonly EqualityComparer<T> DefaultEqualityComparer;

        static TypeCheck()
        {
            IsNullableStruct = Nullable.GetUnderlyingType(typeof(T)) != null;
            IsReferenceType = !typeof(T).GetTypeInfo().IsValueType;
            DefaultEqualityComparer = EqualityComparer<T>.Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsDefault([AllowNull] T value) => DefaultEqualityComparer.Equals(value, default(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsNull([AllowNull] T value) => IsNullableStruct ? value.Equals(default(T)) : IsReferenceType && DefaultEqualityComparer.Equals(value, default(T));
    }

    public static class TypeCheck
    {
        public static bool IsReferenceType(this Type type)
        {
            return !type.GetTypeInfo().IsValueType;
        }

        public static bool IsNullableStruct(this Type type)
        {
            return type.GetTypeInfo().IsValueType && Nullable.GetUnderlyingType(type) != null;
        }
    }
}
