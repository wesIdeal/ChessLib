using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;

namespace ChessLib.Data.Tests
{
    [TestFixture]
    public class MoveTranslatorServiceTests
    {
        private static void ValidateHasDestInfo(MoveDetail m, string moveText)
        {
            Assert.IsNotNull(m.DestinationRank, $"Destination rank should be specified for move {moveText}");
            Assert.IsNotNull(m.DestinationFile, $"Destination file should be specified for move {moveText}");
        }
        MoveTranslatorService _moveTranslatorService = new MoveTranslatorService();

        //[Test]
        //public static void ShouldReturnCorrectDetailWhenBlackCastlesShort()
        //{
        //    var move = "O-O";
        //    var mdExpected = new MoveDetail(7, 4, 7, 6, Piece.King, Color.Black, "O-O", false, MoveType.Castle);
        //    var actual = _moveTranslatorService.GetMoveFromSAN(move, Color.Black);
        //    Assert.AreEqual(mdExpected, actual);
        //    ValidateHasDestInfo(actual, move);
        //}
        //[Test]
        //public static void ShouldReturnCorrectDetailWhenWhiteCastlesShort()
        //{
        //    var move = "O-O";
        //    var mdExpected = new MoveDetail(0, 4, 0, 6, Piece.King, Color.White, "O-O", false, MoveType.Castle);
        //    var actual = _moveTranslatorService.GetMoveFromSAN(move, Color.White);
        //    Assert.AreEqual(mdExpected, actual);
        //}
        //[Test]
        //public static void ShouldReturnCorrectDetailWhenBlackCastlesLong()
        //{
        //    var move = "O-O-O";
        //    var mdExpected = new MoveDetail(7, 4, 7, 2, Piece.King, Color.Black, "O-O-O", false, MoveType.Castle);
        //    var actual = _moveTranslatorService.GetMoveFromSAN(move, Color.Black);
        //    Assert.AreEqual(mdExpected, actual);
        //    ValidateHasDestInfo(actual, move);
        //}
        //[Test]
        //public static void ShouldReturnCorrectDetailWhenWhiteCastlesLong()
        //{
        //    var move = "O-O-O";
        //    var mdExpected = new MoveDetail(0, 4, 0, 2, Piece.King, Color.White, "O-O-O", false, MoveType.Castle);
        //    var actual = _moveTranslatorService.GetMoveFromSAN(move, Color.White);
        //    Assert.AreEqual(mdExpected, actual);
        //    ValidateHasDestInfo(actual, move);
        //}

        [TestCase("4r2k/5P2/8/8/8/8/8/6K1 w - - 0 1", "fxe8=Q+", (ushort)53, (ushort)60, MoveType.Promotion, PromotionPiece.Queen)]
        [TestCase("4r2k/3P4/8/8/8/8/8/6K1 w - - 0 1", "dxe8=N", (ushort)51, (ushort)60, MoveType.Promotion, PromotionPiece.Knight)]
        [TestCase("6k1/8/8/8/8/8/3p4/2R3K1 b - - 0 1", "dxc1=B", (ushort)11, (ushort)2, MoveType.Promotion, PromotionPiece.Bishop)]
        [TestCase("6k1/8/8/8/8/8/3p4/2R3K1 b - - 0 1", "bxc1=R+", (ushort)9, (ushort)2, MoveType.Promotion, PromotionPiece.Rook)]
        public void SANTranslator_PawnMove_Capture_Promotion(string fen, string move, ushort src, ushort dst, MoveType mt, PromotionPiece pp)
        {
            _moveTranslatorService.InitializeBoard(fen);
            var expectedMove = MoveHelpers.GenerateMove(src, dst, mt, pp);
            var actual = _moveTranslatorService.GetMoveFromSAN(move);
            Assert.AreEqual(expectedMove, actual);
        }

