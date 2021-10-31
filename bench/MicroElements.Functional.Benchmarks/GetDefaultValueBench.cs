using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using MicroElements.Reflection.CodeCompiler;
using MicroElements.Reflection.TypeExtensions;

namespace MicroElements.Functional.Benchmarks
{
    [MemoryDiagnoser]
    [Config(typeof(CustomConfig))]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class GetDefaultValueBench
    {
        [Benchmark]
        public void GetDefaultValue()
        {
            TypeExtensions.GetDefaultValue(typeof(int));
            TypeExtensions.GetDefaultValue(typeof(object));
            TypeExtensions.GetDefaultValue(typeof(Nullable<int>));
        }

        [Benchmark]
        public void GetDefaultValueCompiled()
        {
            TypeExtensions2.GetDefaultValueCompiled(typeof(int));
            TypeExtensions2.GetDefaultValueCompiled(typeof(object));
            TypeExtensions2.GetDefaultValueCompiled(typeof(Nullable<int>));
        }
    }

    public static class TypeExtensions2
    {
        public static object? GetDefaultValueCompiled(this Type type)
        {
            Func<Unit, object> func = CodeCompiler.CachedCompiledFunc<Unit, object>(type, "GetDefaultValue", GetDefaultValueInternal<CodeCompiler.GenericType>);
            return func(Unit.Default);
        }

        internal static object GetDefaultValueInternal<T>(Unit unit) => default(T);
    }
}
