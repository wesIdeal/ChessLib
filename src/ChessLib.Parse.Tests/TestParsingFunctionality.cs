using System;
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
        private ParsePgn _parser = new ParsePgn();
        private Game<MoveText> _largeDb;
        Tags expectedTags;
        readonly AutoResetEvent _finishedWithLargeDb = new AutoResetEvent(false);
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _columnStylePgn = PGNResources.ColumnStyle;
            _gameWithNag = PGNResources.GameWithNAG;
            _pgnDb = Encoding.UTF8.GetString(PGNResources.talLarge);
        }

        private void ParseLargeGame()
        {
            Task.Factory.StartNew(() => _parser.GetGameTexts(new AntlrInputStream(_pgnDb)))
                .ContinueWith(
                    (t) => { _finishedWithLargeDb.Set(); });
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
            var game = _parser.GetGameTexts(new AntlrInputStream(_gameWithNag)).First();

        }

        [Test]
        public void TestParsingLargeDb()
        {
            _finishedWithLargeDb.WaitOne();
            const int expectedGameCount = 2971;
            var parser = new ChessLib.Parse.PGN.ParsePgn();
            var games = parser.GetGameTexts(new AntlrInputStream(_pgnDb)).ToArray();
            Assert.AreEqual(expectedGameCount, games.Length, $"Expected {expectedGameCount} games, but found {games.Length}.");
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
