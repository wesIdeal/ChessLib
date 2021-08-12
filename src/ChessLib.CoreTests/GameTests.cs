using System;
using System.Linq;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Core.Tests
{
  

    [TestFixture(TestOf = typeof(PostMoveState))]
    public class PostMoveNodeTests
    {

    }

    [TestFixture(TestOf = typeof(Game))]
    public class GameTests
    {
        //[SetUp]
        //public void Setup()
        //{
        //    var fen = CoreTestConstants.EnglishTabiyaFen;
        //    var tags = new Tags { Event = "Unit Testing" };
        //    _game = new Game(fen, tags);
        //    //_moveNodePostMove = _game.AddMove(CoreTestConstants.EnglishTabiyaNextMove);
        //    _game.AddParsingLogItem(new PgnParsingLog(ParsingErrorLevel.Info,
        //        $"Unit test commenced on {DateTime.Now}"));
        //}

        //protected Game GetEnglishMainLine
        //{
        //    get
        //    {
        //        var g = new Game();
        //        foreach (var move in CoreTestConstants.EnglishTabiyaMoves)
        //        {
        //            g.ApplySanMove(move, MoveApplicationStrategy.ContinueMainLine);
        //        }

        //        return g;
        //    }
        //}


        //private Game _game;

        //[Test]
        //public void CopyConstructor_Test()
        //{
        //    var gameCopy = new Game(_game);
        //    Assert.IsTrue(_game.Equals(gameCopy));

        //    Assert.AreNotSame(_game, gameCopy);
        //    Assert.AreNotSame(_game.TagSection, gameCopy.TagSection,
        //        "TagSection should not point to the same object when copied.");
        //    Assert.AreNotSame(_game.Continuations, gameCopy.Continuations,
        //        "MainMoveTree should not point to the same object when copied.");
        //    Assert.AreNotSame(_game.ParsingLog, gameCopy.ParsingLog,
        //        "Parsing should not point to the same object when copied.");
        //}


        //[Test]
        //public void ApplySanMoveTest_NormalMove()
        //{
        //    _game = new Game(CoreTestConstants.EnglishTabiyaFen);
        //    var postMoveNode = _game.ApplySanMove(CoreTestConstants.EnglishTabiyaNextMoveSan,
        //        MoveApplicationStrategy.ContinueMainLine);

        //    Assert.AreEqual(CoreTestConstants.EnglishTabiyaPostMove, _game.Fen);
        //    Assert.AreEqual(CoreTestConstants.EnglishTabiyaNextMoveSan, _game.CurrentBoard.San);
        //}

        //[Test]
        //public void ApplySanMoveTest_Variation()
        //{
        //    _game.MovePrevious();
        //    Console.WriteLine(_game.Fen);
        //    _game.ApplySanMove("d4", MoveApplicationStrategy.Variation);
        //    Assert.AreEqual(CoreTestConstants.EnglishTabiyaPostMoveAlternate, _game.Fen);
        //}

        //[Test]
        //public void ExitVariation_ShouldReverseOutOfVariationToParent()
        //{
        //    var g = GetEnglishMainLine;

        //    foreach (var mv in CoreTestConstants.EnglishTabiyaContinuation)
        //    {
        //        g.ApplySanMove(mv, MoveApplicationStrategy.NewMainLine);
        //    }

        //    for (var i = 0; i < CoreTestConstants.EnglishTabiyaContinuation.Length; i++)
        //    {
        //        g.MovePrevious();
        //    }

        //    Assert.AreEqual(CoreTestConstants.EnglishTabiyaFen, g.Fen);
        //    Console.WriteLine($"Current FEN after exiting variation: {g.Fen}");
        //    g.ApplySanMove(CoreTestConstants.EnglishTabiyaVariation[0], MoveApplicationStrategy.Variation);
        //    for (var i = 1; i < CoreTestConstants.EnglishTabiyaVariation.Length; i++)
        //    {
        //        var mv = CoreTestConstants.EnglishTabiyaVariation[i];
        //        g.ApplySanMove(mv, MoveApplicationStrategy.ContinueMainLine);
        //    }

        //    g.ExitVariation();

        //    Assert.AreEqual(CoreTestConstants.EnglishTabiyaFen, g.Fen);
        //}

        //[Test]
        //public void ExitVariation_ShouldReturnToParentNode_WhenInVariation()
        //{
        //    var g = GetEnglishMainLine;
        //    AddContinuation(g, CoreTestConstants.EnglishTabiyaContinuation, MoveApplicationStrategy.ContinueMainLine);
        //    g.MovePrevious();
        //    g.AddContinuation(CoreTestConstants.EnglishTabiyaNextMoveAlternate);
        //    g.ExitVariation();
        //    Assert.AreEqual(CoreTestConstants.EnglishTabiyaFen, g.Fen);
        //}

        //[Test]
        //public void ExitVariation_ShouldReturnToInitialBoard_WhenNotInVariation()
        //{
        //    var g = GetEnglishMainLine;
        //    AddContinuation(g, CoreTestConstants.EnglishTabiyaContinuation, MoveApplicationStrategy.ContinueMainLine);
        //    g.ExitVariation();
        //    Assert.AreEqual(BoardConstants.FenStartingPosition, g.Fen);
        //}

        //private Game Traverse(Game game, string[] englishMainLineContinuation)
        //{
        //    for (var i = 0; i < englishMainLineContinuation.Length; i++)
        //    {
        //        var fromMove = game.CurrentBoard.San;
        //        game.MovePrevious();
        //        var toMove = game.CurrentBoard.San;
        //        Console.WriteLine($"Moving backward, from state {fromMove} to state {toMove}");
        //    }

        //    return game;
        //}

        //[Test]
        //public void AddParsingLogItemTest_ConstructedFromParams()
        //{
        //    var level = ParsingErrorLevel.Warning;
        //    var msg = "Warning issued.";
        //    var parseInput = "1. e4";
        //    _game.ClearParsingLog();
        //    _game.AddParsingLogItem(level, msg, parseInput);
        //    var addedItem = _game.ParsingLog.First();
        //    Assert.AreEqual(level, addedItem.ParsingErrorLevel);
        //    Assert.AreEqual(msg, addedItem.Message);
        //    Assert.AreEqual(parseInput, addedItem.ParseInput);
        //}

        //[Test]
        //public void AddParsingLogItemTest_FromParams()
        //{
        //    var level = ParsingErrorLevel.Warning;
        //    var msg = "Warning issued.";
        //    var parseInput = "1. e4";
        //    var itemToAdd = new PgnParsingLog(level, msg, parseInput);
        //    _game.ClearParsingLog();
        //    _game.AddParsingLogItem(itemToAdd);
        //    var addedItem = _game.ParsingLog.First();
        //    Assert.AreEqual(level, addedItem.ParsingErrorLevel);
        //    Assert.AreEqual(msg, addedItem.Message);
        //    Assert.AreEqual(parseInput, addedItem.ParseInput);
        //}

        //[Test]
        //public void ShouldReturnEqualForSameGame()
        //{
        //    _game.MovePrevious();
        //    _game.ApplySanMove("d4", MoveApplicationStrategy.Variation);
        //    var game2 = new Game(_game);
        //    Assert.IsTrue(_game.IsMainLineEqual(game2));
        //    Assert.IsTrue(_game.Equals(game2));
        //}

        //[Test]
        //public void ShouldReturnCorrectValueSameGame_VariationAndNoVariation()
        //{
        //    var game1 = new Game(CoreTestConstants.EnglishTabiyaFen);
        //    game1.ApplySanMove(CoreTestConstants.EnglishTabiyaNextMoveSan, MoveApplicationStrategy.ContinueMainLine);
        //    var game2 = new Game(game1);
        //    game1.MovePrevious();
        //    game1.ApplySanMove("d4", MoveApplicationStrategy.Variation);

        //    Assert.IsTrue(game1.IsMainLineEqual(game2), "Should return true, ignoring variations.");
        //    Assert.IsFalse(game1.Equals(game2), "Should return false when including variations");
        //}
    }
}