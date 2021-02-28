using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.MagicBitboard.Tests.MovingPieces
{
    [TestFixture]
    public class Bishop
    {
        private readonly MagicBitboard.MovingPieces.Bishop _b = new MagicBitboard.MovingPieces.Bishop();

        [TestCase((ushort)0, (ulong)0x8040201008040200)]
        [TestCase((ushort)63, (ulong)0x40201008040201)]
        [TestCase((ushort)27, (ulong)0x8041221400142241)]
        [TestCase((ushort)28, (ulong)0x182442800284482)]
        public void BishopMovesShouldBeInitialized(ushort squareIndex, ulong expectedBoardValue)
        {
            Assert.AreEqual(expectedBoardValue, _b.GetMovesFromSquare(squareIndex));
        }
    }
}
