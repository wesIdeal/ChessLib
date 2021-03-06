﻿using System;
using System.Text;
using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.Magic;
using ChessLib.Data.Types.Enums;
using NUnit.Framework;

// ReSharper disable once CheckNamespace
namespace ChessLib.Data.Tests
{
    [TestFixture]
    public class BoardInfoTests
    {
        [SetUp]
        public void Setup()
        {
            _biScandi = new BoardInfo(FENScandi);
        }

        private const string FENScandi = "rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2";
        private const string FENQueenAttacksd4 = "8/1k6/8/3q4/3P4/8/6K1/8 w - - 0 2";
        private const string FENQueenIsBlockedFromAttackingSquared4 = "8/1k6/3q4/3P4/3P4/8/6K1/8 w - - 0 2";

        private BoardInfo _biScandi;

        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
        }

        private const string InitialBoard = FENHelpers.FENInitial;
        private const string After1E4 = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";
        private const string After1E4C5 = "rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2";
        private const string InitialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        [TestFixture]
        private class CastlingValidation
        {
            /// <summary>
            ///     Tests that availability is unset appropriately, depending on color and piece moved.
            /// </summary>
            [TestFixture(Description =
                "Tests that availability is unset appropriately, depending on color and piece moved.")]
            private class AvailabilityUnset
            {
                [SetUp]
                public void Setup()
                {
                    _bi = new BoardInfo(CastlingFen);
                }

                private BoardInfo _bi;
                private const string CastlingFen = "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1";

                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetBlackKingside_When_h8RookMoves()
                {
                    var move = MoveHelpers.GenerateMove(63, 62);
                    var expected = CastlingAvailability.BlackQueenside | CastlingAvailability.WhiteKingside |
                                   CastlingAvailability.WhiteQueenside;
                    _bi.CastlingAvailability = BoardHelpers.GetCastlingAvailabilityPostMove(_bi, move, Piece.Rook);
                    Assert.AreEqual(expected, _bi.CastlingAvailability,
                        "Expected castling availability to equal qKQ after h8 Rook moves.");
                }


                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetBlackQueenside_When_a8RookMoves()
                {
                    var move = MoveHelpers.GenerateMove(56, 57);
                    var expected = CastlingAvailability.BlackKingside | CastlingAvailability.WhiteKingside |
                                   CastlingAvailability.WhiteQueenside;
                    _bi.CastlingAvailability = BoardHelpers.GetCastlingAvailabilityPostMove(_bi, move, Piece.Rook);
                    Assert.AreEqual(expected, _bi.CastlingAvailability,
                        "Expected castling availability to equal kKQ after a8 Rook moves.");
                }

                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetBoth_WhenBlackKingMoves()
                {
                    var move = MoveHelpers.GenerateMove(60, 61);
                    var expected = CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside;
                    _bi.CastlingAvailability = BoardHelpers.GetCastlingAvailabilityPostMove(_bi, move, Piece.King);
                    Assert.AreEqual(expected, _bi.CastlingAvailability,
                        "Expected castling availability to equal KQ after Black King moves.");
                }

                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetBoth_WhenWhiteKingMoves()
                {
                    var move = MoveHelpers.GenerateMove(4, 5);
                    var expected = CastlingAvailability.BlackQueenside | CastlingAvailability.BlackKingside;
                    _bi.CastlingAvailability = BoardHelpers.GetCastlingAvailabilityPostMove(_bi, move, Piece.King);
                    Assert.AreEqual(expected, _bi.CastlingAvailability,
                        "Expected castling availability to equal kq after White King moves.");
                }

                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetWhiteKingside_When_h1RookMoves()
                {
                    var move = MoveHelpers.GenerateMove(7, 6);
                    var expected = CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside |
                                   CastlingAvailability.WhiteQueenside;
                    _bi.CastlingAvailability = BoardHelpers.GetCastlingAvailabilityPostMove(_bi, move, Piece.Rook);
                    Assert.AreEqual(expected, _bi.CastlingAvailability,
                        "Expected castling availability to equal kqQ after h1 Rook moves.");
                }

                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetWhiteQueenside_When_a1RookMoves()
                {
                    var move = MoveHelpers.GenerateMove(0, 1);
                    var expected = CastlingAvailability.BlackQueenside | CastlingAvailability.BlackKingside |
                                   CastlingAvailability.WhiteKingside;
                    _bi.CastlingAvailability = BoardHelpers.GetCastlingAvailabilityPostMove(_bi, move, Piece.Rook);
                    Assert.AreEqual(expected, _bi.CastlingAvailability,
                        "Expected castling availability to equal kqK after a1 Rook moves.");
                }
            }
        }


