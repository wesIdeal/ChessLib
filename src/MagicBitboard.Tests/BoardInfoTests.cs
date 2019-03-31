using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using NUnit.Framework;
using System;
using ChessLib.Data;

// ReSharper disable once CheckNamespace
namespace MagicBitboard.Tests
{
    [TestFixture]
    public class BoardInfoTests
    {

        const string FENScandi = "rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2";
        const string FENQueenAttacksd4 = "8/1k6/8/3q4/3P4/8/6K1/8 w - - 0 2";
        const string FENQueenIsBlockedFromAttackingSquared4 = "8/1k6/3q4/3P4/3P4/8/6K1/8 w - - 0 2";
        GameInfo _giScandi;
        BoardInfo _biScandi;
        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
            DateTime dtStart = DateTime.Now;
            double totalMs = DateTime.Now.Subtract(dtStart).TotalMilliseconds;
            Console.WriteLine($"Bitboard made in {totalMs} ms");
        }
        [SetUp]
        public void Setup()
        {
            _giScandi = new GameInfo(FENScandi);
            _biScandi = _giScandi.BoardInfo;
        }

        #region FEN Tests
        [Test(Description = "Test piece section retrieval")]
        public static void GetPiecePlacement_ShouldReturnCorrectString()
        {
            var bi = BoardInfo.BoardInfoFromFen(InitialFEN);
            var expected = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
            var piecePlacementActual = bi.GetPiecePlacement();
            Assert.AreEqual(expected, piecePlacementActual);
        }

        [Test(Description = "Test side-to-move retrieval")]
        public static void GetSideToMoveChar_ShouldReturnCorrectString()
        {
            var bi = BoardInfo.BoardInfoFromFen(InitialFEN);
            var expected = "w";
            var stm = bi.GetSideToMoveStrRepresentation();
            Assert.AreEqual(expected, stm);
        }

        [Test(Description = "Test Castling Availability Retrieval")]
        public static void GetCastlingAvailabilityString()
        {
            var bi = BoardInfo.BoardInfoFromFen(InitialFEN);

            var expected = "KQkq";
            var stm = bi.GetCastlingAvailabilityString();
            Assert.AreEqual(expected, stm);
        }

        [Test(Description = "Test En Passant Retrieval to return a string square representation or '-'")]
        public static void GetEnPassantString_ShouldReturnEPRepresentation()
        {
            var bi = BoardInfo.BoardInfoFromFen(InitialFEN);

            var expected = "-";
            var stm = bi.GetEnPassantString();
            Assert.AreEqual(expected, stm);
            bi = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1");
            expected = "e3";
            Assert.AreEqual(expected, bi.GetEnPassantString());
        }

        [Test(Description = "Tests halfmove clock to string representation")]
        public static void GetHalfMoveClockString_ShouldReturnCorrectValue()
        {
            var bi = BoardInfo.BoardInfoFromFen(InitialFEN);
            var expected = "0";
            Assert.AreEqual(expected, bi.GetHalfMoveClockString());

            bi = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 30 1");
            expected = "30";
            Assert.AreEqual(expected, bi.GetHalfMoveClockString());
        }

        [Test(Description = "Tests move counter to string representation")]
        public static void GetMoveCountString_ShouldReturnCorrectValue()
        {
            var bi = BoardInfo.BoardInfoFromFen(InitialFEN);
            var expected = "1";
            Assert.AreEqual(expected, bi.GetMoveCounterString());

            bi = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 30 31");
            expected = "31";
            Assert.AreEqual(expected, bi.GetMoveCounterString());
        }



        [Test(Description = "ToFEN() should return the FEN of the current board's state")]
        public static void ToFEN_ShouldReturnCurrentBoardState()
        {
            const string initialFEN = InitialFEN;
            var bi = BoardInfo.BoardInfoFromFen(initialFEN);
            var actual = bi.ToFEN();
            Assert.AreEqual(initialFEN, actual);

            const string closedRuyFEN = "r1bqkbnr/1ppp1ppp/p1n5/4p3/B3P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 4";
            bi = BoardInfo.BoardInfoFromFen(closedRuyFEN);
            actual = bi.ToFEN();
            Assert.AreEqual(closedRuyFEN, actual);
        }

        #endregion
        [Test]
        public void Should_Return_True_When_d5_Is_Attacked()
        {
            ushort? d5 = "d5".SquareTextToIndex();
            Assert.IsNotNull(d5);
            bool isAttacked = _biScandi.IsAttackedBy(Color.White, d5.Value);
            Assert.IsTrue(isAttacked);
        }

        [Test]
        public static void Should_Return_True_When_d5_Is_Attacked_2()
        {
            GameInfo gi = new GameInfo(FENQueenIsBlockedFromAttackingSquared4);
            ushort? d5 = "d5".SquareTextToIndex();
            Assert.IsNotNull(d5);
            bool isAttacked = gi.BoardInfo.IsAttackedBy(Color.Black, d5.Value);
            Assert.IsTrue(isAttacked);
        }

        [Test]
        public void Should_Return_False_When_d4_Is_Not_Attacked()
        {
            ushort? d4 = "d4".SquareTextToIndex();
            Assert.IsNotNull(d4);
            bool isAttacked = _biScandi.IsAttackedBy(Color.White, d4.Value);
            Assert.IsFalse(isAttacked);
        }

        [Test]
        public static void Should_Return_False_When_d4_Is_Not_Attacked_2()
        {
            GameInfo gi = new GameInfo(FENQueenIsBlockedFromAttackingSquared4);
            ushort? d4 = "d4".SquareTextToIndex();
            Assert.IsNotNull(d4);
            bool isAttacked = gi.BoardInfo.IsAttackedBy(Color.Black, d4.Value);
            Assert.IsFalse(isAttacked);
        }

