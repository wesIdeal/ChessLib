using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.GameTree;
using Moq;
using NUnit.Framework;

// ReSharper disable PossibleNullReferenceException

namespace ChessLib.Core.Tests
{
    [TestFixture(TestOf = typeof(PostMoveState))]
    public class PostMoveNodeTests
    {
    }

    [TestFixture(TestOf = typeof(Game), Category = "External, Game")]
    public class GameTests
    {
        [SetUp]
        public void SetUp()
        {
            game = new Game();
        }

        [Test]
        public void FindMoveIndexInContinuations_WhenMoveIsInContinuation_ShouldReturnZero()
        {
            const int returnVal = 0;
            var mock = new Mock<Game> { CallBase = true };
            mock.Object.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
            mock.Object.MovePrevious();

            mock.SetupGet(x => x.NextMoves).Returns(new[]
                { new PostMoveState(new Board(), Move.NullMove, "null") });
            var actual = mock.Object.FindMoveIndexInContinuations(Move.NullMove);
            Assert.AreEqual(returnVal, actual);
        }

        [Test]
        public void FindMoveIndexInContinuations_WhenMoveIsNotInContinuation_ShouldReturnNeg1()
        {
            const int returnVal = -1;
            var mock = new Mock<Game> { CallBase = true };
            mock.Object.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
            mock.Object.MovePrevious();

            mock.SetupGet(x => x.NextMoves).Returns(new[]
                { new PostMoveState(new Board(), Move.NullMove, "null") });
            var actual = mock.Object.FindMoveIndexInContinuations(1);
            Assert.AreEqual(returnVal, actual);
        }

        [Test]
        public void MoveNext_WhenMoveNotFound_ShouldReturnFalse()
        {
            var mock = new Mock<Game> { CallBase = true };
            mock.Object.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
            mock.Object.MovePrevious();
            mock.Setup(t => t.MoveNext(It.IsAny<int>())).Returns(true).Verifiable();
            mock.Setup(t => t.FindMoveIndexInContinuations(It.IsAny<ushort>())).Returns(-1); //Move not found
            var actual = mock.Object.MoveNext(666);
            Assert.IsFalse(actual);
        }

        [Test]
        public void MoveNext_WhenMoveFound_ShouldReturnTrue()
        {
            var mock = new Mock<Game> { CallBase = true };
            ushort move = 600;
            mock.Object.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
            mock.Object.MovePrevious();
            mock.Setup(t => t.MoveNext(It.Is<int>(x => x == 0))).Returns(true).Verifiable();
            mock.Setup(t => t.FindMoveIndexInContinuations(It.IsAny<ushort>())).Returns(0); //Move at [0]
            var actual = mock.Object.MoveNext(move);
            Assert.True(actual);
        }

        [Test]
        public void DefaultConstructor_ShouldMakeInitialBoard()
        {
            var expected = new Board();
            Assert.AreEqual(expected, game.InitialNode.Board);
            Assert.IsTrue(((Move)game.InitialNode.Node.Value.MoveValue).IsNullMove);
            Assert.AreEqual(string.Empty, game.InitialNode.Node.Value.San);
            Assert.AreEqual(string.Empty, game.InitialNode.Node.Comment);
            Assert.AreEqual(new NumericAnnotation(), game.InitialNode.Node.Annotation);
        }

        [TestCaseSource(nameof(GetConstructorTestCases))]
        public void Constructors_ShouldSetCurrentToInitialReference(Game gameTestCase)
        {
            Assert.IsNotNull(gameTestCase.InitialNode);
            Assert.AreSame(gameTestCase.InitialNode, gameTestCase.Current);
        }

        [Test]
        public void SetComment_ShouldCorrectlySetCommentForMove()
        {
            game.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
            var comment = "Great move!";
            game.SetComment(comment);
            Assert.AreEqual(comment, game.Current.Node.Comment);
            game.SetComment(string.Empty);
            Assert.AreEqual(string.Empty, game.Current.Node.Comment);
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
            game = new Game(board.Fen);
            Assert.AreEqual(board, game.InitialNode.Board);
        }

        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();

        [TestCaseSource(nameof(MakeTagTestCases), new object[] { CoreTestConstants.EnglishTabiyaFen })]
        public void ParameterizedConstructor_ShouldRetainTagsFromParameter(Tags tags)
        {
            var board = fenTextToBoard.Translate(CoreTestConstants.EnglishTabiyaFen);
            game = new Game(board.Fen, tags);
            Assert.AreEqual(tags, game.Tags);
            Assert.AreNotSame(tags, game.Tags);
        }


