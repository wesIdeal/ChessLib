using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using NUnit.Framework;
using System;

namespace MagicBitboard.Helpers.Tests
{
    [TestFixture]
    public class BoardInfoTests
    {

        const string fenScandi = "rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2";
        const string fenQueenAttacksd4 = "8/1k6/8/3q4/3P4/8/6K1/8 w - - 0 2";
        const string fenQueenIsBlockedFromAttackingd4 = "8/1k6/3q4/3P4/3P4/8/6K1/8 w - - 0 2";
        GameInfo giScandi;
        BoardInfo biScandi;
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            DateTime dtStart = DateTime.Now;
            double totalMs = DateTime.Now.Subtract(dtStart).TotalMilliseconds;
            Console.WriteLine($"Bitboard made in {totalMs} ms");
        }
        [SetUp]
        public void Setup()
        {
            giScandi = new GameInfo(fenScandi);
            biScandi = giScandi.BoardInfo;
        }
        [Test]
        public void Should_Return_True_When_d5_Is_Attacked()
        {
            ushort? d5 = BoardHelpers.SquareTextToIndex("d5");
            bool isAttacked = biScandi.IsAttackedBy(Color.White, d5.Value);
            Assert.IsTrue(isAttacked);
        }

        [Test]
        public void Should_Return_True_When_d5_Is_Attacked_2()
        {
            GameInfo gi = new GameInfo(fenQueenIsBlockedFromAttackingd4);
            ushort? d5 = BoardHelpers.SquareTextToIndex("d5");
            bool isAttacked = gi.BoardInfo.IsAttackedBy(Color.Black, d5.Value);
            Assert.IsTrue(isAttacked);
        }

        [Test]
        public void Should_Return_False_When_d4_Is_Not_Attacked()
        {
            ushort? d4 = BoardHelpers.SquareTextToIndex("d4");
            bool isAttacked = biScandi.IsAttackedBy(Color.White, d4.Value);
            Assert.IsFalse(isAttacked);
        }

        [Test]
        public void Should_Return_False_When_d4_Is_Not_Attacked_2()
        {
            GameInfo gi = new GameInfo(fenQueenIsBlockedFromAttackingd4);
            ushort? d4 = BoardHelpers.SquareTextToIndex("d4");
            bool isAttacked = gi.BoardInfo.IsAttackedBy(Color.Black, d4.Value);
            Assert.IsFalse(isAttacked);
        }

        [Test]
        public void Should_Return_True_When_d4_Is_Attacked()
        {
            GameInfo gi = new GameInfo(fenQueenAttacksd4);
            ushort? d4 = BoardHelpers.SquareTextToIndex("d4");
            bool isAttacked = gi.BoardInfo.IsAttackedBy(Color.Black, d4.Value);
            Assert.IsTrue(isAttacked);
        }

        [Test]
        public void ShouldGetCorrectMoveWhenPromotionIsSent()
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
            //biEnPassent.ActivePlayer = Color.Black;
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
        public void ShouldFailWhenNoPawnIsIncapableOfPromotion()
        {
            string fen = "8/PPPP1PPP/8/2k5/8/2K5/pppp1ppp/8 w - - 0 1";
            BoardInfo bi = BoardInfo.BoardInfoFromFen(fen);
            Assert.Throws(typeof(MoveException), () =>
            {
                bi.ActivePlayer = Color.White;
                bi.ValidateMove(bi.GenerateMoveFromText("e8=Q"));
            });
            Assert.Throws(typeof(MoveException), () =>
            {
                bi.ActivePlayer = Color.Black;
                bi.ValidateMove(bi.GenerateMoveFromText("e1=Q"));
            });
        }

        [Test]
        public void ShouldFailWhenAPieceBlocksPromotion()
        {
            string fen = "4q3/PPPPPPPP/8/2k5/8/2K5/pppppppp/4Q3 w - - 0 1";
            BoardInfo bi = BoardInfo.BoardInfoFromFen(fen);
            Assert.Throws(typeof(MoveException), () =>
            {
                bi.ActivePlayer = Color.White;
                bi.ValidateMove(bi.GenerateMoveFromText("e8=Q"));
            });
            Assert.Throws(typeof(MoveException), () =>
            {
                bi.ActivePlayer = Color.Black;
                bi.ValidateMove(bi.GenerateMoveFromText("e1=Q"));
            });
        }

