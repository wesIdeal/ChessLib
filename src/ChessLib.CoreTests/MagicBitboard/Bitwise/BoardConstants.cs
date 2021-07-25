using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using NUnit.Framework;

namespace ChessLib.MagicBitboard.Tests.Bitwise
{
    using Constants = Core.MagicBitboard.Bitwise.BoardConstants;
    [TestFixture]
    public class BoardConstants
    {
        [Test]
        public void TestAllSquares([Range(0, 63)] int sq)
        {
            Assert.Contains((ushort)sq, Constants.AllSquares.ToArray());
        }
    }
}
