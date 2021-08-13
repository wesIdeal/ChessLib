using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using Moq;
using NUnit.Framework;
// ReSharper disable PossibleNullReferenceException

namespace ChessLib.Core.Tests
{
    [TestFixture(TestOf = typeof(PostMoveState))]
    public class PostMoveNodeTests
    {
    }

    [TestFixture(TestOf = typeof(Game), Category = "External")]
    public class GameTests
    {
        [Test]
        public void DefaultConstructor_ShouldMakeInitialBoard()
        {
            var expected = new Board();
            var game = new Game();
            Assert.AreEqual(expected, game.InitialNode.Board);
            Assert.IsTrue(((Move)game.InitialNode.Node.Value.MoveValue).IsNullMove);
            Assert.AreEqual(string.Empty, game.InitialNode.Node.Value.San);
            Assert.AreEqual(string.Empty, game.InitialNode.Node.Value.Comment);
            Assert.AreEqual(null, game.InitialNode.Node.Value.Annotation);
        }

        [TestCaseSource(nameof(GetConstructorTestCases))]
        public void Constructors_ShouldSetCurrentToInitialReference(Game game)
        {
            Assert.IsNotNull(game.InitialNode);
            Assert.AreSame(game.InitialNode, game.Current);
        }


        public static IEnumerable<Game> GetConstructorTestCases()
        {
            yield return new Game();
            yield return new Game(CoreTestConstants.EnglishTabiyaFen);
            yield return new Game(CoreTestConstants.EnglishTabiyaFen, new Tags());
        }


        [Test]
        public void ParameterizedConstructor_ShouldMakeNonStandardStartingPosition()
        {
            var board = fenTextToBoard.Translate(CoreTestConstants.EnglishTabiyaFen);
            var game = new Game(board.Fen);
            Assert.AreEqual(board, game.InitialNode.Board);
        }

        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();

        [TestCaseSource(nameof(MakeTagTestCases), new object[] { CoreTestConstants.EnglishTabiyaFen })]
        public void ParameterizedConstructor_ShouldRetainTagsFromParameter(Tags tags)
        {
            var board = fenTextToBoard.Translate(CoreTestConstants.EnglishTabiyaFen);
            var game = new Game(board.Fen, tags);
            Assert.AreEqual(tags, game.Tags);
            Assert.AreNotSame(tags, game.Tags);
        }

        [TestFixture]
        [Category("Game Traversal")]
        public class GameTraversalTests
        {
            [SetUp]
            public void SetUp()
            {
                game = new Game();
                mockGame = new Mock<Game>();
            }

            private Game game;
            private Mock<Game> mockGame;

            [Test]
            public void MoveNext_ShouldCallParameterizedVersionWith0()
            {
                const int expectedIndex = 0;
                mockGame.Setup(t => t.MoveNext(It.Is<int>(p => p == expectedIndex)))
                    .Returns(true)
                    .Verifiable($"Expected MoveNext to be called with {expectedIndex}");
                mockGame.Object.MoveNext();
                mockGame.Verify();
            }

            [Test]
            public void MovePrevious_ShouldSetBoardToPreviousState()
            {
                var expectedFen = game.Current.Fen;
                game.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
                var success = game.MovePrevious();
                Assert.IsTrue(success, "MovePrevious() returned false.");
                Assert.AreEqual(expectedFen, game.Current.Fen);
            }

            [Test]
            public void ApplyMove_WhenVariation_ShouldFollowVariationTree()
            {
                var expectedFen = game.Current.Fen;
                game.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
                game.MovePrevious();
                var firstMove = true;
                foreach (var move in CoreTestConstants.d4Variation)
                {
                    game.ApplyMove(move, firstMove ? MoveApplicationStrategy.Variation : MoveApplicationStrategy.ContinueMainLine);
                    firstMove = false;
                }
                Assert.AreEqual(CoreTestConstants.d4VariationFen, game.Current.Fen);
            }

