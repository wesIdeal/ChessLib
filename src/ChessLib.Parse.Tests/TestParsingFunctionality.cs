//using System;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using ChessLib.Core;
//using ChessLib.Core.Types;
//using ChessLib.Core.Types.Enums;
//using ChessLib.Core.Types.Enums.NAG;
//using ChessLib.Core.Types.Tree;
//using ChessLib.Parse.PGN;
//using ChessLib.Parse.PGN.Base;
//using NUnit.Framework;

//// ReSharper disable PossibleNullReferenceException

//namespace ChessLib.Parse.Tests
//{
//    //[TestFixture]
//    //public class TestPgnReader
//    //{


//    //}
//    [TestFixture]
//    public class TestParsingFunctionality
//    {
//        private string AddGame(string originalDatabase, string game)
//        {
//            return originalDatabase + Environment.NewLine + Environment.NewLine + game;
//        }

//        [TestCase("[Event \"New York\"]\r\n", "Event", "New York")]
//        public void TestParsingTagPair(string tagPair, string expectedKey, string expectedValue)
//        {
//            var pgnParser = new PgnVisitor();
//            var tagKeyValuePair = pgnParser.VisitTagPair(tagPair);
//            Assert.IsNotNull(tagKeyValuePair);
//            Assert.AreEqual(expectedKey, tagKeyValuePair.Value.Key);
//            Assert.AreEqual(expectedValue, tagKeyValuePair.Value.Value);
//        }

//        [TestCase(
//            "[Event \"New York\"]\r\n[Site \"New York, NY USA\"]\r\n[Date \"1857.??.??\"]\r\n[Round \"1\"]\r\n[White \"Morphy, Paul\"]\r\n[Black \"Meek, Alexander Beaufort\"]\r\n[Result \"1-0\"]\r\n[ECO \"A43g\"]")]
//        public void TestVisitingTags(string section)
//        {
//            var parser = new PgnVisitor();

//            var tags = parser.VisitTagPairSection(section);
//            Assert.AreEqual("New York", tags.Event);
//            Assert.AreEqual("New York, NY USA", tags.Site);
//            Assert.AreEqual("1857.??.??", tags.Date);
//            Assert.AreEqual("1", tags.Round);
//            Assert.AreEqual("Morphy, Paul", tags.White);
//            Assert.AreEqual("Meek, Alexander Beaufort", tags.Black);
//            Assert.AreEqual("1-0", tags.Result);
//            Assert.AreEqual("A43g", tags["ECO"]);
//        }

//        private readonly PGNParser _parser = new PGNParser();

//        private string MoveDisplay(int moveNumber, string SAN)
//        {
//            var str = (moveNumber / 2 + 1).ToString();
//            str += moveNumber % 2 == 1 ? "... " : ". ";
//            str += SAN;
//            return str;
//        }

//        private PGNParserOptions SlavFilterOptions
//        {
//            get
//            {
//                var opts = new PGNParserOptions("rnbqkbnr/pp2pppp/2p5/3p4/2PP4/8/PP2PPPP/RNBQKBNR w KQkq - 0 3");
//                return opts;
//            }
//        }

//        private PGNParserOptions FilterByFENOptions
//        {
//            get
//            {
//                var opts = new PGNParserOptions("rnbqkbnr/pppp1ppp/8/4p3/2P5/2N5/PP1PPPPP/R1BQKBNR b KQkq - 1 2");
//                return opts;
//            }
//        }

//        public const string EnglishReverseSicilian = "rnbqkbnr/pppp1ppp/8/4p3/2P5/2N5/PP1PPPPP/R1BQKBNR b KQkq - 1 2";

//        private PGNParserOptions MoveCountFilterOptions(int moveCount)
//        {
//            var opts = new PGNParserOptions(EnglishReverseSicilian, moveCount);
//            return opts;
//        }

//        private void WritePgn(Game game)
//        {
//            var opts = PGNFormatterOptions.ExportFormatOptions;
//            var parser = new PgnFormatter<Move>(opts);
//            Console.WriteLine(new string('*', 20));
//            Console.WriteLine(parser.BuildPgn(game));
//            Console.WriteLine(new string('*', 20));
//        }

//        public void Filter_SanityCheck(string filterDb, int expectedGameCount)
//        {
//            var parser = new PGNParser();
//            var games = parser.GetGamesFromPGNAsync(filterDb).Result.ToArray();
//            Assert.AreEqual(expectedGameCount, games.Length, "Failed Sanity Check for Filter Games");
//        }

