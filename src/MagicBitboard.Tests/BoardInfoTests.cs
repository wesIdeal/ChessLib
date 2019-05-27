using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Game;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using ChessLib.Validators.MoveValidation;
using NUnit.Framework;
using System;
using System.Text;

// ReSharper disable once CheckNamespace
namespace MagicBitboard.Tests
{
    [TestFixture]
    public class BoardInfoTests
    {
        [SetUp]
        public void Setup()
        {
            _biScandi = new BoardInfo(FENScandi);
        }

        private const string FENScandi = "rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2";
        private const string FENQueenAttacksd4 = "8/1k6/8/3q4/3P4/8/6K1/8 w - - 0 2";
        private const string FENQueenIsBlockedFromAttackingSquared4 = "8/1k6/3q4/3P4/3P4/8/6K1/8 w - - 0 2";

        private BoardInfo _biScandi;

        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
        }

        private const string InitialBoard = FENHelpers.FENInitial;
        private const string After1E4 = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";
        private const string After1E4C5 = "rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2";
        private const string InitialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        [TestFixture]
        private class CastlingValidation
        {
            /// <summary>
            ///     Tests that availability is unset appropriately, depending on color and piece moved.
            /// </summary>
            [TestFixture(Description =
                "Tests that availability is unset appropriately, depending on color and piece moved.")]
            private class AvailabilityUnset
            {
                [SetUp]
                public void Setup()
                {
                    _bi = new BoardInfo(CastlingFen);
                }

                private BoardInfo _bi;
                private const string CastlingFen = "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1";

                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetBlackKingside_When_h8RookMoves()
                {
                    var move = MoveHelpers.GenerateMove(63, 62);
                    var expected = CastlingAvailability.BlackQueenside | CastlingAvailability.WhiteKingside |
                                   CastlingAvailability.WhiteQueenside;
                    _bi.CastlingAvailability = BoardHelpers.GetCastlingAvailabilityPostMove(_bi, move, Piece.Rook);
                    Assert.AreEqual(expected, _bi.CastlingAvailability,
                        "Expected castling availability to equal qKQ after h8 Rook moves.");
                }

                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetBlackQueenside_When_a8RookMoves()
                {
                    var move = MoveHelpers.GenerateMove(56, 57);
                    var expected = CastlingAvailability.BlackKingside | CastlingAvailability.WhiteKingside |
                                   CastlingAvailability.WhiteQueenside;
                    _bi.CastlingAvailability = BoardHelpers.GetCastlingAvailabilityPostMove(_bi, move, Piece.Rook);
                    Assert.AreEqual(expected, _bi.CastlingAvailability,
                        "Expected castling availability to equal kKQ after a8 Rook moves.");
                }

                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetBoth_WhenBlackKingMoves()
                {
                    var move = MoveHelpers.GenerateMove(60, 61);
                    var expected = CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside;
                    _bi.CastlingAvailability = BoardHelpers.GetCastlingAvailabilityPostMove(_bi, move, Piece.King);
                    Assert.AreEqual(expected, _bi.CastlingAvailability,
                        "Expected castling availability to equal KQ after Black King moves.");
                }

                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetBoth_WhenWhiteKingMoves()
                {
                    var move = MoveHelpers.GenerateMove(4, 5);
                    var expected = CastlingAvailability.BlackQueenside | CastlingAvailability.BlackKingside;
                    _bi.CastlingAvailability = BoardHelpers.GetCastlingAvailabilityPostMove(_bi, move, Piece.King);
                    Assert.AreEqual(expected, _bi.CastlingAvailability,
                        "Expected castling availability to equal kq after White King moves.");
                }

                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetWhiteKingside_When_h1RookMoves()
                {
                    var move = MoveHelpers.GenerateMove(7, 6);
                    var expected = CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside |
                                   CastlingAvailability.WhiteQueenside;
                    _bi.CastlingAvailability = BoardHelpers.GetCastlingAvailabilityPostMove(_bi, move, Piece.Rook);
                    Assert.AreEqual(expected, _bi.CastlingAvailability,
                        "Expected castling availability to equal kqQ after h1 Rook moves.");
                }

                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetWhiteQueenside_When_a1RookMoves()
                {
                    var move = MoveHelpers.GenerateMove(0, 1);
                    var expected = CastlingAvailability.BlackQueenside | CastlingAvailability.BlackKingside |
                                   CastlingAvailability.WhiteKingside;
                    _bi.CastlingAvailability = BoardHelpers.GetCastlingAvailabilityPostMove(_bi, move, Piece.Rook);
                    Assert.AreEqual(expected, _bi.CastlingAvailability,
                        "Expected castling availability to equal kqK after a1 Rook moves.");
                }
            }
        }