            [Test]
            public void ExitVariation_ShouldReturnToVariationParentBoard()
            {
                ApplyEnglishTabiya();
                game.MovePrevious();
                var expectedFen = game.Current.Fen;
                ApplyEnglishTabiyaAltContinuation();
                game.ExitVariation();
                Assert.AreEqual(expectedFen, game.Current.Fen);
            }

            [Test]
            public void ExitVariation_WhenMainLine_ShouldReturnToInitialBoard()
            {
                ApplyEnglishTabiya();
                game.ExitVariation();
                game.ExitVariation();
                Applyd4Variation();
                Assert.AreEqual(CoreTestConstants.d4VariationFen, game.Current.Fen);
                game.ExitVariation();
                Assert.IsTrue(game.MoveNext());
                Assert.IsTrue(game.MoveNext());
                game.ExitVariation();
                Assert.AreEqual(game.InitialNode, game.Current);
            }

            private void ApplyEnglishTabiya()
            {
                ApplyMoves(CoreTestConstants.EnglishTabiyaMoves);
            }

            private void Applyd4Variation()
            {
                ApplyMoves(CoreTestConstants.d4Variation);
            }
            private void ApplyEnglishTabiyaAltContinuation()
            {
                ApplyMoves(CoreTestConstants.EnglishTabiyaAlternateContinuation);
            }

            private void ApplyAsVariation(string[] moves)
            {
                Debug.Assert(moves.Any());
                game.ApplyMove(moves[0], MoveApplicationStrategy.Variation);
                var remaining = moves.Skip(1).ToArray();
                if (remaining.Any())
                {
                    ApplyMoves(remaining);
                }
            }

            private void ApplyMoves(string[] moves)
            {
                foreach (var move in moves)
                {
                    game.ApplyMove(move, MoveApplicationStrategy.ContinueMainLine);
                }
            }

            [Test]
            public void MovePrevious_FromRootNode_ShouldReturnFalse()
            {
                var success = game.MovePrevious();
                Assert.IsFalse(success, "MovePrevious() should have returned false from the root.");
            }

            [Test]
            public void MovePrevious_FromRootNode_ShouldNotChangeCurrentNode()
            {
                game.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
                game.MovePrevious();
                var success = game.MovePrevious();
                Assert.AreEqual(game.InitialNode, game.Current);
            }

            [TestCaseSource(nameof(GetBadContinuationIndices))]
            public void MoveNext_ParameterizedVersion_WhenVariationNotPresent_ShouldReturnFalse(int index)
            {
                Assert.IsFalse(game.MoveNext(index));
            }

            protected static IEnumerable<int> GetBadContinuationIndices => Enumerable.Range(0, 10);
            protected static IEnumerable<int> GetGoodContinuationIndices => Enumerable.Range(0, 1);

            protected static IEnumerable<(int Index, bool ExpectedResult, Game Game)> GetMoveNextContinuationTestCases()
            {
                var game = new Game();
                foreach (var badContinuation in GetBadContinuationIndices)
                {
                    yield return (badContinuation, false, game);
                }
            }
        }

        [TestFixture]
        [Category("Game Move Application")]
        public class GameMoveApplication
        {
            [SetUp]
            public void SetUp()
            {
                game = new Game();
                Debug.Assert(game.Current != null);
            }

            private Game game;

            [Test]
            public void ApplyMove_ShouldSetCurrentBoardToNewState()
            {
                game.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
                Assert.IsNotNull(game.Current);
                Assert.AreEqual("rnbqkbnr/pppppppp/8/8/2P5/8/PP1PPPPP/RNBQKBNR b KQkq c3 0 1", game.Current.Board.Fen);
            }


        }

        protected static IEnumerable<Tags> MakeTagTestCases(string fen)
        {
            yield return new Tags(fen);
            var tags = new Tags(fen)
            {
                Event = "Test",
                Black = "GreenResult",
                White = "RedResult",
                Date = "2021.10.31",
                Result = "1-0"
            };
            yield return tags;
        }
    }
}