//        protected string FilterDb => AddGame(PGNResources.EnglishRevSic, PGNResources.TranspositionToFilter);

//        [Test]
//        public void Filter_IsValidFilterDatabase()
//        {
//            var pgnDb = PGNResources.EnglishRevSic;
//            var sw = new Stopwatch();
//            sw.Start();
//            var largeDb = _parser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
//            sw.Stop();
//            Debug.WriteLine($"Finished parsing {largeDb.Count()} games in {sw.ElapsedMilliseconds / 1000} seconds");
//            const int expectedGameCount = 10;
//            Assert.AreEqual(expectedGameCount, largeDb.Length,
//                $"Expected {expectedGameCount} games from filter database, but found {largeDb.Length}.");
//        }

//        [Test]
//        public void Filter_ShouldBeAbleToCombineFenAndMaxPlyFilter()
//        {
//            var options = FilterByFENOptions;
//            options.MaximumPlyPerGame = 6;
//            var filterParser = new PGNParser(options);
//            var pgnDb = PGNResources.EnglishRevSic;
//            pgnDb += Environment.NewLine + Environment.NewLine + PGNResources.FilterOut;
//            var filteredGames = filterParser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
//            var nonFilterCount = _parser.GetGamesFromPGNAsync(pgnDb).Result.Count();
//            Assert.AreEqual(11, nonFilterCount, "Expected 11 total games to be parsed.");
//            const int expectedGameCount = 10;
//            Assert.AreEqual(expectedGameCount, filteredGames.Length,
//                $"Expected {expectedGameCount} games from filter database, but found {filteredGames.Length}.");
//            Assert.IsTrue(filteredGames.All(x => x.PlyCount == options.MaximumPlyPerGame));
//        }

//        [Test]
//        public void Filter_ShouldFilterGameIfShorterThanFenPlyLimit()
//        {
//            var filterParser = new PGNParser(MoveCountFilterOptions(2));
//            var nonFilterParser = new PGNParser();
//            var pgnDb = PGNResources.FilterTooShort;
//            var filteredGames = filterParser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
//            var nonFilterCount = nonFilterParser.GetGamesFromPGNAsync(pgnDb).Result.Count();
//            Assert.AreEqual(1, nonFilterCount, "Expected 1 total games to be parsed.");
//            const int expectedGameCount = 0;
//            Assert.AreEqual(expectedGameCount, filteredGames.Length,
//                $"Expected {expectedGameCount} games from filter database, but found {filteredGames.Length}.");
//        }

//        [Test]
//        public void Filter_ShouldFilterOneGame()
//        {
//            var filterParser = new PGNParser(FilterByFENOptions);
//            var pgnDb = AddGame(PGNResources.EnglishRevSic, PGNResources.FilterOut);
//            var filteredGames = filterParser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
//            var nonFilterCount = _parser.GetGamesFromPGNAsync(pgnDb).Result.Count();
//            Assert.AreEqual(11, nonFilterCount, "Expected 11 total games to be parsed.");
//            const int expectedGameCount = 10;
//            Assert.AreEqual(expectedGameCount, filteredGames.Length,
//                $"Expected {expectedGameCount} games from filter database, but found {filteredGames.Length}.");
//        }

//        [Test]
//        public void Filter_ShouldFindSlavGamesInLargeDb()
//        {
//            var filterParser = new PGNParser(SlavFilterOptions);
//            var pgnDb = Encoding.UTF8.GetString(PGNResources.talLarge);
//            var filteredGames = filterParser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
//            const int expectedGameCount = 33;
//            Assert.AreEqual(expectedGameCount, filteredGames.Length,
//                $"Expected {expectedGameCount} games from filter database, but found {filteredGames.Length}.");
//        }

//        [Test]
//        public void Filter_ShouldReturnAllGames()
//        {
//            var parser = new PGNParser(FilterByFENOptions);
//            var pgnDb = PGNResources.EnglishRevSic;
//            var sw = new Stopwatch();
//            sw.Start();
//            var largeDb = parser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
//            sw.Stop();
//            Debug.WriteLine($"Finished parsing {largeDb.Count()} games in {sw.ElapsedMilliseconds / 1000} seconds");
//            const int expectedGameCount = 10;
//            Assert.AreEqual(expectedGameCount, largeDb.Length,
//                $"Expected {expectedGameCount} games from filter database, but found {largeDb.Length}.");
//        }

