using System.Collections;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.PgnExport;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Types.GameTree.Traversal
{
    public class TraversalData
    {
        private static readonly PgnMoveInformation whiteNode =
            new PgnMoveInformation(Color.White, "e4", "1", true, false,0, false);

        private static readonly PgnMoveInformation blackNode =
            new PgnMoveInformation(Color.Black, "e5",  "1", false, false,0, false);

        private static readonly (int[] nodeIndex, string[] variationText) LongVariation1 = (nodeIndex: new[] { 1 },
            variationText: new[]
            {
                "g3"
            });
        private static readonly (int[] nodeIndex, string[] variationText) LongVariation2 = (nodeIndex: new[] { 2 },
            variationText: new[]
            {
                "e5","Nf3"
            });

        internal static string[] longGameMainLine = { "c4", "Nf6", "Nc3", "e6", "Nf3", "d5", "d4", "Nbd7", "Bg5", "h6" };
        private static readonly string[] longGameExpected = { "c4", "Nf6", "Nc3", "g3", "e6", "e5", "Nf3", "Nf3", "d5", "d4", "Nbd7", "Bg5", "h6" };

        private static TestCaseData BuildTraversalTestCase(string[] mainLine, string fen, string[] expected, params (int[] nodeIndex, string[] variationText)[] variations)
        {

            var game = new Game(fen);
            game.ApplyMove(mainLine);
            game.Reset();
            var orderedVariations = variations.OrderBy(x => x.nodeIndex.First()).ThenBy(x => x.nodeIndex.Length);
            BuildVariations(game, orderedVariations.ToArray());
            return new TestCaseData(game, mainLine, expected, variations);
        }

        private static void BuildVariations(Game game, (int[] nodeIndex, string[] variationText)[] variations)
        {
            var index = 0;
            while (game.MoveNext())
            {
                var foundVariations = FindVariationForIndex(index, variations.ToArray());
                if (foundVariations.Any())
                {
                    foreach (var variation in foundVariations)
                    {
                        game.ApplyMove(variation.variationText);
                        var variationsOnVariations =
                            foundVariations
                                .Select(x => (nodeIndex: x.nodeIndex.Skip(1).ToArray(),
                                    variationText: x.variationText.Skip(1).ToArray()))
                                .Where(x => x.variationText.Any() && x.nodeIndex.Any())
                                .ToArray();

                        BuildVariations(game, variationsOnVariations);
                        game.ExitVariation();
                    }
                }

                index++;
            }
        }

        private static (int[] nodeIndex, string[] variationText)[] FindVariationForIndex(int i,
            (int[] nodeIndex, string[] variationText)[] variations)
        {
            var found = variations.Where(x => x.nodeIndex.First() == i);
            return found
                .Select(x => (nodeIndex: x.nodeIndex.ToArray(),
                    variationText: x.variationText.ToArray()))
                .Where(x => x.variationText.Any() && x.nodeIndex.Any()).ToArray();
        }

        public static IEnumerable GetTraversalTestCases()
        {
            yield return BuildTraversalTestCase(longGameMainLine, BoardConstants.FenStartingPosition, longGameExpected, LongVariation1, LongVariation2);
        }



        public static IEnumerable GetMovePairIsFullTests()
        {
            yield return new TestCaseData(whiteNode, blackNode).Returns(true)
                .SetDescription("Both White and Black have value");
            yield return new TestCaseData(null, blackNode).Returns(false)
                .SetDescription("White is null, Black has value");
            yield return new TestCaseData(whiteNode, null).Returns(false)
                .SetDescription("White has value, Black is null");
            yield return new TestCaseData(null, null).Returns(false).SetDescription("Neither have value");
        }

        public static IEnumerable GetMovePairIsEmptyTests()
        {
            yield return new TestCaseData(whiteNode, blackNode).Returns(false)
                .SetDescription("Both White and Black have value");
            yield return new TestCaseData(null, blackNode).Returns(false)
                .SetDescription("White is null, Black has value");
            yield return new TestCaseData(whiteNode, null).Returns(false)
                .SetDescription("White has value, Black is null");
            yield return new TestCaseData(null, null).Returns(true).SetDescription("Neither have value");
        }




        public static IEnumerable GetWhiteNodeTests()
        {
            yield return new TestCaseData(whiteNode).SetDescription("Should set and get White node value");
        }

        public static IEnumerable GetBlackNodeTests()
        {
            yield return new TestCaseData(whiteNode).SetDescription("Should set and get Black node value");
        }

        internal static void SetupLongGame(PgnFormatterTests.PgnFormatterVariationTestCase testCase)
        {
            testCase.Game.Reset();
            testCase.Game.MoveNext();
            testCase.Game.MoveNext();
            testCase.Game.ApplyMove("g3", MoveApplicationStrategy.ContinueMainLine);
            testCase.Game.ExitVariation();
            testCase.Game.MoveNext();
            testCase.Game.ApplyMove("e5", MoveApplicationStrategy.ContinueMainLine)
                .ApplyMove("Nf3", MoveApplicationStrategy.ContinueMainLine);
            testCase.Game.ExitVariation();
            testCase.Game.MoveNext();
            testCase.Game.MoveNext();
            testCase.Game.MoveNext();

            testCase.Game.ApplyMove("cxd5", "exd5", "d4");
            testCase.Game.MovePrevious();
            testCase.Game.ApplyMove("e3", "Bd6");
            testCase.Game.Current.Node.Annotation.ApplyNag("=+");
            testCase.Game.MovePrevious();
            testCase.Game.ApplyMove("c6");
            testCase.Game.AddNag(new NumericAnnotation("?!"));
            testCase.Game.ExitVariation();
            testCase.Game.ExitVariation();
            testCase.Game.ExitVariation();
            testCase.Game.MoveNext();
            testCase.Game.MoveNext();
            testCase.Game.MoveNext();
            testCase.Game.ApplyMove("c6", MoveApplicationStrategy.ContinueMainLine);
            testCase.Game.MovePrevious();
            testCase.Game.ApplyMove("Bb4", "cxd5", "exd5");
            testCase.Game.Current.Node.Comment = "Best move";
            testCase.Game.ApplyMove("e3", MoveApplicationStrategy.ContinueMainLine);
            testCase.Game.Current.Node.Comment = "White has a slight advantage.";
            testCase.Game.MovePrevious();
            testCase.Game.ApplyMove("Qc2", "h6");
            testCase.Game.AddNag(new NumericAnnotation("+="));
        }
    }
}