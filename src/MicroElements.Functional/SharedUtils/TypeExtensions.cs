// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace MicroElements.Functional
{
    public static class TypeExtensions
    {
        public static object GetDefaultValue(this Type type)
        {
            return type.IsNullableStruct() ? Activator.CreateInstance(type) : null;
        }

        public static object GetDefaultValueCompiled(this Type type)
        {
            Func<Unit, object> func = CodeCompiler.CachedCompiledFunc<Unit, object>(type, GetDefaultValueInternal<CodeCompiler.GenericType>);
            return func(Unit.Default);
        }

        internal static object GetDefaultValueInternal<T>(Unit unit) => default(T);
    }
}
