#region

using System.Collections.Generic;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Validation.Validators.BoardValidation.Rules;
using NUnit.Framework;

#endregion

namespace ChessLib.Core.Tests.Validation.Validators.BoardValidation.Rules
{

    [TestFixture]
    public class EnPassantSquareRuleTests
    {
        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();
        private EnPassantPositionRule rule = new EnPassantPositionRule();

        [SetUp]
        public void SetUp()
        {
            rule = new EnPassantPositionRule();
        }

        //[TestCaseSource(nameof(GetSquareTestCases))]

        public void IsEnPassantSquareCorrectForActive(EnPassantSquareLocationTestCase testCase)
        {
            var result = rule.IsEnPassantSquareCorrectForActive(testCase.ActivePlayerColor, testCase.SquareIndex);
            Assert.AreEqual(testCase.ExpectedValue, rule, testCase.Description);
        }

        public static IEnumerable<EnPassantSquareLocationTestCase> GetSquareTestCases()
        {
            yield return new EnPassantSquareLocationTestCase("d5".SquareTextToIndex(), Color.Black, false,
                "Non-en passant square(d5), black to move.");
            yield return new EnPassantSquareLocationTestCase("d5".SquareTextToIndex(), Color.White, false,
                "Non-en passant square(d5), white to move.");
            yield return new EnPassantSquareLocationTestCase("h8".SquareTextToIndex(), Color.Black, false,
                "Non-en passant square(h8), black to move.");
            yield return new EnPassantSquareLocationTestCase("h1".SquareTextToIndex(), Color.White, false,
                "Non-en passant square(h1), white to move.");
            yield return new EnPassantSquareLocationTestCase("e3".SquareTextToIndex(), Color.White, false,
                "En passant square(e3), wrong color (white to move).");
            yield return new EnPassantSquareLocationTestCase("e6".SquareTextToIndex(), Color.Black, false,
                "En passant square(e6), wrong color (black to move).");
            yield return new EnPassantSquareLocationTestCase("e3".SquareTextToIndex(), Color.Black, true,
                "En passant square(e3), correct color (black to move).");
            yield return new EnPassantSquareLocationTestCase("e6".SquareTextToIndex(), Color.White, true,
                "En passant square(e6), correct color (white to move).");
        }
    }

    public class EnPassantSquareLocationTestCase
    {
        public EnPassantSquareLocationTestCase(ushort squareIndex, Color activePlayerColor, bool expectedValue, string description)
        {
            SquareIndex = squareIndex;
            ActivePlayerColor = activePlayerColor;
            ExpectedValue = expectedValue;
            Description = description;
        }
        public string Description { get; }
        public bool ExpectedValue { get; }
        public ushort SquareIndex { get; }
        public Color ActivePlayerColor { get; }

    }
}