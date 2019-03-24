using ChessLib.Data.Helpers;
using ChessLib.Data.Types;
using NUnit.Framework;
using System;

namespace MagicBitboard.Helpers.Tests
{
    [TestFixture]
    public class MoveTests
    {
        const string fenEP = "8/PPPPPPPP/8/2k5/8/2K5/pppppppp/8 w - - 0 1";
        BoardInfo biEnPassent = BoardInfo.BoardInfoFromFen(fenEP);
        [SetUp]
        public void Setup()
        {

        }


        [Test]
        public void ShouldGetPromotionPieceFromMove()
        {
            foreach (PromotionPiece pp in Enum.GetValues(typeof(PromotionPiece)))
            {
                var move = MoveHelpers.GenerateMove(45, 53, MoveType.Promotion, pp);
                Assert.AreEqual(pp, move.PromotionPiece);
            }
        }

        [Test]
        public void ShouldGetMoveTypeFromMove()
        {
            foreach (MoveType mt in Enum.GetValues(typeof(MoveType)))
            {
                var move = MoveHelpers.GenerateMove(45, 53, mt, PromotionPiece.Knight);
                Assert.AreEqual(mt, move.MoveType);
            }
        }
    }
}
