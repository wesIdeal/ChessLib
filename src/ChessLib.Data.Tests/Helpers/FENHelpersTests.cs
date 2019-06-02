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

        private static string GetFENFromProvidedFENPieceInfo(string piecePlacement, string activeColor, string castling,
            string enPassant, int halfMove, int fullMove)
        {
            return string.Format(FENFormat, piecePlacement, activeColor, castling, enPassant, halfMove, fullMove);
        }

        private static void AssertThrowsFenException(ValidationDelegate validate, string fen,
            FENError errorTypeExpected = FENError.None, string assertMessage = "")
        {
            var exc = new FENException(fen, FENError.None);
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
            if (errorTypeExpected != FENError.None) Assert.AreEqual(errorTypeExpected, exc.FENError);
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

       

      
    }
}