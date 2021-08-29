using System.Collections;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using NUnit.Framework;

namespace ChessLib.Core.Tests
{
    [TestFixture]
    public class PostMoveStateTests
    {
        [TestCaseSource(typeof(CoreTestData), nameof(CoreTestData.GetPostBoardStateEqualityCases))]
        public bool PostMoveState_Equals(PostMoveState postMoveState1, PostMoveState postMoveState2)
        {
            return postMoveState1.Equals(postMoveState2);
        }

    }

    public class CoreTestData
    {
        private static FenTextToBoard fenTextToBoard = new FenTextToBoard();
        public static IEnumerable GetPostBoardStateEqualityCases()
        {
            SanToMove sanToMove = new SanToMove();
            var board1 = fenTextToBoard.Translate("2bq1rk1/3p1npp/p1p3N1/1rbB1Pp1/1pQ5/P5N1/1PP3PP/R3R2K w - - 0 23");
            var san1 = "Bxf7";
            var move1 = sanToMove.GetMoveFromSAN(board1, san1);
            var postMoveBoardState1 = new PostMoveState(board1,
                move1,
                san1);
            var postMoveBoardState2 = (PostMoveState)postMoveBoardState1.Clone();
            yield return new TestCaseData(postMoveBoardState1, postMoveBoardState2).Returns(true).SetDescription("Board 2 is a clone of board 1");

            var move2 = sanToMove.GetMoveFromSAN(board1, "Be6");

            postMoveBoardState2 = new PostMoveState(board1, move2, "Be6");
            yield return new TestCaseData(postMoveBoardState1, postMoveBoardState2).Returns(false).SetDescription("Board 2 has a different move and SAN.");

            var board2 = fenTextToBoard.Translate("2b2rk1/3pqnpp/p1p3N1/1rbB1Pp1/1pQ5/P5N1/1PP3PP/R3R2K w - - 0 23");
            postMoveBoardState2 = new PostMoveState(board2, move1, san1);
            yield return new TestCaseData(postMoveBoardState1, postMoveBoardState2).Returns(false).SetDescription("Board 2 has a different board.");

            yield return new TestCaseData(postMoveBoardState1, null).Returns(false).SetDescription("Board 2 is null");

            postMoveBoardState2 = new PostMoveState(board1, move1, "Bxf7");
            yield return new TestCaseData(postMoveBoardState1, postMoveBoardState2).Returns(true)
                .SetDescription("Board 2 has the same values as Board 1.");

        }
    }
}