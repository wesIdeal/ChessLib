using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using ChessLib.Data;
using ChessLib.Parse.PGN;
using NUnit.Framework;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Antlr4.Runtime;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Interfaces;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using System.Collections.Generic;

namespace ChessLib.Parse.Tests
{
    [TestFixture]
    public class TestParsingFunctionality
    {
        ~TestParsingFunctionality()
        {
            _finishedWithLargeDb.Dispose();
        }


        private Game<MoveStorage> _withVariation;
        private ParsePgn _parser = new ParsePgn();
        private Game<MoveStorage>[] _largeDb;
        Tags expectedTags;
        private Task _finishedWithLargeDb;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //  _finishedWithLargeDb = ParseLargeGame();
            //_withVariation = _parser.GetGamesFromPGN(PGNResources.GameWithVariation).First();
        }


        [Test]
        public void TestVariationParsing()
        {
            const int variationOnMovePosition = 0;
            const int expectedVariationCount = 1;
            const string variationSAN = "c4";
            var variationMove = GetNodeAt(variationOnMovePosition, _withVariation.MainMoveTree);
            Assert.AreEqual(expectedVariationCount, variationMove.Value.Variations.Count);
        }

        private LinkedListNode<MoveStorage> GetNodeAt(int index, MoveTree tree)
        {
            var count = 0;
            var rv = tree.First;
            while (count < index)
            {
                rv = rv.Next;
                count++;
            }

            return rv;
        }

        [Test]
        public void TestSimpleGameParsing()
        {
            var pgn = PGNResources.Simple;
            var game = _parser.GetGamesFromPGN(pgn).ToArray();
            Assert.AreEqual(1, game.Length, $"Expected only one game, but found {game.Length}.");
        }
        [Test]
        public void TestRealGameParsing()
        {
            var pgn = PGNResources.GameWithVars;
            var game = _parser.GetGamesFromPGN(pgn).ToArray();
            Assert.AreEqual(1, game.Length, $"Expected only one game, but found {game.Length}.");

        }
        [Test]
        public void TestRealGameParsingVarsAndComments()
        {
            var pgn = PGNResources.VariationsAndComments;
            var game = _parser.GetGamesFromPGN(pgn).ToArray();
            Assert.AreEqual(1, game.Length, $"Expected only one game, but found {game.Length}.");

        }
        [Test]
        public void TestColumnStylePGN()
        {
            var games = _parser.GetGamesFromPGN(PGNResources.ColumnStyle).ToArray();
            Assert.AreEqual(1, games.Length, $"Expected only one game, but found {games.Length}.");
            Assert.AreEqual(50, games[0].MainMoveTree.Count(), "Game should have 50 moves.");
        }

        [Test]
        public void TestNAGParsing()
        {
            const string expected = "$1";
            const int moveIndex = 15; //Black's move 9...Qc7
            var game = _parser.GetGamesFromPGN(PGNResources.GameWithNAG).First();
            var move = game.MainMoveTree.ElementAt(moveIndex);
            Assert.AreEqual(expected, move.Annotation, $"Expected NAG to be '{expected}' at move {MoveDisplay(moveIndex, move.SAN)}.");
        }

        [Test]
        public void TestCommentParsing()
        {
            const string expected = "Qc7 is a great move, here.";
            const int movePosition = 17; //Black's move 9...Qc7
            var game = _parser.GetGamesFromPGN(PGNResources.GameWithNAG).First();
            var move = game.MainMoveTree.ElementAt(movePosition);
            Assert.AreEqual(expected, move.Comment, $"Expected comment '{expected}' at move {MoveDisplay(movePosition, move.SAN)}.");
        }

        [Test]
        public void LongWait_TestParsingLargeDb()
        {
            var pgnDb = Encoding.UTF8.GetString(PGNResources.talLarge);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var largeDb = _parser.GetGamesFromPGN(pgnDb).ToArray();
            sw.Stop();
            Debug.WriteLine($"Finished parsing {largeDb.Count()} games in {sw.ElapsedMilliseconds / 1000} seconds");
            const int expectedGameCount = 2971;
            Assert.AreEqual(expectedGameCount, largeDb.Length, $"Expected {expectedGameCount} games, but found {_largeDb.Length}.");
        }

        private string MoveDisplay(int moveNumber, string SAN)
        {
            var str = ((moveNumber / 2) + 1).ToString();
            str += moveNumber % 2 == 1 ? "... " : ". ";
            str += SAN;
            return str;
        }
        //[Test]
        //public void ShouldRetrieveTagsWithNoNewLines()
        //{
        //    var actualTags = GetTagValues(tagsNoNewLines);
        //    Assert.AreEqual(expectedTags, actualTags);
        //}
        //[Test]
        //public void ShouldRetrieveTagsWithRandomWhitespace()
        //{
        //    var actualTags = GetTagValues(tagsRandomWhiteSpace);
        //    Console.WriteLine(tagsRandomWhiteSpace);
        //    Assert.AreEqual(expectedTags, actualTags);
        //}
        //[Test]
        //public void RemoveCommentsShouldRemoveAllComments()
        //{
        //    var s = RemoveTags(commentedPgn, out Dictionary<string, string> d);
        //    Assert.AreEqual(unCommentedPgn, RemoveComments(commentedPgn));
        //}

    }
}
