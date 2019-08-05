using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using NUnit.Framework;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Boards;

namespace ChessLib.Data.Tests
{
    [TestFixture]
    public class MoveDisplayServiceTests
    {
        readonly BoardInfo bi = new BoardInfo();
        private MoveDisplayService _moveDisplayService;

        [SetUp]
        public void Setup()
        {
            _moveDisplayService = new MoveDisplayService(bi);
        }
        [Test]
        public void GetSANSourceString_King()
        {
            var poc = new PieceOfColor() { Color = Color.Black, Piece = Piece.King };
            var expected = "K";
            Assert.AreEqual(expected, _moveDisplayService.GetSANSourceString(new MoveExt(0), poc.Piece));
        }

        [Test]
        public void GetSANSourceString_Pawn()
        {
            var poc = new PieceOfColor() { Color = Color.White, Piece = Piece.Pawn };
            var expected = "";
            Assert.AreEqual(expected, _moveDisplayService.GetSANSourceString(MoveHelpers.GenerateMove(12, 28), poc.Piece));
        }

        [TestCase("5r2/6Pk/1R6/7P/6K1/8/8/8 w - - 0 62", 54, 61, "gxf8=Q 1/2-1/2", PromotionPiece.Queen, MoveType.Promotion)]
        [TestCase("6K1/4k1P1/8/7q/8/8/8/8 b - - 9 56", 52, 60, "Ke8 1/2-1/2")]
        [TestCase("6K1/4k1P1/8/6q1/8/8/8/8 b - - 9 56", 38, 39, "Qh5 1/2-1/2")]
        public void MoveToSAN_Stalemate(string fen, int f, int t, string expected, PromotionPiece p = PromotionPiece.Knight, MoveType type = MoveType.Normal)
        {
            var board = new BoardInfo(fen);
            _moveDisplayService = new MoveDisplayService(board);
            var move = MoveHelpers.GenerateMove((ushort)f, (ushort)t, type, p);
            Assert.AreEqual(expected, _moveDisplayService.MoveToSAN(move));
        }

        [TestCase("rnbqkbnr/1pp1pppp/p7/3p4/2P1P3/8/PP1P1PPP/RNBQKBNR w KQkq - 0 1", 28, 35, "exd5")]
        [TestCase("rnbqkbnr/1pp1pppp/p7/3p4/2P1P3/8/PP1P1PPP/RNBQKBNR w KQkq - 0 1", 26, 35, "cxd5")]
        [TestCase("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1", 28, 35, "exd5")]
        [TestCase(FENHelpers.FENInitial, 12, 28, "e4")]
        [TestCase(FENHelpers.FENInitial, 12, 20, "e3")]
        [TestCase(FENHelpers.FENInitial, 10, 26, "c4")]
        [TestCase("rnbqkbnr/1pp1pppp/p7/2Pp4/8/8/PP1PPPPP/RNBQKBNR w KQkq d6 0 3", 34, 43, "cxd6", MoveType.EnPassant)]
        [TestCase("rnbqkbnr/1pp1pppp/p7/2Pp4/2P5/8/P2PPPPP/RNBQKBNR w KQkq d6 0 3", 26, 35, "cxd5")]
        public void MoveToSAN_Pawn(string fen, int from, int to, string expected, MoveType mt = MoveType.Normal)
        {
            var board = new BoardInfo(fen);
            _moveDisplayService = new MoveDisplayService(board);
            var move = MoveFromInt(from, to, PromotionPiece.Knight, mt);
            Assert.AreEqual(expected, _moveDisplayService.MoveToSAN(move));
        }

        [TestCase("2bq1rk1/3p1npp/p1p3N1/1rbB1Pp1/1pQ5/P5N1/1PP3PP/R3R2K w - - 0 23", "d5f7", "Bxf7+")]
        public void TestCheckDisplay(string fen, string lan, string expected)
        {
            BoardInfo bi = new BoardInfo(fen);
            MoveTranslatorService mts = new MoveTranslatorService(bi);
            MoveDisplayService mds = new MoveDisplayService(bi);
            var actual = mds.MoveToSAN(mts.FromLongAlgebraicNotation(lan));
            Assert.AreEqual(expected, actual);
        }


        [Test]
        [TestCase("7k/8/8/3bR3/8/8/7K/8 w - - 0 1", 36, 35, "Rxd5")]
        [TestCase("7k/8/8/4R3/8/8/7K/8 w - - 0 1", 36, 35, "Rd5")]
        [TestCase("7k/8/3R4/3b4/8/3R4/7K/8 w - - 0 1", 43, 35, "R6xd5")]
        [TestCase("7k/8/3R4/8/8/3R4/7K/8 w - - 0 1", 43, 35, "R6d5")]
        [TestCase("2k5/8/8/1b6/4Q2Q/8/7K/7Q w - - 0 1", 31, 4, "Qh4e1")]
        [TestCase("7k/8/8/3bR3/8/3R4/7K/8 w - - 0 1", 36, 35, "Rexd5")]
        [TestCase("7k/8/8/4R3/8/3R4/7K/8 w - - 0 1", 36, 35, "Red5")]
        public void MoveToSAN_Piece(string fen, int from, int to, string expected)
        {
            var board = new BoardInfo(fen);
            _moveDisplayService = new MoveDisplayService(board);
            var move = MoveFromInt(from, to);
            Assert.AreEqual(expected, _moveDisplayService.MoveToSAN(move));

        }

        [TestCase("8/4P3/8/8/8/8/6k1/4K3 w - - 0 1", 52, 60, "e8=Q")]
        [TestCase("3q4/4P3/8/8/8/8/6k1/4K3 w - - 0 1", 52, 59, "exd8=Q")]
        public void ManufacturessAppropriatePawnPromotion(string fen, int from, int to, string expected)
        {
            var board = new BoardInfo(fen);
            _moveDisplayService = new MoveDisplayService(board);
            var poc = new PieceOfColor() { Color = Color.White, Piece = Piece.Pawn };
            var move = MoveFromInt(from, to, PromotionPiece.Queen, MoveType.Promotion);
            Assert.AreEqual(expected, _moveDisplayService.MoveToSAN(move));

        }

        [TestCase("8/8/8/8/5Q2/8/6k1/4K3 w - - 0 1", 29, 28, "Qe4+")]
        [TestCase("rnbqkbnr/ppp2ppp/3p4/4p3/2B1P3/8/PPPP1PPP/RNBQK1NR w KQkq - 0 3", 26, 53, "Bxf7+")]
        [TestCase("rnbqkbnr/ppp2ppp/3p4/4N2Q/2B1P3/8/PPPP1PPP/RNB1K2R w KQkq - 0 3", 39, 53, "Qxf7# 1-0")]
        public void DisplaysAppropriateCheckSymbol(string fen, int from, int to, string expected)
        {
            var boardInfo = new BoardInfo(fen);
            _moveDisplayService = new MoveDisplayService(boardInfo);
            var move = MoveHelpers.GenerateMove((ushort)from, (ushort)to);
            var actual = _moveDisplayService.MoveToSAN(move);
            Assert.AreEqual(expected, actual);
        }

        public static MoveExt MoveFromInt(int f, int t, PromotionPiece p = PromotionPiece.Knight, MoveType mt = MoveType.Normal) =>
             MoveHelpers.GenerateMove((ushort)f, (ushort)t, mt, p);
    }
}
