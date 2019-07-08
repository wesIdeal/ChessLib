using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Validators.Tests.BoardValidation
{
    [TestFixture]
    public sealed class CastlingAvailabilityRule
    {
        [TestCase(FENHelpers.FENInitial, BoardException.None)]
        [TestCase("1r2k3/8/8/8/8/8/8/1R2K3 w Q - 0 1", BoardException.WhiteCastleLong, "White cannot castle long if Rook isn't on A1.")]
        [TestCase("1r2k3/8/8/8/8/8/8/1R2K3 w q - 0 1", BoardException.BlackCastleLong, "Black cannot castle long if Rook isn't on A8.")]
        [TestCase("4k1r1/8/8/8/8/8/8/4K1R1 w K - 0 1", BoardException.WhiteCastleShort, "White cannot castle short if Rook isn't on H1.")]
        [TestCase("4k1r1/8/8/8/8/8/8/4K1R1 w k - 0 1", BoardException.BlackCastleShort, "Black cannot castle short if Rook isn't on H8.")]
        [TestCase("r4k1r/8/8/8/8/8/8/R4K1R w K - 0 1", BoardException.WhiteCastleMisplacedKing, "White cannot castle if King isn't on e1")]
        [TestCase("r4k1r/8/8/8/8/8/8/R4K1R w k - 0 1", BoardException.BlackCastleMisplacedKing, "Black cannot castle if King isn't on e8")]
        [TestCase("r4k1r/8/8/8/8/8/8/R4K1R w Q - 0 1", BoardException.WhiteCastleMisplacedKing, "White cannot castle if King isn't on e1")]
        [TestCase("r4k1r/8/8/8/8/8/8/R4K1R w q - 0 1", BoardException.BlackCastleMisplacedKing, "Black cannot castle if King isn't on e8")]

        public static void TestCastling(string fen, BoardException expectedException, string message = "")
        {
            var board = new BoardInfo(fen);
            var rule = new Data.Validators.BoardValidation.Rules.CastlingAvailabilityRule();
            var actual = rule.Validate(board);
            Assert.AreEqual(expectedException, actual, message);
        }
    }
}
