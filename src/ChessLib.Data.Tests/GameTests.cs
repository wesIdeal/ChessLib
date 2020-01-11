using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN;
using NUnit.Framework;

namespace ChessLib.Data.Tests
{
    public class GameTests
    {

        [TestFixture]
        public class OtherTests
        {
            [Test]
            public void ShouldReturnEqualForSameGame()
            {
                var parser = new PGNParser();
                var g1Str = PGN.g1;
                var g2Str = PGN.g1;
                var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
                var g2 = parser.GetGamesFromPGNAsync(g2Str).Result.First();
                Assert.IsTrue(g1.IsEqualTo(g2));
                Assert.IsTrue(g1.IsEqualTo(g2, true));
            }

            [Test]
            public void ShouldReturnEqualIgnoringVariations()
            {
                var parser = new PGNParser();
                var g1Str = PGN.g1;
                var g2Str = PGN.g1Variation;
                var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
                var g2 = parser.GetGamesFromPGNAsync(g2Str).Result.First();
                Assert.IsTrue(g1.IsEqualTo(g2));
            }

            [Test]
            public void ShouldReturnEqualIncludingVariations()
            {
                var parser = new PGNParser();
                var g1Str = PGN.g1Variation;
                var g2Str = PGN.g1Variation;
                var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
                var g2 = parser.GetGamesFromPGNAsync(g2Str).Result.First();
                Assert.IsTrue(g1.IsEqualTo(g2, true));
            }

            [Test]
            public void ShouldReturnNotEqualIncludingVariations()
            {
                var parser = new PGNParser();
                var g1Str = PGN.g1;
                var g2Str = PGN.g1Variation;
                var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
                var g2 = parser.GetGamesFromPGNAsync(g2Str).Result.First();
                Assert.IsFalse(g1.IsEqualTo(g2, true));
            }

            [Test]
            public void ShouldSplitFromDifferentSetup()
            {
                var parser = new PGNParser();
                var g1Str = PGN.Puzzle;
                var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
                g1.GoToLastMove();
                var endNode = g1.CurrentMoveNode;
                g1.GoToInitialState();
                var splitGame = g1.SplitFromMove(endNode);
                try
                {
                    Assert.IsTrue(splitGame.IsEqualTo(g1));
                }
                catch
                {
                    Console.WriteLine($"Original Game PGN{Environment.NewLine}{g1}");
                    Console.WriteLine(new string('*', 25));
                    Console.WriteLine($"Split Game PGN{Environment.NewLine}{splitGame}");
                    throw;
                }
            }

            [Test]
            public void ShouldSplitWithNoVariations()
            {
                var parser = new PGNParser();
                var g1Str = PGN.s1;
                var g2Str = PGN.s2;
                var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
                var g2 = parser.GetGamesFromPGNAsync(g2Str).Result.First();
                var move = g1.MainMoveTree.First;
                while (move != null && (move = move.Next)?.Value.SAN != "e6") ;
                var splitGame = g1.SplitFromMove(move);
                Assert.IsTrue(splitGame.IsEqualTo(g2));
            }

            [Test]
            public void ShouldSplitCorrectlyFromVariations()
            {
                var parser = new PGNParser();
                var g1Str = PGN.g1Variation;
                var g2Str = PGN.g1SplitFromVariation;
                var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
                var g2 = parser.GetGamesFromPGNAsync(g2Str).Result.First();
                var move = g1.MainMoveTree.ElementAt(4).Variations[1].First;
                var splitGame = g1.SplitFromMove(move);
                Assert.IsTrue(splitGame.IsEqualTo(g2));
            }
        }

        [TestFixture]
        public class MergeGameTests
        {
            private Game<MoveStorage> _simple01To;
            private Game<MoveStorage> _simple01From;
            private Game<MoveStorage> _simple01Expected;
            private Game<MoveStorage> _simpleVariationExpected;

            private Game<MoveStorage> _simpleVariationExpectedReversed;
            private Game<MoveStorage> _simpleVariationFrom;
            private Game<MoveStorage> _simpleVariationTo;

            [OneTimeSetUp]
            public void OneTimeSetup()
            {
                var parser = new PGNParser();

                _simple01To = parser.GetGamesFromPGNAsync(MergeGame.Simple01To).Result.First();
                _simple01From = parser.GetGamesFromPGNAsync(MergeGame.Simple01From).Result.First();
                _simple01Expected = parser.GetGamesFromPGNAsync(MergeGame.Simple01Expected).Result.First();

                _simpleVariationTo = parser.GetGamesFromPGNAsync(MergeGame.SimpleVariationTo01).Result.First();
                _simpleVariationFrom = parser.GetGamesFromPGNAsync(MergeGame.SimpleVariationFrom01).Result.First();
                _simpleVariationExpected = parser.GetGamesFromPGNAsync(MergeGame.SimpleVariationExpected01).Result.First();
                _simpleVariationExpectedReversed =
                    parser.GetGamesFromPGNAsync(MergeGame.SimpleVariationExpectedRev01).Result.First();
            }

            [Test]
            public void ShouldMergeSimpleGames_01()
            {
                var expected = _simple01Expected;
                var result = Game<MoveStorage>.MergeGames(_simple01To, _simple01From);
                Assert.IsTrue(expected.IsEqualTo(result,true));
            }
            [Test]
            public void ShouldMergeSimpleGamesRev_01()
            {
                var expected = _simple01Expected;
                var result = Game<MoveStorage>.MergeGames(_simple01From, _simple01To);
                Assert.IsTrue(expected.IsEqualTo(result, true));
            }

            [Test]
            public void ShouldMergeSimpleVariationGames_01()
            {
                var expected = _simpleVariationExpected;
                var result = Game<MoveStorage>.MergeGames(_simpleVariationTo, _simpleVariationFrom);
                Assert.IsTrue(expected.IsEqualTo(result, true));
            }
            [Test]
            public void ShouldMergeSimpleVariationGamesRev_01()
            {
                var expected = _simpleVariationExpectedReversed;
                var result = Game<MoveStorage>.MergeGames(_simpleVariationFrom, _simpleVariationTo);
                Assert.IsTrue(expected.IsEqualTo(result, true));
            }
        }
    }
}