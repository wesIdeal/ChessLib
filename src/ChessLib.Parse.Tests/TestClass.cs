// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace ChessLib.Parse.Tests
{
    [TestFixture]
    public class TestClass : ParsePGN
    {
        string commentedPgn;
        string unCommentedPgn;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            commentedPgn = File.ReadAllText(".\\PGN\\commented.pgn");
            unCommentedPgn = File.ReadAllText(".\\PGN\\uncommented.pgn");
        }

        [Test]
        public void RemoveCommentsShouldRemoveAllComments()
        {
            var s = RemoveTags(commentedPgn, out Dictionary<string, string> d);
            Assert.AreEqual(unCommentedPgn, RemoveComments(commentedPgn));
        }
    }
}
