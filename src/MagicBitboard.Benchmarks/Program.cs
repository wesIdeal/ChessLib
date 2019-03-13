using BenchmarkDotNet.Running;
using MagicBitboard.Benchmarks.Helpers;
using System;

namespace MagicBitboard.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BitHelpersBenchmark>();
        }
    }
}
