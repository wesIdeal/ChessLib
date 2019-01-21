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
            var e4FromString = Square.FromString("e4");
            var c4FromString = Square.FromString("c4");
            Assert.AreEqual(e4, e4FromString);
            Assert.AreEqual(c4, c4FromString);
        }

        [Test]
        public void StringToSquareBadArgument()
        {
            Assert.Throws<ArgumentException>(() => Square.FromString(""));
            Assert.Throws<ArgumentException>(() => Square.FromString("e44"));
            Assert.Throws<ArgumentException>(() => Square.FromString("33"));
            Assert.Throws<ArgumentException>(() => Square.FromString("i3"));
        }

    }
}