        [Test]
        public static void Should_Return_True_When_d4_Is_Attacked()
        {
            GameInfo gi = new GameInfo(FENQueenAttacksd4);
            ushort? d4 = "d4".SquareTextToIndex();
            Assert.IsNotNull(d4);
            bool isAttacked = gi.BoardInfo.IsAttackedBy(Color.Black, d4.Value);
            Assert.IsTrue(isAttacked);
        }

        [Test]
        public static void ShouldGetCorrectMoveWhenPromotionIsSent()
        {
            //for (ushort i = 48; i < 56; i++)
            //{
            //    for (var pieceIdx = PromotionPiece.Knight; pieceIdx < PromotionPiece.Queen; pieceIdx++)
            //    {
            //        var expected = MoveHelpers.GenerateMove(i, (ushort)(i + 8), MoveType.Promotion, pieceIdx);
            //        var input = DisplayHelpers.IndexToSquareDisplay((ushort)(i + 8)) + $"={PieceHelpers.GetCharFromPromotionPiece(pieceIdx)}";
            //        Assert.AreEqual(expected, MoveHelpers.GenerateMoveFromText(input, Color.White));
            //    }
            //}
            //biEnPassant.ActivePlayer = Color.Black;
            //for (ushort i = 8; i < 16; i++)
            //{
            //    for (var pieceIdx = PromotionPiece.Knight; pieceIdx < PromotionPiece.Queen; pieceIdx++)
            //    {
            //        var expected = MoveHelpers.GenerateMove(i, (ushort)(i - 8), MoveType.Promotion, pieceIdx);
            //        var input = DisplayHelpers.IndexToSquareDisplay((ushort)(i - 8)) + $"={PieceHelpers.GetCharFromPromotionPiece(pieceIdx)}";
            //        Assert.AreEqual(expected, MoveHelpers.GenerateMoveFromText(input, Color.Black));
            //    }
            //}
        }
        [Test]
        public static void ShouldFailWhenNoPawnIsIncapableOfPromotion()
        {
            string fen = "8/PPPP1PPP/8/2k5/8/2K5/pppp1ppp/8 w - - 0 1";
            BoardInfo bi = BoardInfo.BoardInfoFromFen(fen);
            Assert.Throws(typeof(MoveException), () =>
            {
                bi.ActivePlayer = Color.White;
                bi.ValidateAndGetResultingBoardFromMove(bi.GenerateMoveFromText("e8=Q"));
            });
            Assert.Throws(typeof(MoveException), () =>
            {
                bi.ActivePlayer = Color.Black;
                bi.ValidateAndGetResultingBoardFromMove(bi.GenerateMoveFromText("e1=Q"));
            });
        }

        [Test]
        public static void ShouldFailWhenAPieceBlocksPromotion()
        {
            string fen = "4q3/PPPPPPPP/8/2k5/8/2K5/pppppppp/4Q3 w - - 0 1";
            BoardInfo bi = BoardInfo.BoardInfoFromFen(fen);
            Assert.Throws(typeof(MoveException), () =>
            {
                bi.ActivePlayer = Color.White;
                bi.ValidateAndGetResultingBoardFromMove(bi.GenerateMoveFromText("e8=Q"));
            });
            Assert.Throws(typeof(MoveException), () =>
            {
                bi.ActivePlayer = Color.Black;
                bi.ValidateAndGetResultingBoardFromMove(bi.GenerateMoveFromText("e1=Q"));
            });
        }

        [Test]
        public static void ShouldFindCorrectSource_PawnMove_NoCapture_StartingPosition()
        {
            const ulong blackPawnOcc = 0xff000000000000;
            const ulong whitePawnOcc = 0x0000000000ff00;
            BoardInfo boardInfo = new GameInfo().BoardInfo;
            MoveDetail md = new MoveDetail
            {
                Color = Color.Black
            };
            for (ushort destIndex = 40; destIndex >= 32; destIndex--)
            {
                md.MoveText = destIndex.IndexToSquareDisplay();
                md.DestinationFile = (ushort)(destIndex % 8);
                md.DestinationRank = (ushort)(destIndex / 8);
                int expectedSource = 48 + destIndex.FileFromIdx();
                ushort actual = BoardInfo.FindPawnMoveSourceIndex(md, blackPawnOcc, boardInfo.TotalOccupancy);
                Assert.AreEqual(expectedSource, actual);
            }
            md.Color = Color.White;

            for (ushort destIndex = 31; destIndex >= 16; destIndex--)
            {
                md.MoveText = destIndex.IndexToSquareDisplay();
                md.DestinationFile = (ushort)(destIndex % 8);
                md.DestinationRank = (ushort)(destIndex / 8);
                int expectedSource = 8 + destIndex.FileFromIdx();
                ushort actual = BoardInfo.FindPawnMoveSourceIndex(md, whitePawnOcc, boardInfo.TotalOccupancy);
                Assert.AreEqual(expectedSource, actual);
            }
        }

