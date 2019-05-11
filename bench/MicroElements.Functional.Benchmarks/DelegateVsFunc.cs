using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace MicroElements.Functional.Benchmarks
{
    public delegate int Sum(int a, int b);

    [MemoryDiagnoser]
    [Config(typeof(CustomConfig))]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class DelegateVsFunc
    {
        private readonly Sum sumDelegate = (a, b) => a + b;
        private readonly Func<int, int, int> sumFunc = (a, b) => a + b; 

        [Benchmark]
        public int SumMethod() => SumMethodImpl(2, 3);

        [Benchmark]
        public int SumDelegate() => sumDelegate(2, 3);

        [Benchmark]
        public int SumFunc() => sumFunc(2, 3);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public int SumMethodImpl(int a, int b) => a + b;
    }
}
