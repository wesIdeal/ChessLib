using NUnit.Framework;
namespace ChessLib.Data.Helpers.Tests
{
    [TestFixture]
    public static class DisplayHelpersTests
    {
        [Test]
        public static void GetDisplayBits_PlacesMSBAtH8()
        {
            ulong testVal = 0x8000000000000000;
            var expected =
                "0 0 0 0 0 0 0 1\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n";
            Assert.AreEqual(expected, DisplayHelpers.GetDisplayBits(testVal), "GetDisplayBits() didn't locate H8 as MSB.");
        }

        [Test]
        public static void GetDisplayBits_PlacesLSBAtA1()
        {
            ulong testVal = 0x0000000000000001;
            var expected =
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "0 0 0 0 0 0 0 0\r\n" +
                "1 0 0 0 0 0 0 0\r\n";
            Assert.AreEqual(expected, DisplayHelpers.GetDisplayBits(testVal), "GetDisplayBits() didn't locate A1 as LSB.");
        }



    }
}
