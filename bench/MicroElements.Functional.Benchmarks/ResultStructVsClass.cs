using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace MicroElements.Functional.Benchmarks
{

    public readonly struct ResultStruct
    {
        public object Value { get; }

        public bool IsSuccess { get; }

        public string Message { get; }

        public ResultStruct(object value, bool isSuccess, string message)
        {
            Value = value;
            IsSuccess = isSuccess;
            Message = message;
        }
    }


    public class ResultClass
    {
        public object Value { get; }

        public bool IsSuccess { get; }

        public string Message { get; }

        public ResultClass(object value, bool isSuccess, string message)
        {
            Value = value;
            IsSuccess = isSuccess;
            Message = message;
        }
    }


    [MemoryDiagnoser]
    [Config(typeof(CustomConfig))]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class StructVsClass
    {
        ResultStruct ParseAsStruct()
        {
            return new ResultStruct("value", true, Guid.NewGuid().ToString());
        }

        ResultClass ParseAsClass()
        {
            return new ResultClass("value", true, Guid.NewGuid().ToString());
        }

        void UseResult(string text)
        {

        }

        [Benchmark]
        public void ParseAsStructBench()
        {
            ResultStruct result = ParseAsStruct();
            UseResult(result.Message);
        }

        [Benchmark]
        public void ParseAsClasstBench()
        {
            ResultClass result = ParseAsClass();
            UseResult(result.Message);
        }
    }
}
