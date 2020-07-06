// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace MicroElements
{
    internal static class CodeCompiler
    {
        #region Caches

        static class Cache<Arg0, Result>
        {
            public static ConcurrentDictionary<Type, Func<Arg0, Result>> FuncCache = new ConcurrentDictionary<Type, Func<Arg0, Result>>();
        }

        static class Cache<Arg0, Arg1, Result>
        {
            public static ConcurrentDictionary<Type, Func<Arg0, Arg1, Result>> FuncCache = new ConcurrentDictionary<Type, Func<Arg0, Arg1, Result>>();
        }

        static class Cache<Arg0, Arg1, Arg2, Result>
        {
            public static ConcurrentDictionary<Type, Func<Arg0, Arg1, Arg2, Result>> FuncCache = new ConcurrentDictionary<Type, Func<Arg0, Arg1, Arg2, Result>>();
        }

        #endregion

        /// <summary>
        /// Маркерный тип для указания, что функция будет сгенерирована для произвольных типов.
        /// </summary>
        public class GenericType { }

        public static Func<Arg0, Result> CachedCompiledFunc<Arg0, Result>(Type type, Func<Arg0, Result> genericMethodFunc)
        {
            if (!Cache<Arg0, Result>.FuncCache.TryGetValue(type, out var cachedFunc))
            {
                cachedFunc = CompileGeneric(type, genericMethodFunc);
                Cache<Arg0, Result>.FuncCache.TryAdd(type, cachedFunc);
            }

            return cachedFunc;
        }

        public static Func<Arg0, Arg1, Result> CachedCompiledFunc<Arg0, Arg1, Result>(Type type, Func<Arg0, Arg1, Result> genericMethodFunc)
        {
            if (!Cache<Arg0, Arg1, Result>.FuncCache.TryGetValue(type, out var cachedFunc))
            {
                cachedFunc = CompileGeneric(type, genericMethodFunc);
                Cache<Arg0, Arg1, Result>.FuncCache.TryAdd(type, cachedFunc);
            }

            return cachedFunc;
        }

        public static Func<Arg0, Arg1, Arg2, Result> CachedCompiledFunc<Arg0, Arg1, Arg2, Result>(Type type, Func<Arg0, Arg1, Arg2, Result> genericMethodFunc)
        {
            if (!Cache<Arg0, Arg1, Arg2, Result>.FuncCache.TryGetValue(type, out var cachedFunc))
            {
                cachedFunc = CompileGeneric(type, genericMethodFunc);
                Cache<Arg0, Arg1, Arg2, Result>.FuncCache.TryAdd(type, cachedFunc);
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
