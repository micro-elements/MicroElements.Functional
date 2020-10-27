// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace MicroElements
{
    /// <summary>
    /// Creates compiled functions.
    /// </summary>
    public static class CodeCompiler
    {
        #region Caches

        private readonly struct FuncKey : IEquatable<FuncKey>
        {
            /// <summary>
            /// Function type.
            /// </summary>
            private readonly Type _type;

            /// <summary>
            /// Function name.
            /// </summary>
            private readonly string _name;

            /// <summary>
            /// Initializes a new instance of the <see cref="FuncKey"/> struct.
            /// </summary>
            /// <param name="type">Function type.</param>
            /// <param name="name">Function name.</param>
            public FuncKey(Type type, string name)
            {
                _type = type;
                _name = name;
            }

            /// <inheritdoc />
            public bool Equals(FuncKey other) => _name == other._name && _type == other._type;

            /// <inheritdoc />
            public override bool Equals(object? obj) => obj is FuncKey other && Equals(other);

            /// <inheritdoc />
            public override int GetHashCode() => HashCode.Combine(_name, _type);

            public static bool operator ==(FuncKey left, FuncKey right) => left.Equals(right);

            public static bool operator !=(FuncKey left, FuncKey right) => !left.Equals(right);
        }

        private static class Cache<Arg0, Result>
        {
            public static readonly ConcurrentDictionary<FuncKey, Func<Arg0, Result>> FuncCache =
                new ConcurrentDictionary<FuncKey, Func<Arg0, Result>>();
        }

        private static class Cache<Arg0, Arg1, Result>
        {
            public static readonly ConcurrentDictionary<FuncKey, Func<Arg0, Arg1, Result>> FuncCache =
                new ConcurrentDictionary<FuncKey, Func<Arg0, Arg1, Result>>();
        }

        private static class Cache<Arg0, Arg1, Arg2, Result>
        {
            public static readonly ConcurrentDictionary<FuncKey, Func<Arg0, Arg1, Arg2, Result>> FuncCache =
                new ConcurrentDictionary<FuncKey, Func<Arg0, Arg1, Arg2, Result>>();
        }

        #endregion

        /// <summary>
        /// Marker type for generic function type.
        /// </summary>
        public class GenericType { }

        /// <summary>
        /// Creates compiled function from generic function.
        /// <paramref name="type"/> will be used instead <see cref="GenericType"/>.
        /// </summary>
        /// <typeparam name="Arg0">Input Arg0 type.</typeparam>
        /// <typeparam name="Result">Result type.</typeparam>
        /// <param name="type">Runtime type that will be used instead <see cref="GenericType"/> in <paramref name="genericMethodFunc"/>.</param>
        /// <param name="name">Function name to distinguish functions with the same arg types in cache.</param>
        /// <param name="genericMethodFunc">Generic function that will be compiled for each input <paramref name="type"/>.</param>
        /// <returns>Compiled function for <paramref name="type"/>.</returns>
        public static Func<Arg0, Result> CachedCompiledFunc<Arg0, Result>(Type type, string name, Func<Arg0, Result> genericMethodFunc)
        {
            if (!Cache<Arg0, Result>.FuncCache.TryGetValue(new FuncKey(type, name), out var cachedFunc))
            {
                cachedFunc = CompileGeneric(type, genericMethodFunc);
                Cache<Arg0, Result>.FuncCache.TryAdd(new FuncKey(type, name), cachedFunc);
            }

            return cachedFunc;
        }

        /// <summary>
        /// Creates compiled function from generic function.
        /// <paramref name="type"/> will be used instead <see cref="GenericType"/>.
        /// </summary>
        /// <typeparam name="Arg0">Input Arg0 type.</typeparam>
        /// <typeparam name="Arg1">Input Arg1 type.</typeparam>
        /// <typeparam name="Result">Result type.</typeparam>
        /// <param name="type">Runtime type that will be used instead <see cref="GenericType"/> in <paramref name="genericMethodFunc"/>.</param>
        /// <param name="name">Function name to distinguish functions with the same arg types in cache.</param>
        /// <param name="genericMethodFunc">Generic function that will be compiled for each input <paramref name="type"/>.</param>
        /// <returns>Compiled function for <paramref name="type"/>.</returns>
        public static Func<Arg0, Arg1, Result> CachedCompiledFunc<Arg0, Arg1, Result>(Type type, string name, Func<Arg0, Arg1, Result> genericMethodFunc)
        {
            if (!Cache<Arg0, Arg1, Result>.FuncCache.TryGetValue(new FuncKey(type, name), out var cachedFunc))
            {
                cachedFunc = CompileGeneric(type, genericMethodFunc);
                Cache<Arg0, Arg1, Result>.FuncCache.TryAdd(new FuncKey(type, name), cachedFunc);
            }

            return cachedFunc;
        }

        /// <summary>
        /// Creates compiled function from generic function.
        /// <paramref name="type"/> will be used instead <see cref="GenericType"/>.
        /// </summary>
        /// <typeparam name="Arg0">Input Arg0 type.</typeparam>
        /// <typeparam name="Arg1">Input Arg1 type.</typeparam>
        /// <typeparam name="Arg2">Input Arg2 type.</typeparam>
        /// <typeparam name="Result">Result type.</typeparam>
        /// <param name="type">Runtime type that will be used instead <see cref="GenericType"/> in <paramref name="genericMethodFunc"/>.</param>
        /// <param name="name">Function name to distinguish functions with the same arg types in cache.</param>
        /// <param name="genericMethodFunc">Generic function that will be compiled for each input <paramref name="type"/>.</param>
        /// <returns>Compiled function for <paramref name="type"/>.</returns>
        public static Func<Arg0, Arg1, Arg2, Result> CachedCompiledFunc<Arg0, Arg1, Arg2, Result>(Type type, string name, Func<Arg0, Arg1, Arg2, Result> genericMethodFunc)
        {
            if (!Cache<Arg0, Arg1, Arg2, Result>.FuncCache.TryGetValue(new FuncKey(type, name), out var cachedFunc))
            {
                cachedFunc = CompileGeneric(type, genericMethodFunc);
                Cache<Arg0, Arg1, Arg2, Result>.FuncCache.TryAdd(new FuncKey(type, name), cachedFunc);
            }

            return cachedFunc;
        }

        private static Func<Arg0, Result> CompileGeneric<Arg0, Result>(Type propertyType, Func<Arg0, Result> genericMethodFunc)
        {
            MethodInfo methodInfo = genericMethodFunc.Method.GetGenericMethodDefinition();
            return Compile<Arg0, Result>(propertyType, methodInfo);
        }

        private static Func<Arg0, Arg1, Result> CompileGeneric<Arg0, Arg1, Result>(Type propertyType, Func<Arg0, Arg1, Result> genericMethodFunc)
        {
            MethodInfo methodInfo = genericMethodFunc.Method.GetGenericMethodDefinition();
            return Compile<Arg0, Arg1, Result>(propertyType, methodInfo);
        }

        private static Func<Arg0, Arg1, Arg2, Result> CompileGeneric<Arg0, Arg1, Arg2, Result>(Type propertyType, Func<Arg0, Arg1, Arg2, Result> genericMethodFunc)
        {
            MethodInfo methodInfo = genericMethodFunc.Method.GetGenericMethodDefinition();
            return Compile<Arg0, Arg1, Arg2, Result>(propertyType, methodInfo);
        }

        private static Func<Arg0, Result> Compile<Arg0, Result>(Type propertyType, MethodInfo methodInfo)
        {
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(propertyType);
            ParameterExpression arg0 = Expression.Parameter(typeof(Arg0));
            MethodCallExpression callExpression = Expression.Call(genericMethod, arg0);
            return Expression
                .Lambda<Func<Arg0, Result>>(callExpression, arg0)
                .Compile();
        }

        private static Func<Arg0, Arg1, Result> Compile<Arg0, Arg1, Result>(Type propertyType, MethodInfo methodInfo)
        {
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(propertyType);
            ParameterExpression arg0 = Expression.Parameter(typeof(Arg0));
            ParameterExpression arg1 = Expression.Parameter(typeof(Arg1));
            MethodCallExpression callExpression = Expression.Call(genericMethod, arg0, arg1);
            return Expression
                .Lambda<Func<Arg0, Arg1, Result>>(callExpression, arg0, arg1)
                .Compile();
        }

        private static Func<Arg0, Arg1, Arg2, Result> Compile<Arg0, Arg1, Arg2, Result>(Type propertyType, MethodInfo methodInfo)
        {
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(propertyType);
            ParameterExpression arg0 = Expression.Parameter(typeof(Arg0));
            ParameterExpression arg1 = Expression.Parameter(typeof(Arg1));
            ParameterExpression arg2 = Expression.Parameter(typeof(Arg2));
            MethodCallExpression callExpression = Expression.Call(genericMethod, arg0, arg1, arg2);
            return Expression
                .Lambda<Func<Arg0, Arg1, Arg2, Result>>(callExpression, arg0, arg1, arg2)
                .Compile();
        }
    }
}
