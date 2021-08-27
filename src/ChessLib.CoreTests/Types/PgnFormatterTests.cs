using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Tests.Types.GameTree.Traversal;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.PgnExport;
using NUnit.Framework;
// ReSharper disable PossibleNullReferenceException

namespace ChessLib.Core.Tests.Types
{
    [TestFixture]
    public class PgnFormatterTests
    {
        private static readonly PGNFormatterOptions options = new PGNFormatterOptions
        { NewLineIndicator = Environment.NewLine };


        [TestCaseSource(nameof(GetVariationTestCases))]
        public void BuildPgn_TestVariousPositionsAndVariations(PgnFormatterVariationTestCase testCase)
        {
            Console.WriteLine(testCase.ExpectedPgn);
            var expected = string.Format(testCase.ExpectedPgn, options.NewLineIndicator);
            GameSerializer gs = new GameSerializer(new PGNFormatterOptions());
            var actual = gs.SerializeToString(testCase.Game);
            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual, testCase.ToString());
        }

        protected static IEnumerable<PgnFormatterVariationTestCase> GetVariationTestCases()
        {
            yield return new PgnFormatterVariationTestCase(BoardConstants.FenStartingPosition,
                PGN.PgnFormatterd4Variation,
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
                PGN.PgnFormatterVariationDefende5,
                new[] { "c4", "e5", "Nc3", "Nf6", "Nf3", "Nc6" },
                new[] { "d6" }, 5,
                "Black defends e5 with 3...Nc6 (3...d6)");

            yield return new PgnFormatterVariationTestCase(
                "2bq1rk1/3p1npp/p1p3N1/1rbB1Pp1/1pQ5/P5N1/1PP3PP/R3R2K w - - 0 23",
                PGN.PgnFormatterCaptureWithCheck,
                new[] { "Bxf7" }, "Alternate starting position with 23. Bxf7+");
            yield return GetLongVariation();
        }

        public class PgnTraversalTestCase
        {
            public Game Game { get; }
            public string Fen { get; }
            public string Description { get; }
            protected PgnTraversalTestCase(string fen, string description)
            {
                Game = new Game(fen);
                Description = description;
                Fen = fen;
            }

            public PgnTraversalTestCase(string fen, string description, string[] main) : this(fen, description)
            {
                Main = main.ToArray();
                foreach (var move in main)
                {
                    Game.ApplyMove(move, MoveApplicationStrategy.ContinueMainLine);

                }

                Game.ExitVariation();
            }

            public string[] ExpectedMoveTraversal
            {
                get
                {
                    if (VariationIndex == -1)
                    {
                        return Main.ToArray();
                    }
                    var str = Main.Take(VariationIndex + 1).ToArray()
                        .Concat(Variation);
                    return str.Concat(Main.Skip(VariationIndex + 1)).ToArray();
                }
            }
            public string[] Main { get; }

            public PgnTraversalTestCase(string fen, string description, string[] main, string[] variation,
                int variationMoveOffset)
                : this(fen, description, main)
            {
                Variation = variation.ToArray();
                VariationIndex = variationMoveOffset;
                AddVariationAtIndex(variation, variationMoveOffset);
                Game.ExitVariation();
            }

            public override string ToString()
            {
                return "Traversal Test: "+Description;
            }

            public int VariationIndex { get; } = -1;

            public string[] Variation { get; }

            protected void AddVariationAtIndex(string[] variation, int variationMoveOffset)
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

        public class PgnFormatterVariationTestCase : PgnTraversalTestCase
        {
            public string ExpectedPgn { get; }

            public PgnFormatterVariationTestCase(string fen, string expectedPgn, string[] main,
                string[] variation = null,
                int variationMoveOffset = -1, string description = "") : base(fen, description, main, variation,
                variationMoveOffset)
            {
                ExpectedPgn = expectedPgn;
            }

            public PgnFormatterVariationTestCase(string fen, string expectedPgn, string[] main,
                string description = "") : base(fen, description, main)
            {
                ExpectedPgn = expectedPgn;
            }


            public override string ToString()
            {
                return Description;
            }
        }

        public static PgnFormatterVariationTestCase GetLongVariation()
        {
            

            var testCase = new PgnFormatterVariationTestCase(BoardConstants.FenStartingPosition,
                PGN.PgnFormatterAllAccoutrements,
                TraversalData.longGameMainLine, "Game with comments, multiple variations on variations, annotations, tags filled");
            TraversalData.SetupLongGame(testCase);
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
    }
}