        [TestFixture(Description = "Tests move application on board.")]
        private class MoveApplication
        {
            [SetUp]
            public void Setup()
            {
                _bInitial = new BoardInfo(InitialFEN);
            }

            private BoardInfo _bInitial;

            [Test(Description = "1. e4 test")]
            public void ApplyMove_ShouldReflectCorrectBoardStatusAfter_e4()
            {
                var move = MoveHelpers.GenerateMove(12, 28);
                var expectedFEN = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";
                _bInitial.ApplyMove(move);
                Assert.AreEqual(expectedFEN, _bInitial.ToFEN());
            }


            [Test(Description = "Ruy - applying series of moves")]
            public void ApplyMove_ShouldReflectCorrectBoardStatusAfterSeriesOfMoves()
            {
                var expectedFEN = new[]
                {
                    "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", //1. e4
                    "rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e6 0 2", //1...e5
                    "rnbqkbnr/pppp1ppp/8/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2", //2. Nf3
                    "r1bqkbnr/pppp1ppp/2n5/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 2 3", //2...Nc6
                    "r1bqkbnr/pppp1ppp/2n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3", //3. Bb5
                    "r1bqkbnr/1ppp1ppp/p1n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 0 4", //3...a6
                    "r1bqkbnr/1ppp1ppp/p1n5/4p3/B3P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 4", //4. Ba4
                    "r1bqkb1r/1ppp1ppp/p1n2n2/4p3/B3P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 2 5", // 4...Nf6
                    "r1bqkb1r/1ppp1ppp/p1n2n2/4p3/B3P3/5N2/PPPP1PPP/RNBQ1RK1 b kq - 3 5" //O-O
                };
                var moves = new[]
                {
                    MoveHelpers.GenerateMove(12, 28),
                    MoveHelpers.GenerateMove(52, 36),
                    MoveHelpers.GenerateMove(6, 21),
                    MoveHelpers.GenerateMove(57, 42),
                    MoveHelpers.GenerateMove(5, 33),
                    MoveHelpers.GenerateMove(48, 40),
                    MoveHelpers.GenerateMove(33, 24),
                    MoveHelpers.GenerateMove(62, 45),
                    MoveHelpers.GenerateMove(4, 6, MoveType.Castle)
                };
                Assert.AreEqual(expectedFEN.Length, moves.Length);
                for (var i = 0; i < moves.Length; i++)
                {
                    var expected = expectedFEN[i];
                    _bInitial.ApplyMove(moves[i]);
                    Assert.AreEqual(expected, _bInitial.ToFEN());
                }
            }
        }

        [Test(Description = "Test Castling Availability Retrieval")]
        public static void GetCastlingAvailabilityString()
        {
            var bi = new BoardInfo(InitialFEN);

            var expected = "KQkq";
            var stm = bi.GetCastlingAvailabilityString();
            Assert.AreEqual(expected, stm);
        }