        [TestCase("rnbqkbnr/p1p1pppp/1p6/3p4/2P1P3/8/PP1P1PPP/RNBQKBNR w KQkq - 0 3", "e8=Q", (ushort)52, (ushort)60, PromotionPiece.Queen)]
        [TestCase("rnbqkbnr/p1p1pppp/1p6/3p4/2P1P3/8/PP1P1PPP/RNBQKBNR w KQkq - 0 3", "e8=R", (ushort)52, (ushort)60, PromotionPiece.Rook)]
        [TestCase("rnbqkbnr/p1p1pppp/8/1p1p4/2P1P3/8/PP1P1PPP/RNBQKBNR b KQkq - 0 3", "d1=N", (ushort)11, (ushort)3, PromotionPiece.Knight)]
        [TestCase("rnbqkbnr/p1p1pppp/8/1p1p4/2P1P3/8/PP1P1PPP/RNBQKBNR b KQkq - 0 3", "d1=B", (ushort)11, (ushort)3, PromotionPiece.Bishop)]
        public void SANTranslator_PawnMove_Capture(string fen, string move, ushort src, ushort dst, PromotionPiece pp)
        {
            _moveTranslatorService.InitializeBoard(fen);
            var expectedMove = MoveHelpers.GenerateMove(src, dst, MoveType.Promotion, pp);
            var actual = _moveTranslatorService.GetMoveFromSAN(move);
            Assert.AreEqual(expectedMove, actual);
        }

        [TestCase("8/4P3/8/k7/7K/8/3p4/8 w - - 0 1", "exd5", (ushort)28, (ushort)35)]
        [TestCase("8/4P3/8/k7/7K/8/3p4/8 w - - 0 1", "cxd5", (ushort)26, (ushort)35)]
        [TestCase("8/4P3/8/k7/7K/8/3p4/8 b - - 0 1", "dxe4", (ushort)35, (ushort)28)]
        [TestCase("8/4P3/8/k7/7K/8/3p4/8 b - - 0 1", "bxc4", (ushort)33, (ushort)26)]
        public void SANTranslator_PawnMove_Promotion(string fen, string move, ushort src, ushort dst)
        {
            _moveTranslatorService.InitializeBoard(fen);
            var expectedMove = MoveHelpers.GenerateMove(src, dst);
            var actual = _moveTranslatorService.GetMoveFromSAN(move);
            Assert.AreEqual(expectedMove, actual);
        }

        [TestCase("r3k2r/8/8/8/8/8/8/R3K2R w - - 0 1", "O-O", (ushort)4, (ushort)6)]
        [TestCase("r3k2r/8/8/8/8/8/8/R3K2R w - - 0 1", "O-O-O", (ushort)4, (ushort)2)]
        [TestCase("r3k2r/8/8/8/8/8/8/R3K2R b - - 0 1", "O-O", (ushort)60, (ushort)62)]
        [TestCase("r3k2r/8/8/8/8/8/8/R3K2R b - - 0 1", "O-O-O", (ushort)60, (ushort)58)]
        public void SANTranslator_Castling(string fen, string move, ushort src, ushort dst)
        {
            _moveTranslatorService.InitializeBoard(fen);
            var expectedMove = MoveHelpers.GenerateMove(src, dst, MoveType.Castle);
            var actualMove = _moveTranslatorService.GetMoveFromSAN(move);
            Assert.AreEqual(expectedMove, actualMove);
        }
        [TestCase("r1bqkbnr/pppp1ppp/2n5/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 2 3", "Nxe5", 21, 36)]
        [TestCase("r1bqkbnr/pppp1ppp/2n5/4N3/4P3/8/PPPP1PPP/RNBQKB1R b KQkq - 0 3", "Nxe5", 42, 36)]
        [TestCase("r1bqkbnr/pppp1ppp/2n5/4p3/2N1P3/5N2/PPPP1PPP/R1BQKB1R w KQkq - 2 3", "Nfxe5", 21, 36)]
        [TestCase("r1bqkbnr/pppp1ppp/2n5/4p3/2N1P3/5N2/PPPP1PPP/R1BQKB1R w KQkq - 2 3", "Ncxe5", 26, 36)]
        [TestCase("r1bqkbnr/pppp1ppp/2n5/1R2p2R/4P3/8/PPPP1PPP/R1BQKB1R w KQkq - 2 3", "Rbxe5",33, 36  )]
        [TestCase("r1bqkbnr/pppp1ppp/2n5/1R2p2R/4P3/8/PPPP1PPP/R1BQKB1R w KQkq - 2 3", "Rhxe5", 39, 36)]
        [TestCase("1k6/8/8/3Q3Q/K7/8/8/7Q w - - 0 1", "Qh5d1", 39, 3)]
        public void SANTranslator_PieceTakes(string fen, string move, int src, int dst)
        {
            _moveTranslatorService.InitializeBoard(fen);
            var expectedMove = MoveHelpers.GenerateMove((ushort) src, (ushort) dst);
            var actualMove = _moveTranslatorService.GetMoveFromSAN(move);
            Assert.AreEqual(expectedMove,actualMove);
        }

