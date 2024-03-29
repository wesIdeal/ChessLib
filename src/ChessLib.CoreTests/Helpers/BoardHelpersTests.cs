﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;
using NUnit.Framework;

// ReSharper disable StringLiteralTypo

namespace ChessLib.Core.Tests.Helpers
{
    [TestFixture(Category = "Board State", TestOf = typeof(BoardHelpers))]
    public class BoardStateHelpersTests
    {
    }

    [TestFixture(Category = "Board Helpers", TestOf = typeof(BoardHelpers))]
    public class BoardHelpersTests
    {


        [TestCase(Color.Black, Color.White)]
        [TestCase(Color.White, Color.Black)]
        public void ToggleTest(Color from, Color to)
        {
            Assert.AreEqual(from.Toggle(), to);
        }

        [TestCase("a1", "a8", 0x1010101010100ul)]
        [TestCase("a8", "h8", 0x7e00000000000000ul)]
        [TestCase("h8", "a8", 0x7e00000000000000ul)]
        [TestCase("a1", "h8", 0x40201008040200ul)]
        [TestCase("a1", "a2", 0ul)]
        [TestCase("a1", "c2", 0ul)]
        [TestCase("e1", "h4", 0x402000ul)]
        public void InBetweenTest(string strFrom, string strTo, ulong expected)
        {
            var from = strFrom.ToBoardIndex();
            var to = strTo.ToBoardIndex();
            Assert.IsNotNull(from);
            Assert.IsNotNull(to);
            var actual = BoardHelpers.InBetween(from, to);
            Assert.AreEqual(expected, actual, $"{strFrom}->{strTo}");
        }


        [TestCaseSource(nameof(GetOccupancyTestCases))]
        public void OccupancyTest(TestCase<ulong, ulong[][]> testCase)
        {
            var color = (Color?)testCase.AdditionalInputs.FirstOrDefault();
            var piece = (Piece?)testCase.AdditionalInputs?.Skip(1).FirstOrDefault();
            var actual = testCase.TestMethodInputValue.Occupancy(color, piece);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.Description);
        }

        protected static IEnumerable<TestCase<ulong, ulong[][]>> GetOccupancyTestCases()
        {
            yield return new TestCase<ulong, ulong[][]>(0xffff00000000ffff, BoardHelpers.InitialBoard, "All Pieces");
            yield return new TestCase<ulong, ulong[][]>(0xfffful, BoardHelpers.InitialBoard, "All Pieces for White",
                Color.White);
            yield return new TestCase<ulong, ulong[][]>(0xffff000000000000, BoardHelpers.InitialBoard,
                "All Pieces for Black", Color.Black);
            yield return new TestCase<ulong, ulong[][]>(0xff00ul, BoardHelpers.InitialBoard, "All Pawns for White",
                Color.White, Piece.Pawn);
            yield return new TestCase<ulong, ulong[][]>(0x00ff000000000000, BoardHelpers.InitialBoard,
                "All Pawns for Black", Color.Black, Piece.Pawn);
        }

        [TestCaseSource(nameof(GetPieceAtIndexTestCases))]
        public void GetPieceAtIndexTest(TestCase<Piece?, ushort> testCase)
        {
            var actual = BoardHelpers.GetPieceAtIndex(BoardHelpers.InitialBoard, testCase.TestMethodInputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.Description);
        }

        protected static IEnumerable<TestCase<Piece?, ushort>> GetPieceAtIndexTestCases()
        {
            yield return new TestCase<Piece?, ushort>(Piece.Pawn, 12, "White Pawn on e4");
            yield return new TestCase<Piece?, ushort>(Piece.Knight, 57, "Black Knight on b8");
            yield return new TestCase<Piece?, ushort>(Piece.Bishop, 5, "White Bishop on f1");
            yield return new TestCase<Piece?, ushort>(Piece.Rook, 63, "Black Rook on h8");
            yield return new TestCase<Piece?, ushort>(Piece.Queen, 3, "White Queen on d1");
            yield return new TestCase<Piece?, ushort>(Piece.King, 60, "Black King on e8");
            yield return new TestCase<Piece?, ushort>(null, 26, "[Empty Square]");
        }

        [TestCaseSource(nameof(GetPieceOfColorAtIndexTestCases))]
        public void GetPieceOfColorAtIndexTest(TestCase<PieceOfColor?, ushort> testCase)
        {
            var actual = BoardHelpers.InitialBoard.GetPieceOfColorAtIndex(testCase.TestMethodInputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.Description);
        }

