using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using NUnit.Framework;
using ChessLib.Data.Boards;
using ChessLib.Types.Enums;
using ChessLib.Types.Exceptions;

// ReSharper disable StringLiteralTypo

namespace ChessLib.Data.Tests
{
    [TestFixture]
    public class MoveTranslatorServiceTests
    {

        readonly MoveTranslatorService _moveTranslatorService = new MoveTranslatorService();

        [TestCase("r4rk1/1bqn1pbp/pp1p1np1/2pPp3/P3PB2/2N4P/1PPNBPP1/R2Q1RK1 w - e6 0 13", "dxe6", (ushort)35, (ushort)44)]
        [TestCase("rnbqkbnr/ppp1pppp/8/8/3pP3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", "dxe3", (ushort)27, (ushort)20)]
        [TestCase("rnbqkbnr/ppp1pppp/8/8/3pP3/8/PPPP1KPP/RNBQ1BNR b kq e3 0 1", "dxe3+", (ushort)27, (ushort)20)]
        public void SANTranslator_PawnMove_Capture_EnPassant(string fen, string move, ushort src, ushort dst)
        {
            _moveTranslatorService.InitializeBoard(fen);
            var expectedMove = MoveHelpers.GenerateMove(src, dst, MoveType.EnPassant);
            var actual = _moveTranslatorService.GetMoveFromSAN(move);
            AssertMovesAreEqual(expectedMove, actual);
        }

        private void AssertMovesAreEqual(MoveExt expectedMove, MoveExt actual)
        {
            Assert.AreEqual(expectedMove.SourceIndex, actual.SourceIndex, $"Expected source to be {expectedMove.SourceIndex.IndexToSquareDisplay()}, but was {actual.SourceIndex.IndexToSquareDisplay()}.");
            Assert.AreEqual(expectedMove.DestinationIndex, actual.DestinationIndex, $"Expected destination to be {expectedMove.DestinationIndex.IndexToSquareDisplay()}, but was {actual.DestinationIndex.IndexToSquareDisplay()}.");
            Assert.AreEqual(expectedMove.MoveType, actual.MoveType, $"Expected move type to be {expectedMove.MoveType}, but was {actual.MoveType}.");
            if (expectedMove.MoveType == MoveType.Promotion)
            {
                Assert.AreEqual(expectedMove.PromotionPiece, actual.PromotionPiece,
                    $"Expected promotion piece to be {expectedMove.PromotionPiece}, but was {actual.PromotionPiece}.");
            }
            else
            {
                Assert.AreEqual(PromotionPiece.Knight, actual.PromotionPiece, $"Expected no promotion piece (Knight == 0 on non-promotion moves), but was {actual.PromotionPiece}.");
            }

        }

