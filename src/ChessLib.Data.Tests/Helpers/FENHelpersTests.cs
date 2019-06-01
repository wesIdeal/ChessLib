using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using EnumsNET;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessLib.Data.Tests.Helpers
{
    [TestFixture]
    public class FENHelpersTests
    {
        private struct CastleInfo
        {
            public string CastlingAvailabilityString { get; set; }
            public CastlingAvailability CastlingAvailability { get; set; }
        }

        private char[] _disallowedCastlingChars;
        private CastleInfo[] _castleInfos;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            SetupDisallowedChars();
            SetupAllowedCastlingStrings();
        }

        private void SetupAllowedCastlingStrings()
        {
            var permutationsOfCastlingChars = GetPermutations(FENHelpers.ValidCastlingStringChars.Take(4).ToArray());
            permutationsOfCastlingChars.Add("-");
            var dict = new Dictionary<char, CastlingAvailability>();
            var castlingInfos = new List<CastleInfo>();
            foreach (var ca in Enum.GetValues(typeof(CastlingAvailability)))
                dict.Add(((CastlingAvailability)ca).AsString(EnumFormat.Description)[0], (CastlingAvailability)ca);
            foreach (var perm in permutationsOfCastlingChars)
            {
                CastlingAvailability ca = 0;
                foreach (var c in perm) ca |= dict[c];
                castlingInfos.Add(new CastleInfo { CastlingAvailability = ca, CastlingAvailabilityString = perm });
            }

            _castleInfos = castlingInfos.ToArray();
        }

        private void SetupDisallowedChars()
        {
            var chars = new List<char>();
            for (var c = 'a'; c < 'z'; c++)
            {
                if (FENHelpers.ValidCastlingStringChars.Contains(c)) continue;
                chars.Add(c);
            }

            for (var c = 'A'; c < 'Z'; c++)
            {
                if (FENHelpers.ValidCastlingStringChars.Contains(c)) continue;
                chars.Add(c);
            }

            _disallowedCastlingChars = chars.ToArray();
        }

        private static List<string> GetPermutations(char[] set)
        {
            var n = set.Length;
            var storage = new List<string>();
            for (var i = n; i > 0; i--) storage.AddRange(GetKCombs(set, i).Select(x => string.Concat(x)));
            return storage;
        }

        private static IEnumerable<IEnumerable<T>> GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new[] { t });
            var enumerable = list as T[] ?? list.ToArray();
            return GetKCombs(enumerable, length - 1)
                .SelectMany(t => enumerable.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new[] { t2 }));
        }

        private const string FENFormat = "{0} {1} {2} {3} {4} {5}";
        private const string ValidPiecePlacement = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
        private const string ValidColor = "w";
        private const string ValidEnPassant = "-";

        private delegate void ValidationDelegate(string s);

        private readonly ValidationDelegate _mainValidation = FENHelpers.ValidateFENString;

        private static string GetFENFromProvidedFENPieceInfo(string piecePlacement, string activeColor, string castling,
            string enPassant, int halfMove, int fullMove)
        {
            return string.Format(FENFormat, piecePlacement, activeColor, castling, enPassant, halfMove, fullMove);
        }

        private static void AssertThrowsFenException(ValidationDelegate validate, string fen,
            FENError errorTypeExpected = FENError.Null, string assertMessage = "")
        {
            var exc = new FENException(fen, FENError.Null);
            Assert.Throws(typeof(FENException), () =>
            {
                try
                {
                    validate(fen);
                }
                catch (FENException e)
                {
                    exc = e;
                    throw;
                }
            }, assertMessage);
            if (errorTypeExpected != FENError.Null) Assert.AreEqual(errorTypeExpected, exc.FENError);
        }

        [Test]
        public static void GetActiveColor_ShouldReturnAppropriateColor_GivenValidInput()
        {
            Assert.AreEqual(Color.White, FENHelpers.GetActiveColor("w"));
            Assert.AreEqual(Color.Black, FENHelpers.GetActiveColor("b"));
            Assert.Throws(typeof(FENException),
                () =>
                {
                    try
                    {
                        FENHelpers.GetActiveColor("x");
                    }
                    catch (FENException fenException)
                    {
                        Assert.AreEqual(FENError.InvalidActiveColor, fenException.FENError,
                            "FEN Exception error was not the proper type.");
                        throw;
                    }
                });
        }

        [Test]
        public static void GetMoveNumberFromString_ShouldReturnAppropriateUINT_GivenValidInput()
        {
            Assert.AreEqual((uint?)24, FENHelpers.GetMoveNumberFromString("24"));
        }

        [Test(Description = "Should make appropriate FEN Castling Availability string (-) when empty.")]
        public static void MakeCastlingAvailabilityStringFromBitFlags_ShouldReturnAppropriateResults_WhenEmpty()
        {
            Assert.AreEqual("-",
                FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(CastlingAvailability.NoCastlingAvailable));
        }

        [Test(Description = "Should make appropriate FEN Castling Availability string (k) when Black Kingside.")]
        public static void MakeCastlingAvailabilityStringFromBitFlags_ShouldReturnAppropriateResults_When_k()
        {
            Assert.AreEqual("k",
                FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(CastlingAvailability.BlackKingside));
        }

        [Test(Description = "Should make appropriate FEN Castling Availability string (K) when White Kingside.")]
        public static void MakeCastlingAvailabilityStringFromBitFlags_ShouldReturnAppropriateResults_When_K()
        {
            Assert.AreEqual("K",
                FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(CastlingAvailability.WhiteKingside));
        }

        [Test(Description =
            "Should make appropriate FEN Castling Availability string (kq) when Black Kingside and Queenside.")]
        public static void MakeCastlingAvailabilityStringFromBitFlags_ShouldReturnAppropriateResults_When_kq()
        {
            Assert.AreEqual("kq",
                FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(
                    CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside));
        }

        [Test(Description =
            "Should make appropriate FEN Castling Availability string (Qk) when Black Kingside and White Queenside.")]
        public static void MakeCastlingAvailabilityStringFromBitFlags_ShouldReturnAppropriateResults_When_kQ()
        {
            Assert.AreEqual("Qk",
                FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(
                    CastlingAvailability.BlackKingside | CastlingAvailability.WhiteQueenside));
        }

        [Test(Description =
            "Should make appropriate FEN Castling Availability string (Kq) when White Kingside and Black Queenside.")]
        public static void MakeCastlingAvailabilityStringFromBitFlags_ShouldReturnAppropriateResults_When_Kq()
        {
            Assert.AreEqual("Kq",
                FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(
                    CastlingAvailability.BlackQueenside | CastlingAvailability.WhiteKingside));
        }

        [Test(Description =
            "Should make appropriate FEN Castling Availability string (KQ) when White Kingside and Queenside.")]
        public static void MakeCastlingAvailabilityStringFromBitFlags_ShouldReturnAppropriateResults_WhenKQ()
        {
            Assert.AreEqual("KQ",
                FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(
                    CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside));
        }

        [Test(Description =
            "Should make appropriate FEN Castling Availability string (KQkq) when all castling is available.")]
        public static void MakeCastlingAvailabilityStringFromBitFlags_ShouldReturnAppropriateResults_WhenKQkq()
        {
            Assert.AreEqual("KQkq",
                FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(
                    CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside |
                    CastlingAvailability.WhiteQueenside | CastlingAvailability.WhiteKingside));
        }

        [Test(Description = "Should make appropriate FEN Castling Availability string (Q) when White Queenside.")]
        public static void MakeCastlingAvailabilityStringFromBitFlags_ShouldReturnAppropriateResults_WhenQ()
        {
            Assert.AreEqual("Q",
                FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(CastlingAvailability.WhiteQueenside));
        }

        [Test]
        public static void ValidateActiveColor_ShouldReturnAppropriateFENError_GivenInvalidInput()
        {
            Assert.AreEqual(FENError.InvalidActiveColor, FENHelpers.ValidateActiveColor("z"));
            Assert.AreEqual(FENError.InvalidActiveColor, FENHelpers.ValidateActiveColor(""));
        }

        [Test]
        public static void ValidateActiveColor_ShouldReturnNullFENError_GivenValidInput()
        {
            Assert.AreEqual(FENError.Null, FENHelpers.ValidateActiveColor("w"));
            Assert.AreEqual(FENError.Null, FENHelpers.ValidateActiveColor("b"));
        }

        [Test]
        public void ValidateCastlingAvailabilityString_ShouldNotHaveError_WhenCastlingAvailabilityStringIsValid()
        {
            foreach (var info in _castleInfos)
            {
                var s = string.Join(" | ", info.CastlingAvailability.GetFlags().Select(x => x.ToString()));
                Console.WriteLine($"{info.CastlingAvailabilityString} should equal {s}");
                Assert.AreEqual(FENError.Null,
                    FENHelpers.ValidateCastlingAvailabilityString(info.CastlingAvailabilityString));
            }
        }

        [Test]
        public static void ValidateCastlingAvailabilityString_ShouldReturnError_WhenCastlingHasDashAndOtherCharacters()
        {
            var expected = FENError.CastlingStringBadSequence;
            Assert.AreEqual(expected, FENHelpers.ValidateCastlingAvailabilityString("KQ-"));
        }

        [Test]
        public static void ValidateCastlingAvailabilityString_ShouldReturnError_WhenCastlingHasRepeatedCharacters()
        {
            var expected = FENError.CastlingStringRepetition;
            Assert.AreEqual(expected, FENHelpers.ValidateCastlingAvailabilityString("KQK"));
        }

        [Test]
        public static void ValidateCastlingAvailabilityString_ShouldReturnError_WhenCastlingStringIsEmpty()
        {
            var expected = FENError.CastlingNoStringPresent;
            Assert.AreEqual(expected, FENHelpers.ValidateCastlingAvailabilityString(""));
            Assert.AreEqual(expected, FENHelpers.ValidateCastlingAvailabilityString(null));
        }

        [Test]
        public static void ValidateCastlingAvailabilityString_ShouldReturnNoError_WhenCastlingNotAvailable()
        {
            var expected = FENError.Null;
            Assert.AreEqual(expected, FENHelpers.ValidateCastlingAvailabilityString("-"));
        }

        [Test]
        public void ValidateCastlingAvailabilityString_ShouldThrowException_WhenGivenInvalidCastlingCharacters()
        {
            var message = "";
            foreach (var c in _disallowedCastlingChars)
            {
                Assert.AreEqual(FENError.CastlingUnrecognizedChar,
                    FENHelpers.ValidateCastlingAvailabilityString(c.ToString()));
                AssertThrowsFenException(_mainValidation,
                    GetFENFromProvidedFENPieceInfo(ValidPiecePlacement, ValidColor, c.ToString(), ValidEnPassant, 0,
                        0));
            }

            Console.WriteLine(message);
        }

        [Test]
        public static void ValidateEnPassantSquare_ShouldNotReturnError_GivenValidSquare()
        {
            Assert.AreEqual(FENError.Null, FENHelpers.ValidateEnPassantSquare("e6"));
            Assert.AreEqual(FENError.Null, FENHelpers.ValidateEnPassantSquare("a3"));
            Assert.AreEqual(FENError.Null, FENHelpers.ValidateEnPassantSquare("h3"));
            Assert.AreEqual(FENError.Null, FENHelpers.ValidateEnPassantSquare("-"));
        }

        [Test]
        public static void ValidateEnPassantSquare_ShouldThrowException_WhenEnPassantSqIsInvalid()
        {
            Assert.AreEqual(FENError.InvalidEnPassantSquare, FENHelpers.ValidateEnPassantSquare("4e"));
            Assert.AreEqual(FENError.InvalidEnPassantSquare, FENHelpers.ValidateEnPassantSquare("4"));
            Assert.AreEqual(FENError.InvalidEnPassantSquare, FENHelpers.ValidateEnPassantSquare("e9"));
            Assert.AreEqual(FENError.InvalidEnPassantSquare, FENHelpers.ValidateEnPassantSquare("i3"));
            Assert.AreEqual(FENError.InvalidEnPassantSquare, FENHelpers.ValidateEnPassantSquare("z8"));
            Assert.AreEqual(FENError.InvalidEnPassantSquare, FENHelpers.ValidateEnPassantSquare("--"));
            Assert.AreEqual(FENError.InvalidEnPassantSquare, FENHelpers.ValidateEnPassantSquare("12"));
            Assert.AreEqual(FENError.InvalidEnPassantSquare, FENHelpers.ValidateEnPassantSquare("ee"));
        }

        [Test]
        public static void ValidateFENStructure_ShouldThrowExc_WhenFENIsNullOrEmpty()
        {
            Assert.Throws(typeof(FENException),
                () =>
                {
                    try
                    {
                        FENHelpers.ValidateFENStructure("");
                    }
                    catch (FENException exc)
                    {
                        Assert.AreEqual(FENError.InvalidFENString, exc.FENError);
                        throw;
                    }
                });
            Assert.Throws(typeof(FENException),
                () =>
                {
                    try
                    {
                        FENHelpers.ValidateFENStructure(null);
                    }
                    catch (FENException exc)
                    {
                        Assert.AreEqual(FENError.InvalidFENString, exc.FENError);
                        throw;
                    }
                });
        }

        [Test]
        public static void ValidateFENStructure_ShouldThrowExc_WhenFENLacksPieces()
        {
            Assert.Throws(typeof(FENException),
                () =>
                {
                    try
                    {
                        FENHelpers.ValidateFENStructure("fen s struct");
                    }
                    catch (FENException exc)
                    {
                        Assert.AreEqual(FENError.InvalidFENString, exc.FENError);
                        throw;
                    }
                });
        }

        [Test]
        public static void ValidateFullMoveCounter_ShouldReturnAppropriateFENError_GivenInvalidInput()
        {
            Assert.AreEqual(FENError.FullMoveCounter, FENHelpers.ValidateFullMoveCounter("-1"));
            Assert.AreEqual(FENError.FullMoveCounter, FENHelpers.ValidateFullMoveCounter(""));
            Assert.AreEqual(FENError.FullMoveCounter, FENHelpers.ValidateFullMoveCounter("-"));
        }

        [Test]
        public static void ValidateFullMoveCounter_ShouldReturnNullFENError_GivenValidInput()
        {
            Assert.AreEqual(FENError.Null, FENHelpers.ValidateFullMoveCounter("31"));
            Assert.AreEqual(FENError.Null, FENHelpers.ValidateFullMoveCounter("0"));
            Assert.AreEqual(FENError.Null, FENHelpers.ValidateFullMoveCounter("1"));
        }

        [Test]
        public static void ValidateHalfmoveClock_ShouldReturnAppropriateFENError_GivenInvalidInput()
        {
            Assert.AreEqual(FENError.HalfmoveClock, FENHelpers.ValidateHalfmoveClock("-1"));
            Assert.AreEqual(FENError.HalfmoveClock, FENHelpers.ValidateHalfmoveClock(""));
            Assert.AreEqual(FENError.HalfmoveClock, FENHelpers.ValidateHalfmoveClock("-"));
        }

        [Test]
        public static void ValidateHalfmoveClock_ShouldReturnNullFENError_GivenValidInput()
        {
            Assert.AreEqual(FENError.Null, FENHelpers.ValidateHalfmoveClock("1"));
            Assert.AreEqual(FENError.Null, FENHelpers.ValidateHalfmoveClock("0"));
            Assert.AreEqual(FENError.Null, FENHelpers.ValidateHalfmoveClock("3"));
        }

        [Test]
        public static void ValidatePiecePlacement_ShouldReturnAppropriateFENError_GivenInvalidPieceInput()
        {
            // ReSharper disable once StringLiteralTypo
            var fen = "rnbqkbnr/ppspppzp/8/8/8/8/PPPPPPPP/RNBQKBNR";
            Assert.AreEqual(FENError.PiecePlacementInvalidChars, FENHelpers.ValidatePiecePlacement(fen));
        }

        [Test]
        public static void ValidatePiecePlacement_ShouldReturnAppropriateFENError_GivenInvalidRankCount()
        {
            var fen = ValidPiecePlacement;
            Assert.AreEqual(FENError.PiecePlacementRankCount, FENHelpers.ValidatePiecePlacement(fen + "/rnbqkbnr"));
            Assert.AreEqual(FENError.PiecePlacementRankCount, FENHelpers.ValidatePiecePlacement("rnbqkbnr/rnbqkbnr"));
        }

        [Test]
        public static void ValidatePiecePlacement_ShouldReturnAppropriateFENError_GivenTooManyPieceInRank()
        {
            var fen = "rnbqkbnr/ppppppppp/8/8/8/8/PPPPPPPP/RNBQrKBNR";

            Assert.AreEqual(FENError.PiecePlacementPieceCountInRank, FENHelpers.ValidatePiecePlacement(fen));
        }

        [Test]
        public static void ValidatePiecePlacement_ShouldReturnNullFENError_GivenValidPieceInput()
        {
            Assert.AreEqual(FENError.Null, FENHelpers.ValidatePiecePlacement(ValidPiecePlacement));
        }
    }
}