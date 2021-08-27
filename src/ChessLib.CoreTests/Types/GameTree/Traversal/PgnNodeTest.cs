using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.GameTree.Traversal;
using ChessLib.Core.Types.PgnExport;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Types.GameTree.Traversal
{
    [TestFixture(TestOf = typeof(PgnNode))]
    public class PgnNodeTest
    {
        [Test(Description = "Test MovePair partitioning")]
        public void GetMovePairs_WWB_ShouldReturn4Nodes()
        {
            var game = new Game("rnbqkb1r/pppppppp/5n2/8/2P5/2N5/PP1PPPPP/R1BQKBNR b KQkq - 2 2");
            game.ApplyMove("e6").MovePrevious();
            game.ApplyMove("e5").ApplyMove("Nf3").ExitVariation();
            game.MoveNext();
            game.ApplyMove("Nf3");

            var pgnNode = new PgnNode(game.InitialNode.Node);
            var pairs = pgnNode.GetMovePairs().ToArray();

            AssertMovePair(pairs[0].WhiteNode, null);
            AssertMovePair(pairs[0].BlackNode, "e6");
            AssertMovePair(pairs[1].WhiteNode, null);
            AssertMovePair(pairs[1].BlackNode, "e5");
            AssertMovePair(pairs[2].WhiteNode, "Nf3");
            AssertMovePair(pairs[2].BlackNode, null);
            AssertMovePair(pairs[3].WhiteNode, "Nf3");
            AssertMovePair(pairs[3].BlackNode, null);
            Assert.AreEqual(4, pairs.Length);
        }

        [Test]
        public void GetMovePairs_WWWBW_ShouldReturn4Nodes()
        {
            var game = new Game();
            game.ApplyMove("c4").ApplyMove("Nf6").ApplyMove("Nc3").ApplyMove("e6");
            game.MovePrevious();
            game.MovePrevious();
            game.ApplyMove("g3");

            var pgnNode = new PgnNode(game.InitialNode.Node);
            var pairs = pgnNode.GetMovePairs().ToArray();

            AssertMovePair(pairs[0].WhiteNode, "c4");
            AssertMovePair(pairs[0].BlackNode, "Nf6");
            AssertMovePair(pairs[1].WhiteNode, "Nc3");
            AssertMovePair(pairs[1].BlackNode, null);
            AssertMovePair(pairs[2].WhiteNode, "g3");
            AssertMovePair(pairs[2].BlackNode, null);
            AssertMovePair(pairs[3].WhiteNode, null);
            AssertMovePair(pairs[3].BlackNode, "e6");
            Assert.AreEqual(4, pairs.Length);
        }

        [TestCaseSource(typeof(TraversalData), nameof(TraversalData.GetTraversalTestCases))]
        public void TestFlatten(Game g, string[] mainLine, string[] expected,
            params (int[] nodeIndex, string[] variationText)[] variations)
        {
            var pgnNode = new PgnNode(g.InitialNode.Node);
            var flattened = pgnNode.FlattenToPgnOrder();
            
            var testCases = flattened.Zip(expected, (actual, expected) => new { Actual = actual.Value.San, Expected = expected }).ToList();
            foreach (var testCase in testCases)
            {
                Assert.AreEqual(testCase.Expected, testCase.Actual);
            }
        }

       

        private static IEnumerable<string> BuildVariation(string[] mainLine,
            (int[] nodeIndex, string[] variationText)[] variations)
        {
            for (var i = 0; i < mainLine.Length; i++)
            {
                var mainMove = mainLine[i];
                yield return mainMove;
                var foundVariations = FindVariationForIndex(i, variations).ToArray();
                if (foundVariations.Any())
                {
                    foreach (var variation in foundVariations)
                    {
                        var variationsOnVariation = foundVariations
                            .Select(x => (nodeIndex: x.nodeIndex.Skip(1).ToArray(),
                                variationText: x.variationText.Skip(1).ToArray()))
                            .Where(x => x.variationText.Any() && x.nodeIndex.Any());
                        
                        foreach (var move in BuildVariation(variation.variationText, variationsOnVariation.ToArray()))
                        {
                           yield return move;
                        }
                    }
                }
            }
        }

        private static IEnumerable<(int[] nodeIndex, string[] variationText)> FindVariationForIndex(int i,
            (int[] nodeIndex, string[] variationText)[] variations)
        {
            var found = variations.Where(x => x.nodeIndex.First() == i);
            return found
                .Select(x => (nodeIndex: x.nodeIndex.ToArray(),
                    variationText: x.variationText.ToArray()))
                .Where(x => x.variationText.Any() && x.nodeIndex.Any());
        }

        private void AssertMovePair(PgnMoveInformation? node, string san)
        {
            if (san == null)
            {
                Assert.IsNull(node);
            }
            else
            {
                Assert.AreEqual(san, node?.MoveSan);
            }
        }
    }
}