//        /// <summary>
//        ///     An extensive test for comments, variations and nags in a real db scenario
//        /// </summary>
//        [Test]
//        public void LongWait_TestParsingLargeDb()
//        {
//            var pgnDb = Encoding.UTF8.GetString(PGNResources.talMedium);
//            var sw = new Stopwatch();
//            sw.Start();
//            var largeDb = _parser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
//            sw.Stop();
//            Debug.WriteLine($"Finished parsing {largeDb.Length} games in {sw.ElapsedMilliseconds / 1000} seconds");
//            const int expectedGameCount = 1000;
//            Assert.AreEqual(expectedGameCount, largeDb.Length,
//                $"Expected {expectedGameCount} games, but found {largeDb.Length}.");
//        }

//        [Test(Description = "Filters out a game that meets filter, but in too many plies.")]
//        public void ShouldFilterTranspositionalGame()
//        {
//            var pgnDatabase = FilterDb;
//            Filter_SanityCheck(pgnDatabase, 11);
//            var filterParser = new PGNParser(MoveCountFilterOptions(3));
//            var filteredGames = filterParser.GetGamesFromPGNAsync(pgnDatabase).Result.ToArray();
//            const int expectedGameCount = 10;
//            Assert.AreEqual(expectedGameCount, filteredGames.Length,
//                $"Expected {expectedGameCount} games from filter database, but found {filteredGames.Length}.");
//        }


//        [Test]
//        public void ShouldIgnoreVariationsWhenSetToTrue()
//        {
//            var pgnDb = PGNResources.GameWithVars;
//            var parserOptions = new PGNParserOptions {IgnoreVariations = true};
//            var parser = new PGNParser(parserOptions);
//            var largeDb = parser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
//            foreach (var move in largeDb.First().MainLine())
//            {
//                Assert.IsEmpty(move.Continuations,
//                    $"Found a variation on move {move.Value.San} and shouldn't have any.");
//            }
//        }

//        [Test(Description = "Includes game that meets filter by transposition.")]
//        public void ShouldNotFilterTranspositionalGame()
//        {
//            var pgnDatabase = FilterDb;
//            Filter_SanityCheck(pgnDatabase, 11);
//            var filterParser = new PGNParser(MoveCountFilterOptions(7));
//            var filteredGames = filterParser.GetGamesFromPGNAsync(pgnDatabase).Result.ToArray();
//            const int expectedGameCount = 11;
//            Assert.AreEqual(expectedGameCount, filteredGames.Length,
//                $"Expected {expectedGameCount} games from filter database, but found {filteredGames.Length}.");
//        }

//        [Test]
//        public void ShouldParseNAGNumbers()
//        {
//            const MoveNAG expected = MoveNAG.VeryGoodMove;
//            var parser = new PGNParser();
//            var game = parser.GetGamesFromPGNAsync(PGNResources.MoveNagNumber).Result.First();
//            while (game.MoveNext())
//            {
//            }

//            Assert.AreEqual(expected, game.CurrentAnnotation.MoveNAG);
//        }

//        [Test]
//        public void ShouldParseNAGSymbols()
//        {
//            const MoveNAG expected = MoveNAG.VeryGoodMove;
//            var parser = new PGNParser();
//            var game = parser.GetGamesFromPGNAsync(PGNResources.MoveNagSymbol).Result.First();
//            while (game.MoveNext())
//            {
//            }

//            Assert.AreEqual(expected, game.CurrentAnnotation.MoveNAG);
//        }

//        [Test]
//        public void ShouldParsePreGameComment()
//        {
//            var game = _parser.GetGamesFromPGNAsync(PGNResources.PregameComment).Result.First();
//            Assert.IsNotEmpty(game.InitialNode.Node.Comment);
//        }

//        [Test]
//        public void ShouldParsePromotion()
//        {
//            var pgnWithPromotion = PGNResources.GameWithPromotion;
//            var parser = new PGNParser();
//            var game = parser.GetGamesFromPGNAsync(pgnWithPromotion).Result.ToArray();
//            Assert.AreEqual(1, game.Length);
//            Assert.AreEqual(80, game.First().PlyCount);
//        }

