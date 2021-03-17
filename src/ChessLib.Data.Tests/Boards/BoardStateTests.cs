using System.Collections;
using System.Collections.Generic;
using ChessLib.Data.Boards;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Interfaces;
using NUnit.Framework;

namespace ChessLib.Data.Tests.Boards
{
    public class BoardStateTestCase : IBoardState
    {


        public uint BoardStateStorage {get; set;}
        public GameState GameState {get; set;}
        public CastlingAvailability CastlingAvailability {get; set;}
        public Piece? PieceCaptured {get; set;}
        public ushort? EnPassantSquare { get; set; }
        public ushort HalfMoveClock {get; set;}
        public uint FullMoveCounter {get; set;}
        public Color ActivePlayer {get; set;}

        public IBoardState GetBoardState()
        {
            return new BoardState(HalfMoveClock, EnPassantSquare, PieceCaptured, CastlingAvailability, GameState,
                ActivePlayer, FullMoveCounter);
        }
        public bool Equals(IBoardState other)
        {
            if (other == null)
            {
                return false;
            }
            return this.BoardStateStorage.Equals(other.BoardStateStorage);
        }
        public BoardStateTestCase(ushort halfMoveClock, ushort? enPassantSquare, Piece? pieceCaptured,
            CastlingAvailability castlingAvailability, GameState gameState, Color activePlayer, uint fullMoveCounter)
        {
            ActivePlayer = activePlayer;
            CastlingAvailability = castlingAvailability;
            EnPassantSquare = enPassantSquare;
            FullMoveCounter = fullMoveCounter;
            GameState = gameState;
            HalfMoveClock = halfMoveClock;
            PieceCaptured = pieceCaptured;
        }
    }
    [TestFixture(Category = "Board State")]
    public class BoardStateTests
    {
        [TestCaseSource(nameof(GetBoardStateTestCases))]
        public void TestBoardStateStorage(BoardStateTestCase testCase)
        {
            var boardState = testCase.GetBoardState();
            Assert.AreEqual(testCase.HalfMoveClock, boardState.HalfMoveClock);
            Assert.AreEqual(testCase.EnPassantSquare, boardState.EnPassantSquare);
            Assert.AreEqual(testCase.PieceCaptured, boardState.PieceCaptured);
            Assert.AreEqual(testCase.CastlingAvailability, boardState.CastlingAvailability);
            Assert.AreEqual(testCase.GameState, boardState.GameState);
            Assert.AreEqual(testCase.ActivePlayer, boardState.ActivePlayer);
            Assert.AreEqual(testCase.FullMoveCounter, boardState.FullMoveCounter);
        }

       
        public static IEnumerable<BoardStateTestCase> GetBoardStateTestCases()
        {
            yield return new BoardStateTestCase(255, 47, Piece.Queen,
                CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside |
                CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside, GameState.None, Color.Black,
                3);
            yield return new BoardStateTestCase(0, null, null, CastlingAvailability.NoCastlingAvailable,
                GameState.StaleMate, Color.White, 4);
            yield return new BoardStateTestCase(21, null, Piece.Pawn, CastlingAvailability.WhiteKingside,
                GameState.Checkmate, Color.Black, 10);
        }
    }
}