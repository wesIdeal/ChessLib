using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Types
{
    [TestFixture]
    public class _moveFormatterTests
    {
        private readonly PgnFormatter _moveFormatter =
            new PgnFormatter(new PGNFormatterOptions { ResultOnNewLine = true });

        [TestCase("2bq1rk1/3p1npp/p1p3N1/1rbB1Pp1/1pQ5/P5N1/1PP3PP/R3R2K w - - 0 23", "d5f7", "23. Bxf7+")]
        public void TestCheckDisplay(string fen, string lan, string expected)
        {
            var game = new Game(fen);
            var lanToMove = new LanToMove();
            var move = lanToMove.Translate(lan);
            game.ApplyMove(move, MoveApplicationStrategy.ContinueMainLine);
            var expectedOutput =
                string.Format("[Event \"?\"]{0}[Site \"?\"]{0}[Date \"????.??.??\"]{0}[Round \"?\"]{0}[White \"?\"]{0}[Black \"?\"]{0}[Result \"*\"]{0}[SetUp \"1\"]{0}[FEN \"2bq1rk1/3p1npp/p1p3N1/1rbB1Pp1/1pQ5/P5N1/1PP3PP/R3R2K w - - 0 23\"]{0}{0}23.Bxf7+{0}*{0}", _moveFormatter.NewLine);
            var pgnOutput = _moveFormatter.BuildPgn(game);
            Debug.WriteLine(pgnOutput);
            Assert.AreEqual(expectedOutput, pgnOutput);
        }

        [Test]
        public void TestPgnWithVariations()
        {
            var expectedOutput =
                string.Format("[Event \"?\"]{0}[Site \"?\"]{0}[Date \"????.??.??\"]{0}[Round \"?\"]{0}[White \"?\"]{0}[Black \"?\"]{0}[Result \"*\"]{0}{0}1.c4 g6 2.Nc3 {0}    ( 2.d4 Bg7 3.Nc3 ){0}*{0}", _moveFormatter.NewLine);
            var game = new Game();
            string[] main = { "c4", "g6", "Nc3" };
            string[] variation = { "d4", "Bg7", "Nc3" };
            AddLineToGame(game, main);
            game.MoveNext();
            game.MoveNext();
            AddLineToGame(game, variation);
            var output = _moveFormatter.BuildPgn(game);
            Debug.WriteLine(new string('*', 80));
            Debug.WriteLine(output);
            Debug.WriteLine(new string('*', 80));

            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public void BuildPgn_VariationOnInitialMove()
        {
            var expected =
                string.Format("", _moveFormatter.NewLine);
            var game = new Game();
            game.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
            game.ExitVariation();
            game.ApplyMove("d4", MoveApplicationStrategy.ContinueMainLine);
            var actual = _moveFormatter.BuildPgn(game);
            Assert.AreEqual(expected, actual);
        }

        [TestCaseSource(nameof(GetVariationTestCases))]
        public void BuildPgn_VariationOnInitialMove(PgnFormatterVariationTestCase testCase)
        {
            var expected = string.Format(testCase.ExpectedPgn, _moveFormatter.NewLine);
            var actual = _moveFormatter.BuildPgn(testCase.Game);
            Assert.AreEqual(expected, actual, testCase.ToString());
        }

        protected static IEnumerable<PgnFormatterVariationTestCase> GetVariationTestCases()
        {
            yield return new PgnFormatterVariationTestCase(BoardConstants.FenStartingPosition,
                ChessLib.Core.Tests.PGN.PgnFormatterVariationMiddle,
                new string[] { "c4", "g6", "Nc3" },
                new string[] { "d4", "Bg7", "Nc3" },
                2, "Variation in middle of game"
            );
            yield return new PgnFormatterVariationTestCase(BoardConstants.FenStartingPosition,
                PGN.PgnFormatterVariationInitial,
                new[] { "c4" }, new[] { "d4" }, 0, "Variation on initial position");

            yield return new PgnFormatterVariationTestCase(BoardConstants.FenStartingPosition,
                PGN.PgnFormatterVariationEnd,
                new[] { "c4", "e5", "Nc3", "Nf6", "Nf3", "Nc6" }, new[] { "d6" }, 5, "Variation on last position");
        }


        /// <summary>.
        ///     Make a node tree on a game
        /// </summary>
        /// <param name="game">Game with CurrentNode set to correct position</param>
        /// <param name="moves">Moves to add</param>
        private void AddLineToGame(Game game, string[] moves)
        {
            moves.ToList().ForEach(m => game.ApplyMove(m, MoveApplicationStrategy.ContinueMainLine));
            game.ExitVariation();
        }

        public class PgnFormatterVariationTestCase
        {
            private readonly string description;
            public Game Game { get; }
            public string Fen { get; }
            public string ExpectedPgn { get; }
            public override string ToString()
            {
                return description;
            }

            public PgnFormatterVariationTestCase(string fen, string expectedPgn, string[] main, string[] variation = null,
                int variationMoveOffset = -1, string description = "")
            {
                Fen = fen;
                ExpectedPgn = expectedPgn;
                this.description = description;
                Game = new Game(fen);
                foreach (var move in main)
                {
                    Game.ApplyMove(move, MoveApplicationStrategy.ContinueMainLine);
                }

                Game.ExitVariation();
                AddVariationAtIndex(variation, variationMoveOffset);
                Game.ExitVariation();
            }

            private void AddVariationAtIndex(string[] variation, int variationMoveOffset)
            {
                for (var i = 0; i < variationMoveOffset; i++)
                {
                    Game.MoveNext();
                }

                foreach (var move in variation)
                {
                    Game.ApplyMove(move, MoveApplicationStrategy.ContinueMainLine);
                }
            }
        }
    }

 
}