        [Test]
        [TestCaseSource(nameof(GetPlyCountTestCases))]
        public void PlyCount_ShouldEqualNumberOfMoves(IEnumerable<string> moves)
        {
            var movesToApply = moves.ToList();
            var expectedPlyCount = movesToApply.Count;
            game = new Game();
            movesToApply.ForEach(mv => game.ApplyMove(mv, MoveApplicationStrategy.ContinueMainLine));
            Assert.AreEqual(expectedPlyCount, game.PlyCount);
        }

        protected static IEnumerable<IEnumerable<string>> GetPlyCountTestCases()
        {
            yield return new string[] { };
            //Seems peculiar at first glance, but List<string> is necessary to prevent param method overload from being called.
            // [NUnit.Framework.Internal.Reflect.InvokeMethod(MethodInfo method, Object fixture, Object[] args)]
            yield return new List<string>(new[] { "c4" });
            yield return new[] { "c4", "e5" };
            yield return CoreTestConstants.EnglishTabiyaMoves;
        }

        [Test(Description = "Tests adding a parsing log item with single parameter method.")]
        [Category("Parsing Log")]
        public void PgnParsingLog_ObjectParam_ShouldAddLogItem()
        {
            var expected = new PgnParsingLog(ParsingErrorLevel.Error, "Error occurred.", "[bad input]");
            game.AddParsingLogItem(expected);
            Assert.AreEqual(expected, game.ParsingLogs.First());
        }

        [Test(Description = "Tests that calling the multi-param version calls the object param version.")]
        public void PgnParsingLog_Parameterized_ShouldCallObjectParamOverload()
        {
            var expected = new PgnParsingLog(ParsingErrorLevel.Error, "Error occurred.", "[bad input]");
            var mock = new Mock<Game>();
            mock.Setup(t => t.AddParsingLogItem(It.Is<PgnParsingLog>(x => x.Equals(expected))))
                .Verifiable($"Failed calling {nameof(Game.AddParsingLogItem)}(PgnParsingLog log)");
            mock.Object.AddParsingLogItem(expected.ParsingErrorLevel, expected.Message, expected.ParseInput);
            mock.Verify();
        }

        [TestCaseSource(nameof(GetNextMovesTestCases))]
        public void NextMoves_ShouldReturnProperArray651094363(
            (ushort[] expectedNextMoves, ushort[] currentNextMoves) testCase)
        {
            Assert.AreEqual(testCase.expectedNextMoves, testCase.currentNextMoves);
        }

        [TestCaseSource(nameof(GetResetTestCases))]
        public void Reset_ShouldSetCurrentBoardToInitialBoard((Game gameContext, string testDescription) testCase)
        {
            var testCaseGameContext = testCase.gameContext;
            testCaseGameContext.Reset();
            Assert.AreEqual(testCaseGameContext.InitialNode, testCaseGameContext.Current, testCase.testDescription);
        }

        protected static IEnumerable<(Game game, string description)> GetResetTestCases()
        {
            yield return (new Game(), "New game.");
            var game = new Game();
            ApplyEnglishTabiya(game);
            Debug.Assert(game.PlyCount != 0);
            yield return (game, description: $"English Tabiya Applied to {BoardConstants.FenStartingPosition}.");

            game = new Game();
            ApplyEnglishTabiya(game);
            game.ExitVariation();
            ApplyQueenPawnVariation(game);

            Debug.Assert(game.PlyCount != 0);
            Debug.Assert(game.InitialNode.Variations.Count == 1);
            yield return (game,
                description: $"English Tabiya and 1.d4 variation Applied to {BoardConstants.FenStartingPosition}.");

            game = new Game();
            ApplyEnglishTabiya(game);
            game.MovePrevious();
            ApplyEnglishTabiyaAltContinuation(game);
            Debug.Assert(game.PlyCount != 0);
            yield return (game,
                description:
                $"English Tabiya to 4.g3 and 4.d4 variation Applied to {BoardConstants.FenStartingPosition}.");

            game = new Game(CoreTestConstants.EnglishTabiyaFen);
            game.ApplyMove("g3", MoveApplicationStrategy.ContinueMainLine);
            game.MovePrevious();
            ApplyEnglishTabiyaAltContinuation(game);
            Debug.Assert(game.PlyCount != 0);
            yield return (game,
                description: $"4.g3 and 4.d4 variation Applied to {CoreTestConstants.EnglishTabiyaFen}.");
        }

        [Test]
        public void AddNag_ShouldAddNagToCurrentNode()
        {
            game.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
            Assert.AreEqual(0, game.Current.Node.Annotation.All().Sum());
            var numericAnnotation = new NumericAnnotation((int)PositionalNAG.Drawish);
            numericAnnotation.ApplyNag((int)MoveNAG.GoodMove);
            numericAnnotation.ApplyNag((int)PositionalNAG.EqualQuietPosition);
            numericAnnotation.ApplyNag((int)NonStandardNAG.Diagram);
            game.AddNag(numericAnnotation);
            Assert.AreEqual(numericAnnotation, game.Current.Node.Annotation);
        }

