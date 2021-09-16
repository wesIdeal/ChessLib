using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types;
using Moq;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Types
{
    [TestFixture(TestOf = typeof(Tags))]
    public class TagsTests
    {
        private const string name = nameof(Tags) + ": ";
        [TestCaseSource(nameof(GetEqualsTestCases))]
        public bool TestEquals(Tags t1, Tags t2)
        {
            return t1.Equals(t2);
        }

        protected static IEnumerable<TestCaseData> GetEqualsTestCases()
        {
            const string testName = name + "Equals: ";
            var t1 = new Tags();
            var t2 = (Tags)null;
            yield return new TestCaseData(t1, t2).SetName(testName + "(t1, null)").Returns(false);

            t2 = new Tags("1q3rbk/Q5pp/8/3P4/1r3n2/R3N3/2B2PPP/5RK1 b - - 0 1");
            yield return new TestCaseData(t1, t2).SetName(testName + "(t1, [custom fen])").Returns(false);

            t1["Extra"] = "Extra value";
            yield return new TestCaseData(t1, t2).SetName(testName + "Counts are same, different tags").Returns(false);

            t1 = new Tags();
            t2 = new Tags();
            t1.Black = "Frank Marshall";
            t2.Black = "Paul Morphy";
            yield return new TestCaseData(t1, t2).SetName(testName + "Same tags, different values").Returns(false);

            t1 = new Tags();
            t2 = new Tags();
            t1.Black = "Frank Marshall";
            t2.Black = "Frank Marshall";
            yield return new TestCaseData(t1, t2).SetName(testName + "Equal Tags").Returns(true);
        }

        [Test]
        public void AddFen_ShouldCallSetFen()
        {
            var fen = BoardConstants.FenStartingPosition;
            var tagMock = new Mock<Tags>() { CallBase = true };
            tagMock.Setup(x => x.SetFen(fen))
                .Verifiable();
            tagMock.Object.Add("FEN", fen);
            tagMock.Verify(x => x.SetFen(fen), Times.Once);
        }

        protected static IEnumerable<TestCaseData> GetTestCasesForGet()
        {
            var player = "Garry Kasparov";
            yield return new TestCaseData(new KeyValuePair<string, string>("Black", player))
                .SetName("Tags: Get() should return value that was added")
                .Returns(player);

            yield return new TestCaseData(new KeyValuePair<string, string>("Black", string.Empty))
                .SetName("Tags: Get() should return '' for key that was added with empty value")
                .Returns(string.Empty);
        }

        [TestCase("BlackElo")]
        public void Get_ShouldReturnSpecialChar_WhenKeyNotFound(string key)
        {
            var tags = new Tags();
            var expected = "?";
            Assert.AreEqual(expected, tags.Get(key));
        }

        protected static IEnumerable<TestCaseData> GetRequiredTagsTestCases()
        {
            const string name = "Required Tags: ";
            yield return new TestCaseData("Event").SetName(name + "Event").Returns("?");
            yield return new TestCaseData("Site").SetName(name + "Site").Returns("?");
            yield return new TestCaseData("Date").SetName(name + "Date").Returns("????.??.??");
            yield return new TestCaseData("Round").SetName(name + "Round").Returns("?");
            yield return new TestCaseData("White").SetName(name + "White").Returns("?");
            yield return new TestCaseData("Black").SetName(name + "Black").Returns("?");
            yield return new TestCaseData("Result").SetName(name + "Result").Returns("*");

        }

        [TestCaseSource(nameof(GetRequiredTagsTestCases))]
        public string TestRequiredTags(string tag)
        {
            var tags = new Tags();
            return tags.RequiredTags.First(x => x.Key == tag).Value;
        }

        [Test]
        public void RequiredTags_ShouldReturnFen_IfBoardHasSetup()
        {
            var tags = new Tags();
            var fen = "3r2k1/p1rbnpbp/1p2p1p1/1N6/1P1N4/4PB2/P4PPP/2RR2K1 b - - 1 1";
            tags.SetFen(fen);
            Assert.AreEqual(fen, tags.RequiredTags.First(x => x.Key == "FEN").Value);
            Assert.AreEqual("1", tags.RequiredTags.First(x=>x.Key == "SetUp").Value);
        }
        [Test]
        public void SupplementalTags_ShouldNotReturnARequiredTag()
        {
            var tags = new Tags();
            Assert.IsEmpty(tags.SupplementalTags);
        }

        [Test]
        public void SupplementalTags_ShouldReturnANonRequiredTag()
        {
            var tags = new Tags();
            var nonStandardTag = "BlackElo";
            tags[nonStandardTag] = "2600";
            Assert.AreEqual("2600", tags.SupplementalTags.First(x=>x.Key == nonStandardTag).Value);
        }

        [TestCaseSource(nameof(GetTestCasesForGet))]
        public string TestGet(KeyValuePair<string, string> tagPair)
        {
            var tags = new Tags();
            tags.Add(tagPair.Key, tagPair.Value);
            return tags.Get(tagPair.Key);
        }
        [TestCaseSource(nameof(GetAddTestCases))]
        public static string TestAdd(string val1, string val2)
        {
            var tags = new Tags
            {
                ["White"] = val1
            };
            tags.Add("White", val2);
            return tags["White"];
        }
        protected static IEnumerable<TestCaseData> GetAddTestCases()
        {
            const string testName = name + "Add: ";
            var t1 = new Tags();

            t1.Black = "Frank Marshall";

            yield return new TestCaseData("Frank Marshall", "Paul Morphy").SetName(testName + "Adding value already added").Returns("Paul Morphy");

        }
    }
}