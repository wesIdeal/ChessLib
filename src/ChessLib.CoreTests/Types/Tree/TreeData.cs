using System.Collections;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Tree;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Types.Tree
{
    public class TreeData
    {
        //public static IEnumerable GetVariationMoveSetTestCases()
        //{
        //    Assert.AreEqual("d4", VariationOnFirstMove.CurrentSan);
        //    yield return new TestCaseData(VariationOnFirstMove).Returns(GameMoveFlags.Variation);
        //    var game = VariationOnFirstMove;
        //    game.ExitVariation();
        //    game.MoveNext();
        //    yield return new TestCaseData(game).Returns(GameMoveFlags.NullMove);
        //    yield return new TestCaseData(NoVariation).Returns(GameMoveFlags.NullMove);
        //    var beginning = NoVariation;
        //    beginning.MovePrevious();
        //    yield return new TestCaseData(beginning).Returns(GameMoveFlags.NullMove);

        //}
        protected static Game NoVariation
        {
            get
            {
                var game = new Game();
                game.ApplyMove("c4").ApplyMove("e5");
                return game;
            }
        }

        protected static Game VariationOnFirstMove
        {
            get
            {
                var g = new Game();
                g.ApplyMove("c4");
                g.MovePrevious();
                g.ApplyMove("d4");
                return g;
            }
        }

        public static IEnumerable GetInitialMoveSetTestCases()
        {
            yield return new TestCaseData(VariationOnFirstMove).Returns(true);
            var game = VariationOnFirstMove;
            game.ExitVariation();
            game.MoveNext();
            yield return new TestCaseData(game).Returns(true);
            yield return new TestCaseData(NoVariation).Returns(false);
            var beginning = NoVariation;
            beginning.MovePrevious();
            yield return new TestCaseData(beginning).Returns(true);
        }

        public static IEnumerable GetVariationMoveSetTestCases()
        {
            Assert.AreEqual("d4", VariationOnFirstMove.CurrentSan);
            yield return new TestCaseData(VariationOnFirstMove).Returns(true);
            var game = VariationOnFirstMove;
            game.ExitVariation();
            game.MoveNext();
            yield return new TestCaseData(game).Returns(false);
            yield return new TestCaseData(NoVariation).Returns(false);
            var beginning = NoVariation;
            beginning.MovePrevious();
            yield return new TestCaseData(beginning).Returns(false);
        }

        public static IEnumerable GetEndOfVariationTestCases()
        {
            var game = new Game("rnbqkb1r/pppp1ppp/4pn2/8/2P5/2N5/PP1PPPPP/R1BQKBNR w KQkq - 0 3");
            game.ApplyMove("Nf3")
                .ApplyMove("d5")
                .ApplyMove("d4")
                .MovePrevious();
            game.ApplyMove("cxd5")
                .ApplyMove("exd5")
                .ApplyMove("d4");
            game.MovePrevious();
            game.ApplyMove("e3");
            game.Reset();
            game.MoveNext();
            yield return new TestCaseData((MoveTreeNode<PostMoveState>)game.Current.Node.Clone()).Returns(false)
                .SetDescription("Nf3");
            game.MoveNext();
            yield return new TestCaseData((MoveTreeNode<PostMoveState>)game.Current.Node.Clone()).Returns(false)
                .SetDescription("d5");
            game.MoveNext();
            yield return new TestCaseData((MoveTreeNode<PostMoveState>)game.Current.Node.Clone()).Returns(true)
                .SetDescription("d4");
            game.MovePrevious();
            game.MoveNext(game.NextMoves[1].MoveValue);
            yield return new TestCaseData((MoveTreeNode<PostMoveState>)game.Current.Node.Clone()).Returns(false)
                .SetDescription("cxd5");
            game.MoveNext();
            yield return new TestCaseData((MoveTreeNode<PostMoveState>)game.Current.Node.Clone()).Returns(false)
                .SetDescription("exd5");
            game.MoveNext();
            yield return new TestCaseData((MoveTreeNode<PostMoveState>)game.Current.Node.Clone()).Returns(true)
                .SetDescription("d4");
            game.MovePrevious();
            game.MoveNext(game.NextMoves[1].MoveValue);
            yield return new TestCaseData((MoveTreeNode<PostMoveState>)game.Current.Node.Clone()).Returns(true)
                .SetDescription("e3");
        }
    }
}