        [Test]
        public void ShouldFindCorrectSource_PawnMove_NoCapture_StartingPosition()
        {
            const ulong blackPawnOcc = 0xff000000000000;
            const ulong whitePawnOcc = 0x0000000000ff00;
            BoardInfo boardInfo = new GameInfo().BoardInfo;
            MoveDetail md = new MoveDetail();
            md.Color = Color.Black;
            for (ushort destIndex = 40; destIndex >= 32; destIndex--)
            {
                md.MoveText = destIndex.IndexToSquareDisplay();
                md.DestinationFile = (ushort)(destIndex % 8);
                md.DestinationRank = (ushort)(destIndex / 8);
                int expectedSource = 48 + destIndex.FileFromIdx();
                ushort actual = boardInfo.FindPawnMoveSourceIndex(md, blackPawnOcc);
                Assert.AreEqual(expectedSource, actual);
            }
            md.Color = Color.White;

            for (ushort destIndex = 31; destIndex >= 16; destIndex--)
            {
                md.MoveText = destIndex.IndexToSquareDisplay();
                md.DestinationFile = (ushort)(destIndex % 8);
                md.DestinationRank = (ushort)(destIndex / 8);
                int expectedSource = 8 + destIndex.FileFromIdx();
                ushort actual = boardInfo.FindPawnMoveSourceIndex(md, whitePawnOcc);
                Assert.AreEqual(expectedSource, actual);
            }
        }

        [Test]
        public void ValidatePawnMove_ShouldThrowExc_WhenMoveIsBlocked()
        {
            const ulong cOccupancyBothRanks = 0x1010000;
            const ulong cOccupancy3rdRank = 0x10000;

            BoardInfo bi = new GameInfo().BoardInfo;
            ulong occBoth, occ3, occ4;
            MoveDetail md = new MoveDetail { SourceRank = 1, Color = Color.White };
            ushort pawnSourceIndex = 8;
            for (ushort idx = 16; idx < 23; idx++)
            {
                md.DestinationFile = (ushort)(idx.FileFromIdx());
                md.DestinationRank = (ushort)idx.RankFromIdx();
                occBoth = (cOccupancyBothRanks << (pawnSourceIndex - 8));
                occ3 = cOccupancy3rdRank << (pawnSourceIndex - 8);
                occ4 = occ3 << 8;

                ulong pawnOcc = BoardHelpers.RankMasks[1];
                ushort destinationIndex = (ushort)(pawnSourceIndex + (ushort)8);
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
        public void ValidatePawnMove_ShouldThrowExc_IfNoPieceAvailableForCapture()
        {

            BoardInfo bi = new GameInfo().BoardInfo;
            MoveDetail md = new MoveDetail { SourceRank = 1, Color = Color.White };
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
        public void ShouldFindKnightMoveSource()
        {
            BoardInfo bi = new GameInfo().BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 2, 5, Piece.Knight, Color.White, "Nf3");
            ushort actual = bi.FindKnightMoveSourceIndex(md);
            Assert.AreEqual(6, actual);
        }

        [Test]
        public void ShouldThrowExceptionWhenNoKnightAttacksSquare()
        {
            BoardInfo bi = new GameInfo("rnb1kbnr/1p1ppppp/p7/1q6/2pPP3/PNP5/1P2NPPP/R2QKB1R w KQkq - 1 5").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 3, 4, Piece.Knight, Color.White, "Ne4");
            Assert.Throws(typeof(MoveException), () => { bi.FindKnightMoveSourceIndex(md); });
        }

        [Test]
        public void ShouldThrowExceptionWhenTwoKnightsAttackSquare()
        {
            BoardInfo bi = new GameInfo("rnb1kbnr/1p1ppppp/p7/1q6/2pPP3/PNP5/1P2NPPP/R2QKB1R w KQkq - 1 5").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 5, 0, Piece.Knight, Color.White, "Nxd4");
            Assert.Throws(typeof(MoveException), () => { bi.FindKnightMoveSourceIndex(md); });
        }

