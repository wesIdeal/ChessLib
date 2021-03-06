using ChessLib.Data.Boards;
using ChessLib.Data.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Data.Tests.Boards
{
    [TestFixture]
    public class BoardStateTests
    {
        [TestCase(255, 47, Piece.Queen, CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside |
                                        CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside, GameState.None, Color.Black, 3)]
        [TestCase(0, null, null, CastlingAvailability.NoCastlingAvailable, GameState.StaleMate, Color.White, 4)]
        [TestCase(21, null, Piece.Pawn, CastlingAvailability.WhiteKingside, GameState.Checkmate, Color.Black, 10)]
        public void TestBoardStateStorage(int hm, int? ep, Piece? capPiece, CastlingAvailability ca, GameState gameState, Color activePlayer, int fullMoveCount)
        {
            var boardState = new BoardState((ushort)hm, (ushort?)ep, capPiece, ca, gameState, activePlayer, (ushort)fullMoveCount );
            Assert.AreEqual(hm, boardState.HalfMoveClock);
            Assert.AreEqual(ep, boardState.EnPassantSquare);
            Assert.AreEqual(capPiece, boardState.PieceCaptured);
            Assert.AreEqual(ca, boardState.CastlingAvailability);
            Assert.AreEqual(gameState, boardState.GameState);
            Assert.AreEqual(activePlayer, boardState.ActivePlayer);
            Assert.AreEqual(fullMoveCount, boardState.FullMoveCounter);
        }
    }
}