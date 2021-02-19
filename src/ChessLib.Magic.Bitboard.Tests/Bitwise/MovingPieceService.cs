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
