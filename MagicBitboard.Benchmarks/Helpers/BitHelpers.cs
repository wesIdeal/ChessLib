using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using MagicBitboard.Helpers;
using System.Linq;

namespace MagicBitboard.Benchmarks.Helpers
{
    public class BitHelpersBenchmark
    {
        ulong[] data;

        public ulong[] Data => data;

        [ParamsSource(nameof(Data))]
        public ulong dataForTest;

        public BitHelpersBenchmark()
        {
            data = PieceAttackPatternHelper.BishopMoveMask.MoveBoard.Take(8).ToArray();//.Concat(PieceAttackPatternHelper.RookMoveMask.MoveBoard).ToArray();
        }

        [Benchmark]
        public int[] GetSetBits_UsingArray() => BitHelpers.GetSetBits2(dataForTest);

        [Benchmark]
        public int[] GetSetBits_UsingList() => BitHelpers.GetSetBits2(dataForTest);
    }
}