        public void SANTranslator_PieceMoves(string fen, int src, int dst)
        {

        }
        //[Test]
        //public static void ShouldReturnCorrectPiece_Pawn()
        //{
        //    var moveFormat = new[] { "{0}xe4", "{0}4" };
        //    foreach (var fmt in moveFormat)
        //    {
        //        for (char i = 'a'; i <= 'h'; i++)
        //        {
        //            var move = string.Format(fmt, i);
        //            var actual = _moveTranslatorService.GetMoveFromSAN(move, Color.White);
        //            Assert.AreEqual(Piece.Pawn, actual.Piece);
        //            ValidateHasDestInfo(actual, move);
        //            if (fmt.Contains("x"))
        //            {
        //                Assert.IsTrue(actual.IsCapture, $"Capture flag should be set on pawn capture for move {move}");
        //                Assert.IsNotNull(actual.SourceFile, $"Source file should be set on pawn capture for move {move}");
        //                Assert.IsNotNull(actual.SourceRank, $"Source rank should be set on pawn capture for move {move}");
        //            }
        //        }
        //    }
        //}

        [Test]
        public void GetMoveFromSAN_ShouldThrowException_IfMoveIsLessThan2Chars()
        {
            var expectedMessage = "Invalid move. Must have at least 2 characters.";
            Assert.Throws<MoveException>(() =>
            {
                try
                {
                    _moveTranslatorService.GetMoveFromSAN("e");
                }
                catch (MoveException e)
                {
                    Assert.AreEqual(expectedMessage, e.Message);
                    throw;
                }
            });


        }

       

  
        [TestCase("rnbqkbnr/pppppppp/8/8/8/PPPPPPPP/8/RNBQKBNR w KQkq - 0 1", (ushort)16, (ushort)23, (ushort)1, "All white pawns on the third rank, moving to the 4th.")]
        [TestCase(FENHelpers.FENInitial, (ushort)8, (ushort)15, (ushort)1, "All white pawns in the starting position, moving up one square.")]
        [TestCase(FENHelpers.FENInitial, (ushort)8, (ushort)15, (ushort)2, "All white pawns in the starting position, moving up two squares.")]
        [TestCase("rnbqkbnr/8/pppppppp/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", (ushort)40, (ushort)47, (ushort)1, "All black pawns are starting on 6th rank, moving up one square.")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", (ushort)48, (ushort)55, (ushort)1, "All black pawns are in starting position, moving up one square.")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", (ushort)48, (ushort)55, (ushort)2, "All black pawns are in starting position, moving up two squares.")]
        public void SANTranslator_PawnMove_StartingPosition(string fen, ushort dstLower, ushort dstUpper, ushort moveSquares, string desc)
        {
            _moveTranslatorService.InitializeBoard(fen);
            for (ushort index = dstLower; index <= dstUpper; index++)
            {
                var color = new BoardInfo(fen).ActivePlayer;
                var destination = index + (color == Color.Black ? -1 : 1) * (moveSquares * 8);
                var expectedMove = MoveHelpers.GenerateMove(index, (ushort)destination);
                var moveText = expectedMove.DestinationIndex.IndexToSquareDisplay();
                var actualMove = _moveTranslatorService.GetMoveFromSAN(moveText);
                Assert.AreEqual(expectedMove, actualMove, desc + $" - expected {expectedMove} but was {actualMove}.");
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
            var moveTranslatorService = new MoveTranslatorService(new BoardInfo(fen) { ActivePlayer = c });
            //var md = new MoveDetail((ushort?)sourceIdx, (ushort)destIdx, p, c, moveText).GetMoveFromSAN();
            var actualMove = moveTranslatorService.GetMoveFromSAN(moveText);
            Assert.AreEqual(expected, actualMove.SourceIndex);
        }

        [TestCase("rnb1kbnr/1p1ppppp/p7/1qp5/3PP3/P1P5/1P3PPP/RNBQKBNR w KQkq - 1 5", "Ba6")]
        public static void FindPieceSourceIndex_ShouldThrowException(string fen, string moveText)
        {
            var moveTranslatorService = new MoveTranslatorService(new BoardInfo(fen));

            Assert.Throws(typeof(MoveException),
                () => { moveTranslatorService.GetMoveFromSAN(moveText); });
        }
    }
}