        [Test(Description = "Test En Passant Retrieval to return a string square representation or '-'")]
        public static void GetEnPassantString_ShouldReturnEPRepresentation()
        {
            var bi = new BoardInfo(InitialFEN);

            var expected = "-";
            var stm = bi.GetEnPassantString();
            Assert.AreEqual(expected, stm);
            bi = new BoardInfo("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1");
            expected = "e3";
            Assert.AreEqual(expected, bi.GetEnPassantString());
        }

        [Test(Description = "Tests halfmove clock to string representation")]
        public static void GetHalfMoveClockString_ShouldReturnCorrectValue()
        {
            var bi = new BoardInfo(InitialFEN);
            var expected = "0";
            Assert.AreEqual(expected, bi.GetHalfMoveClockString());

            bi = new BoardInfo("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 30 1");
            expected = "30";
            Assert.AreEqual(expected, bi.GetHalfMoveClockString());
        }

        [Test(Description = "Tests move counter to string representation")]
        public static void GetMoveCountString_ShouldReturnCorrectValue()
        {
            var bi = new BoardInfo(InitialFEN);
            var expected = "1";
            Assert.AreEqual(expected, bi.GetMoveCounterString());

            bi = new BoardInfo("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 30 31");
            expected = "31";
            Assert.AreEqual(expected, bi.GetMoveCounterString());
        }

        [Test(Description = "Test piece section retrieval")]
        public static void GetPiecePlacement_ShouldReturnCorrectString()
        {
            var bi = new BoardInfo();
            var expected = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
            var piecePlacementActual = bi.GetPiecePlacement();
            Assert.AreEqual(expected, piecePlacementActual);
        }


        [TestCase("rnbqkbnr/pp1ppppp/8/1Bp5/4P3/8/PPPP1PPP/RNBQK1NR b KQkq - 1 2", 0x8000000000000ul)]
        [TestCase("rnbqkbnr/pp1ppppp/8/2p5/B3P3/8/PPPP1PPP/RNBQK1NR b KQkq - 1 2", 0x8000000000000ul)]
        [TestCase("rnbqk1nr/pp1pbppp/2p5/8/B7/8/PPPPQPPP/RNB1K1NR b KQkq - 1 2", 0x10000000000000ul)]
        [TestCase("rnbqk1nr/pp1pb1pp/2p1p3/8/B7/8/PPPPQPPP/RNB1K1NR b KQkq - 1 2", 0ul)]
        [TestCase("4k3/3p4/2p5/1B6/8/8/6K1/8 b - - 0 1", 0ul)]
        [TestCase("rnbqk1nr/pp1pb1pp/5p2/8/B7/2p5/PPPPQPPP/RNB1K1NR b KQkq - 1 2", 0x18000000000000ul)]
        public static void GetPinnedPieces_ShouldReturnValueOfPinnedPiece(string fen, ulong expected)
        {
            var bi = new BoardInfo(fen);
            var actual = bi.GetAbsolutePins();
            Assert.AreEqual(expected, actual,
                "Method did not determine piece was pinned.");
        }

        [Test(Description = "Test side-to-move retrieval")]
        public static void GetSideToMoveChar_ShouldReturnCorrectString()
        {
            var bi = new BoardInfo(InitialFEN);
            var expected = "w";
            var stm = bi.GetSideToMoveStrRepresentation();
            Assert.AreEqual(expected, stm);
        }


        [Test]
        public static void SetEnPassantFlag_ShouldSetFlagToEnPassantCaptureSquare_WhenPawnsMove2SquaresForward()
        {
            for (ushort i = 8; i < 16; i++)
            {
                var bi = new BoardInfo(InitialFEN);
                var move = MoveHelpers.GenerateMove(i, (ushort)(i + 16));
                bi.ApplyMove(move);
                Assert.AreEqual(i + 8, bi.EnPassantSquare);
            }
        }



