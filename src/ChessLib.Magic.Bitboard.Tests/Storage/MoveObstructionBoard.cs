using NUnit.Framework;

namespace ChessLib.MagicBitboard.Tests.Storage
{


    [TestFixture]
    public class MoveObstructionBoard
    {
        private ChessLib.MagicBitboard.Storage.MoveObstructionBoard _mob;

        [Test]
        public void TestSetMoveBoard()
        {
            ulong occupancy = 0x402000;
            ulong moves = 0x200400002000;
            _mob = new ChessLib.MagicBitboard.Storage.MoveObstructionBoard(occupancy, moves);
            Assert.AreEqual(moves, _mob.MoveBoard);
        }

        [Test]
        public void TestSetObstructionBoard()
        {
            ulong occupancy = 0x402000;
            ulong moves = 0x200400002000;
            _mob = new ChessLib.MagicBitboard.Storage.MoveObstructionBoard(occupancy, moves);
            Assert.AreEqual(occupancy, _mob.Occupancy);
        }
    }
}