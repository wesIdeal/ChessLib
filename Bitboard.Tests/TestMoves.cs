using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
namespace ChessLib.Tests
{
    [TestFixture]
    public class TestMoves
    {
        [Test]
        public void TestMoveW()
        {
            var arr = new[] { 1, 2, 3, 4, 5, 6, 7 };
            List<int?> results = new List<int?>();
            int? initialSquare = 0;
            while ((initialSquare = MoveHelpers.MoveW((int)initialSquare)) != null)
            {
                results.Add(initialSquare);
            }
            Assert.AreEqual(arr, results);
        }

        [Test]
        public void TestMoveWRank8()
        {
            var arr = new[] { 57, 58, 59, 60, 61, 62, 63 };
            List<int?> results = new List<int?>();
            int? initialSquare = 56;
            while ((initialSquare = MoveHelpers.MoveW((int)initialSquare)) != null)
            {
                results.Add(initialSquare);
            }
            Assert.AreEqual(arr, results);
        }

        [Test]
        public void TestMoveE()
        {
            var arr = new[] { 6, 5, 4, 3, 2, 1, 0 };
            List<int?> results = new List<int?>();
            int? initialSquare = 7;
            while ((initialSquare = MoveHelpers.MoveE((int)initialSquare)) != null)
            {
                results.Add(initialSquare);
            }
            Assert.AreEqual(arr.OrderBy(x=>x), results.OrderBy(x=>x));
        }

        [Test]
        public void TestMoveENoMoves()
        {
            var arr = new int[] { };
            List<int?> results = new List<int?>();
            int? initialSquare = 8;
            while ((initialSquare = MoveHelpers.MoveE((int)initialSquare)) != null)
            {
                results.Add(initialSquare);
            }
            Assert.AreEqual(arr, results);
        }

        [Test]
        public void TestMoveERank8()
        {
            var arr = new[] { 62, 61, 60, 59, 58, 57, 56 };
            List<int?> results = new List<int?>();
            int? initialSquare = 63;
            while ((initialSquare = MoveHelpers.MoveE((int)initialSquare)) != null)
            {
                results.Add(initialSquare);
            }
            Assert.AreEqual(arr, results);
        }

    }
}
