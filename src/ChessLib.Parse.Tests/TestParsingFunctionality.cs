using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.MoveRepresentation.NAG;
using ChessLib.Parse.PGN;
using NUnit.Framework;

namespace ChessLib.Parse.Tests
{
    [TestFixture]
    public class TestPgnReader
    {

        [Test]
        public void TestSimpleGameParsing_OneGame()
        {
            var pgnReader = GetReader(PGNResources.Simple);
            var game = pgnReader.Parse();
            var gameCount = pgnReader.GameCount;
            Assert.AreEqual(1, gameCount, $"Expected only one game, but found {gameCount}.");
        }

        [Test]
        public void TestSimpleGameParsing_FiveGames()
        {
            var pgnReader = GetReader(PGNResources.FiveGames);
            var game = pgnReader.Parse();
            var gameCount = pgnReader.GameCount;
            Assert.AreEqual(5, gameCount, $"Expected five games, but found {gameCount}.");
        }

        [TestCase("[Event \"New York\"]\r\n", "Event", "New York")]
        public void TestParsingTagPair(string tagPair, string expectedKey, string expectedValue)
        {
            var pgnParser = new PgnParser();
            using (var reader = new StringReader(tagPair))
            {
                pgnParser.VisitTagPair(reader);
                Assert.IsTrue(pgnParser.Game.TagSection.ContainsKey(expectedKey));
                Assert.AreEqual(expectedValue, pgnParser.Game.TagSection[expectedKey]);
            }
        }

        private PgnReader GetReader(string strPgn)
        {
            return new PgnReader(strPgn);
        }
    }
    [TestFixture]
    public class TestParsingFunctionality
    {
        private readonly PGNParser _parser = new PGNParser();




        private string MoveDisplay(int moveNumber, string SAN)
        {
            var str = (moveNumber / 2 + 1).ToString();
            str += moveNumber % 2 == 1 ? "... " : ". ";
            str += SAN;
            return str;
        }

        private PGNParserOptions SlavFilterOptions
        {
            get
            {
                var opts = new PGNParserOptions();
                opts.SetFenFiltering("rnbqkbnr/pp2pppp/2p5/3p4/2PP4/8/PP2PPPP/RNBQKBNR w KQkq - 0 3");
                return opts;
            }
        }

        private PGNParserOptions DefaultFilterOptions
        {
            get
            {
                var opts = new PGNParserOptions();
                opts.SetFenFiltering("rnbqkbnr/pppp1ppp/8/4p3/2P5/2N5/PP1PPPPP/R1BQKBNR b KQkq - 1 2");
                return opts;
            }
        }

        private PGNParserOptions MoveCountFilterOptions(int moveCount)
        {
            var opts = new PGNParserOptions();
            opts.SetFenFiltering("rnbqkbnr/pppp1ppp/8/4p3/2P5/2N5/PP1PPPPP/R1BQKBNR b KQkq - 1 2", moveCount);
            return opts;
        }

        private void WritePgn(Game<MoveStorage> game)
        {
            var opts = PGNFormatterOptions.ExportFormatOptions;
            var parser = new PGNFormatter<MoveStorage>(opts);
            Console.WriteLine(new string('*', 20));
            Console.WriteLine(parser.BuildPGN(game));
            Console.WriteLine(new string('*', 20));
        }

        [Test]
        public void Filter_IncreasingFilterPlyShouldIncludeExtraGame()
        {
            var filterParser = new PGNParser(MoveCountFilterOptions(4));
            var pgnDb = PGNResources.EnglishRevSic;
            pgnDb += Environment.NewLine + PGNResources.TranspositionToFilter;
            var filteredGames = filterParser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
            var nonFilterCount = _parser.GetGamesFromPGNAsync(pgnDb).Result.Count();
            Assert.AreEqual(11, nonFilterCount, "Expected 11 total games to be parsed.");
            const int expectedGameCount = 11;
            Assert.AreEqual(expectedGameCount, filteredGames.Length,
                $"Expected {expectedGameCount} games from filter database, but found {filteredGames.Length}.");
        }

        [Test]
        public void Filter_IsValidFilterDatabase()
        {
            var pgnDb = PGNResources.EnglishRevSic;
            var sw = new Stopwatch();
            sw.Start();
            var largeDb = _parser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
            sw.Stop();
            Debug.WriteLine($"Finished parsing {largeDb.Count()} games in {sw.ElapsedMilliseconds / 1000} seconds");
            const int expectedGameCount = 10;
            Assert.AreEqual(expectedGameCount, largeDb.Length,
                $"Expected {expectedGameCount} games from filter database, but found {largeDb.Length}.");
        }

