using ChessLib.Types.Enums;
using NUnit.Framework;

namespace ChessLib.MagicBitboard.Tests.MovingPieces
{
    [TestFixture]
    public class Knight
    {
        [Test]
        public void TestMovesFromMiddle()
        {
            ushort squareIndex = 28;
            ulong expectedValue = 0x284400442800;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.Knight, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }

        [Test]
        public void TestFromSouth()
        {
            ushort squareIndex = 3;
            ulong expectedValue = 0x142200;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.Knight, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }

        [Test]
        public void TestFromSouthWest()
        {
            ushort squareIndex = 0;
            ulong expectedValue = 0x20400;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.Knight, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }


        [Test]
        public void TestFromWest()
        {
            ushort squareIndex = 24;
            ulong expectedValue = 0x20400040200;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.Knight, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }


        [Test]
        public void TestFromNorthWest()
        {
            ushort squareIndex = 56;
            ulong expectedValue = 0x4020000000000;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.Knight, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }

        [Test]
        public void TestFromNorth()
        {
            ushort squareIndex = 59;
            ulong expectedValue = 0x22140000000000;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.Knight, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }


        [Test]
        public void TestFromNorthEast()
        {
            ushort squareIndex = 63;
            ulong expectedValue = 0x20400000000000;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.Knight, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }


        [Test]
        public void TestFromEast()
        {
            ushort squareIndex = 39;
            ulong expectedValue = 0x40200020400000;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.Knight, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }

        [Test]
        public void TestFromSouthEast()
        {
            ushort squareIndex = 7;
            ulong expectedValue = 0x402000;
            var actual = Bitboard.Instance.GetPseudoLegalMoves(squareIndex, Piece.Knight, Color.Black, 0);
            Assert.AreEqual(expectedValue, actual);
        }
    }
}