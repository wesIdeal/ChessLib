using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Services;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Helpers
{
    [TestFixture()]
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
            var from = strFrom.SquareTextToIndex();
            var to = strTo.SquareTextToIndex();
            Assert.IsNotNull(from);
            Assert.IsNotNull(to);
            var actual = BoardHelpers.InBetween((int)from, (int)to);
            Assert.AreEqual(expected, actual, $"{strFrom}->{strTo}");
        }


        [TestCaseSource(nameof(GetOccupancyTestCases))]
        public void OccupancyTest(TestCase<ulong, ulong[][]> testCase)
        {
            var color = (Color?)testCase.AdditionalInputs.FirstOrDefault();
            var piece = (Piece?)testCase.AdditionalInputs?.Skip(1).FirstOrDefault();
            var actual = BoardHelpers.Occupancy(testCase.InputValue, color, piece);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.Description);
        }

        protected static IEnumerable<TestCase<ulong, ulong[][]>> GetOccupancyTestCases()
        {
            yield return new TestCase<ulong, ulong[][]>(0xffff00000000ffff, BoardHelpers.InitialBoard, "All Pieces");
            yield return new TestCase<ulong, ulong[][]>(0xfffful, BoardHelpers.InitialBoard, "All Pieces for White", Color.White);
            yield return new TestCase<ulong, ulong[][]>(0xffff000000000000, BoardHelpers.InitialBoard, "All Pieces for Black", Color.Black);
            yield return new TestCase<ulong, ulong[][]>(0xff00ul, BoardHelpers.InitialBoard, "All Pawns for White", Color.White, Piece.Pawn);
            yield return new TestCase<ulong, ulong[][]>(0x00ff000000000000, BoardHelpers.InitialBoard, "All Pawns for Black", Color.Black, Piece.Pawn);
        }

        [TestCaseSource(nameof(GetPieceAtIndexTestCases))]
        public void GetPieceAtIndexTest(TestCase<Piece?, ushort> testCase)
        {
            var actual = BoardHelpers.GetPieceAtIndex(BoardHelpers.InitialBoard, testCase.InputValue);
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
            var actual = BoardHelpers.GetPieceOfColorAtIndex(BoardHelpers.InitialBoard, testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.Description);
        }
        protected static IEnumerable<TestCase<PieceOfColor?, ushort>> GetPieceOfColorAtIndexTestCases()
        {
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor() { Color = Color.White, Piece = Piece.Pawn }, 12, "White Pawn on e4");
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor() { Color = Color.Black, Piece = Piece.Knight }, 57, "Black Knight on b8");
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor() { Color = Color.White, Piece = Piece.Bishop }, 5, "White Bishop on f1");
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor() { Color = Color.Black, Piece = Piece.Rook }, 63, "Black Rook on h8");
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor() { Color = Color.White, Piece = Piece.Queen }, 3, "White Queen on d1");
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor() { Color = Color.Black, Piece = Piece.King }, 60, "Black King on e8");
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
            var occupancy = testCase.InputValue.Occupancy;
            var currentCastlingAvailability = testCase.InputValue.CastlingAvailability;
            var move = (Move)testCase.AdditionalInputs.Single();
            var actual = BoardHelpers.GetCastlingAvailabilityPostMove(occupancy, move, currentCastlingAvailability);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<CastlingAvailability, Board>> GetCastlingAvailabilityPostMoveTestCases()
        {
            var allCastlingAvailable = new Board("r3k2r/ppp5/8/8/8/8/PPP5/R3K2R w KQkq - 0 1");
            var knightsOnAFile = new Board("n3k2r/8/8/8/8/8/PPP5/N3K2R w Kk - 0 1");
            var knightsOnHFile = new Board("r3k2n/8/8/8/8/8/PPP5/R3K2N w Qq - 0 1");
            var bishopsOnAFile = new Board("b3k2r/8/8/8/8/1P6/P1P5/B3K2R w Kk - 0 1");
            var bishopsOnHFile = new Board("r3k2b/8/8/8/8/1P6/P1P5/R3K2B w Qq - 0 1");
            var queensOnAFile = new Board("q3k2r/8/8/8/8/8/PPP5/Q3K2R w Kk - 0 1");
            var queensOnHFile = new Board("r3k2q/8/8/8/8/8/PPP5/R3K2Q w Qq - 0 1");
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
            yield return new TestCase<CastlingAvailability, Board>(CastlingAvailability.WhiteKingside | CastlingAvailability.BlackKingside,
                knightsOnAFile,
                "White Knight from a1 -> b3- Castling not affected", MoveHelpers.GenerateMove(0, 17));
            //White Knight from h1 -> g3
            yield return new TestCase<CastlingAvailability, Board>(CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackQueenside,
                knightsOnHFile,
                "White Knight from h1 -> g3- Castling not affected", MoveHelpers.GenerateMove(7, 22));
            //Black Knight from a8 -> b6
            yield return new TestCase<CastlingAvailability, Board>(CastlingAvailability.WhiteKingside | CastlingAvailability.BlackKingside,
                knightsOnAFile,
                "Black Knight from a8 -> b6- Castling not affected", MoveHelpers.GenerateMove(56, 41));
            //Black Knight from h8 ->g6
            yield return new TestCase<CastlingAvailability, Board>(CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackQueenside,
                knightsOnHFile,
                "Black Knight from h8 ->g6- Castling not affected", MoveHelpers.GenerateMove(63, 46));

            //White Bishop from a1 -> c3
            yield return new TestCase<CastlingAvailability, Board>(CastlingAvailability.WhiteKingside | CastlingAvailability.BlackKingside,
                bishopsOnAFile,
                "White Bishop from a1 -> c3- Castling not affected", MoveHelpers.GenerateMove(0, 18));
            //White Bishop from h1 -> f3
            yield return new TestCase<CastlingAvailability, Board>(CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackQueenside,
                bishopsOnHFile,
                "White Bishop from h1 -> f3- Castling not affected", MoveHelpers.GenerateMove(7, 21));

            //Black Bishop from a8 -> c6
            yield return new TestCase<CastlingAvailability, Board>(CastlingAvailability.WhiteKingside | CastlingAvailability.BlackKingside,
                bishopsOnAFile,
                "Black Bishop from a8 -> b6- Castling not affected", MoveHelpers.GenerateMove(56, 42));

            //Black Bishop from h8 ->f6
            yield return new TestCase<CastlingAvailability, Board>(CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackQueenside,
                bishopsOnHFile,
                "Black Bishop from h8 -> f6- Castling not affected", MoveHelpers.GenerateMove(63, 45));


            //White Queen from a1 -> c3
            yield return new TestCase<CastlingAvailability, Board>(CastlingAvailability.WhiteKingside | CastlingAvailability.BlackKingside,
                queensOnAFile,
                "White Queen from a1 -> c3- Castling not affected", MoveHelpers.GenerateMove(0, 18));
            //White Queen from h1 -> f3
            yield return new TestCase<CastlingAvailability, Board>(CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackQueenside,
                queensOnHFile,
                "White Queen from h1 -> f3- Castling not affected", MoveHelpers.GenerateMove(7, 21));

            //Black Queen from a8 -> c6
            yield return new TestCase<CastlingAvailability, Board>(CastlingAvailability.WhiteKingside | CastlingAvailability.BlackKingside,
                queensOnAFile,
                "Black Queen from a8 -> b6- Castling not affected", MoveHelpers.GenerateMove(56, 42));

            //Black Queen from h8 ->f6
            yield return new TestCase<CastlingAvailability, Board>(CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackQueenside,
                queensOnHFile,
                "Black Queen from h8 -> f6- Castling not affected", MoveHelpers.GenerateMove(63, 45));

            //Rook Moves
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside | CastlingAvailability.WhiteKingside,
                allCastlingAvailable, "White Rook Move- Queenside eliminated", MoveHelpers.GenerateMove(0, 2));
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside | CastlingAvailability.WhiteQueenside,
                allCastlingAvailable, "White Rook Move- Kingside eliminated", MoveHelpers.GenerateMove(7, 6));
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.BlackKingside | CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside,
                allCastlingAvailable, "Black Rook Move- Queenside eliminated", MoveHelpers.GenerateMove(56, 57));
            yield return new TestCase<CastlingAvailability, Board>(
                CastlingAvailability.BlackQueenside | CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside,
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
            var blackRookOnA1 = new Board("r3k2r/ppp5/8/8/8/8/PPP5/r1B1K2R w Kkq - 0 1");
            var whiteRookOnA8 = new Board("R1b1k2r/ppp5/8/8/8/8/PPP5/RN2K2R w KQk - 0 1");
            yield return new TestCase<CastlingAvailability, Board>(blackRookOnA1.CastlingAvailability, blackRookOnA1,
                "Black Rook on a1- No Change", MoveHelpers.GenerateMove(0, 1));
            yield return new TestCase<CastlingAvailability, Board>(whiteRookOnA8.CastlingAvailability, whiteRookOnA8,
                "White Rook on a8- No Change", MoveHelpers.GenerateMove(56, 57));
        }


        [TestCaseSource(nameof(GetIsEnPassantCaptureAvailableTestCases))]
        public void IsEnPassantCaptureAvailableTest(TestCase<bool, Board> testCase)
        {
            var actual = BoardHelpers.IsEnPassantCaptureAvailable(testCase.InputValue);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<bool, Board>> GetIsEnPassantCaptureAvailableTestCases()
        {
            yield return new TestCase<bool, Board>(false, new Board(), "Initial board");
            yield return new TestCase<bool, Board>(false,
                new Board("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1"), "After 1. e4");
            yield return new TestCase<bool, Board>(true,
                new Board("rnbqkbnr/pp2pppp/8/2ppP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3"), "Available on d6");
            yield return new TestCase<bool, Board>(true,
                new Board("rnbqkbnr/pp2pppp/3P4/8/2pP4/8/PPP2PPP/RNBQKBNR b KQkq d3 0 4"), "Available on d3");
        }

        [TestCaseSource(nameof(GetEnPassantIndexTestCases))]
        public void GetEnPassantIndexTest(TestCase<ushort?, Board> testCase)
        {
            var actual =
                BoardHelpers.GetEnPassantIndex(testCase.InputValue, (IMove)testCase.AdditionalInputs.Single());
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<ushort?, Board>> GetEnPassantIndexTestCases()
        {
            yield return new TestCase<ushort?, Board>(20, new Board(), "After 1. e4", MoveHelpers.GenerateMove(12, 28));
            yield return new TestCase<ushort?, Board>(44,
                new Board("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1"), "After 1. e4 e5",
                MoveHelpers.GenerateMove(52, 36));
            yield return new TestCase<ushort?, Board>(null, new Board("rnbqkbnr/pppppppp/8/8/8/8/PPPPRPPP/RNBQKBNR b KQkq - 0 1"), "After 1. Re4 from e3", MoveHelpers.GenerateMove(12, 28));
            yield return new TestCase<ushort?, Board>(null, new Board(), "No piece on square", MoveHelpers.GenerateMove(28, 29));
            yield return new TestCase<ushort?, Board>(null, new Board(), "After 1. e3", MoveHelpers.GenerateMove(12, 20));
            yield return new TestCase<ushort?, Board>(null,
                new Board("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1"), "After 1.e4 e6",
                MoveHelpers.GenerateMove(52, 44));
        }

        [TestCaseSource(nameof(GetApplyMoveToBoardTestCases))]
        public void ApplyMoveToBoardTest(TestCase<Board, Board> testCase)
        {
            var expected = testCase.ExpectedValue;
            var actual = (Board)BoardHelpers.ApplyMoveToBoard(testCase.InputValue, (IMove)testCase.AdditionalInputs.Single());
            Assert.AreEqual(expected.ToFEN(), actual.ToFEN(), testCase.ToString());
        }

        protected static IEnumerable<TestCase<Board, Board>> GetApplyMoveToBoardTestCases()
        {
            var boardTransitions = new[] { new Board(),
                new Board("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1"),
                new Board("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 2"),
                new Board("rnbqkbnr/ppp1pppp/8/3P4/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 2"),
                new Board("rnb1kbnr/ppp1pppp/8/3q4/8/8/PPPP1PPP/RNBQKBNR w KQkq - 0 3"),
                new Board("rnb1kbnr/ppp1pppp/8/3q4/8/2N5/PPPP1PPP/R1BQKBNR b KQkq - 1 3"),
                new Board("rnbqkbnr/ppp1pppp/8/8/8/2N5/PPPP1PPP/R1BQKBNR w KQkq - 2 4"),
                new Board("rnbqkbnr/ppp1pppp/8/8/2B5/2N5/PPPP1PPP/R1BQK1NR b KQkq - 3 4"),
                new Board("rnbqkbnr/ppp2ppp/4p3/8/2B5/2N5/PPPP1PPP/R1BQK1NR w KQkq - 0 5"),
                new Board("rnbqkbnr/ppp2ppp/4p3/8/2B5/2N2N2/PPPP1PPP/R1BQK2R b KQkq - 1 5"),
                new Board("rnbqkb1r/ppp2ppp/4pn2/8/2B5/2N2N2/PPPP1PPP/R1BQK2R w KQkq - 2 6"),
                new Board("rnbqkb1r/ppp2ppp/4pn2/8/2B5/2N2N2/PPPP1PPP/R1BQ1RK1 b kq - 3 6"),
                new Board("rnbqk2r/ppp1bppp/4pn2/8/2B5/2N2N2/PPPP1PPP/R1BQ1RK1 w kq - 4 7"),
                new Board("rnbqk2r/ppp1bppp/4pn2/8/2BP4/2N2N2/PPP2PPP/R1BQ1RK1 b kq d3 0 7"),
                new Board("rnbq1rk1/ppp1bppp/4pn2/8/2BP4/2N2N2/PPP2PPP/R1BQ1RK1 w - - 1 8")
            };
            var movesToApply = new[]
            {
                MoveHelpers.GenerateMove(12,28),
                MoveHelpers.GenerateMove(51,35),
                MoveHelpers.GenerateMove(28,35),
                MoveHelpers.GenerateMove(59,35),
                MoveHelpers.GenerateMove(1,18),
                MoveHelpers.GenerateMove(35,59),
                MoveHelpers.GenerateMove(5,26),
                MoveHelpers.GenerateMove(52,44),
                MoveHelpers.GenerateMove(6,21),
                MoveHelpers.GenerateMove(62,45),
                MoveHelpers.WhiteCastleKingSide,
                MoveHelpers.GenerateMove(61,52),
                MoveHelpers.GenerateMove(11,27),
                MoveHelpers.BlackCastleKingSide
            };

            for (var boardIndex = 0; boardIndex < movesToApply.Length; boardIndex++)
            {
                var startingBoard = boardTransitions[boardIndex];
                var endingBoard = boardTransitions[boardIndex + 1];
                var move = movesToApply[boardIndex];
                yield return new TestCase<Board, Board>(endingBoard, startingBoard, $"{boardIndex:D2} {move.ToString()}", move);
            }
        }





        [TestCaseSource(nameof(GetCheckTypeTestCases))]
        public void GetCheckTypeTest(TestCase<BoardHelpers.CheckType, Board> testCase)
        {
            var occupancy = testCase.InputValue.Occupancy;
            var color = testCase.InputValue.ActivePlayer;
            ushort[] attackingPieces;
            var actual = BoardHelpers.GetCheckType(occupancy, color, out attackingPieces);
            Assert.AreEqual(testCase.ExpectedValue, actual, testCase.ToString());
        }

        protected static IEnumerable<TestCase<BoardHelpers.CheckType, Board>> GetCheckTypeTestCases()
        {
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Single,
                new Board("2r5/3r2k1/p3b3/1p3p2/2pPpP2/P1N1P3/1P4RP/2R3K1 b - - 1 34"), "White Rook vs Black King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Single,
                new Board("rn5r/pp5p/2p1k1p1/q4p2/1bBPp3/2N4Q/PPP2P1P/2KR3R b - - 1 16"), "White Bishop vs Black King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Single,
                new Board("2brr1k1/5p2/p2p3p/P1pP2p1/1pNbP3/1P3PPq/6QP/2BRR1K1 w - - 1 30"), "Black Bishop vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Single,
                new Board("8/1k6/2P5/2R5/8/8/6K1/8 b - - 0 1"), "White Pawn vs Black King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Single,
                new Board("8/1k6/8/8/8/8/3p3r/4K3 w - - 0 1"), "Black Pawn vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Single,
                new Board("8/1k6/8/8/8/5q2/6K1/8 w - - 0 1"), "Black Queen vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Single,
                new Board("8/1k6/8/8/8/8/3p3r/4K3 w - - 0 1"), "Black Knight vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Double,
                new Board("8/1k6/8/8/5n1n/8/6K1/8 w - - 0 1"), "2 Black Knights + Rook vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Double,
                new Board("8/1k6/8/8/8/5q1q/6K1/8 w - - 0 1"), "2 Black Queens vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Double,
                new Board("8/1k6/5r2/8/8/3b4/8/5K2 w - - 0 1"), "Black Bishop + Rook vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Double,
                new Board("8/1k6/2P5/1R6/8/8/6K1/8 b - - 0 1"), "White Pawn + Rook vs Black King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Double,
                new Board("8/1k6/8/8/8/4r3/3p4/4K3 w - - 0 1"), "Black Pawn and Rook vs White King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Double,
                new Board("k7/1P6/8/8/8/8/6K1/Q7 b - - 0 2"), "White Pawn + Queen vs Black King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.Double,
                new Board("2k5/1P1P4/8/8/8/8/6K1/8 b - - 0 1"), "2 White Pawns vs Black King");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.None,
                new Board(), "No Check - Initial Board");
            yield return new TestCase<BoardHelpers.CheckType, Board>(BoardHelpers.CheckType.None,
                new Board("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2"), "No Check - 1. e4 d5");
        }

        [Test()]
        public void IsColorInCheckTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void IsStalemateTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void IsCheckmateTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void DoesKingHaveEvasionsTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetMoveTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetMoveTypeTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void IsCastlingMoveTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void IsEnPassantCaptureTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ToIntTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ToIntTest1()
        {
            Assert.Fail();
        }

        [Test()]
        public void ToIntTest2()
        {
            Assert.Fail();
        }

        [Test()]
        public void ToIntTest3()
        {
            Assert.Fail();
        }

        [Test()]
        public void ToHexDisplayTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SquareTextToIndexTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetFileTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetFileTest1()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetRankTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetRankTest1()
        {
            Assert.Fail();
        }

        [Test()]
        public void FileFromIdxTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void RankComplimentTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetFENPiecePlacementTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetFENSideToMoveStrRepresentationTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetFENCastlingAvailabilityStringTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetFENEnPassantStringTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetFENHalfMoveClockStringTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetFENMoveCounterStringTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ToFENTest()
        {
            Assert.Fail();
        }
    }
}