using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace MicroElements.Functional.Benchmarks
{
    [MemoryDiagnoser]
    [Config(typeof(CustomConfig))]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class SpanVsString
    {
        [Benchmark]
        public int IterateString()
        {
            int count = 0;
            var text = "User Alex created in 45 ms.";
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    count++;
                }
            }

            return count;
        }

        [Benchmark]
        public int IterateSpan()
        {
            int count = 0;
            var text = "User Alex created in 45 ms.".AsSpan();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    count++;
                }
            }

            return count;
        }
    }
}
