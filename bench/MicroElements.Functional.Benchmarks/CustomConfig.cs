using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;

namespace MicroElements.Functional.Benchmarks
{
    class CustomConfig : ManualConfig
    {
        public CustomConfig()
        {
            Add(MemoryDiagnoser.Default);
        }
    }
}