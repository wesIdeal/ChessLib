using System.Linq;
using NUnit.Framework;

namespace ChessLib.Core.Tests.MagicBitboard.Bitwise
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