        [TestCase("rnbqkbnr/pp1ppppp/8/1Bp5/4P3/8/PPPP1PPP/RNBQK1NR b KQkq - 1 2", 0x8000000000000ul)]
        [TestCase("rnbqkbnr/pp1ppppp/8/2p5/B3P3/8/PPPP1PPP/RNBQK1NR b KQkq - 1 2", 0x8000000000000ul)]
        [TestCase("rnbqk1nr/pp1pbppp/2p5/8/B7/8/PPPPQPPP/RNB1K1NR b KQkq - 1 2", 0x10000000000000ul)]
        [TestCase("rnbqk1nr/pp1pb1pp/2p1p3/8/B7/8/PPPPQPPP/RNB1K1NR b KQkq - 1 2", 0ul)]
        [TestCase("4k3/3p4/2p5/1B6/8/8/6K1/8 b - - 0 1", 0ul)]
        [TestCase("rnbqk1nr/pp1pb1pp/5p2/8/B7/2p5/PPPPQPPP/RNB1K1NR b KQkq - 1 2", 0x18000000000000ul)]
        public static void GetPinnedPieces_ShouldReturnValueOfPinnedPiece(string fen, ulong expected)
        {
            var bi = new BoardInfo(fen);
            var actual = bi.GetAbsolutePins();
            Assert.AreEqual(expected, actual,
                "Method did not determine piece was pinned.");
        }


        private readonly StringBuilder sb = new StringBuilder();

        [TestCase("5r2/6Pk/1R6/7P/6K1/8/8/8 w - - 0 62", 54, 61, PromotionPiece.Queen, MoveType.Promotion)]
        [TestCase("6K1/4k1P1/8/7q/8/8/8/8 b - - 9 56", 52, 60)]
        [TestCase("6K1/4k1P1/8/6q1/8/8/8/8 b - - 9 56", 38, 39)]
        public static void IsStalemateAfterMove(string fen, int f, int t, PromotionPiece p = PromotionPiece.Knight,
            MoveType type = MoveType.Normal)
        {
            var board = new MoveTraversalService(fen);
            var move = MoveHelpers.GenerateMove((ushort) f, (ushort) t, type, p);
            var result = board.ApplyMove(move);
            Assert.AreEqual(true, board.Board.IsStalemate());
        }

        [TestCase("5Q2/7k/1R6/7P/6K1/8/8/8 b - - 0 62", true)]
        [TestCase("4k1K1/6P1/8/7q/8/8/8/8 w - - 10 57", true)]
        [TestCase("6K1/4k1P1/8/7q/8/8/8/8 w - - 10 57", true)]
        [TestCase("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", false)]
        [TestCase("rnbqkbnr/pppppppp/8/8/3P4/8/PPP1PPPP/RNBQKBNR b KQkq d3 0 1", false)]
        [TestCase("8/8/8/8/8/8/5Qk1/4K3 b - - 0 1", false)]
        [TestCase("8/8/8/8/8/8/5QkQ/4K3 b - - 0 1", false)]
        [TestCase("8/8/8/8/3b4/8/3Q2k1/4K3 b - - 0 1", false)]
        public void IsStalemate(string fen, bool isStalematedPosition)
        {
            var board = new BoardInfo(fen);
            Assert.AreEqual(isStalematedPosition, board.IsStalemate());
            Console.WriteLine(sb.ToString());
        }

        [TestCase("8/8/8/8/8/8/5Qk1/4K3 b - - 0 1", Color.Black, true)]
        [TestCase("8/8/8/8/8/8/5QkQ/4K3 b - - 0 1", Color.Black, false)]
        [TestCase("8/8/8/8/3b4/8/3Q2k1/4K3 b - - 0 1", Color.Black, true)]
        public static void CanKingMoveToAnotherSquare(string fen, Color c, bool expectedResult)
        {
            var pieces = new BoardInfo(fen);
            var kingIndex = pieces.ActivePlayerKingIndex;
            var result = pieces.CanPieceMove(kingIndex);
            Assert.AreEqual(expectedResult, result);
        }

