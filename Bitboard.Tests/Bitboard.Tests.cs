using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicBitboard;
namespace MagicBitboard.Tests
{
    [TestFixture]
    public class BitboardTests
    {
        private readonly Bitboard _bb =   new Bitboard();
        [OneTimeSetUp]
        public void OneTimeSetup()
        {

        }
        [Test]
        public void TestQueenAttacksAtRandom()
        {
            var occupancy = 0x1000818011030ul;
            var expectedAttackBoard = 0x805031e;
            var actual = _bb.GetAttackedSquares(Piece.Queen, 0, occupancy);
            Assert.AreEqual(expectedAttackBoard, actual);
        }
    }
}
