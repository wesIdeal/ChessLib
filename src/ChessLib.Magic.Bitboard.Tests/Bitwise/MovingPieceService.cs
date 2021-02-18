using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.MagicBitboard.Bitwise.Tests
{
    [TestFixture]
    public class MovingPieceService : Bitwise.MovingPieceService
    {
        [Test]
        public void TestWestExtent()
        {
            for (ushort i = 0; i < 64; i += 8)
            {
                var rankExtent = i + 7;
                var expectedExtent = 1 << i;
                for (ushort square = i; square <= rankExtent; square++)
                {
                    var actual = GetWestExtent(square);
                    Assert.AreEqual(expectedExtent, actual);
                }
            }
        }

        [Test]
        public void TestEastExtent()
        {
            for (ushort rank = 0; rank < 7; rank++)
            {
                ushort startingSquare = (ushort)(rank * 8);
                var extentIndex = startingSquare + 7;
                ulong expectedExtent = (ulong)1 << extentIndex;
                Console.WriteLine($"Calculated extent {expectedExtent} from index {extentIndex}");
                for (ushort square = startingSquare; square <= extentIndex; square++)
                {
                    var actual = GetEastExtent(square);
                    Assert.AreEqual(expectedExtent, actual,
                        $"When testing east extent for {square} the most east square is {extentIndex}, the extent was expected to be {expectedExtent}, but was {actual}.");
                }
            }
        }

        [Test]
        public void TesNEDiagonal_Fromh3()
        {
            var expected = new ushort[] { 5, 14, 23 };
            var actual = GetNorthEastDiagonal(23);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TesNWDiagonal_Fromh3()
        {
            var expected = new ushort[] { 23, 30, 37, 44, 51, 58 };
            var actual = GetNorthWestDiagonal(23);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestNEDiagonal_Froma3()
        {
            var expected = new ushort[] { 24, 33, 42, 51, 60 };
            var actual = GetNorthEastDiagonal(24);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TesNWDiagonal_Froma3()
        {
            var expected = new ushort[] { 2, 9, 16 };
            var actual = GetNorthWestDiagonal(2);
            Assert.AreEqual(expected, actual);
        }

    }
}