        [Test]
        public static void Should_Set_Board_After_1e4()
        {
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
            var rv = new BoardInfo(After1E4);

            Assert.AreEqual(whitePawns, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Pawn]);
            Assert.AreEqual(blackPawns, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Pawn]);

            Assert.AreEqual(whiteRooks, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Rook]);
            Assert.AreEqual(blackRooks, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Rook]);

            Assert.AreEqual(whiteKnights, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Knight]);
            Assert.AreEqual(blackKnights, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Knight]);

            Assert.AreEqual(whiteBishops, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Bishop]);
            Assert.AreEqual(blackBishops, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Bishop]);

            Assert.AreEqual(whiteQueen, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Queen]);
            Assert.AreEqual(blackQueen, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Queen]);

            Assert.AreEqual(whiteKing, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.King]);
            Assert.AreEqual(blackKing, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.King]);
        }

        [Test]
        public static void Should_Set_Board_After_1e4_c5()
        {
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
            var rv = new BoardInfo(After1E4C5);

            Assert.AreEqual(whitePawns, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Pawn]);
            Assert.AreEqual(blackPawns, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Pawn]);

            Assert.AreEqual(whiteRooks, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Rook]);
            Assert.AreEqual(blackRooks, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Rook]);

            Assert.AreEqual(whiteKnights, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Knight]);
            Assert.AreEqual(blackKnights, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Knight]);

            Assert.AreEqual(whiteBishops, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Bishop]);
            Assert.AreEqual(blackBishops, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Bishop]);

            Assert.AreEqual(whiteQueen, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Queen]);
            Assert.AreEqual(blackQueen, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Queen]);

            Assert.AreEqual(whiteKing, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.King]);
            Assert.AreEqual(blackKing, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.King]);
        }

        [Test]
        public static void Should_Set_Initial_Board()
        {
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
            var rv = new BoardInfo(InitialBoard);
            Assert.AreEqual(whitePawns, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Pawn]);
            Assert.AreEqual(blackPawns, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Pawn]);

            Assert.AreEqual(whiteRooks, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Rook]);
            Assert.AreEqual(blackRooks, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Rook]);

            Assert.AreEqual(whiteKnights, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Knight]);
            Assert.AreEqual(blackKnights, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Knight]);

            Assert.AreEqual(whiteBishops, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Bishop]);
            Assert.AreEqual(blackBishops, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Bishop]);

            Assert.AreEqual(whiteQueen, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.Queen]);
            Assert.AreEqual(blackQueen, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.Queen]);

            Assert.AreEqual(whiteKing, rv.PiecePlacement[BoardHelpers.WHITE][(int)Piece.King]);
            Assert.AreEqual(blackKing, rv.PiecePlacement[BoardHelpers.BLACK][(int)Piece.King]);
        }


        [Test]
        public static void ShouldFindCorrectSource_PawnMove_NoCapture_StartingPosition()
        {
            const ulong blackPawnOcc = 0xff000000000000;
            const ulong whitePawnOcc = 0x0000000000ff00;
            var boardInfo = new BoardInfo();
            var md = new MoveDetail
            {
                Color = Color.Black
            };
            boardInfo.ActivePlayer = Color.Black;
            for (ushort destIndex = 40; destIndex >= 32; destIndex--)
            {
                md.MoveText = destIndex.IndexToSquareDisplay();
                md.DestinationFile = (ushort)(destIndex % 8);
                md.DestinationRank = (ushort)(destIndex / 8);
                var expectedSource = MoveHelpers.GenerateMove((ushort)(48 + (destIndex % 8)), destIndex);
                var actual = boardInfo.GenerateMoveFromText<MoveValidator>(destIndex.IndexToSquareDisplay());
                Assert.AreEqual(expectedSource.Move, actual.Move);
            }

            md.Color = Color.White;
            boardInfo.ActivePlayer = Color.White;
            for (ushort destIndex = 31; destIndex >= 16; destIndex--)
            {
                md.MoveText = destIndex.IndexToSquareDisplay();
                md.DestinationFile = (ushort)(destIndex % 8);
                md.DestinationRank = (ushort)(destIndex / 8);
                var expectedSource = MoveHelpers.GenerateMove((ushort)(8 + (destIndex % 8)), destIndex);
                var actual = boardInfo.GenerateMoveFromText<MoveValidator>(destIndex.IndexToSquareDisplay());
                Assert.AreEqual(expectedSource.Move, actual.Move);
            }
        }


        [TestCase("r1b2rk1/p3np1p/1pn1pp1q/2b5/2B1N3/3Q1NP1/PPP2P1P/1K1R3R w - - 1 15", null, 30, Piece.Pawn, Color.White, "g4", 22)]
        [TestCase("r4rk1/p4p1p/1p4n1/2b1pbNP/1nB1Nq2/8/PPP2PQ1/1K1R3R b - - 0 21", null, 54, Piece.King, Color.Black, "Kg7", 62)]
        [TestCase("4k3/8/8/8/8/8/8/4K3 w - - 0 1", null, 3, Piece.King, Color.White, "Kd1", 4)]
        [TestCase(FENHelpers.FENInitial, null, 21, Piece.Knight, Color.White, "Nf3", 6)]
        [TestCase("4k3/8/8/8/8/8/8/Q1qbK3 w - - 0 1", null, 56, Piece.Queen, Color.White, "Qa8+", 0)]
        [TestCase("4k3/8/8/8/8/8/8/R1qbK3 w - - 0 1", null, 56, Piece.Rook, Color.White, "Ra8+", 0)]
        [TestCase("rnb1kbnr/pp3ppp/4p3/2pq4/3P4/8/PPPN1PPP/R1BQKBNR w KQkq - 0 5", null, 21, Piece.Knight, Color.White, "Ngf3", 6)]
        [TestCase("4r1k1/1p1qrb2/p5p1/3p1p2/P1pb1P2/4N2R/1PBN1KP1/3QR3 w - - 4 31", null, 5, Piece.Knight, Color.White, "Nf1", 11)]
        [TestCase("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2", null, 26, Piece.Bishop, Color.White, "Bc4", 5)]
        public static void ShouldMoveSource(string fen, int? sourceIdx, int destIdx, Piece p, Color c, string moveText, int expected)
        {
            var bi = new BoardInfo(fen) { ActivePlayer = c };
            //var md = new MoveDetail((ushort?)sourceIdx, (ushort)destIdx, p, c, moveText).GetAvailableMoveDetails();
            var actualMove = bi.GenerateMoveFromText<MoveValidator>(moveText);
            Assert.AreEqual(expected, actualMove.SourceIndex);
        }

        [TestCase("rnb1kbnr/1p1ppppp/p7/1qp5/3PP3/P1P5/1P3PPP/RNBQKBNR w KQkq - 1 5", "Ba6")]
        public static void FindPieceSourceIndex_ShouldThrowException(string fen, string moveText)
        {
            var bi = new BoardInfo(fen);
            Assert.Throws(typeof(MoveException),
                () => { bi.GenerateMoveFromText<MoveValidator>(moveText); });
        }






        //[Test]
        //public static void ShouldThrowExceptionWhenNoBishopAttacksSquare()
        //{

        //}

        //[Test]
        //public static void ShouldThrowExceptionWhenNoKingAttacksSquare()
        //{
        //    var bi = new BoardInfo("4k3/8/8/8/8/8/8/Q1qbK3 w - - 0 1");
        //    var md = new MoveDetail(null, null, 0, 2, Piece.King, Color.White, "Kc1");
        //    Assert.Throws(typeof(MoveException),
        //        () => { bi.FindKingMoveSourceIndex(md, bi.ActivePlayerKingIndex, bi.TotalOccupancy); });
        //}

        //[Test]
        //public static void ShouldThrowExceptionWhenNoKnightAttacksSquare()
        //{
        //    var bi = new BoardInfo("rnb1kbnr/1p1ppppp/p7/1q6/2pPP3/PNP5/1P2NPPP/R2QKB1R w KQkq - 1 5");
        //    var md = new MoveDetail(null, null, 3, 4, Piece.Knight, Color.White, "Ne4");
        //    Assert.Throws(typeof(MoveException), () => { bi.FindKnightMoveSourceIndex(md, bi.ActiveKnightOccupancy); });
        //}

        //[Test]
        //public static void ShouldThrowExceptionWhenNoQueenAttacksSquare()
        //{
        //    var bi = new BoardInfo("4k3/8/8/8/8/8/8/Q1qbK3 w - - 0 1");
        //    var md = new MoveDetail(null, null, 0, 3, Piece.Queen, Color.White, "Qxd1");
        //    Assert.Throws(typeof(MoveException),
        //        () => { bi.FindQueenMoveSourceIndex(md, bi.ActiveQueenOccupancy, bi.TotalOccupancy); });
        //}

        //[Test]
        //public static void ShouldThrowExceptionWhenNoRookAttacksSquare()
        //{
        //    var bi = new BoardInfo("4k3/8/8/8/8/8/8/R1qbK3 w - - 0 1");
        //    var md = new MoveDetail(null, null, 0, 3, Piece.Rook, Color.White, "Rxd1");
        //    Assert.Throws(typeof(MoveException),
        //        () => { bi.FindRookMoveSourceIndex(md, bi.ActiveRookOccupancy, bi.TotalOccupancy); });
        //}

        //[Test]
        //public static void ShouldThrowExceptionWhenTwoBishopsAttackSquare()
        //{
        //    var bi = new BoardInfo("rnb1kbnr/1p1ppppp/p7/1q6/2pPP3/PBP5/1P3PPP/RN1QKBNR w KQkq - 1 5");
        //    var md = new MoveDetail(null, null, 3, 2, Piece.Bishop, Color.White, "Bc4");
        //    Assert.Throws(typeof(MoveException),
        //        () => { bi.FindBishopMoveSourceIndex(md, bi.ActiveBishopOccupancy, bi.TotalOccupancy); });
        //}

        //[Test]
        //public static void ShouldThrowExceptionWhenTwoKnightsAttackSquare()
        //{
        //    var bi = new BoardInfo("rnb1kbnr/1p1ppppp/p7/1q6/2pPP3/PNP5/1P2NPPP/R2QKB1R w KQkq - 1 5");
        //    var md = new MoveDetail(null, null, 5, 0, Piece.Knight, Color.White, "Nxd4");
        //    Assert.Throws(typeof(MoveException), () => { bi.FindKnightMoveSourceIndex(md, bi.ActiveKnightOccupancy); });
        //}

        //[Test]
        //public static void ShouldThrowExceptionWhenTwoQueensAttackSquare()
        //{
        //    var bi = new BoardInfo("4k3/8/8/8/8/8/2Q5/Q1qbK3 w - - 0 1");
        //    var md = new MoveDetail(null, null, 0, 2, Piece.Queen, Color.White, "Qxc1");
        //    Assert.Throws(typeof(MoveException),
        //        () => { bi.FindQueenMoveSourceIndex(md, bi.ActiveQueenOccupancy, bi.TotalOccupancy); });
        //}

        //[Test]
        //public static void ShouldThrowExceptionWhenTwoRooksAttackSquare()
        //{
        //    var bi = new BoardInfo("4k3/8/8/8/8/8/2R5/R1qbK3 w - - 0 1");
        //    var md = new MoveDetail(null, null, 0, 2, Piece.Rook, Color.White, "Rxd1");
        //    Assert.Throws(typeof(MoveException),
        //        () => { bi.FindRookMoveSourceIndex(md, bi.ActiveRookOccupancy, bi.TotalOccupancy); });
        //}


        [Test(Description = "ToFEN() should return the FEN of the current board's state")]
        public static void ToFEN_ShouldReturnCurrentBoardState()
        {
            const string initialFEN = InitialFEN;
            var bi = new BoardInfo(initialFEN);
            var actual = bi.ToFEN();
            Assert.AreEqual(initialFEN, actual);

            const string closedRuyFEN = "r1bqkbnr/1ppp1ppp/p1n5/4p3/B3P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 4";
            bi = new BoardInfo(closedRuyFEN);
            actual = bi.ToFEN();
            Assert.AreEqual(closedRuyFEN, actual);
        }

        [Test]
        public static void ValidatePawnMove_ShouldThrowExc_IfNoPieceAvailableForCapture()
        {
            var pawnAttackBoards = PieceAttackPatternHelper.PawnAttackMask[(int)Color.White];
            for (ushort idx = 8; idx < 16; idx++)
            {
                var attackBoard = pawnAttackBoards[idx];
                var notAttackBoard = ~attackBoard;
                var attackSquare = attackBoard.GetSetBits()[0];
                var pawnOccupancy = BoardHelpers.RankMasks[1];
                Assert.Throws(typeof(MoveException),
                    () =>
                    {
                        BoardInfo.ValidatePawnMove(Color.White, idx, attackSquare, pawnOccupancy,
                            pawnOccupancy | notAttackBoard);
                    });
                Assert.DoesNotThrow(
                    () =>
                    {
                        BoardInfo.ValidatePawnMove(Color.White, idx, attackSquare, pawnOccupancy,
                            pawnOccupancy | attackBoard);
                    });
            }
        }

        [Test]
        public static void ValidatePawnMove_ShouldThrowExc_WhenMoveIsBlocked()
        {
            const ulong occupancyBothRanks = 0x1010000;
            const ulong occupancy3RdRank = 0x10000;

            ulong occBoth, occ3, occ4;
            var md = new MoveDetail { SourceRank = 1, Color = Color.White };
            ushort pawnSourceIndex = 8;
            for (ushort idx = 16; idx < 23; idx++)
            {
                md.DestinationFile = idx.FileFromIdx();
                md.DestinationRank = idx.RankFromIdx();
                occBoth = occupancyBothRanks << (pawnSourceIndex - 8);
                occ3 = occupancy3RdRank << (pawnSourceIndex - 8);
                occ4 = occ3 << 8;

                var pawnOcc = BoardHelpers.RankMasks[1];
                var destinationIndex = (ushort)(pawnSourceIndex + 8);
                Assert.Throws(typeof(MoveException),
                    () =>
                    {
                        BoardInfo.ValidatePawnMove(md.Color, pawnSourceIndex, destinationIndex, pawnOcc,
                            occBoth | pawnOcc);
                    });
                Assert.Throws(typeof(MoveException),
                    () =>
                    {
                        BoardInfo.ValidatePawnMove(md.Color, pawnSourceIndex, destinationIndex, pawnOcc,
                            occ3 | pawnOcc);
                    });
                if (pawnSourceIndex >= 16)
                    Assert.Throws(typeof(MoveException),
                        () =>
                        {
                            BoardInfo.ValidatePawnMove(md.Color, pawnSourceIndex, destinationIndex, pawnOcc,
                                occ4 | pawnOcc);
                        });
                else
                    Assert.DoesNotThrow(() =>
                    {
                        BoardInfo.ValidatePawnMove(md.Color, pawnSourceIndex, destinationIndex, pawnOcc,
                            occ4 | pawnOcc);
                    });
            }
        }
        StringBuilder sb = new StringBuilder();
        [TestCase("5r2/6Pk/1R6/7P/6K1/8/8/8 w - - 0 62", 54, 61, PromotionPiece.Queen, MoveType.Promotion)]
        [TestCase("6K1/4k1P1/8/7q/8/8/8/8 b - - 9 56", 52, 60)]
        [TestCase("6K1/4k1P1/8/6q1/8/8/8/8 b - - 9 56", 38, 39)]
        public void IsStalemateAfterMove(string fen, int f, int t, PromotionPiece p = PromotionPiece.Knight, MoveType type = MoveType.Normal)
        {
            var board = new BoardInfo(fen);
            var move = MoveHelpers.GenerateMove((ushort)f, (ushort)t, type, p);
            var result = board.ApplyMove(move);
            Assert.AreEqual(true, board.IsStalemate<MoveValidator>());

        }

        [TestCase("5Q2/7k/1R6/7P/6K1/8/8/8 b - - 0 62", true)]
        [TestCase("4k1K1/6P1/8/7q/8/8/8/8 w - - 10 57", true)]
        [TestCase("6K1/4k1P1/8/7q/8/8/8/8 w - - 10 57", true)]
        [TestCase("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", false)]
        [TestCase("rnbqkbnr/pppppppp/8/8/3P4/8/PPP1PPPP/RNBQKBNR b KQkq d3 0 1", false)]
        [TestCase("8/8/8/8/8/8/5Qk1/4K3 b - - 0 1", false)]
        [TestCase("8/8/8/8/8/8/5QkQ/4K3 b - - 0 1", false)]
        [TestCase("8/8/8/8/3b4/8/3Q2k1/4K3 b - - 0 1", false)]
        public void IsStalemate(string fen, bool isStalematedPosition)
        {
            var board = new BoardInfo(fen);
            Assert.AreEqual(isStalematedPosition, board.IsStalemate<MoveValidator>());
            Console.WriteLine(sb.ToString());
        }

        [TestCase("8/8/8/8/8/8/5Qk1/4K3 b - - 0 1", Color.Black, true)]
        [TestCase("8/8/8/8/8/8/5QkQ/4K3 b - - 0 1", Color.Black, false)]
        [TestCase("8/8/8/8/3b4/8/3Q2k1/4K3 b - - 0 1", Color.Black, true)]
        public static void CanKingMoveToAnotherSquare(string fen, Color c, bool expectedResult)
        {
            var pieces = new BoardInfo(fen);
            var kingIndex = pieces.ActivePlayerKingIndex;
            var result = pieces.CanPieceMove<MoveValidator>(kingIndex);
            Assert.AreEqual(expectedResult, result);

        }

        [TestCase("8/8/8/8/3bB2n/6Q1/6k1/4K3 b - - 0 1", 1)]
        [TestCase("8/8/8/8/3b1B1n/6Q1/6k1/4K3 b - - 0 1", 1)]
        [TestCase("8/8/8/8/3b4/5N1Q/6k1/4K3 b - - 0 1", 1)]
        [TestCase("8/8/8/8/3bB2n/8/3Q2k1/4K3 b - - 0 1", 3)]
        [TestCase("8/8/8/8/3b2B1/5N1Q/6k1/4K3 b - - 0 1", 0)]
        public static void CanEvadeThroughBlockOrCapture_CaptureChecker(string fen, int expectedMoveCount)
        {
            var bi = new BoardInfo(fen);
            var actual = bi.GetEvasions();
            Assert.AreEqual(expectedMoveCount, actual.Length);
        }

        [TestCase("8/8/8/8/3b2B1/5N1Q/6k1/4K3 b - - 0 1")]
        [TestCase("3qk3/5Q1p/8/p1p1N3/Pp2bP1P/1P1r4/8/4RnK1 b - - 6 38")]
        [TestCase("7R/pp4p1/2p3Bk/5P2/7P/8/PP4p1/4K3 b - - 1 55")]
        [TestCase("4R3/2p3pk/pp3p2/5n1p/2P2P1P/P5r1/1P4q1/3QR2K w - - 6 41")]
        public static void GetEvasions_ReturnsNoMovesWhenMate(string fen)
        {
            var bi = new BoardInfo(fen);
            Assert.AreEqual(0, bi.GetEvasions().Length);
        }
    }
}