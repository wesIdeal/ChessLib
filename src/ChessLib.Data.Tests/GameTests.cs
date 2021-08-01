using System;
using System.Linq;
using ChessLib.Parse.PGN;
using NUnit.Framework;

namespace ChessLib.Data.Tests
{

    [TestFixture]
    public class GameTests
    {
      

        //[Test]
        //public void ShouldReturnNotEqualIncludingVariations()
        //{
        //    var parser = new PGNParser();
        //    var g1Str = PGN.g1;
        //    var g2Str = PGN.g1Variation;
        //    var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
        //    var g2 = parser.GetGamesFromPGNAsync(g2Str).Result.First();
        //    Assert.IsFalse(g1.IsEqualTo(g2, true));
        //}

        //[Test]
        //public void ShouldSplitFromDifferentSetup()
        //{
        //    var parser = new PGNParser();
        //    var g1Str = PGN.Puzzle;
        //    var games = parser.GetGamesFromPGNAsync(g1Str).Result;
        //    var g1 = games.First();
        //    g1.GoToLastMove();
        //    var endNode = g1.CurrentMoveNode;
        //    g1.GoToInitialState();
        //    var splitGame = g1.SplitFromMove(endNode);
        //    try
        //    {
        //        Assert.IsTrue(splitGame.IsEqualTo(g1));
        //    }
        //    catch
        //    {
        //        Console.WriteLine($"Original Game PGN{Environment.NewLine}{g1}");
        //        Console.WriteLine(new string('*', 25));
        //        Console.WriteLine($"Split Game PGN{Environment.NewLine}{splitGame}");
        //        throw;
        //    }
        //}

        //[Test]
        //public void ShouldSplitWithNoVariations()
        //{
        //    var parser = new PGNParser();
        //    var g1Str = PGN.s1;
        //    var g2Str = PGN.s2;
        //    var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
        //    var g2 = parser.GetGamesFromPGNAsync(g2Str).Result.First();
        //    var move = g1.MainMoveTree.First;
        //    while (move != null && (move = move.Next)?.Value.SAN != "e6") ;
        //    var splitGame = g1.SplitFromMove(move);
        //    Assert.IsTrue(splitGame.IsEqualTo(g2));
        //}

        //[Test]
        //public void ShouldSplitCorrectlyFromVariations()
        //{
        //    var parser = new PGNParser();
        //    var g1Str = PGN.g1Variation;
        //    var g2Str = PGN.g1SplitFromVariation;
        //    var g1 = parser.GetGamesFromPGNAsync(g1Str).Result.First();
        //    var g2 = parser.GetGamesFromPGNAsync(g2Str).Result.First();
        //    var move = g1.MainMoveTree.ElementAt(4).Variations[1].First;
        //    var splitGame = g1.SplitFromMove(move);
        //    Assert.IsTrue(splitGame.IsEqualTo(g2));
        //}
    }
}