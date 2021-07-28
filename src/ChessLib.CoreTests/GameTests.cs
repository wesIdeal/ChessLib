using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Core.Tests
{
    [TestFixture(TestOf = typeof(Game))]
    public class GameTests
    {
        [SetUp]
        public void Setup()
        {
            var fen = CoreTestConstants.EnglishTabiya;
            var tags = new Tags { Event = "Unit Testing" };
            _game = new Game(fen, tags);
            _moveNodePostMove = _game.ApplyMove(CoreTestConstants.EnglishTabiyaNextMove);
            _game.AddParsingLogItem(new PgnParsingLog(ParsingErrorLevel.Info,
                $"Unit test commenced on {DateTime.Now}"));
        }

        private Game _game;
        private LinkedListNode<BoardSnapshot> _moveNodePostMove;

        [Test]
        public void CopyConstructor_Test()
        {
            var gameCopy = new Game(_game);
            Assert.AreEqual(_game.TagSection, gameCopy.TagSection, "TagSections should be equal values.");
            Assert.AreEqual(_game.MainMoveTree, gameCopy.MainMoveTree, "MainMoveTrees should be equal values.");
            Assert.AreEqual(_game.ParsingLog, gameCopy.ParsingLog, "ParsingLogs should be equal values.");

            Assert.AreNotSame(_game, gameCopy);
            Assert.AreNotSame(_game.TagSection, gameCopy.TagSection,
                "TagSection should not point to the same object when copied.");
            Assert.AreNotSame(_game.MainMoveTree, gameCopy.MainMoveTree,
                "MainMoveTree should not point to the same object when copied.");
            Assert.AreNotSame(_game.ParsingLog, gameCopy.ParsingLog,
                "Parsing should not point to the same object when copied.");
        }


        [Test]
        public void ApplySanMoveTest_NormalMove()
        {
            _game = new Game(CoreTestConstants.EnglishTabiya);
            var postMoveNode = _game.ApplySanMove(CoreTestConstants.EnglishTabiyaNextMoveSan, MoveApplicationStrategy.ContinueMainLine);
            Assert.AreEqual(CoreTestConstants.EnglishTabiyaPostMove, _game.CurrentFEN);
            Assert.AreEqual(CoreTestConstants.EnglishTabiyaNextMoveSan, postMoveNode.Value.SAN);
            Assert.AreEqual(CoreTestConstants.EnglishTabiyaNextMove.MoveValue, postMoveNode.Value.MoveValue);
        }

        [Test]
        public void ApplySanMoveTest_Variation()
        {
            _game.TraverseBackward();
            _game.ApplySanMove("d4", MoveApplicationStrategy.Variation);
            Assert.AreEqual(CoreTestConstants.EnglishTabiyaPostMoveAlternate, _game.CurrentFEN);
        }

        [Test]
        public void AddParsingLogItemTest_ConstructedFromParams()
        {
            
            var level = ParsingErrorLevel.Warning;
            var msg = "Warning issued.";
            var parseInput = "1. e4";
            _game.ClearParsingLog();
            _game.AddParsingLogItem(level, msg, parseInput);
            var addedItem = _game.ParsingLog.First();
            Assert.AreEqual(level, addedItem.ParsingErrorLevel);
            Assert.AreEqual(msg, addedItem.Message);
            Assert.AreEqual(parseInput, addedItem.ParseInput);
        }

        [Test]
        public void AddParsingLogItemTest_FromParams()
        {

            var level = ParsingErrorLevel.Warning;
            var msg = "Warning issued.";
            var parseInput = "1. e4";
            var itemToAdd = new PgnParsingLog(level, msg, parseInput);
            _game.ClearParsingLog();
            _game.AddParsingLogItem(itemToAdd);
            var addedItem = _game.ParsingLog.First();
            Assert.AreEqual(level, addedItem.ParsingErrorLevel);
            Assert.AreEqual(msg, addedItem.Message);
            Assert.AreEqual(parseInput, addedItem.ParseInput);
        }

        [Test]
        public void ShouldReturnEqualForSameGame()
        {
            _game.TraverseBackward();
            _game.ApplySanMove("d4", MoveApplicationStrategy.Variation);
            var game2 = new Game(_game);
            Assert.IsTrue(_game.IsEqualTo(game2));
            Assert.IsTrue(_game.IsEqualTo(game2, true));
        }
        [Test]
        public void ShouldReturnCorrectValueSameGame_VariationAndNoVariation()
        {
            var game1 = new Game(CoreTestConstants.EnglishTabiya);
            game1.ApplySanMove(CoreTestConstants.EnglishTabiyaNextMoveSan, MoveApplicationStrategy.ContinueMainLine);
            var game2 = new Game(game1);
            game1.TraverseBackward();
            game1.ApplySanMove("d4", MoveApplicationStrategy.Variation);
            Assert.IsTrue(game1.IsEqualTo(game2),"Should return true, ignoring variations.");
            Assert.IsFalse(game1.IsEqualTo(game2, true), "Should return false when including variations");
        }
    }
}