        [Test]
        public static void ValidatePawnMove_ShouldThrowExc_WhenMoveIsBlocked()
        {
            const ulong occupancyBothRanks = 0x1010000;
            const ulong occupancy3RdRank = 0x10000;

            ulong occBoth, occ3, occ4;
            MoveDetail md = new MoveDetail { SourceRank = 1, Color = Color.White };
            ushort pawnSourceIndex = 8;
            for (ushort idx = 16; idx < 23; idx++)
            {
                md.DestinationFile = idx.FileFromIdx();
                md.DestinationRank = idx.RankFromIdx();
                occBoth = (occupancyBothRanks << (pawnSourceIndex - 8));
                occ3 = occupancy3RdRank << (pawnSourceIndex - 8);
                occ4 = occ3 << 8;

                ulong pawnOcc = BoardHelpers.RankMasks[1];
                ushort destinationIndex = (ushort)(pawnSourceIndex + 8);
                Assert.Throws(typeof(MoveException), () => { BoardInfo.ValidatePawnMove(md.Color, pawnSourceIndex, destinationIndex, pawnOcc, occBoth | pawnOcc); });
                Assert.Throws(typeof(MoveException), () => { BoardInfo.ValidatePawnMove(md.Color, pawnSourceIndex, destinationIndex, pawnOcc, occ3 | pawnOcc); });
                if (pawnSourceIndex >= 16)
                {
                    Assert.Throws(typeof(MoveException), () => { BoardInfo.ValidatePawnMove(md.Color, pawnSourceIndex, destinationIndex, pawnOcc, occ4 | pawnOcc); });
                }
                else
                {
                    Assert.DoesNotThrow(() => { BoardInfo.ValidatePawnMove(md.Color, pawnSourceIndex, destinationIndex, pawnOcc, occ4 | pawnOcc); });
                }
            }
        }

        [Test]
        public static void ValidatePawnMove_ShouldThrowExc_IfNoPieceAvailableForCapture()
        {
            Board pawnAttackBoards = PieceAttackPatternHelper.PawnAttackMask[(int)Color.White];
            for (ushort idx = 8; idx < 16; idx++)
            {
                ulong attackBoard = pawnAttackBoards[idx];
                ulong notAttackBoard = ~attackBoard;
                ushort attackSquare = attackBoard.GetSetBits()[0];
                ulong pawnOccupancy = BoardHelpers.RankMasks[1];
                Assert.Throws(typeof(MoveException),
                    () =>
                    {
                        BoardInfo.ValidatePawnMove(Color.White, idx, attackSquare, pawnOccupancy, pawnOccupancy | notAttackBoard);
                    });
                Assert.DoesNotThrow(
                    () =>
                    {
                        BoardInfo.ValidatePawnMove(Color.White, idx, attackSquare, pawnOccupancy, pawnOccupancy | attackBoard);
                    });
            }
        }