        [TestCase("8/8/8/8/3bB2n/6Q1/6k1/4K3 b - - 0 1", 1)]
        [TestCase("8/8/8/8/3b1B1n/6Q1/6k1/4K3 b - - 0 1", 1)]
        [TestCase("8/8/8/8/3b4/5N1Q/6k1/4K3 b - - 0 1", 1)]
        [TestCase("8/8/8/8/3bB2n/8/3Q2k1/4K3 b - - 0 1", 3)]
        [TestCase("8/8/8/8/3b2B1/5N1Q/6k1/4K3 b - - 0 1", 0)]
        public static void CanEvadeThroughBlockOrCapture_CaptureChecker(string fen, int expectedMoveCount)
        {
            var bi = new BoardInfo(fen);
            var actual = bi.GetEvasions();
            Assert.AreEqual(expectedMoveCount, actual.Length);
        }

        [TestCase("8/8/8/8/3b2B1/5N1Q/6k1/4K3 b - - 0 1")]
        [TestCase("3qk3/5Q1p/8/p1p1N3/Pp2bP1P/1P1r4/8/4RnK1 b - - 6 38")]
        [TestCase("7R/pp4p1/2p3Bk/5P2/7P/8/PP4p1/4K3 b - - 1 55")]
        [TestCase("4R3/2p3pk/pp3p2/5n1p/2P2P1P/P5r1/1P4q1/3QR2K w - - 6 41")]
        public static void GetEvasions_ReturnsNoMovesWhenMate(string fen)
        {
            var bi = new BoardInfo(fen);
            Assert.AreEqual(0, bi.GetEvasions().Length);
        }


        [Test(Description = "Test Castling Availability Retrieval")]
        public static void GetCastlingAvailabilityString()
        {
            var bi = new BoardInfo(InitialFEN);

            var expected = "KQkq";
            var stm = bi.GetFENCastlingAvailabilityString();
            Assert.AreEqual(expected, stm);
        }

        [Test(Description = "Test En Passant Retrieval to return a string square representation or '-'")]
        public static void GetEnPassantString_ShouldReturnEPRepresentation()
        {
            var bi = new BoardInfo(InitialFEN);

            var expected = "-";
            var stm = bi.GetFENEnPassantString();
            Assert.AreEqual(expected, stm);
            bi = new BoardInfo("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1");
            expected = "e3";
            Assert.AreEqual(expected, bi.GetFENEnPassantString());
        }

        [Test(Description = "Tests halfmove clock to string representation")]
        public static void GetHalfMoveClockString_ShouldReturnCorrectValue()
        {
            var bi = new BoardInfo(InitialFEN);
            var expected = "0";
            Assert.AreEqual(expected, bi.GetFENHalfMoveClockString());

            bi = new BoardInfo("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 30 1");
            expected = "30";
            Assert.AreEqual(expected, bi.GetFENHalfMoveClockString());
        }

        [Test(Description = "Tests move counter to string representation")]
        public static void GetMoveCountString_ShouldReturnCorrectValue()
        {
            var bi = new BoardInfo(InitialFEN);
            var expected = "1";
            Assert.AreEqual(expected, bi.GetFENMoveCounterString());

            bi = new BoardInfo("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 30 31");
            expected = "31";
            Assert.AreEqual(expected, bi.GetFENMoveCounterString());
        }

        [Test(Description = "Test piece section retrieval")]
        public static void GetPiecePlacement_ShouldReturnCorrectString()
        {
            var bi = new BoardInfo();
            var expected = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
            var piecePlacementActual = bi.GetFENPiecePlacement();
            Assert.AreEqual(expected, piecePlacementActual);
        }

        [Test(Description = "Test side-to-move retrieval")]
        public static void GetSideToMoveChar_ShouldReturnCorrectString()
        {
            var bi = new BoardInfo(InitialFEN);
            var expected = "w";
            var stm = bi.GetFENSideToMoveStrRepresentation();
            Assert.AreEqual(expected, stm);
        }


        [Test]
        public static void SetEnPassantFlag_ShouldSetFlagToEnPassantCaptureSquare_WhenPawnsMove2SquaresForward()
        {
            for (ushort i = 8; i < 16; i++)
            {
                var bi = new MoveTraversalService(InitialFEN);
                var move = MoveHelpers.GenerateMove(i, (ushort) (i + 16));
                bi.ApplyMove(move);
                Assert.AreEqual(i + 8, bi.Board.EnPassantSquare);
            }
        }


