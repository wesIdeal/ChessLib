using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.Parse.Tests.PGNPieces
{
    [TestFixture]
    public class TagsTests : ChessLib.Parse.PGNPieces.Tags
    {

        //#region Result Tests
        //[Test]
        //public void TestResultGetter_WhiteWins()
        //{
            //var whiteWins = new string[] { "1-0", " 1-0", " 1-0 ", " 1 - 0", "    1-    0" };
            //foreach (var ww in whiteWins)
            //{
            //    ResultValue = ww;
            //    Assert.AreEqual(Result.WhiteWins, ResultObj);
            //}
        //}

        //[Test]
        //public void TestResultGetter_BlackWins()
        //{
        //    var blackWins = new string[] { "0-1", " 0-1", " 0-1 ", " 0 - 1", "    0-    1" };
        //    foreach (var bw in blackWins)
        //    {
        //        ResultValue = bw;
        //        Assert.AreEqual(Result.BlackWins, ResultObj);
        //    }
        //}

        //[Test]
        //public void TestResultGetter_Draw()
        //{
        //    var draws = new string[] { "1/2-1/2", " 1/2-1/2", " 1/2-1/2 ", " 1/2  - 1/2", "   1/2-   1/2" };
        //    foreach (var bw in draws)
        //    {
        //        ResultValue = bw;
        //        Assert.AreEqual(Result.Draw, ResultObj);
        //        Assert.AreEqual("1/2-1/2", ResultValue);
        //    }
        //}

        //[Test]
        //public void TestResultGetter_InProgress()
        //{
        //    var draws = new string[] { " ", "", "a", "\t" };
        //    foreach (var bw in draws)
        //    {
        //        ResultValue = bw;
        //        Assert.AreEqual(Result.InProgress, ResultObj);
        //        Assert.AreEqual("*", ResultValue);
        //    }
        //}
        //#endregion

        //#region Date Tests
        //[Test]
        //public void TestProperlyFormattedDate()
        //{
        //    DateValue = "1978.11.08";
        //    Assert.AreEqual(DateOfGame, new DateTime(1978, 11, 8));
        //    Assert.AreEqual("1978.11.08", DateValue);
        //}

        //[Test]
        //public void TestGoodDateBadFormat()
        //{
        //    DateValue = "11/8/78";
        //    Assert.AreEqual(DateOfGame, new DateTime(1978, 11, 8));
        //    Assert.AreEqual("1978.11.08", DateValue);
        //} 
        //#endregion
    }
}
