using EnumsNET;
using MagicBitboard;
using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicBitboard.Helpers.Tests
{
    [TestFixture]
    public class FENHelpersTests
    {
        struct CastleInfo
        {
            public string CastlingAvailabilityString { get; set; }
            public CastlingAvailability CastlingAvailability { get; set; }
        }
        char[] DisallowedCastlingChars;
        CastleInfo[] CastleInfos;

        #region One Time Setup

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
            {
                dict.Add(((CastlingAvailability)ca).AsString(EnumFormat.Description)[0], (CastlingAvailability)ca);
            }
            foreach (var perm in permutationsOfCastlingChars)
            {
                CastlingAvailability ca = 0;
                foreach (var c in perm)
                {
                    ca |= dict[c];
                }
                castlingInfos.Add(new CastleInfo() { CastlingAvailability = ca, CastlingAvailabilityString = perm });
            }
            CastleInfos = castlingInfos.ToArray();
        }

        private void SetupDisallowedChars()
        {
            List<char> chars = new List<char>();
            for (char c = 'a'; c < 'z'; c++)
            {
                if (FENHelpers.ValidCastlingStringChars.Contains(c)) continue;
                chars.Add(c);
            }
            for (char c = 'A'; c < 'Z'; c++)
            {
                if (FENHelpers.ValidCastlingStringChars.Contains(c)) continue;
                chars.Add(c);
            }
            DisallowedCastlingChars = chars.ToArray();
        }
        static List<string> GetPermutations(char[] set)
        {
            int n = set.Length;
            var storage = new List<string>();
            for (int i = n; i > 0; i--)
            {
                storage.AddRange(GetKCombs(set, i).Select(x => string.Concat(x)));
            }
            return storage;
        }

        static IEnumerable<IEnumerable<T>> GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetKCombs(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        #endregion

        #region Making Boards
        const string initialBoard = FENHelpers.InitialFEN;
        const string after1e4 = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";
        const string after1e4c5 = "rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2";

        [Test]
        public void Should_Set_Initial_Board()
        {
            var white = (int)Color.White;
            var black = (int)Color.Black;
            var whitePawns = 0xff00;
            var blackPawns = 0xff000000000000;
            var whiteRooks = 0x81;
            var blackRooks = 0x8100000000000000;
            var whiteKnights = 0x42;
            var blackKnights = 0x4200000000000000;
            var whiteBishops = 0x24;
            var blackBishops = 0x2400000000000000;
            var whiteQueen = 0x8;
            var blackQueen = 0x800000000000000;
            var whiteKing = 0x10;
            var blackKing = 0x1000000000000000;
            var rv = FENHelpers.BoardInfoFromFen(initialBoard);
            Assert.AreEqual(whitePawns, rv.PiecesOnBoard[white][(int)Piece.Pawn]);
            Assert.AreEqual(blackPawns, rv.PiecesOnBoard[black][(int)Piece.Pawn]);

            Assert.AreEqual(whiteRooks, rv.PiecesOnBoard[white][(int)Piece.Rook]);
            Assert.AreEqual(blackRooks, rv.PiecesOnBoard[black][(int)Piece.Rook]);

            Assert.AreEqual(whiteKnights, rv.PiecesOnBoard[white][(int)Piece.Knight]);
            Assert.AreEqual(blackKnights, rv.PiecesOnBoard[black][(int)Piece.Knight]);

            Assert.AreEqual(whiteBishops, rv.PiecesOnBoard[white][(int)Piece.Bishop]);
            Assert.AreEqual(blackBishops, rv.PiecesOnBoard[black][(int)Piece.Bishop]);

            Assert.AreEqual(whiteQueen, rv.PiecesOnBoard[white][(int)Piece.Queen]);
            Assert.AreEqual(blackQueen, rv.PiecesOnBoard[black][(int)Piece.Queen]);

            Assert.AreEqual(whiteKing, rv.PiecesOnBoard[white][(int)Piece.King]);
            Assert.AreEqual(blackKing, rv.PiecesOnBoard[black][(int)Piece.King]);
        }

        [Test]
        public void Should_Set_Board_After_1e4()
        {
            var white = (int)Color.White;
            var black = (int)Color.Black;
            var whitePawns = 0x1000ef00;
            var blackPawns = 0xff000000000000;
            var whiteRooks = 0x81;
            var blackRooks = 0x8100000000000000;
            var whiteKnights = 0x42;
            var blackKnights = 0x4200000000000000;
            var whiteBishops = 0x24;
            var blackBishops = 0x2400000000000000;
            var whiteQueen = 0x8;
            var blackQueen = 0x800000000000000;
            var whiteKing = 0x10;
            var blackKing = 0x1000000000000000;
            var rv = FENHelpers.BoardInfoFromFen(after1e4);

            Assert.AreEqual(whitePawns, rv.PiecesOnBoard[white][(int)Piece.Pawn]);
            Assert.AreEqual(blackPawns, rv.PiecesOnBoard[black][(int)Piece.Pawn]);

            Assert.AreEqual(whiteRooks, rv.PiecesOnBoard[white][(int)Piece.Rook]);
            Assert.AreEqual(blackRooks, rv.PiecesOnBoard[black][(int)Piece.Rook]);

            Assert.AreEqual(whiteKnights, rv.PiecesOnBoard[white][(int)Piece.Knight]);
            Assert.AreEqual(blackKnights, rv.PiecesOnBoard[black][(int)Piece.Knight]);

            Assert.AreEqual(whiteBishops, rv.PiecesOnBoard[white][(int)Piece.Bishop]);
            Assert.AreEqual(blackBishops, rv.PiecesOnBoard[black][(int)Piece.Bishop]);

            Assert.AreEqual(whiteQueen, rv.PiecesOnBoard[white][(int)Piece.Queen]);
            Assert.AreEqual(blackQueen, rv.PiecesOnBoard[black][(int)Piece.Queen]);

            Assert.AreEqual(whiteKing, rv.PiecesOnBoard[white][(int)Piece.King]);
            Assert.AreEqual(blackKing, rv.PiecesOnBoard[black][(int)Piece.King]);
        }

        [Test]
        public void Should_Set_Board_After_1e4_c5()
        {
            var white = (int)Color.White;
            var black = (int)Color.Black;
            var whitePawns = 0x1000ef00;
            var blackPawns = 0xfb000400000000;
            var whiteRooks = 0x81;
            var blackRooks = 0x8100000000000000;
            var whiteKnights = 0x42;
            var blackKnights = 0x4200000000000000;
            var whiteBishops = 0x24;
            var blackBishops = 0x2400000000000000;
            var whiteQueen = 0x8;
            var blackQueen = 0x800000000000000;
            var whiteKing = 0x10;
            var blackKing = 0x1000000000000000;
            var rv = FENHelpers.BoardInfoFromFen(after1e4c5);

            Assert.AreEqual(whitePawns, rv.PiecesOnBoard[white][(int)Piece.Pawn]);
            Assert.AreEqual(blackPawns, rv.PiecesOnBoard[black][(int)Piece.Pawn]);

            Assert.AreEqual(whiteRooks, rv.PiecesOnBoard[white][(int)Piece.Rook]);
            Assert.AreEqual(blackRooks, rv.PiecesOnBoard[black][(int)Piece.Rook]);

            Assert.AreEqual(whiteKnights, rv.PiecesOnBoard[white][(int)Piece.Knight]);
            Assert.AreEqual(blackKnights, rv.PiecesOnBoard[black][(int)Piece.Knight]);

            Assert.AreEqual(whiteBishops, rv.PiecesOnBoard[white][(int)Piece.Bishop]);
            Assert.AreEqual(blackBishops, rv.PiecesOnBoard[black][(int)Piece.Bishop]);

            Assert.AreEqual(whiteQueen, rv.PiecesOnBoard[white][(int)Piece.Queen]);
            Assert.AreEqual(blackQueen, rv.PiecesOnBoard[black][(int)Piece.Queen]);

            Assert.AreEqual(whiteKing, rv.PiecesOnBoard[white][(int)Piece.King]);
            Assert.AreEqual(blackKing, rv.PiecesOnBoard[black][(int)Piece.King]);
        }

        #endregion

        #region FEN

        const string startingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        const string fenFormat = "{0} {1} {2} {3} {4} {5}";
        const string ValidPiecePlacement = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
        const string ValidColor = "w";
        const string ValidCastling = "KQkq";
        const string ValidEnPassent = "-";
        const string ValidEnPassentSquare = "e6";
        const int ValidHalfMove = 0;
        const int ValidFullMove = 0;
        private string GetFen(string piecePlacement, string activeColor, string castling, string enPassent, int halfMove, int fullMove)
        {
            return string.Format(fenFormat, piecePlacement, activeColor, castling, enPassent, halfMove, fullMove);
        }

        private delegate void ValidationDelegate(string s);
        private ValidationDelegate MainValidation = FENHelpers.ValidateFENString;
        private FENException AssertThrowsFenException(ValidationDelegate validate, string fen, FENError errorTypeExpected = FENError.NULL)
        {
            FENException exc = new FENException(fen, FENError.NULL);
            Assert.Throws(typeof(FENException), () =>
            {
                try { validate(fen); }
                catch (FENException e) { exc = e; throw; }
            });
            if (errorTypeExpected != FENError.NULL)
            {
                Assert.AreEqual(errorTypeExpected, exc.FENError);
            }
            return exc;
        }



        [Test]
        public void Should_Throw_Exception_On_Invalid_Castling_Characters()
        {
            var message = "";
            foreach (var c in DisallowedCastlingChars)
            {
                Assert.AreEqual(FENError.CastlingUnrecognizedChar, FENHelpers.ValidateCastlingAvailabilityString(c.ToString()));
                AssertThrowsFenException(MainValidation, GetFen(ValidPiecePlacement, ValidColor, c.ToString(), ValidEnPassent, 0, 0));
            }
            Console.WriteLine(message);
        }

        [Test]
        public void Should_Be_Valid_When_Valid_Castling_Chars_Are_Passed()
        {
            foreach (var info in CastleInfos)
            {
                var s = string.Join(" | ", info.CastlingAvailability.GetFlags().Select(x => x.ToString()));
                Console.WriteLine($"{info.CastlingAvailabilityString} should equal {s}");
                Assert.AreEqual(FENError.NULL, FENHelpers.ValidateCastlingAvailabilityString(info.CastlingAvailabilityString));
            }
        }

        [Test]
        public void Should_Be_Invalid_When_EnPassent_Is_Bad()
        {
            Assert.AreEqual(FENError.InvalidEnPassentSquare, FENHelpers.ValidateEnPassentSquare("4e"));
            Assert.AreEqual(FENError.InvalidEnPassentSquare, FENHelpers.ValidateEnPassentSquare("4"));
            Assert.AreEqual(FENError.InvalidEnPassentSquare, FENHelpers.ValidateEnPassentSquare("e9"));
            Assert.AreEqual(FENError.InvalidEnPassentSquare, FENHelpers.ValidateEnPassentSquare("i3"));
            Assert.AreEqual(FENError.InvalidEnPassentSquare, FENHelpers.ValidateEnPassentSquare("z8"));
            Assert.AreEqual(FENError.InvalidEnPassentSquare, FENHelpers.ValidateEnPassentSquare("--"));
            Assert.AreEqual(FENError.InvalidEnPassentSquare, FENHelpers.ValidateEnPassentSquare("12"));
            Assert.AreEqual(FENError.InvalidEnPassentSquare, FENHelpers.ValidateEnPassentSquare("ee"));
            AssertThrowsFenException(MainValidation, GetFen(ValidPiecePlacement, ValidColor, ValidCastling, "e9", 0, 0), FENError.InvalidEnPassentSquare);
        }

        [Test]
        public void Should_Be_Valid_When_EnPassent_Square_Is_OK()
        {
            Assert.AreEqual(FENError.NULL, FENHelpers.ValidateEnPassentSquare("e6"));
            Assert.AreEqual(FENError.NULL, FENHelpers.ValidateEnPassentSquare("a3"));
            Assert.AreEqual(FENError.NULL, FENHelpers.ValidateEnPassentSquare("h3"));
            Assert.AreEqual(FENError.NULL, FENHelpers.ValidateEnPassentSquare("-"));
        }

        [Test]
        public void Should_Return_Error_When_Bad_Move_Number_Passed_Into_NumberParsers()
        {

            Assert.AreEqual(FENError.HalfmoveClock, FENHelpers.ValidateHalfmoveClock("-1"));
            Assert.AreEqual(FENError.HalfmoveClock, FENHelpers.ValidateHalfmoveClock(""));
            Assert.AreEqual(FENError.HalfmoveClock, FENHelpers.ValidateHalfmoveClock("-"));
            Assert.AreEqual(FENError.FullMoveCounter, FENHelpers.ValidateFullMoveCounter("-1"));
            Assert.AreEqual(FENError.FullMoveCounter, FENHelpers.ValidateFullMoveCounter(""));
            Assert.AreEqual(FENError.FullMoveCounter, FENHelpers.ValidateFullMoveCounter("-"));
        }

        [Test]
        public void Should_Return_0_When_Good_Move_Number_Passed_Into_NumberParsers()
        {

            Assert.AreEqual(FENError.NULL, FENHelpers.ValidateHalfmoveClock("1"));
            Assert.AreEqual(FENError.NULL, FENHelpers.ValidateHalfmoveClock("0"));
            Assert.AreEqual(FENError.NULL, FENHelpers.ValidateHalfmoveClock("3"));
            Assert.AreEqual(FENError.NULL, FENHelpers.ValidateFullMoveCounter("31"));
            Assert.AreEqual(FENError.NULL, FENHelpers.ValidateFullMoveCounter("0"));
            Assert.AreEqual(FENError.NULL, FENHelpers.ValidateFullMoveCounter("1"));
        }


        [Test]
        public void Should_Return_NonNegative_Int_From_FEN_Move_Piece()
        {
            Assert.AreEqual((uint?)24, FENHelpers.GetMoveNumberFromString("24"));
        }

        [Test]
        public void Should_Return_Error_When_Color_String_Is_Invalid()
        {
            Assert.AreEqual(FENError.InvalidActiveColor, FENHelpers.ValidateActiveColor("z"));
            Assert.AreEqual(FENError.InvalidActiveColor, FENHelpers.ValidateActiveColor(""));
        }

        [Test]
        public void Should_Return_Null_When_Color_String_Is_Valid()
        {
            Assert.AreEqual(FENError.NULL, FENHelpers.ValidateActiveColor("w"));
            Assert.AreEqual(FENError.NULL, FENHelpers.ValidateActiveColor("b"));
        }

        [Test]
        public void Should_Set_Active_Color_As_White_When_Passed_w()
        {

            Assert.AreEqual(Color.White, FENHelpers.GetActiveColor("w"));
            Assert.AreEqual(Color.Black, FENHelpers.GetActiveColor("b"));
        }



        //[Test]
        //public void Should_Throw_Exception_For_Pawns_On_First_Ranks()
        //{
        //    var fen1 = "rnbqkbnr/ppp2ppp/1p6/8/8/8/PPP2PPP/RNBQKBNP";
        //    var fen2 = "pnbqkbnr/ppp2pppp/8/8/8/1P6/P2PPPPP/RNBQKBNR";
        //    var fen3 = "rnbqkbnr/ppp1pppp/1p6/8/8/8/PPP2PPP/RNBQKBNp";
        //    var fen4 = "Pnbqkbnr/ppp1ppppp/8/8/8/1P6/PPPP1PPP/RNBQKBNR";

        //    Assert.Throws(typeof(FENPiecePlacementPawnException), () => { FENHelpers.ValidateFENPiecePlacementPawns(fen1); });
        //    Assert.Throws(typeof(FENPiecePlacementPawnException), () => { FENHelpers.ValidateFENPiecePlacementPawns(fen2); });
        //    Assert.Throws(typeof(FENPiecePlacementPawnException), () => { FENHelpers.ValidateFENPiecePlacementPawns(fen3); });
        //    Assert.Throws(typeof(FENPiecePlacementPawnException), () => { FENHelpers.ValidateFENPiecePlacementPawns(fen4); });
        //}

        //[Test]
        //public void Should_Throw_Exception_For_Too_Many_Pieces()
        //{
        //    var fen1 = "rnbqkbnr/pppppppp/1r6/8/8/8/PPPPPPPP/RNBQKBNR";
        //    var fen2 = "rnbqkbnr/pppppppp/8/8/8/1R6/PPPPPPPP/RNBQKBNR";
        //    var fen3 = "rnbqkbnr/pppppppp/1r6/8/8/1R6/PPPPPPPP/RNBQKBNR";


        //    Assert.Throws(typeof(FENPieceCountTooHighException), () => { FENHelpers.ValidateFENPiecePlacement(fen1); });
        //    Assert.Throws(typeof(FENPieceCountTooHighException), () => { FENHelpers.ValidateFENPiecePlacement(fen2); });
        //    Assert.Throws(typeof(FENPieceCountTooHighException), () => { FENHelpers.ValidateFENPiecePlacement(fen3); });
        //}

        //[Test]
        //public void Should_Throw_Exception_On_Incomplete_FEN()
        //{
        //    var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0";
        //    Assert.Throws(typeof(FENException), () => { FENHelpers.ValidateFENStructure(fen); });
        //}

        //[Test]
        //public void Should_Throw_Exception_On_Too_Many_Ranks()
        //{
        //    var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        //    var message = "";
        //    Assert.Throws(typeof(FENPiecePlacementException), () =>
        //    {
        //        try
        //        {
        //            FENHelpers.ValidateFENPiecePlacement(fen);
        //        }
        //        catch (FENPiecePlacementException f)
        //        {
        //            message = f.Message;
        //            throw;
        //        }
        //    });
        //    Console.WriteLine(message);
        //}

        [Test]
        public void Should_Return_Error_On_Invalid_Piece_Characters_In_Fen()
        {
            var fen = "rnbqkbnr/ppspppzp/8/8/8/8/PPPPPPPP/RNBQKBNR";
            Assert.AreEqual(FENError.PiecePlacementInvalidChars, FENHelpers.ValidatePiecePlacement(fen));
        }

        [Test]
        public void Should_Return_Error_When_Ranks_In_Fen_Not_8()
        {

            var fen = ValidPiecePlacement;
            Assert.AreEqual(FENError.PiecePlacementRankCount, FENHelpers.ValidatePiecePlacement(fen + "/rnbqkbnr"));
            Assert.AreEqual(FENError.PiecePlacementRankCount, FENHelpers.ValidatePiecePlacement("rnbqkbnr/rnbqkbnr"));
        }

        [Test]
        public void Should_Return_Error_When_Too_Many_Squares_In_Fen()
        {
            var fen = "rnbqkbnr/ppppppppp/8/8/8/8/PPPPPPPP/RNBQrKBNR";

            Assert.AreEqual(FENError.PiecePlacementPieceCountInRank, FENHelpers.ValidatePiecePlacement(fen));
        }

        [Test]
        public void Should_Return_Null_When_Right_Squares_In_Fen()
        {
            Assert.AreEqual(FENError.NULL, FENHelpers.ValidatePiecePlacement(ValidPiecePlacement));
        }

        //[Test]
        //public void Should_Throw_Exception_When_Too_Many_Kings_In_Fen()
        //{
        //    var fen1 = "rnbqkbnr/pppppppp/1k6/8/8/8/PPPPPPPP/RNBQKBNR";
        //    var fen2 = "rnbqkbnr/ppppppppp/8/8/8/1K6/PPPPPPPP/RNBQKBNR";
        //    var fen3 = "rnbqkbnr/ppppppppp/1k6/8/8/1K6/PPPPPPPP/RNBQKBNR";
        //    var fen4 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQRBNR";
        //    var fen5 = "rnbqrbnr/ppppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
        //    var fen6 = "rnbqrbnr/ppppppppp/8/8/8/8/PPPPPPPP/RNBQRBNR";
        //    var message = "";
        //    Assert.Throws(typeof(FENPiecePlacementKingException), () => { FENHelpers.ValidateFENPiecePlacementKing(fen1); });
        //    Assert.Throws(typeof(FENPiecePlacementKingException), () => { FENHelpers.ValidateFENPiecePlacementKing(fen2); });
        //    Assert.Throws(typeof(FENPiecePlacementKingException), () => { FENHelpers.ValidateFENPiecePlacementKing(fen3); });
        //    Assert.Throws(typeof(FENPiecePlacementKingException), () => { FENHelpers.ValidateFENPiecePlacementKing(fen4); });
        //    Assert.Throws(typeof(FENPiecePlacementKingException), () => { FENHelpers.ValidateFENPiecePlacementKing(fen5); });
        //    Assert.Throws(typeof(FENPiecePlacementKingException), () => { FENHelpers.ValidateFENPiecePlacementKing(fen6); });
        //    Console.WriteLine(message);
        //}

        //[Test]
        //public void Should_Throw_Exception_When_Too_Many_Pawns_In_Fen()
        //{
        //    var fen1 = "rnbqkbnr/pppppppp/1p6/8/8/8/PPPPPPPP/RNBQKBNR";
        //    var fen2 = "rnbqkbnr/ppppppppp/8/8/8/1P6/PPPPPPPP/RNBQKBNR";
        //    var fen3 = "rnbqkbnr/ppppppppp/1p6/8/8/1P6/PPPPPPPP/RNBQKBNR";

        //    Assert.Throws(typeof(FENPiecePlacementPawnException), () => { FENHelpers.ValidateFENPiecePlacementPawns(fen1); });
        //    Assert.Throws(typeof(FENPiecePlacementPawnException), () => { FENHelpers.ValidateFENPiecePlacementPawns(fen2); });
        //    Assert.Throws(typeof(FENPiecePlacementPawnException), () => { FENHelpers.ValidateFENPiecePlacementPawns(fen3); });
        //}

        //[Test]
        //public void Should_Throw_Exception_When_All_Squares_Unaccounted_For_In_Fen()
        //{
        //    var fen = "rnbqkbnr/ppppppp/8/8/8/8/PPPPPPPP/PPPPPPPP/RNBQKBNR";
        //    var message = "";
        //    Assert.Throws(typeof(FENPiecePlacementException), () =>
        //    {
        //        try
        //        {
        //            FENHelpers.ValidateFENPiecePlacement(fen);
        //        }
        //        catch (FENException f)
        //        {
        //            message = f.Message;
        //            throw;
        //        }
        //    });
        //    Console.WriteLine(message);
        //}
        #endregion
    }
}