        [Test]
        public static void Should_Set_Board_After_1e4()
        {
            var whitePawns = 0x1000ef00;
            var blackPawns = 0xff000000000000;
            var whiteRooks = 0x81;
            var blackRooks = 0x8100000000000000;
            var whiteKnights = 0x42;
            var blackKnights = 0x4200000000000000;
            var whiteBishops = 0x24;
            var blackBishops = 0x2400000000000000;
            var whiteQueen = 0x8;
            var blackQueen = 0x800000000000000;
            var whiteKing = 0x10;
            var blackKing = 0x1000000000000000;
            var rv = new BoardInfo(After1E4);

            Assert.AreEqual(whitePawns, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Pawn]);
            Assert.AreEqual(blackPawns, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Pawn]);

            Assert.AreEqual(whiteRooks, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Rook]);
            Assert.AreEqual(blackRooks, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Rook]);

            Assert.AreEqual(whiteKnights, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Knight]);
            Assert.AreEqual(blackKnights, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Knight]);

            Assert.AreEqual(whiteBishops, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Bishop]);
            Assert.AreEqual(blackBishops, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Bishop]);

            Assert.AreEqual(whiteQueen, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Queen]);
            Assert.AreEqual(blackQueen, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Queen]);

            Assert.AreEqual(whiteKing, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.King]);
            Assert.AreEqual(blackKing, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.King]);
        }

        [Test]
        public static void Should_Set_Board_After_1e4_c5()
        {
            var whitePawns = 0x1000ef00;
            var blackPawns = 0xfb000400000000;
            var whiteRooks = 0x81;
            var blackRooks = 0x8100000000000000;
            var whiteKnights = 0x42;
            var blackKnights = 0x4200000000000000;
            var whiteBishops = 0x24;
            var blackBishops = 0x2400000000000000;
            var whiteQueen = 0x8;
            var blackQueen = 0x800000000000000;
            var whiteKing = 0x10;
            var blackKing = 0x1000000000000000;
            var rv = new BoardInfo(After1E4C5);

            Assert.AreEqual(whitePawns, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Pawn]);
            Assert.AreEqual(blackPawns, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Pawn]);

            Assert.AreEqual(whiteRooks, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Rook]);
            Assert.AreEqual(blackRooks, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Rook]);

            Assert.AreEqual(whiteKnights, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Knight]);
            Assert.AreEqual(blackKnights, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Knight]);

            Assert.AreEqual(whiteBishops, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Bishop]);
            Assert.AreEqual(blackBishops, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Bishop]);

            Assert.AreEqual(whiteQueen, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Queen]);
            Assert.AreEqual(blackQueen, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Queen]);

            Assert.AreEqual(whiteKing, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.King]);
            Assert.AreEqual(blackKing, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.King]);
        }

        [Test]
        public static void Should_Set_Initial_Board()
        {
            var whitePawns = 0xff00;
            var blackPawns = 0xff000000000000;
            var whiteRooks = 0x81;
            var blackRooks = 0x8100000000000000;
            var whiteKnights = 0x42;
            var blackKnights = 0x4200000000000000;
            var whiteBishops = 0x24;
            var blackBishops = 0x2400000000000000;
            var whiteQueen = 0x8;
            var blackQueen = 0x800000000000000;
            var whiteKing = 0x10;
            var blackKing = 0x1000000000000000;
            var rv = new BoardInfo(InitialBoard);
            Assert.AreEqual(whitePawns, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Pawn]);
            Assert.AreEqual(blackPawns, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Pawn]);

            Assert.AreEqual(whiteRooks, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Rook]);
            Assert.AreEqual(blackRooks, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Rook]);

            Assert.AreEqual(whiteKnights, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Knight]);
            Assert.AreEqual(blackKnights, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Knight]);

            Assert.AreEqual(whiteBishops, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Bishop]);
            Assert.AreEqual(blackBishops, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Bishop]);

            Assert.AreEqual(whiteQueen, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.Queen]);
            Assert.AreEqual(blackQueen, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.Queen]);

            Assert.AreEqual(whiteKing, rv.GetPiecePlacement()[BoardHelpers.WHITE][(int) Piece.King]);
            Assert.AreEqual(blackKing, rv.GetPiecePlacement()[BoardHelpers.BLACK][(int) Piece.King]);
        }


        [Test(Description = "ToFEN() should return the FEN of the current board's state")]
        public static void ToFEN_ShouldReturnCurrentBoardState()
        {
            const string initialFEN = InitialFEN;
            var bi = new BoardInfo(initialFEN);
            var actual = bi.ToFEN();
            Assert.AreEqual(initialFEN, actual);

            const string closedRuyFEN = "r1bqkbnr/1ppp1ppp/p1n5/4p3/B3P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 4";
            bi = new BoardInfo(closedRuyFEN);
            actual = bi.ToFEN();
            Assert.AreEqual(closedRuyFEN, actual);
        }
    }
}