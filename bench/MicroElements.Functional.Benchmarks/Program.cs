using BenchmarkDotNet.Running;

namespace MicroElements.Functional.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<StructVsClass>();
        }
    }
}
