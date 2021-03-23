#region

using ChessLib.Types.Enums;
using NUnit.Framework;

#endregion

namespace ChessLib.MagicBitboard.Tests.MovingPieces
{
    [TestFixture]
    public class King
    {
        [Test]
        public void TestMovesFromMiddle()
        {
            ushort squareIndex = 28;
            ulong expectedValue = 0x3828380000;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.King, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }

        [Test]
        public void TestFromSouth()
        {
            ushort squareIndex = 3;
            ulong expectedValue = 0x1c14;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.King, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }

        [Test]
        public void TestFromSouthWest()
        {
            ushort squareIndex = 0;
            ulong expectedValue = 0x302;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.King, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }


        [Test]
        public void TestFromWest()
        {
            ushort squareIndex = 24;
            ulong expectedValue = 0x302030000;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.King, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }


        [Test]
        public void TestFromNorthWest()
        {
            ushort squareIndex = 56;
            ulong expectedValue = 0x203000000000000;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.King, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }

        [Test]
        public void TestFromNorth()
        {
            ushort squareIndex = 59;
            ulong expectedValue = 0x141c000000000000;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.King, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }


        [Test]
        public void TestFromNorthEast()
        {
            ushort squareIndex = 63;
            ulong expectedValue = 0x40c0000000000000;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.King, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }


        [Test]
        public void TestFromEast()
        {
            ushort squareIndex = 39;
            ulong expectedValue = 0xc040c0000000;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.King, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }

        [Test]
        public void TestFromSouthEast()
        {
            ushort squareIndex = 7;
            ulong expectedValue = 0xc040;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.King, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }
    }
}