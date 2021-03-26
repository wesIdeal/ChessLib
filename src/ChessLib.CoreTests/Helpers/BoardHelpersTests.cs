using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
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

        [Test()]
        public void GetEnPassantIndexTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ApplyMoveToBoardTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetBoardPostMoveTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void OpponentColorTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ActiveKingIndexTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void IsActivePlayerInCheckTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void IsOpponentInCheckTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetCheckTypeTest()
        {
            Assert.Fail();
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