using System.Collections;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.PgnExport;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Types.Tree
{
    public class TreeData
    {
        public static IEnumerable GetInitialMoveSetTestCases()
        {
            yield return new TestCaseData(VariationOnFirstMove).Returns(GameMoveFlags.InitialMove);
            var game = VariationOnFirstMove;
            game.ExitVariation();
            game.MoveNext();
            yield return new TestCaseData(game).Returns(GameMoveFlags.InitialMove);
            yield return new TestCaseData(NoVariation).Returns(GameMoveFlags.NullMove);
            var beginning = NoVariation;
            beginning.MovePrevious();
            yield return new TestCaseData(beginning).Returns(GameMoveFlags.InitialMove);

        }

        public static IEnumerable GetVariationMoveSetTestCases()
        {
            Assert.AreEqual("d4", VariationOnFirstMove.CurrentSan);
            yield return new TestCaseData(VariationOnFirstMove).Returns(GameMoveFlags.Variation);
            var game = VariationOnFirstMove;
            game.ExitVariation();
            game.MoveNext();
            yield return new TestCaseData(game).Returns(GameMoveFlags.NullMove);
            yield return new TestCaseData(NoVariation).Returns(GameMoveFlags.NullMove);
            var beginning = NoVariation;
            beginning.MovePrevious();
            yield return new TestCaseData(beginning).Returns(GameMoveFlags.NullMove);

        }

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

    }
}