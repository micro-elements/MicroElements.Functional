using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

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
            TypeExtensions.GetDefaultValueCompiled(typeof(int));
            TypeExtensions.GetDefaultValueCompiled(typeof(object));
            TypeExtensions.GetDefaultValueCompiled(typeof(Nullable<int>));
        }
    }
}
