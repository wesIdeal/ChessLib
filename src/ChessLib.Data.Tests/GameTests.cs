using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN;
using NUnit.Framework;

namespace ChessLib.Data.Tests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void ShouldReturnEqualForSameGame()
        {
            var parser = new PGNParser();
            var g1Str = PGN.g1;
            var g2Str = PGN.g1;
            var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
            var g2 = parser.GetGamesFromPGNAsync(g2Str).Result.First();
            Assert.IsTrue(g1.IsEqualTo(g2));
            Assert.IsTrue(g1.IsEqualTo(g2,true));
        }

        [Test]
        public void ShouldReturnEqualIgnoringVariations()
        {
            var parser = new PGNParser();
            var g1Str = PGN.g1;
            var g2Str = PGN.g1Variation;
            var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
            var g2 = parser.GetGamesFromPGNAsync(g2Str).Result.First();
            Assert.IsTrue(g1.IsEqualTo(g2));
        }

        [Test]
        public void ShouldReturnNotEqualIncludingVariations()
        {
            var parser = new PGNParser();
            var g1Str = PGN.g1;
            var g2Str = PGN.g1Variation;
            var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
            var g2 = parser.GetGamesFromPGNAsync(g2Str).Result.First();
            Assert.IsFalse(g1.IsEqualTo(g2, true));
        }

        [Test]
        public void ShouldReturnEqualIncludingVariations()
        {
            var parser = new PGNParser();
            var g1Str = PGN.g1Variation;
            var g2Str = PGN.g1Variation;
            var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
            var g2 = parser.GetGamesFromPGNAsync(g2Str).Result.First();
            Assert.IsTrue(g1.IsEqualTo(g2, true));
        }

        [Test]
        public void ShouldSplitWithNoVariations()
        {
            var parser = new PGNParser();
            var g1Str = PGN.s1;
            var g2Str = PGN.s2;
            var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
            var g2 = parser.GetGamesFromPGNAsync(g2Str).Result.First();
            LinkedListNode<MoveStorage> move = g1.MainMoveTree.First;
            while ((move = move.Next).Value.SAN != "e6") ;
            var splitGame = g1.SplitFromMove(move);
            Assert.IsTrue(splitGame.IsEqualTo(g2));
        }
    }
}
