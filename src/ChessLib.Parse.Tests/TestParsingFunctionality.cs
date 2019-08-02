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

namespace ChessLib.Parse.Tests
{
    [TestFixture]
    public class TestParsingFunctionality
    {
        ~TestParsingFunctionality()
        {
            _finishedWithLargeDb.Dispose();
        }

        string _columnStylePgn;
        string _pgnDb;
        private string _gameWithNag;
        private Game<IMoveText> _withVariation;
        private ParsePgn _parser = new ParsePgn();
        private Game<IMoveText>[] _largeDb;
        Tags expectedTags;
        private Task _finishedWithLargeDb;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _columnStylePgn = PGNResources.ColumnStyle;
            _gameWithNag = PGNResources.GameWithNAG;
            _finishedWithLargeDb = ParseLargeGame();
            _withVariation = _parser.GetGameTexts(new AntlrInputStream(PGNResources.GameWithVariation)).First();
        }

        private async Task ParseLargeGame()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            _pgnDb = Encoding.UTF8.GetString(PGNResources.talLarge);
            await Task.Factory.StartNew(() => _largeDb = _parser.GetGameTexts(new AntlrInputStream(_pgnDb)).ToArray());
            sw.Stop();
            Debug.WriteLine($"Finished parsing {_largeDb.Length} games in {sw.ElapsedMilliseconds / 1000} seconds.");
        }

        [Test]
        public void TestVariationParsing()
        {
            const int variationOnMovePosition = 0;
            const int expectedVariationCount = 1;
            const string variationSAN = "c4";
            var variationMove = GetNodeAt(variationOnMovePosition, _withVariation.MoveSection);
            Assert.AreEqual(expectedVariationCount, variationMove.Variations.Count);
        }

        private MoveNode<IMoveText> GetNodeAt(int index, MoveTree<IMoveText> tree)
        {
            var count = 0;
            var rv = tree.HeadMove;
            while (count < index)
            {
                rv = rv.Next;
                count++;
            }

            return rv;
        }

        [Test]
        public void TestColumnStylePGN()
        {
            var games = _parser.GetGameTexts(new AntlrInputStream(_columnStylePgn)).ToArray();
            Assert.AreEqual(1, games.Length, $"Expected only one game, but found {games.Length}.");
            Assert.AreEqual(50, games[0].MoveSection.AsEnumerable().Count(), "Game should have 50 moves.");
        }

        [Test]
        public void TestNAGParsing()
        {
            const string expected = "$1";
            const int moveIndex = 15; //Black's move 9...Qc7
            var game = _parser.GetGameTexts(new AntlrInputStream(_gameWithNag)).First();
            var move = game.MoveSection.ElementAt(moveIndex);
            Assert.AreEqual(expected, move.NAG, $"Expected NAG to be '{expected}' at move {MoveDisplay(moveIndex, move.SAN)}.");
        }

        [Test]
        public void TestCommentParsing()
        {
            const string expected = "Qc7 is a great move, here.";
            const int movePosition = 17; //Black's move 9...Qc7
            var game = _parser.GetGameTexts(new AntlrInputStream(_gameWithNag)).First();
            var move = game.MoveSection.ElementAt(movePosition);
            Assert.AreEqual(expected, move.Comment, $"Expected comment '{expected}' at move {MoveDisplay(movePosition, move.SAN)}.");
        }

        [Test]
        public void LongWait_TestParsingLargeDb()
        {
            _finishedWithLargeDb.Wait();
            const int expectedGameCount = 2971;
            Assert.AreEqual(expectedGameCount, _largeDb.Length, $"Expected {expectedGameCount} games, but found {_largeDb.Length}.");
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