        [Test]
        public void Filter_ShouldFilterGameIfShorterThanFenPlyLimit()
        {
            var filterParser = new PGNParser(DefaultFilterOptions);
            var pgnDb = PGNResources.FilterTooShort;
            var filteredGames = filterParser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
            var nonFilterCount = _parser.GetGamesFromPGNAsync(pgnDb).Result.Count();
            Assert.AreEqual(1, nonFilterCount, "Expected 1 total games to be parsed.");
            const int expectedGameCount = 0;
            Assert.AreEqual(expectedGameCount, filteredGames.Length,
                $"Expected {expectedGameCount} games from filter database, but found {filteredGames.Length}.");
        }

        [Test]
        public void Filter_ShouldFilterOneGame()
        {
            var filterParser = new PGNParser(DefaultFilterOptions);
            var pgnDb = PGNResources.EnglishRevSic;
            pgnDb += Environment.NewLine + PGNResources.FilterOut;
            var filteredGames = filterParser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
            var nonFilterCount = _parser.GetGamesFromPGNAsync(pgnDb).Result.Count();
            Assert.AreEqual(11, nonFilterCount, "Expected 11 total games to be parsed.");
            const int expectedGameCount = 10;
            Assert.AreEqual(expectedGameCount, filteredGames.Length,
                $"Expected {expectedGameCount} games from filter database, but found {filteredGames.Length}.");
        }

        [Test]
        public void Filter_ShouldBeAbleToCombineFenAndMaxPlyFilter()
        {
            var options = DefaultFilterOptions;
            options.MaximumPlyPerGame = 6;
            var filterParser = new PGNParser(options);
            var pgnDb = PGNResources.EnglishRevSic;
            pgnDb += Environment.NewLine + PGNResources.FilterOut;
            var filteredGames = filterParser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
            var nonFilterCount = _parser.GetGamesFromPGNAsync(pgnDb).Result.Count();
            Assert.AreEqual(11, nonFilterCount, "Expected 11 total games to be parsed.");
            const int expectedGameCount = 10;
            Assert.AreEqual(expectedGameCount, filteredGames.Length,
                $"Expected {expectedGameCount} games from filter database, but found {filteredGames.Length}.");
            Assert.IsTrue(filteredGames.All(x => x.PlyCount == options.MaximumPlyPerGame));

        }

        [Test]
        public void Filter_ShouldFilterOneGameThatTransposesTooLate()
        {
            var filterParser = new PGNParser(DefaultFilterOptions);
            var pgnDb = PGNResources.EnglishRevSic;
            pgnDb += Environment.NewLine + PGNResources.TranspositionToFilter;
            var filteredGames = filterParser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
            var nonFilterCount = _parser.GetGamesFromPGNAsync(pgnDb).Result.Count();
            Assert.AreEqual(11, nonFilterCount, "Expected 11 total games to be parsed.");
            const int expectedGameCount = 10;
            Assert.AreEqual(expectedGameCount, filteredGames.Length,
                $"Expected {expectedGameCount} games from filter database, but found {filteredGames.Length}.");
        }

        [Test]
        public void Filter_ShouldFindSlavGamesInLargeDb()
        {
            var filterParser = new PGNParser(SlavFilterOptions);
            var pgnDb = Encoding.UTF8.GetString(PGNResources.talLarge);
            var filteredGames = filterParser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
            const int expectedGameCount = 33;
            Assert.AreEqual(expectedGameCount, filteredGames.Length,
                $"Expected {expectedGameCount} games from filter database, but found {filteredGames.Length}.");
        }

        [Test]
        public void Filter_ShouldReturnAllGames()
        {
            var parser = new PGNParser(DefaultFilterOptions);
            var pgnDb = PGNResources.EnglishRevSic;
            var sw = new Stopwatch();
            sw.Start();
            var largeDb = parser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
            sw.Stop();
            Debug.WriteLine($"Finished parsing {largeDb.Count()} games in {sw.ElapsedMilliseconds / 1000} seconds");
            const int expectedGameCount = 10;
            Assert.AreEqual(expectedGameCount, largeDb.Length,
                $"Expected {expectedGameCount} games from filter database, but found {largeDb.Length}.");
        }

        /// <summary>
        ///     An extensive test for comments, variations and nags in a real db scenario
        /// </summary>
        [Test]
        public void LongWait_TestParsingLargeDb()
        {
            var pgnDb = Encoding.UTF8.GetString(PGNResources.talMedium);
            var sw = new Stopwatch();
            sw.Start();
            var largeDb = _parser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
            sw.Stop();
            Debug.WriteLine($"Finished parsing {largeDb.Count()} games in {sw.ElapsedMilliseconds / 1000} seconds");
            const int expectedGameCount = 1000;
            Assert.AreEqual(expectedGameCount, largeDb.Length,
                $"Expected {expectedGameCount} games, but found {largeDb.Length}.");
        }


        [Test]
        public void ShouldIgnoreVariationsWhenSetToTrue()
        {
            const int maxPliesToParse = 10;
            var pgnDb = PGNResources.GameWithVars;
            var parserOptions = new PGNParserOptions { IgnoreVariations = true };
            var parser = new PGNParser(parserOptions);
            var largeDb = parser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
            foreach (var move in largeDb.First().MainMoveTree)
            {
                Assert.IsEmpty(move.Variations, $"Found a variation on move {move.SAN} and shouldn't have any.");
            }
        }