//        [Test]
//        public void ShouldRespectMaxGameCount()
//        {
//            const int gamesToParse = 2;
//            var pgnDb = Encoding.UTF8.GetString(PGNResources.talMedium);
//            var parserOptions = new PGNParserOptions {MaxGameCount = 2};
//            var parser = new PGNParser(parserOptions);
//            var largeDb = parser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
//            Assert.AreEqual(gamesToParse, largeDb.Length,
//                $"Expected {gamesToParse} games, but found {largeDb.Length}.");
//        }

//        [Test]
//        public void ShouldRespectMaxPlyCount()
//        {
//            const int maxPliesToParse = 10;
//            var pgnDb = Encoding.UTF8.GetString(PGNResources.talMedium);
//            var parserOptions = new PGNParserOptions {MaxGameCount = 5, MaximumPlyPerGame = maxPliesToParse};
//            var parser = new PGNParser(parserOptions);
//            var largeDb = parser.GetGamesFromPGNAsync(pgnDb).Result.ToArray();
//            WritePgn(largeDb.First());
//            foreach (var game in largeDb)
//            {
//                Assert.IsTrue(game.PlyCount <= maxPliesToParse,
//                    $"Expected ply count of {maxPliesToParse} but was {game.PlyCount}.");
//            }
//        }

//        [Test]
//        public void ShouldSetResult()
//        {
//            var parser = new PGNParser();
//            var games = parser.GetGamesFromPGNAsync(PGNResources.AllResults).Result.ToArray();
//            Assert.AreEqual(4, games.Length);
//            Assert.AreEqual(GameResult.WhiteWins, games[0].GameResult);
//            Assert.AreEqual(GameResult.Draw, games[1].GameResult);
//            Assert.AreEqual(GameResult.BlackWins, games[2].GameResult);
//            Assert.AreEqual(GameResult.None, games[3].GameResult);

//            Assert.AreEqual("1-0", games[0].Tags["Result"]);
//            Assert.AreEqual("1/2-1/2", games[1].Tags["Result"]);
//            Assert.AreEqual("0-1", games[2].Tags["Result"]);
//            Assert.AreEqual("*", games[3].Tags["Result"]);
//        }

//        [Test]
//        public void TestColumnStylePGN()
//        {
//            var games = _parser.GetGamesFromPGNAsync(PGNResources.ColumnStyle).Result.ToArray();
//            Assert.AreEqual(1, games.Length, $"Expected only one game, but found {games.Length}.");
//            Assert.AreEqual(51, games[0].MainLine().Count(), "Game should have 50 moves.");
//        }

//        [Test]
//        public void TestCommentParsing()
//        {
//            const string expected = "Qc7 is a great move, here.";
//            const int movePosition = 18; //Black's move 9...Qc7
//            var game = _parser.GetGamesFromPGNAsync(PGNResources.GameWithNAG).Result.First();
//            var move = game.MainLine().ElementAt(movePosition);
//            MoveTreeNode<PostMoveState> moveWithComment = null;
//            while (game.MoveNext() && !game.Current.Node.Value.Equals(move.Value))
//            {
//            }

//            Assert.AreEqual(expected, moveWithComment.Comment,
//                $"Expected comment '{expected}' at move {MoveDisplay(movePosition, game.Current.Node.Value.San)}.");
//        }

//        [Test]
//        public void TestMoveParsing_NoVariations()
//        {
//            var pgn = PGNResources.Simple;
//            var parser = new PGNParser();
//            var games = parser.GetGamesFromPGNAsync(pgn).Result.ToArray();
//            Assert.IsNotEmpty(games);
//            Assert.IsTrue(games.First().MainLine().Count() == 40);
//        }

//        [Test]
//        public void TestMoveParsing_WithCommentsAndVariations()
//        {
//            var parser = new PGNParser();
//            var games = parser.GetGamesFromPGNAsync(PGNResources.VariationsAndComments).Result.ToArray();
//            Assert.IsNotEmpty(games);
//            Assert.AreEqual(113, games.First().PlyCount);
//        }

//        [Test]
//        public void TestMoveParsing_WithCommentsAndVariations02()
//        {
//            var parser = new PGNParser();
//            var games = parser.GetGamesFromPGNAsync(PGNResources.Variation02).Result.ToArray();
//            Assert.IsNotEmpty(games);
//            Assert.AreEqual(80, games.First().PlyCount);
//        }

//        [Test]
//        public void TestNAGParsing()
//        {
//            const string expected = "$1";
//            const int moveIndex = 16;
//            var game = _parser.GetGamesFromPGNAsync(PGNResources.GameWithNAG).Result.First();
//            var search = game.MainLine().ElementAt(moveIndex);
//            while (game.MoveNext() && game.FindMoveIndexInContinuations(search.Value.MoveValue) != 0)
//            {
//            }

