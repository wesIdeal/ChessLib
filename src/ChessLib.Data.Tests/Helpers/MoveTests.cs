using ChessLib.Data.Helpers;
using ChessLib.Data.Types;
using NUnit.Framework;
using System;

namespace ChessLib.Data.Tests.Helpers
{
    [TestFixture]
    public static class MoveTests
    {
        [SetUp]
        public static void Setup()
        {

        }

        [Test]
        public static void ShouldGetPromotionPieceFromMove()
        {
            foreach (PromotionPiece pp in Enum.GetValues(typeof(PromotionPiece)))
            {
                var move = MoveHelpers.GenerateMove(45, 53, MoveType.Promotion, pp);
                Assert.AreEqual(pp, move.PromotionPiece);
            }
        }

        [Test]
        public static void ShouldGetMoveTypeFromMove()
        {
            foreach (MoveType mt in Enum.GetValues(typeof(MoveType)))
            {
                var move = MoveHelpers.GenerateMove(45, 53, mt);
                Assert.AreEqual(mt, move.MoveType);
            }
        }
    }
}
