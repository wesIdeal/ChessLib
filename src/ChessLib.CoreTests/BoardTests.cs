using NUnit.Framework;
using ChessLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChessLib.Core.Helpers;
using ChessLib.Core.Tests.Helpers;
using ChessLib.Core.Types.Enums;
using EnumsNET;

namespace ChessLib.Core.Tests
{
    [TestFixture()]
    public class BoardTests
    {
        [TestCase(Color.Black, Color.White)]
        [TestCase(Color.White, Color.Black)]
        public void TestOpponentColor(Color input, Color expected)
        {
            var board = new Board();
            board.ActivePlayer = input;
            Assert.AreEqual(expected, board.OpponentColor);
        }


        [Test()]
        public void BoardTest1()
        {
            Assert.Fail();
        }

        [Test()]
        public void BoardTest2()
        {
            Assert.Fail();
        }

        [Test()]
        public void CloneTest()
        {
            var board = new Board("rnbqkbnr/pppp1ppp/8/4p3/2P5/2N5/PP1PPPPP/R1BQKBNR b KQkq - 1 2");
            var clone = board.Clone();
            Assert.AreNotSame(board,clone);
            Assert.AreEqual(board, clone);
        }

        [Test()]
        public void EqualsTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void CloneOccupancyTest()
        {
            var board = new Board();
            var clone = board.CloneOccupancy();
            Assert.AreNotSame(board.Occupancy, clone);
            foreach (var color in Enums.GetValues<Color>())
            {
                foreach (var piece in Enums.GetValues<Piece>())
                {
                    Assert.AreEqual(board.Occupancy.Occupancy(color, piece), clone.Occupancy(color, piece));
                }
            }
        }

        [TestCaseSource(nameof(GetEqualsTestCases))]
        public void EqualsTest(TestCase<bool, Board> testCase)
        {
            Assert.AreEqual(testCase.ExpectedValue, testCase.InputValue.Equals(testCase.AdditionalInputs.Single()));
        }

        protected static IEnumerable<TestCase<bool, Board>> GetEqualsTestCases()
        {
            yield return new TestCase<bool, Board>(true, new Board(), new Board());
            yield return new TestCase<bool, Board>(false,
                new Board("rnbqkbnr/pppp1ppp/8/4p3/2P5/2N5/PP1PPPPP/R1BQKBNR b KQkq - 1 2"), new Board());
        }

       
    }
}