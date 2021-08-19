using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Types
{
    [TestFixture]
    public class PgnFormatterTests
    {
        private readonly PgnFormatter _moveFormatter =
            new PgnFormatter(new PGNFormatterOptions { ResultOnNewLine = true });

        [TestCaseSource(nameof(GetVariationTestCases))]
        public void BuildPgn_TestVariousPositionsAndVariations(PgnFormatterVariationTestCase testCase)
        {
            Console.WriteLine(testCase.ExpectedPgn);
            var expected = string.Format(testCase.ExpectedPgn, _moveFormatter.NewLine);
            var actual = _moveFormatter.BuildPgn(testCase.Game);
            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual, testCase.ToString());
        }

        protected static IEnumerable<PgnFormatterVariationTestCase> GetVariationTestCases()
        {
            yield return new PgnFormatterVariationTestCase(BoardConstants.FenStartingPosition,
                PGN.PgnFormatterVariationMiddle,
                new[] { "c4", "g6", "Nc3" },
                new[] { "d4", "Bg7", "Nc3" },
                2, "Variation in middle of game - 3. Nc3 (3. d4 Bg7 4. Nc3)"
            );
            yield return new PgnFormatterVariationTestCase(BoardConstants.FenStartingPosition,
                PGN.PgnFormatterVariationInitial,
                new[] { "c4" },
                new[] { "d4" },
                0,
                "Variation on initial position - 1. c4 (1. d4)");

            yield return new PgnFormatterVariationTestCase(BoardConstants.FenStartingPosition,
                PGN.PgnFormatterVariationMiddle,
                new[] { "c4", "e5", "Nc3", "Nf6", "Nf3", "Nc6" },
                new[] { "d6" }, 5,
                "Black defends e5 with 3...Nc6 (3...d6)");

            yield return new PgnFormatterVariationTestCase(
                "2bq1rk1/3p1npp/p1p3N1/1rbB1Pp1/1pQ5/P5N1/1PP3PP/R3R2K w - - 0 23",
                PGN.PgnFormatterCaptureWithCheck,
                new[] { "Bxf7" }, "Alternate starting position with 23. Bxf7+");
            yield return GetLongVariation();
        }

        private static PgnFormatterVariationTestCase GetLongVariation()
        {
            var moveset = new[] {"c4", "Nf6", "Nc3", "e6", "Nf3", "d5", "d4", "Nbd7", "Bg5", "h6" };

            var testCase = new PgnFormatterVariationTestCase(BoardConstants.FenStartingPosition,
                PGN.PgnFormatterAllAccoutrements,
                moveset, "Game with comments, multiple variations on variations, annotations, tags filled");
                SetupLongGame(testCase);
                testCase.Game.Tags.Event = "The Mediocre of Chess";
                testCase.Game.Tags.White = "GoodPlayer, One";
                testCase.Game.Tags.Black = "DecentPlayer, A.";
                testCase.Game.Tags.Site = "New York City, NY USA";
                testCase.Game.Tags.Date = "2021.08.18";
                testCase.Game.Tags.Round = "2";
                testCase.Game.Tags["EventDate"] = "2021.08.16";
                testCase.Game.GameResult = GameResult.WhiteWins;
                testCase.Game.Tags["ECO"] = "D51";

            return testCase;
        }

        private static void SetupLongGame(PgnFormatterVariationTestCase testCase)
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


        /// <summary>
        ///     .
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
            public Game Game { get; }
            public string Fen { get; }
            public string ExpectedPgn { get; }

            public PgnFormatterVariationTestCase(string fen, string expectedPgn, string[] main,
                string[] variation = null,
                int variationMoveOffset = -1, string description = "") : this(fen, expectedPgn, main, description)
            {
                AddVariationAtIndex(variation, variationMoveOffset);
                Game.ExitVariation();
            }

            public PgnFormatterVariationTestCase(string fen, string expectedPgn, string[] main,
                string description = "") : this(fen, expectedPgn, description)
            {
                foreach (var move in main)
                {
                    Game.ApplyMove(move, MoveApplicationStrategy.ContinueMainLine);
                }

                Game.ExitVariation();
            }

            private PgnFormatterVariationTestCase(string fen, string expectedPgn, string description)
            {
                this.Description = description;
                this.ExpectedPgn = expectedPgn;
                this.Fen = fen;
                Game = new Game(fen);
            }
            public string Description { get; }

            public override string ToString()
            {
                return Description;
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