        protected static IEnumerable<TestCase<PieceOfColor?, ushort>> GetPieceOfColorAtIndexTestCases()
        {
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor(Piece.Pawn, Color.White),
                12, "White Pawn on e4");
            yield return new TestCase<PieceOfColor?, ushort>(
                new PieceOfColor(Piece.Knight, Color.Black), 57, "Black Knight on b8");
            yield return new TestCase<PieceOfColor?, ushort>(
                new PieceOfColor(Piece.Bishop, Color.White), 5, "White Bishop on f1");
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor(Piece.Rook, Color.Black),
                63, "Black Rook on h8");
            yield return new TestCase<PieceOfColor?, ushort>(
                new PieceOfColor(Piece.Queen, Color.White), 3, "White Queen on d1");
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor(Piece.King, Color.Black),
                60, "Black King on e8");
            yield return new TestCase<PieceOfColor?, ushort>(null, 26, "[Empty Square]");
        }


        [Test]
        public void ValidateIndexTest()
        {
            Assert.Throws<ArgumentException>(delegate { BoardHelpers.ValidateIndex(64); });
            Assert.DoesNotThrow(delegate { BoardHelpers.ValidateIndex(63); });
            Assert.DoesNotThrow(delegate { BoardHelpers.ValidateIndex(0); });
        }

        [TestCaseSource(nameof(GetCastlingAvailabilityPostMoveTestCases))]
        public void GetCastlingAvailabilityPostMoveTest(TestCase<CastlingAvailability, Board> testCase)
        {
            var occupancy = testCase.TestMethodInputValue.Occupancy;
            var currentCastlingAvailability = testCase.TestMethodInputValue.CastlingAvailability;
            var move = (Move)testCase.AdditionalInputs.Single();
            var actual = BoardHelpers.GetCastlingAvailabilityPostMove(occupancy, move, currentCastlingAvailability);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        private static readonly FenTextToBoard FenReader = new FenTextToBoard();

        protected static IEnumerable<TestCase<CastlingAvailability, Board>> GetCastlingAvailabilityPostMoveTestCases()
        {
            var allCastlingAvailable = FenReader.Translate("r3k2r/ppp5/8/8/8/8/PPP5/R3K2R w KQkq - 0 1");
            var knightsOnAFile = FenReader.Translate("n3k2r/8/8/8/8/8/PPP5/N3K2R w Kk - 0 1");
            var knightsOnHFile = FenReader.Translate("r3k2n/8/8/8/8/8/PPP5/R3K2N w Qq - 0 1");
            var bishopsOnAFile = FenReader.Translate("b3k2r/8/8/8/8/1P6/P1P5/B3K2R w Kk - 0 1");
            var bishopsOnHFile = FenReader.Translate("r3k2b/8/8/8/8/1P6/P1P5/R3K2B w Qq - 0 1");
            var queensOnAFile = FenReader.Translate("q3k2r/8/8/8/8/8/PPP5/Q3K2R w Kk - 0 1");
            var queensOnHFile = FenReader.Translate("r3k2q/8/8/8/8/8/PPP5/R3K2Q w Qq - 0 1");
            //
            // No Change in Castling Availability
            //No Piece on Square
            yield return new TestCase<CastlingAvailability, Board>(allCastlingAvailable.CastlingAvailability,
                allCastlingAvailable, "No Piece on Source- Castling not affected", MoveHelpers.GenerateMove(16, 24));

            //White Pawn
            yield return new TestCase<CastlingAvailability, Board>(allCastlingAvailable.CastlingAvailability,
                allCastlingAvailable, "White Pawn Move- Castling not affected", MoveHelpers.GenerateMove(8, 24));
            //Black Pawn
            yield return new TestCase<CastlingAvailability, Board>(allCastlingAvailable.CastlingAvailability,
                allCastlingAvailable, "White Pawn Move- Castling not affected", MoveHelpers.GenerateMove(48, 32));
            //White Knight from a1 -> b3
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.WhiteKingside | CastlingAvailability.BlackKingside,
                knightsOnAFile,
                "White Knight from a1 -> b3- Castling not affected", MoveHelpers.GenerateMove(0, 17));
            //White Knight from h1 -> g3
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackQueenside,
                knightsOnHFile,
                "White Knight from h1 -> g3- Castling not affected", MoveHelpers.GenerateMove(7, 22));
            //Black Knight from a8 -> b6
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.WhiteKingside | CastlingAvailability.BlackKingside,
                knightsOnAFile,
                "Black Knight from a8 -> b6- Castling not affected", MoveHelpers.GenerateMove(56, 41));
            //Black Knight from h8 ->g6
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackQueenside,
                knightsOnHFile,
                "Black Knight from h8 ->g6- Castling not affected", MoveHelpers.GenerateMove(63, 46));

            //White Bishop from a1 -> c3
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.WhiteKingside | CastlingAvailability.BlackKingside,
                bishopsOnAFile,
                "White Bishop from a1 -> c3- Castling not affected", MoveHelpers.GenerateMove(0, 18));
            //White Bishop from h1 -> f3
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackQueenside,
                bishopsOnHFile,
                "White Bishop from h1 -> f3- Castling not affected", MoveHelpers.GenerateMove(7, 21));

            //Black Bishop from a8 -> c6
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.WhiteKingside | CastlingAvailability.BlackKingside,
                bishopsOnAFile,
                "Black Bishop from a8 -> b6- Castling not affected", MoveHelpers.GenerateMove(56, 42));

            //Black Bishop from h8 ->f6
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackQueenside,
                bishopsOnHFile,
                "Black Bishop from h8 -> f6- Castling not affected", MoveHelpers.GenerateMove(63, 45));


            //White Queen from a1 -> c3
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.WhiteKingside | CastlingAvailability.BlackKingside,
                queensOnAFile,
                "White Queen from a1 -> c3- Castling not affected", MoveHelpers.GenerateMove(0, 18));
            //White Queen from h1 -> f3
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackQueenside,
                queensOnHFile,
                "White Queen from h1 -> f3- Castling not affected", MoveHelpers.GenerateMove(7, 21));

            //Black Queen from a8 -> c6
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.WhiteKingside | CastlingAvailability.BlackKingside,
                queensOnAFile,
                "Black Queen from a8 -> b6- Castling not affected", MoveHelpers.GenerateMove(56, 42));

            //Black Queen from h8 ->f6
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackQueenside,
                queensOnHFile,
                "Black Queen from h8 -> f6- Castling not affected", MoveHelpers.GenerateMove(63, 45));

            //Rook Moves
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside |
                CastlingAvailability.WhiteKingside,
                allCastlingAvailable, "White Rook Move- Queenside eliminated", MoveHelpers.GenerateMove(0, 2));
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside |
                CastlingAvailability.WhiteQueenside,
                allCastlingAvailable, "White Rook Move- Kingside eliminated", MoveHelpers.GenerateMove(7, 6));
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.BlackKingside | CastlingAvailability.WhiteKingside |
                CastlingAvailability.WhiteQueenside,
                allCastlingAvailable, "Black Rook Move- Queenside eliminated", MoveHelpers.GenerateMove(56, 57));
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.BlackQueenside | CastlingAvailability.WhiteKingside |
                CastlingAvailability.WhiteQueenside,
                allCastlingAvailable, "Black Rook Move- Kingside eliminated", MoveHelpers.GenerateMove(63, 62));

            //Castling Moves
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside, allCastlingAvailable,
                "White Castles Kingside - White Castling Eliminated",
                MoveHelpers.WhiteCastleKingSide);
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside, allCastlingAvailable,
                "White Castles Queenside - White Castling Eliminated",
                MoveHelpers.WhiteCastleQueenSide);
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside, allCastlingAvailable,
                "Black Castles Kingside - White Castling Eliminated",
                MoveHelpers.BlackCastleKingSide);
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside, allCastlingAvailable,
                "Black Castles Queenside - White Castling Eliminated",
                MoveHelpers.BlackCastleQueenSide);

            //Odd circumstances
            var blackRookOnA1 = FenReader.Translate("r3k2r/ppp5/8/8/8/8/PPP5/r1B1K2R w Kkq - 0 1");
            var whiteRookOnA8 = FenReader.Translate("R1b1k2r/ppp5/8/8/8/8/PPP5/RN2K2R w KQk - 0 1");
            yield return new TestCase<CastlingAvailability, Board>(blackRookOnA1.CastlingAvailability, blackRookOnA1,
                "Black Rook on a1- No Change", MoveHelpers.GenerateMove(0, 1));
            yield return new TestCase<CastlingAvailability, Board>(whiteRookOnA8.CastlingAvailability, whiteRookOnA8,
                "White Rook on a8- No Change", MoveHelpers.GenerateMove(56, 57));
        }


        [TestCaseSource(nameof(GetIsEnPassantCaptureAvailableTestCases))]
        public void IsEnPassantCaptureAvailableTest(TestCase<bool, Board> testCase)
        {
            var actual = testCase.TestMethodInputValue.IsEnPassantCaptureAvailable();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<bool, Board>> GetIsEnPassantCaptureAvailableTestCases()
        {
            yield return new TestCase<bool, Board>(false, new Board(), "Initial board");
            yield return new TestCase<bool, Board>(false,
                FenReader.Translate("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1"), "After 1. e4");
            yield return new TestCase<bool, Board>(true,
                FenReader.Translate("rnbqkbnr/pp2pppp/8/2ppP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3"), "Available on d6");
            yield return new TestCase<bool, Board>(true,
                FenReader.Translate("rnbqkbnr/pp2pppp/3P4/8/2pP4/8/PPP2PPP/RNBQKBNR b KQkq d3 0 4"), "Available on d3");
        }

        [TestCaseSource(nameof(GetEnPassantIndexTestCases))]
        public void GetEnPassantIndexTest(TestCase<ushort?, Board> testCase)
        {
            var actual =
                BoardHelpers.GetEnPassantIndex(testCase.TestMethodInputValue,
                    (IMove)testCase.AdditionalInputs.Single());
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ushort?, Board>> GetEnPassantIndexTestCases()
        {
            yield return new TestCase<ushort?, Board>(20, new Board(), "After 1. e4", MoveHelpers.GenerateMove(12, 28));
            yield return new TestCase<ushort?, Board>(44,
                FenReader.Translate("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1"), "After 1. e4 e5",
                MoveHelpers.GenerateMove(52, 36));
            yield return new TestCase<ushort?, Board>(null,
                FenReader.Translate("rnbqkbnr/pppppppp/8/8/8/8/PPPPRPPP/RNBQKBNR b KQkq - 0 1"), "After 1. Re4 from e3",
                MoveHelpers.GenerateMove(12, 28));
            yield return new TestCase<ushort?, Board>(null, new Board(), "No piece on square",
                MoveHelpers.GenerateMove(28, 29));
            yield return new TestCase<ushort?, Board>(null, new Board(), "After 1. e3",
                MoveHelpers.GenerateMove(12, 20));
            yield return new TestCase<ushort?, Board>(null,
                FenReader.Translate("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1"), "After 1.e4 e6",
                MoveHelpers.GenerateMove(52, 44));
        }

        [TestCaseSource(nameof(GetApplyMoveToBoardTestCases))]
        public void ApplyMoveToBoardTest(TestCase<Board, Board> testCase)
        {
            var expected = testCase.ExpectedValue;
            var actual =
                testCase.TestMethodInputValue.ApplyMoveToBoard((Move)testCase.AdditionalInputs.Single());
            Assert.AreEqual(expected.Fen, actual.Fen, testCase.ToString());
        }

        protected static IEnumerable<TestCase<Board, Board>> GetApplyMoveToBoardTestCases()
        {
            var boardTransitions = new[]
            {
                new Board(),
                FenReader.Translate("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1"),
                FenReader.Translate("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 2"),
                FenReader.Translate("rnbqkbnr/ppp1pppp/8/3P4/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 2"),
                FenReader.Translate("rnb1kbnr/ppp1pppp/8/3q4/8/8/PPPP1PPP/RNBQKBNR w KQkq - 0 3"),
                FenReader.Translate("rnb1kbnr/ppp1pppp/8/3q4/8/2N5/PPPP1PPP/R1BQKBNR b KQkq - 1 3"),
                FenReader.Translate("rnbqkbnr/ppp1pppp/8/8/8/2N5/PPPP1PPP/R1BQKBNR w KQkq - 2 4"),
                FenReader.Translate("rnbqkbnr/ppp1pppp/8/8/2B5/2N5/PPPP1PPP/R1BQK1NR b KQkq - 3 4"),
                FenReader.Translate("rnbqkbnr/ppp2ppp/4p3/8/2B5/2N5/PPPP1PPP/R1BQK1NR w KQkq - 0 5"),
                FenReader.Translate("rnbqkbnr/ppp2ppp/4p3/8/2B5/2N2N2/PPPP1PPP/R1BQK2R b KQkq - 1 5"),
                FenReader.Translate("rnbqkb1r/ppp2ppp/4pn2/8/2B5/2N2N2/PPPP1PPP/R1BQK2R w KQkq - 2 6"),
                FenReader.Translate("rnbqkb1r/ppp2ppp/4pn2/8/2B5/2N2N2/PPPP1PPP/R1BQ1RK1 b kq - 3 6"),
                FenReader.Translate("rnbqk2r/ppp1bppp/4pn2/8/2B5/2N2N2/PPPP1PPP/R1BQ1RK1 w kq - 4 7"),
                FenReader.Translate("rnbqk2r/ppp1bppp/4pn2/8/2BP4/2N2N2/PPP2PPP/R1BQ1RK1 b kq d3 0 7"),
                FenReader.Translate("rnbq1rk1/ppp1bppp/4pn2/8/2BP4/2N2N2/PPP2PPP/R1BQ1RK1 w - - 1 8")
            };
            var movesToApply = new[]
            {
                MoveHelpers.GenerateMove(12, 28),
                MoveHelpers.GenerateMove(51, 35),
                MoveHelpers.GenerateMove(28, 35),
                MoveHelpers.GenerateMove(59, 35),
                MoveHelpers.GenerateMove(1, 18),
                MoveHelpers.GenerateMove(35, 59),
                MoveHelpers.GenerateMove(5, 26),
                MoveHelpers.GenerateMove(52, 44),
                MoveHelpers.GenerateMove(6, 21),
                MoveHelpers.GenerateMove(62, 45),
                MoveHelpers.WhiteCastleKingSide,
                MoveHelpers.GenerateMove(61, 52),
                MoveHelpers.GenerateMove(11, 27),
                MoveHelpers.BlackCastleKingSide
            };

            for (var boardIndex = 0; boardIndex < movesToApply.Length; boardIndex++)
            {
                var startingBoard = boardTransitions[boardIndex];
                var endingBoard = boardTransitions[boardIndex + 1];
                var move = movesToApply[boardIndex];
                yield return new TestCase<Board, Board>(endingBoard, startingBoard,
                    $"{boardIndex:D2} {move}", move);
            }
        }


        [TestCaseSource(nameof(GetCheckTypeTestCases))]
        public void GetCheckTypeTest(TestCase<BoardHelpers.CheckType, Board> testCase)
        {
            var occupancy = testCase.TestMethodInputValue.Occupancy;
            var color = testCase.TestMethodInputValue.ActivePlayer;
            var actual = BoardHelpers.GetCheckType(occupancy, color, out _);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<BoardHelpers.CheckType, Board>> GetCheckTypeTestCases()
        {
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Single,
                FenReader.Translate("2r5/3r2k1/p3b3/1p3p2/2pPpP2/P1N1P3/1P4RP/2R3K1 b - - 1 34"),
                "White Rook vs Black King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Single,
                FenReader.Translate("rn5r/pp5p/2p1k1p1/q4p2/1bBPp3/2N4Q/PPP2P1P/2KR3R b - - 1 16"),
                "White Bishop vs Black King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Single,
                FenReader.Translate("2brr1k1/5p2/p2p3p/P1pP2p1/1pNbP3/1P3PPq/6QP/2BRR1K1 w - - 1 30"),
                "Black Bishop vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Single,
                FenReader.Translate("8/1k6/2P5/2R5/8/8/6K1/8 b - - 0 1"), "White Pawn vs Black King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Single,
                FenReader.Translate("8/1k6/8/8/8/8/3p3r/4K3 w - - 0 1"), "Black Pawn vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Single,
                FenReader.Translate("8/1k6/8/8/8/5q2/6K1/8 w - - 0 1"), "Black Queen vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Single,
                FenReader.Translate("8/1k6/8/8/8/8/3p3r/4K3 w - - 0 1"), "Black Knight vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Double,
                FenReader.Translate("8/1k6/8/8/5n1n/8/6K1/8 w - - 0 1"), "2 Black Knights + Rook vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Double,
                FenReader.Translate("8/1k6/8/8/8/5q1q/6K1/8 w - - 0 1"), "2 Black Queens vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Double,
                FenReader.Translate("8/1k6/5r2/8/8/3b4/8/5K2 w - - 0 1"), "Black Bishop + Rook vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Double,
                FenReader.Translate("8/1k6/2P5/1R6/8/8/6K1/8 b - - 0 1"), "White Pawn + Rook vs Black King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Double,
                FenReader.Translate("8/1k6/8/8/8/4r3/3p4/4K3 w - - 0 1"), "Black Pawn and Rook vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Double,
                FenReader.Translate("k7/1P6/8/8/8/8/6K1/Q7 b - - 0 2"), "White Pawn + Queen vs Black King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Double,
                FenReader.Translate("2k5/1P1P4/8/8/8/8/6K1/8 b - - 0 1"), "2 White Pawns vs Black King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.None,
                new Board(), "No Check - Initial Board");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.None,
                FenReader.Translate("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2"),
                "No Check - 1. e4 d5");
        }


        [TestCaseSource(nameof(GetIsStalemateTestCases))]
        public void IsStalemateTest(TestCase<bool, Board> testCase)
        {
            var actual = BoardHelpers.IsStalemate(testCase.TestMethodInputValue.Occupancy,
                testCase.TestMethodInputValue.ActivePlayer,
                testCase.TestMethodInputValue.EnPassantIndex, testCase.TestMethodInputValue.CastlingAvailability);
            Assert.AreEqual(testCase.ExpectedValue, actual);
        }

        protected static IEnumerable<TestCase<bool, Board>> GetIsStalemateTestCases()
        {
            yield return new TestCase<bool, Board>(false, FenReader.Translate("2k5/1P1P4/8/8/8/8/6K1/8 b - - 0 1"),
                "Double Check");
            yield return new TestCase<bool, Board>(false,
                FenReader.Translate("2r5/3r2k1/p3b3/1p3p2/2pPpP2/P1N1P3/1P4RP/2R3K1 b - - 1 34"), "Single Check");
            yield return new TestCase<bool, Board>(false,
                new Board(), "Initial Board");
            yield return new TestCase<bool, Board>(false,
                FenReader.Translate("3kr3/pppQ4/5N2/1n3p2/3p4/1P6/1P2qPP1/6K1 b - - 1 36"), "Checkmate");
            yield return new TestCase<bool, Board>(true,
                FenReader.Translate("7k/6pP/6P1/8/6P1/8/6K1/8 b - - 0 50"), "01 Stalemate");
            yield return new TestCase<bool, Board>(true,
                FenReader.Translate("8/8/8/5p2/6p1/5kP1/7K/5q2 w - - 0 106"), "02 Stalemate");
            yield return new TestCase<bool, Board>(true,
                FenReader.Translate("7k/7P/7K/8/8/8/8/8 b - - 2 66"), "03 Stalemate");
            yield return new TestCase<bool, Board>(true, FenReader.Translate("5Q2/7k/1R6/7P/6K1/8/8/8 b - - 0 62"),
                "04 Stalemate");
            yield return new TestCase<bool, Board>(true, FenReader.Translate("4k1K1/6P1/8/7q/8/8/8/8 w - - 10 57"),
                "05 Stalemate");
            yield return new TestCase<bool, Board>(true, FenReader.Translate("6K1/4k1P1/8/7q/8/8/8/8 w - - 10 57"),
                "06 Stalemate");
        }

        [TestCaseSource(nameof(GetIsCheckmateTestCases))]
        public void IsCheckmateTest(TestCase<bool, Board> testCase)
        {
            var actual = BoardHelpers.IsCheckmate(testCase.TestMethodInputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<bool, Board>> GetIsCheckmateTestCases()
        {
            //Normal positions
            yield return new TestCase<bool, Board>(false,
                new Board(), "Initial Board");
            yield return new TestCase<bool, Board>(false,
                FenReader.Translate("2bq1rk1/3p1npp/p1p3N1/1rbB1Pp1/1pQ5/P5N1/1PP3PP/R3R2K w - - 0 23"),
                "No checks- Normal Position");
            yield return new TestCase<bool, Board>(false,
                FenReader.Translate("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1"), "1. e4");
            yield return new TestCase<bool, Board>(false,
                FenReader.Translate("rnbqkbnr/pppppppp/8/8/3P4/8/PPP1PPPP/RNBQKBNR b KQkq d3 0 1"), "1. d4");

            //Normal checks
            yield return new TestCase<bool, Board>(false, FenReader.Translate("8/8/8/8/8/8/5Qk1/4K3 b - - 0 1"),
                "Single Check, Can evade");
            yield return new TestCase<bool, Board>(false,
                FenReader.Translate("2bq1rk1/3p1Bpp/p1p3N1/1rb2Pp1/1pQ5/P5N1/1PP3PP/R3R2K b - - 0 23"),
                "Check, attacker can be captured");
            yield return new TestCase<bool, Board>(false, FenReader.Translate("2k5/1P1P4/8/8/8/8/6K1/8 b - - 0 1"),
                "Double Check, Can evade and capture");
            yield return new TestCase<bool, Board>(false,
                FenReader.Translate("2r5/3r2k1/p3b3/1p3p2/2pPpP2/P1N1P3/1P4RP/2R3K1 b - - 1 34"),
                "Single Check, Can evade");
            yield return new TestCase<bool, Board>(false,
                FenReader.Translate("5Bk1/p4p2/5P1p/1Pp4P/2bbrp2/6p1/P2P2P1/5K1R w - - 1 30"),
                "Single check, can block");
            //Stalemates
            yield return new TestCase<bool, Board>(false,
                FenReader.Translate("7k/7P/7K/8/8/8/8/8 b - - 2 66"), "Stalemate 01");
            yield return new TestCase<bool, Board>(false, FenReader.Translate("5Q2/7k/1R6/7P/6K1/8/8/8 b - - 0 62"),
                "Stalemate 02");
            yield return new TestCase<bool, Board>(false, FenReader.Translate("4k1K1/6P1/8/7q/8/8/8/8 w - - 10 57"),
                "Stalemate 03");
            yield return new TestCase<bool, Board>(false, FenReader.Translate("6K1/4k1P1/8/7q/8/8/8/8 w - - 10 57"),
                "Stalemate 04");
            //Mates
            yield return new TestCase<bool, Board>(true,
                FenReader.Translate("3kr3/pppQ4/5N2/1n3p2/3p4/1P6/1P2qPP1/6K1 b - - 1 36"), "00 Checkmate");
            yield return new TestCase<bool, Board>(true,
                FenReader.Translate("5Bk1/p4p2/5P1p/1Pp4P/2bbrp2/6p1/P1P3P1/5K1R w - - 1 30"), "01 Checkmate");
            yield return new TestCase<bool, Board>(true,
                FenReader.Translate("2bQkr2/1p1p2bp/3PNnq1/5pN1/r4BpP/2P3P1/PP6/R4RK1 b - - 10 25"), "02 Checkmate");
            yield return new TestCase<bool, Board>(true, FenReader.Translate("8/8/8/8/8/8/5QkQ/4K3 b - - 0 1"),
                "03 Mate with 2 Queens");
            yield return new TestCase<bool, Board>(false, FenReader.Translate("8/8/8/8/3b4/8/3Q2k1/4K3 b - - 0 1"),
                "Mate in 5");
            yield return new TestCase<bool, Board>(true, FenReader.Translate("8/8/8/8/3b2B1/5N1Q/6k1/4K3 b - - 0 1"),
                "04 Checkmate, single check");
            yield return new TestCase<bool, Board>(true,
                FenReader.Translate("3qk3/5Q1p/8/p1p1N3/Pp2bP1P/1P1r4/8/4RnK1 b - - 6 38"), "05 Checkmate, Queen");
            yield return new TestCase<bool, Board>(true,
                FenReader.Translate("7R/pp4p1/2p3Bk/5P2/7P/8/PP4p1/4K3 b - - 1 55"),
                "06 Checkmate, Rook");
            yield return new TestCase<bool, Board>(true,
                FenReader.Translate("4R3/2p3pk/pp3p2/5n1p/2P2P1P/P5r1/1P4q1/3QR2K w - - 6 41"), "07 Checkmate, Queen");
        }


        [TestCaseSource(nameof(GetMoveTypeTestCases))]
        public MoveType GetMoveTypeTest(Board board, ushort from, ushort to, ushort? epIndex)
        {
            return BoardHelpers.GetMoveType(board.Occupancy, (ushort)@from,
                (ushort)to, (ushort?)epIndex);
        }

        protected static IEnumerable GetMoveTypeTestCases()
        {

            yield return new TestCaseData(
                FenReader.Translate("rnbqkbnr/ppp1pppp/8/4P3/3p4/8/PPPP1PPP/RNBQKBNR w KQkq - 0 3"),
                "c2".ToBoardIndex(),
                "c4".ToBoardIndex(),
                (ushort?)null)
                .SetDescription("Normal- White, c2-c4")
                .SetName("GetMoveType - Normal")
                .Returns(MoveType.Normal);

            yield return new TestCaseData(
                    FenReader.Translate("rnbqkbnr/ppp1pppp/8/4P3/2Pp4/8/PP1P1PPP/RNBQKBNR b KQkq c3 0 3"),
                    "d4".ToBoardIndex(),
                    "c3".ToBoardIndex(),
                    "c3".ToBoardIndex())
                .SetDescription("En Passant capture from black")
                .SetName("GetMoveType - En Passant")
                .Returns(MoveType.EnPassant);

            yield return new TestCaseData(FenReader.Translate("r2qkbnr/ppp2ppp/2n1p3/1B2Pb2/8/2P2N2/P2P1PPP/RNBQK2R w KQkq - 0 7"),
                    MoveHelpers.WhiteCastleKingSide.SourceIndex,
                    MoveHelpers.WhiteCastleKingSide.DestinationIndex,
                    (ushort?)null)
                    .SetDescription("Castles - White - Short")
                    .SetName("GetMoveType - Castles")
                    .Returns(MoveType.Castle);

            yield return new TestCaseData(FenReader.Translate("r2qkbnr/ppp2ppp/2n1p3/1B2Pb2/8/2P2N2/P2P1PPP/RNBQK2R w KQkq - 0 7"),
                    MoveHelpers.WhiteCastleQueenSide.SourceIndex,
                    MoveHelpers.WhiteCastleQueenSide.DestinationIndex,
                    (ushort?)null)
                .SetDescription("Castles - White - Long")
                .SetName("GetMoveType - Castles")
                .Returns(MoveType.Castle);

            yield return new TestCaseData(FenReader.Translate("rnbqk2r/pppp1ppp/5n2/4N3/1bP5/2N5/PP1PPPPP/R1BQKB1R b KQkq - 0 4"),
                    MoveHelpers.BlackCastleKingSide.SourceIndex,
                    MoveHelpers.BlackCastleKingSide.DestinationIndex,
                    (ushort?)null)
                .SetDescription("Castles - Black - Short")
                .SetName("GetMoveType - Castles")
                .Returns(MoveType.Castle);

            yield return new TestCaseData(
                    FenReader.Translate("r2qkbnr/ppp2ppp/2n1p3/1B2Pb2/8/2P2N2/P2P1PPP/RNBQK2R w KQkq - 0 7"),
                    MoveHelpers.BlackCastleQueenSide.SourceIndex,
                    MoveHelpers.BlackCastleQueenSide.DestinationIndex,
                    (ushort?)null)
                .SetDescription("Castles - Black - Long")
                .SetName("GetMoveType - Castles")
                .Returns(MoveType.Castle);

            yield return new TestCaseData(FenReader.Translate("8/3P4/8/1p6/8/P1p3P1/1k2p3/4K3 w - - 0 49"),
                    "d7".ToBoardIndex(),
                    "d8".ToBoardIndex(),
                    (ushort?)null)
                .SetDescription("Promotion - White")
                .SetName("GetMoveType - Promotion")
                .Returns(MoveType.Promotion);

            yield return new TestCaseData(FenReader.Translate("6Q1/8/8/p7/5K2/k7/1p4P1/8 b - - 0 55"),
                    "b2".ToBoardIndex(),
                    "b1".ToBoardIndex(),
                    (ushort?)null)
                .SetDescription("Promotion - Black")
                .SetName("GetMoveType - Promotion")
                .Returns(MoveType.Promotion);

        }


        [TestCaseSource(nameof(GetSquareToIndexTestCases))]
        public void SquareTextToIndexTest(TestCase<ushort, string> testCase)
        {
            var actual = testCase.TestMethodInputValue.ToBoardIndex();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        [TestCase("a0")]
        [TestCase("a9")]
        [TestCase("i1")]
        [TestCase("z8")]
        public void SquareTextToIndexTestShouldThrowExceptionWhenOutOfRange(string testCase)
        {
            Assert.Throws(typeof(ArgumentException), delegate { testCase.ToBoardIndex(); });
        }

        protected static IEnumerable<TestCase<ushort, string>> GetSquareToIndexTestCases()
        {
            foreach (var squareName in BoardConstants.SquareNames.Select(
                (x, i) => new { square = x, index = (ushort)i }))
            {
                yield return new TestCase<ushort, string>(squareName.index, squareName.square);
            }
        }


        [TestCaseSource(nameof(GetFileTestCases))]
        public void GetFileTest(TestCase<ushort, ushort> testCase)
        {
            var actual = testCase.TestMethodInputValue.GetFile();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ushort, ushort>> GetFileTestCases()
        {
            foreach (var sq in BoardConstants.AllSquares)
            {
                for (ushort index = 0; index < 8; index++)
                {
                    var fileMask = BoardConstants.FileMasks[index];
                    if ((fileMask & sq.GetBoardValueOfIndex()) != 0)
                    {
                        yield return new TestCase<ushort, ushort>(index, sq, $"{sq.ToSquareString()}");
                        break;
                    }
                }
            }
        }

        [TestCaseSource(nameof(GetRankTestCases))]
        public void GetRankTest(TestCase<ushort, ushort> testCase)
        {
            var actual = testCase.TestMethodInputValue.GetRank();
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ushort, ushort>> GetRankTestCases()
        {
            foreach (var sq in BoardConstants.AllSquares)
            {
                for (ushort index = 0; index < 8; index++)
                {
                    var rankMask = BoardConstants.RankMasks[index];
                    if ((rankMask & sq.GetBoardValueOfIndex()) != 0)
                    {
                        yield return new TestCase<ushort, ushort>(index, sq, $"{sq.ToSquareString()}");
                        break;
                    }
                }
            }
        }


        [TestCase((ushort)0, (ushort)7)]
        [TestCase((ushort)1, (ushort)6)]
        [TestCase((ushort)2, (ushort)5)]
        [TestCase((ushort)3, (ushort)4)]
        [TestCase((ushort)4, (ushort)3)]
        [TestCase((ushort)5, (ushort)2)]
        [TestCase((ushort)6, (ushort)1)]
        [TestCase((ushort)7, (ushort)0)]
        public void RankComplimentTest(ushort rank, ushort expected)
        {
            var actual = rank.RankCompliment();
            Assert.AreEqual(expected, actual, rank.ToSquareString());
        }
    }
}