        protected static IEnumerable<(ushort[] expectedNextMoves, ushort[] currentNextMoves)> GetNextMovesTestCases()
        {
            var game = new Game();
            yield return (new ushort[0], CurrentNextMoveValues(game));
            game.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
            yield return (new ushort[0], CurrentNextMoveValues(game));
            game.MovePrevious();
            yield return (new ushort[] { 666 }, CurrentNextMoveValues(game));
            game.ApplyMove("d4", MoveApplicationStrategy.ContinueMainLine);
            yield return (new ushort[0], CurrentNextMoveValues(game));
            game.MovePrevious();
            yield return (new ushort[] { 666, 731 }, CurrentNextMoveValues(game));
        }

        protected static ushort[] CurrentNextMoveValues(Game game)
        {
            return game.NextMoves.Select(x => x.MoveValue).ToArray();
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
                GameResult = GameResult.WhiteWins
            };
            yield return tags;
        }

        protected Game game;

        protected void ApplyEnglishTabiya()
        {
            ApplyMoves(CoreTestConstants.EnglishTabiyaMoves);
        }

        protected void ApplyQueenPawnVariation()
        {
            ApplyMoves(CoreTestConstants.d4Variation);
        }

        protected void ApplyEnglishTabiyaAltContinuation()
        {
            ApplyMoves(CoreTestConstants.EnglishTabiyaAlternateContinuation);
        }

        protected static void ApplyEnglishTabiya(Game game)
        {
            ApplyMoves(game, CoreTestConstants.EnglishTabiyaMoves);
        }

        private static void ApplyMoves(Game game, string[] moves)
        {
            foreach (var move in moves)
            {
                game.ApplyMove(move, MoveApplicationStrategy.ContinueMainLine);
            }
        }

        protected static void ApplyQueenPawnVariation(Game game)
        {
            ApplyMoves(game, CoreTestConstants.d4Variation);
        }

        protected static void ApplyEnglishTabiyaAltContinuation(Game game)
        {
            ApplyMoves(game, CoreTestConstants.EnglishTabiyaAlternateContinuation);
        }

        private void ApplyMoves(string[] moves)
        {
            ApplyMoves(game, moves);
        }
    }

    [TestFixture(TestOf = typeof(GameEnumerator), Category = "External, Game, Tree Traversal")]
    public class GameTraversalTests : GameTests
    {
        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            mockGame = new Mock<Game>();
        }


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
            game.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
            game.MovePrevious();
            var firstMove = true;
            foreach (var move in CoreTestConstants.d4Variation)
            {
                game.ApplyMove(move,
                    firstMove ? MoveApplicationStrategy.Variation : MoveApplicationStrategy.ContinueMainLine);
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
            ApplyQueenPawnVariation();
            Assert.AreEqual(CoreTestConstants.d4VariationFen, game.Current.Fen);
            game.ExitVariation();
            Assert.IsTrue(game.MoveNext());
            Assert.IsTrue(game.MoveNext());
            game.ExitVariation();
            Assert.AreEqual(game.InitialNode, game.Current);
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
            game.MovePrevious();
            Assert.AreEqual(game.InitialNode, game.Current);
        }

        [TestCaseSource(nameof(GetBadContinuationIndices))]
        public void MoveNext_ParameterizedVersion_WhenVariationNotPresent_ShouldReturnFalse(int index)
        {
            Assert.IsFalse(game.MoveNext(index));
        }


        public void MoveNext_ParameterizedVersion_WhenVariationNotPresent_ShouldReturnTrue()
        {
            var index = 0;
            game.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
            game.MovePrevious();
            Assert.IsTrue(game.MoveNext(index));
        }

        protected static IEnumerable<int> GetBadContinuationIndices => Enumerable.Range(0, 10);


        [TestCaseSource(nameof(GetNextMovesTestCases))]
        public void NextMoves_ShouldReturnProperArray((ushort[] expectedNextMoves, ushort[] currentNextMoves) testCase)
        {
            Assert.AreEqual(testCase.expectedNextMoves, testCase.currentNextMoves);
        }
    }

    [TestFixture(TestOf = typeof(GameEnumerator), Category = "External, Game, Tree Building")]
    public class GameMoveApplication : GameTests
    {
        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            Debug.Assert(game.Current != null);
        }


        [Test]
        public void ApplyMove_ShouldSetCurrentBoardToNewState()
        {
            game.ApplyMove("c4", MoveApplicationStrategy.ContinueMainLine);
            Assert.IsNotNull(game.Current);
            Assert.AreEqual("rnbqkbnr/pppppppp/8/8/2P5/8/PP1PPPPP/RNBQKBNR b KQkq c3 0 1", game.Current.Board.Fen);
        }
    }
}