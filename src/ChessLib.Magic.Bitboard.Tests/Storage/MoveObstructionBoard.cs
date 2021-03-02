using ChessLib.MagicBitboard.Bitwise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ChessLib.MagicBitboard.Tests.Storage
{


    [TestFixture]
    public class MoveObstructionBoard
    {
        private ChessLib.MagicBitboard.Storage.MoveObstructionBoard mob;

        [Test]
        public void TestSetMoveBoard()
        {
            ulong blocker = 0x402000;
            ulong moves = 0x200400002000;
            mob = new ChessLib.MagicBitboard.Storage.MoveObstructionBoard(blocker, moves);
            Assert.AreEqual(moves, mob.MoveBoard);
        }

        [Test]
        public void TestSetBlockerBoard()
        {
            ulong blocker = 0x402000;
            ulong moves = 0x200400002000;
            mob = new ChessLib.MagicBitboard.Storage.MoveObstructionBoard(blocker, moves);
            Assert.AreEqual(blocker, mob.Occupancy);
        }
    }
}