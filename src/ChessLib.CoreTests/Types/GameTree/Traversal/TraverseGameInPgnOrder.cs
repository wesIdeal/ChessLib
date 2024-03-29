﻿using System;
using System.Collections.Generic;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.GameTree.Traversal;
using ChessLib.Core.Types.PgnExport;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Types.GameTree.Traversal
{
    [TestFixture]
    public class MovePairTests
    {
        [TestCaseSource(typeof(TraversalData), nameof(TraversalData.GetMovePairIsEmptyTests))]
        public bool IsEmpty_Test(PgnMoveInformation? white, PgnMoveInformation? black)
        {
            var mp = new MovePair(white, black);
            return mp.IsEmpty;
        }

        [TestCaseSource(typeof(TraversalData), nameof(TraversalData.GetMovePairIsFullTests))]
        public bool IsFull_Test(PgnMoveInformation? white, PgnMoveInformation? black)
        {
            var mp = new MovePair(white, black);
            return mp.IsFull;
        }

        [TestCaseSource(typeof(TraversalData), nameof(TraversalData.GetWhiteNodeTests))]
        public void WhiteNode_GetAndSet(PgnMoveInformation? white)
        {
            var mp = new MovePair() { WhiteNode = white };
            Assert.AreEqual(white, mp.WhiteNode);
        }

        [TestCaseSource(typeof(TraversalData), nameof(TraversalData.GetBlackNodeTests))]
        public void BlackNode_GetAndSet(PgnMoveInformation? Black)
        {
            var mp = new MovePair() { BlackNode = Black };
            Assert.AreEqual(Black, mp.BlackNode);
        }

    }
    [TestFixture]
    public class TraverseGameInPgnOrder
    {
        [SetUp]
        public void SetUp()
        {
            PgnEnumerator = null;
        }

        private static readonly string[] SimpleMainLine = { "c4", "e5", "Nc3", "Nf6" };

        //[TestCaseSource(nameof(GetTestCases))]
        //public void MoveNext_ShouldReadLineInCorrectOrder(PgnFormatterTests.PgnTraversalTestCase testCase)
        //{
        //    PgnEnumerator = new GameToPgnEnumerator(testCase.Game);
        //    var index = 0;
        //    var strExpected = string.Join(",", testCase.ExpectedMoveTraversal);
        //    Console.WriteLine("Expected");
        //    Console.WriteLine(new string('-',20));
        //    Console.WriteLine(strExpected);
        //    Console.WriteLine(new string('-', 20));
        //    Console.WriteLine("Actual");
        //    Console.WriteLine(new string('-', 20));
        //    while (PgnEnumerator.MoveNext())
        //    {
        //        var expected = testCase.ExpectedMoveTraversal[index];
        //        var actual = PgnEnumerator.Current.Value.San;

        //        Console.WriteLine(actual);

        //        Assert.AreEqual(expected, actual, testCase.ToString());
        //        index++;
        //    }
        //}


        [Test]
        public void MoveNext_NoNextMove_ReturnsFalse()
        {
            var game = new Game();
            var enumerator = new GameToPgnEnumerator(game.InitialNode.Node);
            var actual = enumerator.MoveNext();
            Assert.IsFalse(actual, "Node should return false when current move is last.");
        }

        [Test]
        public void MoveNext_NoCurrentMoveThrowsException()
        {
            var game = new Game();
            var enumerator = new GameToPgnEnumerator(game);
            Assert.Throws<InvalidOperationException>(() =>
            {
                var unused = enumerator.Current;
            });
        }

        internal GameToPgnEnumerator PgnEnumerator;

        protected static IEnumerable<PgnFormatterTests.PgnTraversalTestCase> GetTestCases()
        {
            yield return new PgnFormatterTests.PgnTraversalTestCase(BoardConstants.FenStartingPosition,
                "No next move should return false.",
                new List<string>().ToArray());
            yield return new PgnFormatterTests.PgnTraversalTestCase(BoardConstants.FenStartingPosition,
                "Simple mainline, 4 moves",
                SimpleMainLine);
            yield return new PgnFormatterTests.PgnTraversalTestCase(BoardConstants.FenStartingPosition,
                "Variation on initial node",
                new[] { "c4", "e5" }, new[] { "d4", "Nf6" }, 0);
        }
    }

    
}