//            game.MoveNext();
//            Assert.AreEqual(MoveNAG.GoodMove, game.CurrentAnnotation.MoveNAG,
//                $"Expected NAG to be '{expected}' at move {MoveDisplay(moveIndex, game.CurrentSan)}.");
//        }


//        [Test]
//        public void TestRealGameParsing()
//        {
//            var pgn = PGNResources.GameWithVars;
//            var game = _parser.GetGamesFromPGNAsync(pgn).Result.ToArray();
//            Assert.AreEqual(1, game.Length, $"Expected only one game, but found {game.Length}.");
//        }

//        [Test]
//        public void TestRealGameParsingVarsAndComments()
//        {
//            var pgn = PGNResources.VariationsAndComments;
//            var game = _parser.GetGamesFromPGNAsync(pgn).Result.ToArray();
//            Assert.AreEqual(1, game.Length, $"Expected only one game, but found {game.Length}.");
//        }

//        [Test]
//        public void TestSimpleGameParsing()
//        {
//            var pgn = PGNResources.Simple;
//            var game = _parser.GetGamesFromPGNAsync(pgn).Result.ToArray();
//            Assert.AreEqual(1, game.Length, $"Expected only one game, but found {game.Length}.");
//        }

//        [Test]
//        public void TestSimpleGameParsing_FiveGames()
//        {
//            var parser = new PGNParser();
//            var games = parser.GetGamesFromPGNAsync(PGNResources.FiveGames).Result;
//            var gameCount = games.Count();
//            Assert.AreEqual(5, gameCount, $"Expected five games, but found {gameCount}.");
//        }

//        [Test]
//        public void TestSimpleGameParsing_OneGame()
//        {
//            var parser = new PGNParser();
//            var games = parser.GetGamesFromPGNAsync(PGNResources.Simple).Result;
//            var gameCount = games.Count();
//            Assert.AreEqual(1, gameCount, $"Expected only one game, but found {gameCount}.");
//        }

//        [Test]
//        public void TestSmallPgnGame()
//        {
//            var games = _parser.GetGamesFromPGNAsync(PGNResources.smallPgn).Result.ToArray();
//            Assert.AreEqual(1, games.Length, $"Expected only one game, but found {games.Length}.");
//            //Assert.AreEqual(51, games[0].MainMoveTree.Count(), "Game should have 50 moves.");
//        }

//        [Test]
//        public void TestSplittingSections()
//        {
//            var pgn = PGNResources.GameWithNAG;
//            var sections = PgnLexer.GetSectionsFromPgn(pgn);
//            Assert.IsFalse(string.IsNullOrWhiteSpace(sections.tagSection));
//            Assert.IsFalse(string.IsNullOrWhiteSpace(sections.moveSection));
//        }


//        [Test]
//        public void TestVariationParsing()
//        {
//            const int variationOnMovePosition = 1;
//            const int expectedVariationCount = 1;
//            const string variationSAN = "c4";
//            var withVarDb = _parser.GetGamesFromPGNAsync(PGNResources.GameWithVariation).Result.First();
//            var variationMove = withVarDb.MainLine().ElementAt(variationOnMovePosition);
//            Assert.AreEqual(expectedVariationCount, variationMove.Continuations.Skip(1).Count());
//            Assert.AreEqual(variationSAN, variationMove.Continuations.First().Value.San);
//        }

//        //[Test]
//        //public void ShouldRetrieveTagsWithNoNewLines()
//        //{
//        //    var actualTags = GetTagValues(tagsNoNewLines);
//        //    Assert.AreEqual(expectedTags, actualTags);
//        //}
//        //[Test]
//        //public void ShouldRetrieveTagsWithRandomWhitespace()
//        //{
//        //    var actualTags = GetTagValues(tagsRandomWhiteSpace);
//        //    Console.WriteLine(tagsRandomWhiteSpace);
//        //    Assert.AreEqual(expectedTags, actualTags);
//        //}
//        //[Test]
//        //public void RemoveCommentsShouldRemoveAllComments()
//        //{
//        //    var s = RemoveTags(commentedPgn, out Dictionary<string, string> d);
//        //    Assert.AreEqual(unCommentedPgn, RemoveComments(commentedPgn));
//        //}
//    }
//}