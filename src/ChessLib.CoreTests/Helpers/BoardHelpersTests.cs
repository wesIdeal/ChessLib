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
           Assert.AreEqual(testCase.ExpectedValue,actual, testCase.Description);
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
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor(){Color = Color.White, Piece =Piece.Pawn}, 12, "White Pawn on e4");
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor() { Color = Color.Black, Piece = Piece.Knight}, 57, "Black Knight on b8");
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor() { Color = Color.White, Piece = Piece.Bishop}, 5, "White Bishop on f1");
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor() { Color = Color.Black, Piece = Piece.Rook}, 63, "Black Rook on h8");
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor() { Color = Color.White, Piece = Piece.Queen }, 3, "White Queen on d1");
            yield return new TestCase<PieceOfColor?, ushort>(new PieceOfColor() { Color = Color.Black, Piece = Piece.King}, 60, "Black King on e8");
            yield return new TestCase<PieceOfColor?, ushort>(null, 26, "[Empty Square]");

        }
        

       [Test]
        public void ValidateIndexTest()
        {
            Assert.Throws<ArgumentException>(delegate { BoardHelpers.ValidateIndex(64); });
            Assert.DoesNotThrow(delegate { BoardHelpers.ValidateIndex(63); });
            Assert.DoesNotThrow(delegate { BoardHelpers.ValidateIndex(0); });
        }

        //[TestCaseSource(nameof(GetCastlingAvailabilityPostMoveTestCases))]
        //public void GetCastlingAvailabilityPostMoveTest()
        //{
        //    Assert.Fail();
        //}

        //protected static IEnumerable<TestCase<CastlingAvailability, Board>> GetCastlingAvailabilityPostMoveTestCases()
        //{
        //    yield return new TestCase<CastlingAvailability, Board>()
        //}

        [Test()]
        public void IsEnPassantCaptureAvailableTest()
        {
            Assert.Fail();
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