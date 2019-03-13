using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLib;
namespace ChessLib.Tests
{
    [TestFixture]
    public class SquareTests
    {
        [Test]
        public void StringToSquare()
        {
            var e4 = Square.FromRealRank(File.e, 4);
            var c4 = Square.FromRealRank(File.c, 4);
            var e4FromString = new Square("e4");
            var c4FromString = new Square("c4");
            Assert.AreEqual(e4, e4FromString);
            Assert.AreEqual(c4, c4FromString);
        }

        [Test]
        public void StringToSquareBadArgument()
        {
            Assert.Throws<ArgumentException>(() => new Square(""));
            Assert.Throws<ArgumentException>(() => new Square("e44"));
            Assert.Throws<ArgumentException>(() => new Square("33"));
            Assert.Throws<ArgumentException>(() => new Square("i3"));
        }

    }
}
