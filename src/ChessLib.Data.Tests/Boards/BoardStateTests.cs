using ChessLib.Data.Boards;
using ChessLib.Data.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Data.Boards.Tests
{
    [TestFixture]
    public class BoardStateTests
    {
        [TestCase(255, 47, Piece.Queen, CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside |
                                        CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside, GameState.None)]
        [TestCase(0, null, null, CastlingAvailability.NoCastlingAvailable, GameState.StaleMate)]
        [TestCase(21, null, Piece.Pawn, CastlingAvailability.WhiteKingside, GameState.Checkmate)]
        public void TestBoardStateStorage(int hm, int? ep, Piece? capPiece, CastlingAvailability ca, GameState gameState)
        {
            var boardState = new BoardState((ushort)hm, (ushort?)ep, capPiece, ca, gameState);
            Assert.AreEqual(hm, boardState.GetHalfmoveClock());
            Assert.AreEqual(ep, boardState.GetEnPassantSquare());
            Assert.AreEqual(capPiece, boardState.GetPieceCaptured());
            Assert.AreEqual(ca, boardState.GetCastlingAvailability());
        }
    }
}