        [Test]
        public static void ShouldFindKnightMoveSource()
        {
            BoardInfo bi = new GameInfo().BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 2, 5, Piece.Knight, Color.White, "Nf3");
            ushort actual = BoardInfo.FindKnightMoveSourceIndex(md, bi.ActiveKnightOccupancy);
            Assert.AreEqual(6, actual);
        }

        [Test]
        public static void ShouldThrowExceptionWhenNoKnightAttacksSquare()
        {
            BoardInfo bi = new GameInfo("rnb1kbnr/1p1ppppp/p7/1q6/2pPP3/PNP5/1P2NPPP/R2QKB1R w KQkq - 1 5").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 3, 4, Piece.Knight, Color.White, "Ne4");
            Assert.Throws(typeof(MoveException), () => { BoardInfo.FindKnightMoveSourceIndex(md, bi.ActiveKnightOccupancy); });
        }

        [Test]
        public static void ShouldThrowExceptionWhenTwoKnightsAttackSquare()
        {
            BoardInfo bi = new GameInfo("rnb1kbnr/1p1ppppp/p7/1q6/2pPP3/PNP5/1P2NPPP/R2QKB1R w KQkq - 1 5").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 5, 0, Piece.Knight, Color.White, "Nxd4");
            Assert.Throws(typeof(MoveException), () => { BoardInfo.FindKnightMoveSourceIndex(md, bi.ActiveKnightOccupancy); });
        }

        [Test]
        public static void ShouldFindBishopMoveSource()
        {
            BoardInfo bi = new GameInfo("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 3, 2, Piece.Bishop, Color.White, "Bc4");
            ushort actual = BoardInfo.FindBishopMoveSourceIndex(md, bi.ActiveBishopOccupancy, bi.TotalOccupancy);
            Assert.AreEqual(5, actual);
        }

        [Test]
        public static void ShouldThrowExceptionWhenNoBishopAttacksSquare()
        {
            BoardInfo bi = new GameInfo("rnb1kbnr/1p1ppppp/p7/1qp5/3PP3/P1P5/1P3PPP/RNBQKBNR w KQkq - 1 5").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 5, 0, Piece.Bishop, Color.White, "Ba6");
            Assert.Throws(typeof(MoveException), () => { BoardInfo.FindBishopMoveSourceIndex(md, bi.ActiveBishopOccupancy, bi.TotalOccupancy); });
        }

        [Test]
        public static void ShouldThrowExceptionWhenTwoBishopsAttackSquare()
        {
            BoardInfo bi = new GameInfo("rnb1kbnr/1p1ppppp/p7/1q6/2pPP3/PBP5/1P3PPP/RN1QKBNR w KQkq - 1 5").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 3, 2, Piece.Bishop, Color.White, "Bc4");
            Assert.Throws(typeof(MoveException), () => { BoardInfo.FindBishopMoveSourceIndex(md, bi.ActiveBishopOccupancy, bi.TotalOccupancy); });
        }

        [Test]
        public static void ShouldFindRookMoveSource()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/8/R1qbK3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 7, 0, Piece.Rook, Color.White, "Ra8+");
            Assert.AreEqual(0, BoardInfo.FindRookMoveSourceIndex(md, bi.ActiveRookOccupancy, bi.ActiveRookOccupancy));
        }

        [Test]
        public static void ShouldThrowExceptionWhenNoRookAttacksSquare()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/8/R1qbK3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 0, 3, Piece.Rook, Color.White, "Rxd1");
            Assert.Throws(typeof(MoveException), () => { BoardInfo.FindRookMoveSourceIndex(md, bi.ActiveRookOccupancy, bi.TotalOccupancy); });
        }

        [Test]
        public static void ShouldThrowExceptionWhenTwoRooksAttackSquare()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/2R5/R1qbK3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 0, 2, Piece.Rook, Color.White, "Rxd1");
            Assert.Throws(typeof(MoveException), () => { BoardInfo.FindRookMoveSourceIndex(md, bi.ActiveRookOccupancy, bi.TotalOccupancy); });
        }

        [Test]
        public static void ShouldFindQueenMoveSource()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/8/Q1qbK3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 7, 0, Piece.Queen, Color.White, "Qa8+");
            Assert.AreEqual(0, BoardInfo.FindQueenMoveSourceIndex(md, bi.ActiveQueenOccupancy, bi.TotalOccupancy));
        }

        [Test]
        public static void ShouldThrowExceptionWhenNoQueenAttacksSquare()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/8/Q1qbK3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 0, 3, Piece.Queen, Color.White, "Qxd1");
            Assert.Throws(typeof(MoveException), () =>
            {
                BoardInfo.FindQueenMoveSourceIndex(md, bi.ActiveQueenOccupancy, bi.TotalOccupancy);
            });
        }

        [Test]
        public static void ShouldThrowExceptionWhenTwoQueensAttackSquare()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/2Q5/Q1qbK3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 0, 2, Piece.Queen, Color.White, "Qxc1");
            Assert.Throws(typeof(MoveException), () =>
            {
                BoardInfo.FindQueenMoveSourceIndex(md, bi.ActiveQueenOccupancy, bi.TotalOccupancy);
            });
        }


        [Test]
        public static void ShouldFindKingMoveSource()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/8/4K3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 0, 3, Piece.King, Color.White, "Kd1");
            Assert.AreEqual(4, BoardInfo.FindKingMoveSourceIndex(md, bi.ActivePlayerKingOccupancy, bi.TotalOccupancy));
        }

        [Test]
        public static void ShouldThrowExceptionWhenNoKingAttacksSquare()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/8/Q1qbK3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 0, 2, Piece.King, Color.White, "Kc1");
            Assert.Throws(typeof(MoveException), () => { BoardInfo.FindKingMoveSourceIndex(md, bi.ActivePlayerKingOccupancy, bi.TotalOccupancy); });
        }

        #region Making Boards
        const string InitialBoard = FENHelpers.FENInitial;
        const string After1E4 = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";
        const string After1E4C5 = "rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2";
        private const string InitialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        [Test]
        public static void Should_Set_Initial_Board()
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
            var rv = BoardInfo.BoardInfoFromFen(InitialBoard);
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
        public static void Should_Set_Board_After_1e4()
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
            var rv = BoardInfo.BoardInfoFromFen(After1E4);

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
        public static void Should_Set_Board_After_1e4_c5()
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
            var rv = BoardInfo.BoardInfoFromFen(After1E4C5);

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

        [Test]
        public static void GetPinnedPieces_ShouldReturnValueOfPinnedPiece_WhenPieceIsPinned()
        {
            var bi = BoardInfo.BoardInfoFromFen("rnbqkbnr/pp1ppppp/8/1Bp5/4P3/8/PPPP1PPP/RNBQK1NR b KQkq - 1 2");
            var expectedPinnedPiece = 0x8000000000000ul; //the pawn on d7 is pinned
            var actual = bi.GetPinnedPieces();
            Assert.AreEqual(expectedPinnedPiece, actual, "Method did not determine that the pawn on d7 was pinned by the Bishop.");
            bi = BoardInfo.BoardInfoFromFen("rnbqkbnr/pp1ppppp/8/2p5/B3P3/8/PPPP1PPP/RNBQK1NR b KQkq - 1 2");
            actual = bi.GetPinnedPieces();
            Assert.AreEqual(expectedPinnedPiece, actual, "Method did not determine that the pawn on d7 was pinned by the Bishop.");
            bi = BoardInfo.BoardInfoFromFen("rnbqk1nr/pp1pbppp/2p5/8/B7/8/PPPPQPPP/RNB1K1NR b KQkq - 1 2");
            expectedPinnedPiece = 0x10000000000000ul;
            actual = bi.GetPinnedPieces();
            Assert.AreEqual(expectedPinnedPiece, actual, "Method did not determine that the Bishop on e7 was pinned by the Queen on e2.");
        }
        [Test]
        public static void GetPinnedPieces_ShouldReturnZero_WhenPieceIsNotPinned()
        {
            var bi = BoardInfo.BoardInfoFromFen("rnbqk1nr/pp1pb1pp/2p1p3/8/B7/8/PPPPQPPP/RNB1K1NR b KQkq - 1 2");
            var expectedPinnedPiece = 0x00; //the pawn on d7 is pinned
            var actual = bi.GetPinnedPieces();
            Assert.AreEqual(expectedPinnedPiece, actual, "Method did not determine that the pawn on d7 was pinned by the Bishop.");
        }

        [Test]
        public static void GetPinnedPieces_ShouldReturnNotZero_WhenPieceIsPinnedTwice()
        {
            var bi = BoardInfo.BoardInfoFromFen("rnbqk1nr/pp1pb1pp/5p2/8/B7/2p5/PPPPQPPP/RNB1K1NR b KQkq - 1 2");
            var expectedPinnedPiece = 0x18000000000000; //the pawn on d7 is pinned
            var actual = bi.GetPinnedPieces();
            Console.WriteLine($"The following are pinned:\r\n{actual.GetDisplayBits()}");
            Assert.AreEqual(expectedPinnedPiece, actual, "Method did not determine that the pawn on d7 was pinned by the Bishop.");

        }

        [Test]
        public static void GetPinnedPieces_ShouldReturnValueOfPinnedPieces_WhenPieceIsPinned2()
        {
            var bi = BoardInfo.BoardInfoFromFen("4k3/8/2p5/1B6/8/8/6K1/8 b - - 0 1");
            var actual = bi.GetPinnedPieces();
            Assert.AreEqual(0x40000000000, actual, "Did not calculate pawn on c6 as pinned by the Bishop on b5.");
            Assert.IsTrue(bi.IsPiecePinned(42), "IsPiecePinned() should have returned true for square index 42.");
        }

        [Test]
        public static void GetPinnedPieces_ShouldReturnZero_WhenPieceIsNotPinned2()
        {
            var bi = BoardInfo.BoardInfoFromFen("4k3/3p4/2p5/1B6/8/8/6K1/8 b - - 0 1");
            var actual = bi.GetPinnedPieces();
            Assert.AreEqual(0x00, actual, "No piece is pinned, with two pawns in front of King.");
        }

        [Test]
        public static void IsPiecePinned_ShouldReturnTrue_WhenPieceIsPinned()
        {
            var bi = BoardInfo.BoardInfoFromFen("4k3/8/2p5/1B6/8/8/6K1/8 b - - 0 1");
            Assert.IsTrue(bi.IsPiecePinned(42), "IsPiecePinned() should have returned true for square index 42.");
        }

        [Test]
        public static void IsPiecePinned_ShouldReturnFalse_WhenPieceIsNotPinned2()
        {
            var bi = BoardInfo.BoardInfoFromFen("4k3/3p4/2p5/1B6/8/8/6K1/8 b - - 0 1");
            Assert.IsFalse(bi.IsPiecePinned(42), "IsPiecePinned() should have returned true for square index 42.");
        }
        [Test]
        public static void IsPiecePinned_ShouldReturnTrue_WhenBothPiecesArePinned()
        {
            var bi = BoardInfo.BoardInfoFromFen("rnbqk1nr/pp1pb1pp/5p2/8/B7/2p5/PPPPQPPP/RNB1K1NR b KQkq - 1 2");
            var actual = bi.GetPinnedPieces();
            Console.WriteLine($"The following are pinned:\r\n{actual.GetDisplayBits()}");
            Assert.IsTrue(bi.IsPiecePinned(51), "IsPiecePinned() did not determine that the pawn at index 51 is pinned by the Bishop.");
            Assert.IsTrue(bi.IsPiecePinned(52), "IsPiecePinned() did not determine that the Bishop at index 52 is pinned by the Queen.");

        }

        [Test]
        public static void IsPiecePinned_ShouldReturnTrue_WhenPieceIsPinnedByRook()
        {
            var bi = BoardInfo.BoardInfoFromFen("rnbqk1nr/p2pb1pp/2p2p2/8/B7/2p5/PPPPRPPP/RNBQK1N1 b Qkq - 1 2");
            Assert.IsFalse(bi.IsPiecePinned(51), "IsPiecePinned() should have returned false for square index 42.");//Not pinned
            Assert.IsTrue(bi.IsPiecePinned(52), "IsPiecePinned() should have returned true as the Bishop at index 52 is pinned by the Rook.");//Not pinned
        }



        [Test]
        public static void ValidateMove_ShouldThrowException_IfMoveLeavesKingInCheck()
        {
            var move = MoveHelpers.GenerateMove(62, 44);
            var kingInCheck = BoardInfo.BoardInfoFromFen("5kb1/8/8/8/8/8/6K1/5R2 b - - 1 2");
            Assert.Throws(typeof(MoveException), () =>
            {
                try
                {
                    kingInCheck.ValidateAndGetResultingBoardFromMove(move);
                }
                catch (MoveException e)
                {
                    Assert.AreEqual(MoveExceptionType.MoveLeavesKingInCheck, e.ExceptionType, "Exception type should represent that the move leaves the King in check.");
                    throw;
                }
            }, "ValidateMove should throw and exception if the move leaves the King in check.");
        }

        [Test]
        public static void ValidateMove_ShouldThrowException_IfMoveLeavesKingInCheck2()
        {
            var move = MoveHelpers.GenerateMove(53, 62);
            var kingInCheck = BoardInfo.BoardInfoFromFen("5k2/5b2/8/8/8/8/6K1/5R2 b - - 1 2");
            Assert.Throws(typeof(MoveException), () =>
            {
                try
                {
                    kingInCheck.ValidateAndGetResultingBoardFromMove(move);
                }
                catch (MoveException e)
                {
                    Assert.AreEqual(MoveExceptionType.MoveLeavesKingInCheck, e.ExceptionType, "Exception type should represent that the move leaves the King in check.");
                    throw;
                }
            }, "ValidateMove should throw and exception if the move leaves the King in check. Move is Bg7.");
        }

        [Test]
        public static void ValidateMove_ShouldThrowException_IfMoveLeavesKingInCheck3()
        {
            var move = MoveHelpers.GenerateMove(61, 60);
            var kingInCheck = BoardInfo.BoardInfoFromFen("5k2/3B4/8/8/8/1b6/6K1/5R2 b - - 1 2");
            Assert.Throws(typeof(MoveException), () =>
            {
                try
                {
                    kingInCheck.ValidateAndGetResultingBoardFromMove(move);
                }
                catch (MoveException e)
                {
                    Assert.AreEqual(MoveExceptionType.MoveLeavesKingInCheck, e.ExceptionType, "Exception type should represent that the move leaves the King in check.");
                    throw;
                }
            }, "ValidateMove should throw and exception if the move leaves the King in check.");
        }

        [Test]
        public static void SetEnPassantFlag_ShouldSetFlagToEnPassantCaptureSquare_WhenPawnsMove2SquaresForward()
        {

            for (ushort i = 8; i < 16; i++)
            {
                var bi = BoardInfo.BoardInfoFromFen(InitialFEN);
                var move = MoveHelpers.GenerateMove(i, (ushort)(i + 16));
                bi.ApplyMove(move);
                Assert.AreEqual(i + 8, bi.EnPassantIndex);
            }

        }

        [TestFixture]
        class CastlingValidation
        {
            /// <summary>
            /// Tests that availability is unset appropriately, depending on color and piece moved.
            /// </summary>
            [TestFixture(Description = "Tests that availability is unset appropriately, depending on color and piece moved.")]
            class AvailabilityUnset
            {

                BoardInfo _bi;
                const string CastlingFen = "r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1";
                [SetUp]
                public void Setup()
                {
                    _bi = BoardInfo.BoardInfoFromFen(CastlingFen);
                }
                #region Castling Availabilty

                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetBlackKingside_When_h8RookMoves()
                {
                    var move = MoveHelpers.GenerateMove(63, 62);
                    var expected = CastlingAvailability.BlackQueenside | CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside;
                    _bi.UnsetCastlingAvailability(move, Piece.Rook);
                    Assert.AreEqual(expected, _bi.CastlingAvailability, "Expected castling availability to equal qKQ after h8 Rook moves.");
                }
                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetBlackQueenside_When_a8RookMoves()
                {
                    var move = MoveHelpers.GenerateMove(56, 57);
                    var expected = CastlingAvailability.BlackKingside | CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside;
                    _bi.UnsetCastlingAvailability(move, Piece.Rook);
                    Assert.AreEqual(expected, _bi.CastlingAvailability, "Expected castling availability to equal kKQ after a8 Rook moves.");
                }
                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetWhiteKingside_When_h1RookMoves()
                {
                    var move = MoveHelpers.GenerateMove(7, 6);
                    var expected = CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside | CastlingAvailability.WhiteQueenside;
                    _bi.UnsetCastlingAvailability(move, Piece.Rook);
                    Assert.AreEqual(expected, _bi.CastlingAvailability, "Expected castling availability to equal kqQ after h1 Rook moves.");
                }
                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetWhiteQueenside_When_a1RookMoves()
                {
                    var move = MoveHelpers.GenerateMove(0, 1);
                    var expected = CastlingAvailability.BlackQueenside | CastlingAvailability.BlackKingside | CastlingAvailability.WhiteKingside;
                    _bi.UnsetCastlingAvailability(move, Piece.Rook);
                    Assert.AreEqual(expected, _bi.CastlingAvailability, "Expected castling availability to equal kqK after a1 Rook moves.");
                }
                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetBoth_WhenBlackKingMoves()
                {
                    var move = MoveHelpers.GenerateMove(60, 61);
                    var expected = CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside;
                    _bi.UnsetCastlingAvailability(move, Piece.King);
                    Assert.AreEqual(expected, _bi.CastlingAvailability, "Expected castling availability to equal KQ after Black King moves.");
                }
                [Test]
                public void UnsetCastlingAvailability_ShouldUnsetBoth_WhenWhiteKingMoves()
                {
                    var move = MoveHelpers.GenerateMove(4, 5);
                    var expected = CastlingAvailability.BlackQueenside | CastlingAvailability.BlackKingside;
                    _bi.UnsetCastlingAvailability(move, Piece.King);
                    Assert.AreEqual(expected, _bi.CastlingAvailability, "Expected castling availability to equal kq after White King moves.");

                }
                #endregion
            }

            /// <summary>
            /// Tests for castling through checks.
            /// </summary>
            [TestFixture(Description = "Tests for castling through checks.")]
            class ThroughCheck
            {
                [Test]
                public static void AnySquaresInCheck_ShouldReturnFalse_IfKingUnaffectedByAnyPiece()
                {
                    var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
                    var pos3 = BoardInfo.BoardInfoFromFen("4k2r/8/8/8/8/8/8/4K3 b kq - 1 2");
                    Assert.IsFalse(pos3.AnySquaresInCheck(move), "AnySquaresInCheck() should return false when nothing blocks castling privilege.");
                }

                [Test]
                public static void AnySquaresInCheck_ShouldReturnTrue_IfAnySquareIsAttacked()
                {
                    var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
                    var pos1 = BoardInfo.BoardInfoFromFen("4k2r/8/8/8/8/8/8/4KR2 b kq - 1 2");
                    var pos2 = BoardInfo.BoardInfoFromFen("4k2r/8/8/8/8/8/8/4K1R1 b kq - 1 2");

                    var pos4 = BoardInfo.BoardInfoFromFen("4k2r/8/8/8/8/8/4R3/4K3 b kq - 1 2");

                    Assert.IsTrue(pos1.AnySquaresInCheck(move), "AnySquaresInCheck() should return true when Rook on f1 blocks castling privilege.");
                    Assert.IsTrue(pos2.AnySquaresInCheck(move), "AnySquaresInCheck() should return true when Rook on g1 blocks castling privilege.");

                    Assert.IsTrue(pos4.AnySquaresInCheck(move), "AnySquaresInCheck() should return true when Rook on e2 blocks castling privilege.");
                    Assert.IsTrue(pos4.AnySquaresInCheck(move), "AnySquaresInCheck() should return true when Rooks on f1-h1 block castling privilege.");

                }
                [Test]
                public static void AnySquaresInCheck_ShouldReturnFalse_IfKingUnaffectedByOpposingRook()
                {
                    var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
                    var pos3 = BoardInfo.BoardInfoFromFen("4k2r/8/8/8/8/8/8/4K2R b kq - 1 2");
                    Assert.IsFalse(pos3.AnySquaresInCheck(move), "AnySquaresInCheck() should return false when Rook on h1 doesn't block castling privilege.");
                }

            }

            /// <summary>
            /// Tests for castling through occupied and non-occupied squares between castling King and Rook
            /// </summary>
            [TestFixture(Description = "Tests for castling through occupied and non-occupied squares between castling King and Rook")]
            class OccupiedSquares
            {
                BoardInfo _biOccupied, _biNonOccupied;
                [SetUp]
                public void Setup()
                {
                    _biOccupied = BoardInfo.BoardInfoFromFen("r1N1k1Nr/8/8/8/8/8/8/R1B1K1BR w KQkq - 0 1");
                    _biNonOccupied = BoardInfo.BoardInfoFromFen("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
                }

                #region Occupancy Between Castle
                [Test]
                public void ValidateMove_CastleOccupancyBetween_ShouldThrowExceptionIfPiecesAreBetween_q()
                {
                    var move = MoveHelpers.GenerateMove(60, 58, MoveType.Castle);
                    AssertOccupiedExceptionThrown(move);
                }

                [Test]
                public void ValidateMove_CastleOccupancyBetween_ShouldThrowExceptionIfPiecesAreBetween_k()
                {
                    var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
                    AssertOccupiedExceptionThrown(move);
                }

                [Test]
                public void ValidateMove_CastleOccupancyBetween_ShouldThrowExceptionIfPiecesAreBetween_Q()
                {
                    var move = MoveHelpers.GenerateMove(4, 2, MoveType.Castle);
                    AssertOccupiedExceptionThrown(move);
                }

                [Test]
                public void ValidateMove_CastleOccupancyBetween_ShouldThrowExceptionIfPiecesAreBetween_K()
                {
                    var move = MoveHelpers.GenerateMove(4, 6, MoveType.Castle);
                    AssertOccupiedExceptionThrown(move);
                }
                #endregion

                #region No Occupancy Between Castle
                [Test]
                public void ValidateMove_CastleOccupancyBetween_ShouldNotThrowExceptionIfNoPiecesAreBetween_q()
                {
                    var move = MoveHelpers.GenerateMove(60, 58, MoveType.Castle);
                    AssertOccupiedExceptionNotThrown(move);
                }

                [Test]
                public void ValidateMove_CastleOccupancyBetween_ShouldNotThrowExceptionIfNoPiecesAreBetween_k()
                {
                    var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
                    AssertOccupiedExceptionNotThrown(move);
                }

                [Test]
                public void ValidateMove_CastleOccupancyBetween_ShouldNotThrowExceptionIfNoPiecesAreBetween_Q()
                {
                    var move = MoveHelpers.GenerateMove(4, 2, MoveType.Castle);
                    AssertOccupiedExceptionNotThrown(move);
                }

                [Test]
                public void ValidateMove_CastleOccupancyBetween_ShouldNotThrowExceptionIfNoPiecesAreBetween_K()
                {
                    var move = MoveHelpers.GenerateMove(4, 6, MoveType.Castle);
                    AssertOccupiedExceptionNotThrown(move);
                }
                #endregion


                private void AssertOccupiedExceptionThrown(MoveExt move)
                {
                    Assert.Throws(typeof(MoveException),
                        () =>
                        {
                            try
                            {
                                _biOccupied.ValidateMove_CastleOccupancyBetween(move);
                            }
                            catch (MoveException e)
                            {
                                Assert.AreEqual(MoveExceptionType.Castle_OccupancyBetween, e.ExceptionType, "Expected exception to contain error type Castle_OccupancyBetween.");
                                throw;
                            }
                        });
                }

                private void AssertOccupiedExceptionNotThrown(MoveExt move)
                {
                    Assert.DoesNotThrow(
                        () => { _biNonOccupied.ValidateMove_CastleOccupancyBetween(move); });
                }
            }

            /// <summary>
            /// Tests that appropriate castling availability flag is set for castling.
            /// </summary>
            [TestFixture(Description = "Tests that appropriate castling availability flag is set for castling.")]
            class AvailabilityForCastleSet
            {
                BoardInfo _bi;
                [Test(Description = "Test that no exception is thrown when Black castles Queenside with BlackQueenside flag set.")]
                public void ValidateMove_CastleKeyPiecesMoved_ShouldNotThrowExceptionWhenCastlingFlagIsSet_q()
                {
                    _bi = MakeCastlingBoard(CastlingAvailability.BlackQueenside);
                    var move = MoveHelpers.GenerateMove(60, 58, MoveType.Castle);
                    AssertCastlingAvailabilityExceptionNotThrown(move);
                }

                [Test(Description = "Test that an exception is thrown when Black castles Queenside with BlackQueenside flag not set.")]
                public void ValidateMove_CastleKeyPiecesMoved_ShouldThrowExceptionWhenCastlingFlagIsNotSet_q()
                {
                    _bi = MakeCastlingBoard(CastlingAvailability.BlackKingside | CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside);
                    var move = MoveHelpers.GenerateMove(60, 58, MoveType.Castle);
                    AssertCastlingAvailabilityExceptionThrown(move);
                }

                [Test(Description = "Test that no exception is thrown when Black castles Kingside with BlackKingside flag set.")]
                public void ValidateMove_CastleKeyPiecesMoved_ShouldNotThrowExceptionWhenCastlingFlagIsSet_k()
                {
                    _bi = MakeCastlingBoard(CastlingAvailability.BlackKingside);
                    var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
                    AssertCastlingAvailabilityExceptionNotThrown(move);
                }

                [Test(Description = "Test that an exception is thrown when Black castles Kingside with BlackKingside flag not set.")]
                public void ValidateMove_CastleKeyPiecesMoved_ShouldThrowExceptionWhenCastlingFlagIsNotSet_k()
                {
                    _bi = MakeCastlingBoard(CastlingAvailability.BlackQueenside | CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside);
                    var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
                    AssertCastlingAvailabilityExceptionThrown(move);
                }

                [Test(Description = "Test that no exception is thrown when White castles Queenside with WhiteQueenside flag set.")]
                public void ValidateMove_CastleKeyPiecesMoved_ShouldNotThrowExceptionWhenCastlingFlagIsSet_Q()
                {
                    _bi = MakeCastlingBoard(CastlingAvailability.WhiteQueenside);
                    var move = MoveHelpers.GenerateMove(4, 2, MoveType.Castle);
                    AssertCastlingAvailabilityExceptionNotThrown(move);
                }

                [Test(Description = "Test that an exception is thrown when White castles Queenside with WhiteQueenside flag not set.")]
                public void ValidateMove_CastleKeyPiecesMoved_ShouldThrowExceptionWhenCastlingFlagIsNotSet_Q()
                {
                    _bi = MakeCastlingBoard(CastlingAvailability.WhiteKingside | CastlingAvailability.BlackQueenside | CastlingAvailability.BlackQueenside);
                    var move = MoveHelpers.GenerateMove(4, 2, MoveType.Castle);
                    AssertCastlingAvailabilityExceptionThrown(move);
                }

                [Test(Description = "Test that no exception is thrown when White castles Kingside with WhiteKingside flag set.")]
                public void ValidateMove_CastleKeyPiecesMoved_ShouldNotThrowExceptionWhenCastlingFlagIsSet_K()
                {
                    _bi = MakeCastlingBoard(CastlingAvailability.WhiteKingside);
                    var move = MoveHelpers.GenerateMove(4, 6, MoveType.Castle);
                    AssertCastlingAvailabilityExceptionNotThrown(move);
                }

                [Test(Description = "Test that an exception is thrown when White castles Kingside with WhiteKingside flag not set.")]
                public void ValidateMove_CastleKeyPiecesMoved_ShouldThrowExceptionWhenCastlingFlagIsNotSet_K()
                {
                    _bi = MakeCastlingBoard(CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackKingside | CastlingAvailability.BlackQueenside);
                    var move = MoveHelpers.GenerateMove(4, 6, MoveType.Castle);
                    AssertCastlingAvailabilityExceptionThrown(move);
                }

                [Test(Description = "Test that an exception is thrown when White castles Kingside with WhiteKingside flag not set.")]
                public void ValidateMove_CastleKeyPiecesMoved_ShouldThrowExceptionWhenNoCastlingFlagIsSet()
                {
                    _bi = MakeCastlingBoard(CastlingAvailability.NoCastlingAvailable);
                    var move = MoveHelpers.GenerateMove(4, 6, MoveType.Castle);
                    AssertCastlingAvailabilityExceptionThrown(move);
                    move = MoveHelpers.GenerateMove(60, 58, MoveType.Castle);
                    AssertCastlingAvailabilityExceptionThrown(move);
                    move = MoveHelpers.GenerateMove(4, 2, MoveType.Castle);
                    AssertCastlingAvailabilityExceptionThrown(move);
                    move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
                    AssertCastlingAvailabilityExceptionThrown(move);
                }

                private static BoardInfo MakeCastlingBoard(CastlingAvailability ca, char color = 'b')
                {
                    string baseBoard = $"r3k2r/8/8/8/8/8/8/R3K2R {color} {FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(ca)} - 0 1";
                    return BoardInfo.BoardInfoFromFen(baseBoard);
                }

                private void AssertCastlingAvailabilityExceptionThrown(MoveExt move)
                {
                    Assert.Throws(typeof(MoveException),
                        () =>
                        {
                            try
                            {
                                _bi.ValidateMove_CastleAvailability(move);
                            }
                            catch (MoveException e)
                            {
                                Assert.AreEqual(MoveExceptionType.Castle_Unavailable, e.ExceptionType, "Expected exception to contain error type Castle_Unavailable.");
                                throw;
                            }
                        }, "Expected MoveException to be thrown if castling availability disallows castling.");
                }

                private void AssertCastlingAvailabilityExceptionNotThrown(MoveExt move)
                {
                    Assert.DoesNotThrow(
                        () => { _bi.ValidateMove_CastleAvailability(move); });
                }
            }


        }
        [TestFixture(Description = "Tests move application on board.")]
        class MoveApplication
        {
            BoardInfo _bInitial;
            [SetUp]
            public void Setup()
            {
                _bInitial = BoardInfo.BoardInfoFromFen(InitialFEN);
            }

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
                var expectedFEN = new[] {
                    "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1",      //1. e4
                    "rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e6 0 2",    //1...e5
                    "rnbqkbnr/pppp1ppp/8/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2",   //2. Nf3
                    "r1bqkbnr/pppp1ppp/2n5/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 2 3", //2...Nc6
                    "r1bqkbnr/pppp1ppp/2n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3", //3. Bb5
                    "r1bqkbnr/1ppp1ppp/p1n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 0 4",//3...a6
                    "r1bqkbnr/1ppp1ppp/p1n5/4p3/B3P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 4", //4. Ba4
                    "r1bqkb1r/1ppp1ppp/p1n2n2/4p3/B3P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 2 5",// 4...Nf6
                    "r1bqkb1r/1ppp1ppp/p1n2n2/4p3/B3P3/5N2/PPPP1PPP/RNBQ1RK1 b kq - 3 5" //O-O
                };
                var moves = new[] {
                    MoveHelpers.GenerateMove(12, 28),
                    MoveHelpers.GenerateMove(52,36),
                    MoveHelpers.GenerateMove(6,21),
                    MoveHelpers.GenerateMove(57,42),
                    MoveHelpers.GenerateMove(5, 33),
                    MoveHelpers.GenerateMove(48,40),
                    MoveHelpers.GenerateMove(33,24),
                    MoveHelpers.GenerateMove(62,45),
                    MoveHelpers.GenerateMove(4,6,MoveType.Castle)
            };
                Assert.AreEqual(expectedFEN.Length, moves.Length);
                for (int i = 0; i < moves.Length; i++)
                {
                    var expected = expectedFEN[i];
                    _bInitial.ApplyMove(moves[i]);
                    Assert.AreEqual(expected, _bInitial.ToFEN());
                }
            }
        }
    }
}