        [Test]
        public void ShouldFindBishopMoveSource()
        {
            BoardInfo bi = new GameInfo("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 3, 2, Piece.Bishop, Color.White, "Bc4");
            ushort actual = bi.FindBishopMoveSourceIndex(md);
            Assert.AreEqual(5, actual);
        }

        [Test]
        public void ShouldThrowExceptionWhenNoBishopAttacksSquare()
        {
            BoardInfo bi = new GameInfo("rnb1kbnr/1p1ppppp/p7/1qp5/3PP3/P1P5/1P3PPP/RNBQKBNR w KQkq - 1 5").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 5, 0, Piece.Bishop, Color.White, "Ba6");
            Assert.Throws(typeof(MoveException), () => { bi.FindBishopMoveSourceIndex(md); });
        }

        [Test]
        public void ShouldThrowExceptionWhenTwoBishopsAttackSquare()
        {
            BoardInfo bi = new GameInfo("rnb1kbnr/1p1ppppp/p7/1q6/2pPP3/PBP5/1P3PPP/RN1QKBNR w KQkq - 1 5").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 3, 2, Piece.Bishop, Color.White, "Bc4");
            Assert.Throws(typeof(MoveException), () => { bi.FindBishopMoveSourceIndex(md); });
        }

        [Test]
        public void ShouldFindRookMoveSource()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/8/R1qbK3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 7, 0, Piece.Rook, Color.White, "Ra8+");
            Assert.AreEqual(0, bi.FindRookMoveSourceIndex(md));
        }

        [Test]
        public void ShouldThrowExceptionWhenNoRookAttacksSquare()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/8/R1qbK3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 0, 3, Piece.Rook, Color.White, "Rxd1");
            Assert.Throws(typeof(MoveException), () => { bi.FindRookMoveSourceIndex(md); });
        }

        [Test]
        public void ShouldThrowExceptionWhenTwoRooksAttackSquare()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/2R5/R1qbK3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 0, 2, Piece.Rook, Color.White, "Rxd1");
            Assert.Throws(typeof(MoveException), () => { bi.FindRookMoveSourceIndex(md); });
        }

        [Test]
        public void ShouldFindQueenMoveSource()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/8/Q1qbK3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 7, 0, Piece.Queen, Color.White, "Qa8+");
            Assert.AreEqual(0, bi.FindQueenMoveSourceIndex(md));
        }

        [Test]
        public void ShouldThrowExceptionWhenNoQueenAttacksSquare()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/8/Q1qbK3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 0, 3, Piece.Queen, Color.White, "Qxd1");
            Assert.Throws(typeof(MoveException), () => { bi.FindQueenMoveSourceIndex(md); });
        }

        [Test]
        public void ShouldThrowExceptionWhenTwoQueensAttackSquare()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/2Q5/Q1qbK3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 0, 2, Piece.Queen, Color.White, "Qxc1");
            Assert.Throws(typeof(MoveException), () => { bi.FindQueenMoveSourceIndex(md); });
        }


        [Test]
        public void ShouldFindKingMoveSource()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/8/4K3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 0, 3, Piece.King, Color.White, "Kd1");
            Assert.AreEqual(4, bi.FindKingMoveSourceIndex(md));
        }

        [Test]
        public void ShouldThrowExceptionWhenNoKingAttacksSquare()
        {
            BoardInfo bi = new GameInfo("4k3/8/8/8/8/8/8/Q1qbK3 w - - 0 1").BoardInfo;
            MoveDetail md = new MoveDetail(null, null, 0, 2, Piece.King, Color.White, "Kc1");
            Assert.Throws(typeof(MoveException), () => { bi.FindKingMoveSourceIndex(md); });
        }
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
            var rv = BoardInfo.BoardInfoFromFen(initialBoard);
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
            var rv = BoardInfo.BoardInfoFromFen(after1e4);

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
            var rv = BoardInfo.BoardInfoFromFen(after1e4c5);

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
        public void GetPinnedPieces_ShouldReturnValueOfPinnedPiece_WhenPieceIsPinned()
        {
            var bi = BoardInfo.BoardInfoFromFen("rnbqkbnr/pp1ppppp/8/1Bp5/4P3/8/PPPP1PPP/RNBQK1NR b KQkq - 1 2");
            var expectedPinnedPiece = 0x8000000000000ul; //the pawn on d7 is pinned
            var actual = bi.GetPinnedPieces();
            Assert.AreEqual(expectedPinnedPiece, actual, "Method did not determine that the pawn on d7 was pinned by the Bishop.");
        }
    }
}