        [TestCase("4r2k/5P2/8/8/8/8/8/6K1 w - - 0 1", "fxe8=Q+", (ushort)53, (ushort)60, MoveType.Promotion, PromotionPiece.Queen)]
        [TestCase("4r2k/3P4/8/8/8/8/8/6K1 w - - 0 1", "dxe8=N", (ushort)51, (ushort)60, MoveType.Promotion, PromotionPiece.Knight)]
        [TestCase("6k1/8/8/8/8/8/3p4/2R3K1 b - - 0 1", "dxc1=B", (ushort)11, (ushort)2, MoveType.Promotion, PromotionPiece.Bishop)]
        [TestCase("6k1/8/8/8/8/8/3p4/2R3K1 b - - 0 1", "bxc1=R+", (ushort)9, (ushort)2, MoveType.Promotion, PromotionPiece.Rook)]
        public void SANTranslator_PawnMove_Capture_Promotion(string fen, string move, ushort src, ushort dst, MoveType mt, PromotionPiece pp)
        {
            _moveTranslatorService.InitializeBoard(fen);
            var expectedMove = MoveHelpers.GenerateMove(src, dst, mt, pp);
            var actual = _moveTranslatorService.GetMoveFromSAN(move);
            AssertMovesAreEqual(expectedMove, actual);
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
            AssertMovesAreEqual(expectedMove, actual);
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
            AssertMovesAreEqual(expectedMove, actual);
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
            AssertMovesAreEqual(expectedMove, actualMove);
        }
        [TestCase("r1bqkbnr/pppp1ppp/2n5/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 2 3", "Nxe5", 21, 36)]
        [TestCase("r1bqkbnr/pppp1ppp/2n5/4N3/4P3/8/PPPP1PPP/RNBQKB1R b KQkq - 0 3", "Nxe5", 42, 36)]
        [TestCase("r1bqkbnr/pppp1ppp/2n5/4p3/2N1P3/5N2/PPPP1PPP/R1BQKB1R w KQkq - 2 3", "Nfxe5", 21, 36)]
        [TestCase("r1bqkbnr/pppp1ppp/2n5/4p3/2N1P3/5N2/PPPP1PPP/R1BQKB1R w KQkq - 2 3", "Ncxe5", 26, 36)]
        [TestCase("r1bqkbnr/pppp1ppp/2n5/1R2p2R/4P3/8/PPPP1PPP/R1BQKB1R w KQkq - 2 3", "Rbxe5", 33, 36)]
        [TestCase("r1bqkbnr/pppp1ppp/2n5/1R2p2R/4P3/8/PPPP1PPP/R1BQKB1R w KQkq - 2 3", "Rhxe5", 39, 36)]
        [TestCase("1k6/8/8/3Q3Q/K7/8/8/7Q w - - 0 1", "Qh5d1", 39, 3)]
        [TestCase("r1b2rk1/pp1p1Qp1/1b1p2B1/n1q3p1/8/5N2/P3RPPP/4R1K1 b - - 0 1", "Rxf7", 61, 53 )]

        public void SANTranslator_PieceTakes(string fen, string move, int src, int dst)
        {
            _moveTranslatorService.InitializeBoard(fen);
            var expectedMove = MoveHelpers.GenerateMove((ushort)src, (ushort)dst);
            var actualMove = _moveTranslatorService.GetMoveFromSAN(move);
            AssertMovesAreEqual(expectedMove, actualMove);
        }

        [TestCase("1b1r2k1/ppqr2pb/2p2p2/4pP2/2N1Q2B/2P5/PPN4P/R4B1K w - - 2 25", "N4e3", (ushort)26, (ushort)20)]
        public void TestAmbiguousMoves(string fen, string move, ushort source, ushort dst)
        {
            _moveTranslatorService.InitializeBoard(fen);
            var actual = _moveTranslatorService.GetMoveFromSAN(move);
            var expected = MoveHelpers.GenerateMove(source, dst);
            Assert.AreEqual(expected, actual);
        }
        [TestCase("r1b2r1k/pp1p2p1/1b1p2B1/n1qQ2p1/8/5N2/P3RPPP/4R1K1 b - - 0 1", "f8f7", "Rf7")]
        public void LANTranslator_PieceMoves(string fen, string lanMove, string expectedSAN)
        {
            _moveTranslatorService.InitializeBoard(fen);
            var actualMove = _moveTranslatorService.FromLongAlgebraicNotation(lanMove);
            Assert.AreEqual(expectedSAN, actualMove.SAN);
        }

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
                var color = new Board(fen).ActivePlayer;
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
            var moveTranslatorService = new MoveTranslatorService(new Board(fen));
            //var md = new MoveDetail((ushort?)sourceIdx, (ushort)destIdx, p, c, moveText).GetMoveFromSAN();
            var actualMove = moveTranslatorService.GetMoveFromSAN(moveText);
            Assert.AreEqual(expected, actualMove.SourceIndex);
        }

        [TestCase("rnb1kbnr/1p1ppppp/p7/1qp5/3PP3/P1P5/1P3PPP/RNBQKBNR w KQkq - 1 5", "Ba6")]
        public static void FindPieceSourceIndex_ShouldThrowException(string fen, string moveText)
        {
            var moveTranslatorService = new MoveTranslatorService(new Board(fen));

            Assert.Throws(typeof(MoveException),
                () => { moveTranslatorService.GetMoveFromSAN(moveText); });
        }
    }
}