        [Test]
        public void ShouldParsePreGameComment()
        {
            var game = _parser.GetGamesFromPGNAsync(PGNResources.PregameComment).Result.First();
            Assert.IsNotEmpty(game.MainMoveTree.First.Value.Comment);
        }

        [Test]
        public void ShouldRespectMaxGameCount()
        {
            const int gamesToParse = 2;
            var pgnDb = Encoding.UTF8.GetString(PGNResources.talMedium);
            var parserOptions = new PGNParserOptions { GameCountToParse = 2 };
            var parser = new PGNParser(parserOptions);
            var largeDb = parser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
            Assert.AreEqual(gamesToParse, largeDb.Length,
                $"Expected {gamesToParse} games, but found {largeDb.Length}.");
        }

        [Test]
        public void ShouldRespectMaxPlyCount()
        {
            const int maxPliesToParse = 10;
            var pgnDb = Encoding.UTF8.GetString(PGNResources.talMedium);
            var parserOptions = new PGNParserOptions { GameCountToParse = 5, MaximumPlyPerGame = maxPliesToParse };
            var parser = new PGNParser(parserOptions);
            var largeDb = parser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
            WritePgn(largeDb.First());
            foreach (var game in largeDb)
            {
                Assert.IsTrue(game.PlyCount <= maxPliesToParse,
                    $"Expected ply count of {maxPliesToParse} but was {game.MainMoveTree.Count}.");
            }
        }

        [Test]
        public void TestColumnStylePGN()
        {
            var games = _parser.GetGamesFromPGNAsync(PGNResources.ColumnStyle).Result.ToArray();
            Assert.AreEqual(1, games.Length, $"Expected only one game, but found {games.Length}.");
            Assert.AreEqual(51, games[0].MainMoveTree.Count(), "Game should have 50 moves.");
        }

        [Test]
        public void TestCommentParsing()
        {
            const string expected = "Qc7 is a great move, here.";
            const int movePosition = 18; //Black's move 9...Qc7
            var game = _parser.GetGamesFromPGNAsync(PGNResources.GameWithNAG).Result.First();
            var move = game.MainMoveTree.ElementAt(movePosition);
            Assert.AreEqual(expected, move.Comment,
                $"Expected comment '{expected}' at move {MoveDisplay(movePosition, move.SAN)}.");
        }

        [Test]
        public void TestNAGParsing()
        {
            const string expected = "$1";
            const int moveIndex = 16;
            var game = _parser.GetGamesFromPGNAsync(PGNResources.GameWithNAG).Result.First();
            var move = game.MainMoveTree.ElementAt(moveIndex);
            Assert.AreEqual(MoveNAG.GoodMove, move.Annotation.MoveNAG,
                $"Expected NAG to be '{expected}' at move {MoveDisplay(moveIndex, move.SAN)}.");
        }

        [Test]
        public void TestRealGameParsing()
        {
            var pgn = PGNResources.GameWithVars;
            var game = _parser.GetGamesFromPGNAsync(pgn).Result.ToArray();
            Assert.AreEqual(1, game.Length, $"Expected only one game, but found {game.Length}.");
        }

        [Test]
        public void TestRealGameParsingVarsAndComments()
        {
            var pgn = PGNResources.VariationsAndComments;
            var game = _parser.GetGamesFromPGNAsync(pgn).Result.ToArray();
            Assert.AreEqual(1, game.Length, $"Expected only one game, but found {game.Length}.");
        }

        [Test]
        public void TestSimpleGameParsing()
        {
            var pgn = PGNResources.Simple;
            var game = _parser.GetGamesFromPGNAsync(pgn).Result.ToArray();
            Assert.AreEqual(1, game.Length, $"Expected only one game, but found {game.Length}.");
        }

        [Test]
        public void TestSmallPgnGame()
        {
            var games = _parser.GetGamesFromPGNAsync(PGNResources.smallPgn).Result.ToArray();
            Assert.AreEqual(1, games.Length, $"Expected only one game, but found {games.Length}.");
            //Assert.AreEqual(51, games[0].MainMoveTree.Count(), "Game should have 50 moves.");
        }


        [Test]
        public void TestVariationParsing()
        {
            const int variationOnMovePosition = 1;
            const int expectedVariationCount = 1;
            const string variationSAN = "c4";
            var withVarDb = _parser.GetGamesFromPGNAsync(PGNResources.GameWithVariation).Result.First();
            var variationMove = withVarDb.MainMoveTree.ElementAt(variationOnMovePosition);
            Assert.AreEqual(expectedVariationCount, variationMove.Variations.Count);
            Assert.AreEqual(variationSAN, variationMove.Variations[0].ElementAt(0).SAN);
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