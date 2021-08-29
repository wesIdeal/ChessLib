using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.MoveValidation.MoveRules;
using Moq;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.MoveRules
{
    internal partial class PieceCanMoveToDestinationTests
    {
       [TestFixture(TestOf = typeof(PieceCanMoveToDestination),Category = "Validation - Pawns")]

        private class PawnMoves
        {
            private const string InitialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            private Board _board;
            public PieceCanMoveToDestination pieceCanMoveToDestinationValidator = new PieceCanMoveToDestination();
            [SetUp]
            public void Setup()
            {
                _board = fenTextToBoard.Translate(InitialFEN);
            }

            #region Validation Errors

            [TestCaseSource(nameof(GetPawnTests))]
            public MoveError TestValidation(PieceCanMoveToDestination validator, Board board, Move move)
            {
                return validator.Validate(board, null, move);
            }


            public static IEnumerable<TestCaseData> GetPawnTests()
            {
                var goodDestinationMock = new Mock<IBitboard>();
                goodDestinationMock.Setup(x =>
                        x.GetPseudoLegalMoves(It.IsAny<ushort>(), It.IsAny<Piece>(), It.IsAny<Color>(),
                            It.IsAny<ulong>()))
                    .Returns(0x1c0000000000)
                    .Verifiable();
                var badDestinationMock = new Mock<IBitboard>();
                badDestinationMock.Setup(x =>
                        x.GetPseudoLegalMoves(It.IsAny<ushort>(), It.IsAny<Piece>(), It.IsAny<Color>(),
                            It.IsAny<ulong>()))
                    .Returns(0x1b0000000000)
                    .Verifiable();

                var goodDestValidator =
                    new Core.Validation.Validators.MoveValidation.MoveRules.PieceCanMoveToDestination(
                        goodDestinationMock.Object);
                var badDestValidator =
                    new Core.Validation.Validators.MoveValidation.MoveRules.PieceCanMoveToDestination(badDestinationMock
                        .Object);
                var board = fenTextToBoard.Translate("r1bqkbnr/pp1ppppp/2n5/2pN4/8/8/PPPPPPPP/R1BQKBNR w KQkq - 1 3");

                var move = MoveHelpers.GenerateMove("a1".SquareTextToIndex(), "c6".SquareTextToIndex());
                yield return new TestCaseData(goodDestValidator, board, move)
                    .SetName("Validate Move Destination - should return no error when move is in the result set")
                    .SetDescription("Should call to get pseudolegal moves")
                    .Returns(MoveError.NoneSet);
                yield return new TestCaseData(badDestValidator, board, move)
                    .SetName("Validate Move Destination - should return MoveError.BadDestination when move is in the result set")
                    .SetDescription("Should call to get pseudolegal moves")
                    .Returns(MoveError.BadDestination);
                move = MoveHelpers.GenerateMove("d5".SquareTextToIndex(), "c6".SquareTextToIndex(), MoveType.EnPassant);

                board = fenTextToBoard.Translate("r1bqkbnr/pp1ppppp/8/n1pP4/1N6/8/PPP1PPPP/R1BQKBNR w KQkq - 1 3");
                yield return new TestCaseData(goodDestValidator, board, move)
                    .SetName(
                        "Validate Move Destination - should return MoveError.BadDestination when pawn is capturing but nothing exists to capture")
                    .SetDescription("")
                    .Returns(MoveError.BadDestination);

                board = fenTextToBoard.Translate("r1bqkbnr/pp1ppppp/2n5/2pP4/1N6/8/PPP1PPPP/R1BQKBNR w KQkq - 1 3");

                yield return new TestCaseData(goodDestValidator, board, move)
                    .SetName(
                        "Validate Pawn Move Destination - should return MoveError.NoneSet when pawn is capturing and opponent has a piece on the dest square")
                    .SetDescription("")
                    .Returns(MoveError.NoneSet);
                yield return new TestCaseData(goodDestValidator, board,
                        MoveHelpers.GenerateMove("h3".SquareTextToIndex(), "h4".SquareTextToIndex()))
                    .SetName("Validate Pawn Move Source - should return error when no piece is at source.")
                    .SetDescription("Validates an error is returned when nothing is on source")
                    .Returns(MoveError.ActivePlayerHasNoPieceOnSourceSquare);
            }

            public static IEnumerable GetBadDestinationTestCases()
            {
                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e4".SquareTextToIndex(), "e2".SquareTextToIndex()))
                    .SetName("Validate Move Destination - Bad Destination")
                    .SetDescription("White pawn moving backwards, from e4 to e2.")
                    .Returns(MoveError.BadDestination);

                yield return new TestCaseData(
                        new Board(),
                        MoveHelpers.GenerateMove("e2".SquareTextToIndex(), "d3".SquareTextToIndex()))
                    .SetName("Validate Move Destination - Bad Destination")
                    .SetDescription("White pawn capturing on empty squares (NW)")
                    .Returns(MoveError.BadDestination);

                yield return new TestCaseData(
                        new Board(),
                        MoveHelpers.GenerateMove("e2".SquareTextToIndex(), "f3".SquareTextToIndex()))
                    .SetName("Validate Move Destination - Bad Destination")
                    .SetDescription("White pawn capturing on empty squares (NE)")
                    .Returns(MoveError.BadDestination);

                yield return new TestCaseData(
                        new Board(),
                        MoveHelpers.GenerateMove("e7".SquareTextToIndex(), "d6".SquareTextToIndex()))
                    .SetName("Validate Move Destination - Bad Destination")
                    .SetDescription("Black pawn capturing on empty squares (SW)")
                    .Returns(MoveError.BadDestination);

                yield return new TestCaseData(
                        new Board(),
                        MoveHelpers.GenerateMove("e7".SquareTextToIndex(), "f6".SquareTextToIndex()))
                    .SetName("Validate Move Destination - Bad Destination")
                    .SetDescription("Black pawn capturing on empty squares (SE)")
                    .Returns(MoveError.BadDestination);

                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pp1ppp1p/3p1p2/8/8/3P1P2/PP1PPP1P/RNBQKBNR w KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e2".SquareTextToIndex(), "d3".SquareTextToIndex()))
                    .SetName("Validate Move Destination - Bad Destination (Friendly Capture)")
                    .SetDescription("White pawn capturing on friendly-occupied squares (NW)")
                    .Returns(MoveError.BadDestination);
                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pp1ppp1p/3p1p2/8/8/3P1P2/PP1PPP1P/RNBQKBNR w KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e2".SquareTextToIndex(), "f3".SquareTextToIndex()))
                    .SetName("Validate Move Destination - Bad Destination (Friendly Capture)")
                    .SetDescription("White pawn capturing on friendly-occupied squares (NE)")
                    .Returns(MoveError.BadDestination);

                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pp1ppp1p/3p1p2/8/8/3P1P2/PP1PPP1P/RNBQKBNR b KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e7".SquareTextToIndex(), "d6".SquareTextToIndex()))
                    .SetName("Validate Move Destination - Bad Destination (Friendly Capture)")
                    .SetDescription("Black pawn capturing on friendly-occupied squares (SW)")
                    .Returns(MoveError.BadDestination);

                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pp1ppp1p/3p1p2/8/8/3P1P2/PP1PPP1P/RNBQKBNR b KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e7".SquareTextToIndex(), "f6".SquareTextToIndex()))
                    .SetName("Validate Move Destination - Bad Destination (Friendly Capture)")
                    .SetDescription("Black pawn capturing on friendly-occupied squares (SE)")
                    .Returns(MoveError.BadDestination);


                foreach (var testCase in GetPawnIllegalDoublePushTestCases().Concat(GetBlockedPawnPushTestCases()))
                    yield return testCase;
            }

            private static IEnumerable<TestCaseData> GetBlockedPawnPushTestCases()
            {
                var noError = "Validate Move Destination - No Error (Pawn Push)";

                //Pawn push blocked (White)
                var validateMoveDestinationBadDestination = "Validate Move Destination - Bad Destination";
                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pppp1ppp/8/8/8/4p3/PPPPPPPP/RNBQKBNR w KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e2".SquareTextToIndex(), "e3".SquareTextToIndex()))
                    .SetName($"{validateMoveDestinationBadDestination} (Pawn Push e2-e3)")
                    .SetDescription("White pawn is blocked from e2-e3 due to Black's pawn on e3")
                    .Returns(MoveError.BadDestination);


                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pppp1ppp/8/8/4p3/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e2".SquareTextToIndex(), "e3".SquareTextToIndex()))
                    .SetName(noError)
                    .SetDescription("White pawn is not blocked from e2-e3 due to Black's pawn being on e4")
                    .Returns(MoveError.NoneSet);

                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pppp1ppp/8/8/4p3/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e2".SquareTextToIndex(), "e4".SquareTextToIndex()))
                    .SetName($"{validateMoveDestinationBadDestination} (Pawn Push e2-e4)")
                    .SetDescription("White pawn is blocked from e2-e4 due to Black's pawn being on e4")
                    .Returns(MoveError.BadDestination);
                //-------------------------------------------------------------------------------------------------
                //-------------------------------------------------------------------------------------------------

                //Pawn push blocked (Black)
                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pppppppp/4P3/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e7".SquareTextToIndex(), "e6".SquareTextToIndex()))
                    .SetName($"{validateMoveDestinationBadDestination} (Pawn Push e7-e6)")
                    .SetDescription("Black pawn is blocked from e7-e6 due to White's pawn on e6")
                    .Returns(MoveError.BadDestination);

                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pppppppp/8/4P3/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e7".SquareTextToIndex(), "e6".SquareTextToIndex()))
                    .SetName(noError)
                    .SetDescription("Black pawn is not blocked from e7-e6 due to White's pawn being on e5")
                    .Returns(MoveError.NoneSet);

                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pppppppp/8/4P3/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e7".SquareTextToIndex(), "e5".SquareTextToIndex()))
                    .SetName($"{validateMoveDestinationBadDestination} (Pawn Push e7-e5)")
                    .SetDescription("Black pawn is blocked from e7-e5 due to White's pawn being on e5")
                    .Returns(MoveError.BadDestination);
                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e4".SquareTextToIndex(), "e5".SquareTextToIndex()))
                    .SetName($"{validateMoveDestinationBadDestination} (Pawn Push e4-e5)")
                    .SetDescription("White's pawn is blocked from pushing.")
                    .Returns(MoveError.BadDestination);
                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e5".SquareTextToIndex(), "e4".SquareTextToIndex()))
                    .SetName($"{validateMoveDestinationBadDestination} (Pawn Push e5-e4)")
                    .SetDescription("Black's pawn is blocked from pushing.")
                    .Returns(MoveError.BadDestination);
            }

            private static IEnumerable<TestCaseData> GetPawnIllegalDoublePushTestCases()
            {
                var description = "{0} pawn cannot move 2 squares after it is not on initial.";
                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pppp1ppp/4p3/8/8/4P3/PPPP1PPP/RNBQKBNR w KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e3".SquareTextToIndex(), "e5".SquareTextToIndex()))
                    .SetName("Validate Move Destination - Bad Destination (Pawn Double Push e3-e5)")
                    .SetDescription(string.Format(description, "White's"))
                    .Returns(MoveError.BadDestination);
                yield return new TestCaseData(
                        fenTextToBoard.Translate("rnbqkbnr/pppp1ppp/4p3/8/8/4P3/PPPP1PPP/RNBQKBNR b KQkq - 0 1"),
                        MoveHelpers.GenerateMove("e6".SquareTextToIndex(), "e4".SquareTextToIndex()))
                    .SetName("Validate Move Destination - Bad Destination (Pawn Double Push e6-e4)")
                    .SetDescription(string.Format(description, "Black's"))
                    .Returns(MoveError.BadDestination);
            }

            #endregion

            #region Validation Non-errors

            [Test]
            public void ShouldReturnNoError_WhenWhitePawnMovesForward1()
            {
                var move = MoveHelpers.GenerateMove(12, 20);
                var actual = pieceCanMoveToDestinationValidator.Validate(_board, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual, "Expected no error. Pawns can move 1 forward.");
            }

            [Test]
            public void ShouldReturnNoError_WhenBlackPawnMovesForward1()
            {
                var board = fenTextToBoard.Translate("rnbqkbnr/pppppppp/8/8/3P4/8/PPP1PPPP/RNBQKBNR b KQkq - 0 1");
                var move = MoveHelpers.GenerateMove("e7".SquareTextToIndex(), "e6".SquareTextToIndex());
                var actual = pieceCanMoveToDestinationValidator.Validate(board, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual, "Expected no error. Pawns can move 1 forward.");
            }

            [Test]
            public void ShouldReturnNoError_WhenWhitePawnMoves2From2ndRank()
            {
                var move = MoveHelpers.GenerateMove(12, 28);
                var actual = pieceCanMoveToDestinationValidator.Validate(_board, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual, "Pawn can move 2 squares from the opening rank.");
            }

            [Test]
            public void ShouldReturnNoError_WhenBlackPawnMoves2From7thRank()
            {
                var board = fenTextToBoard.Translate("rnbqkbnr/pppppppp/8/8/3P4/8/PPP1PPPP/RNBQKBNR b KQkq - 0 1");
                var move = MoveHelpers.GenerateMove("e7".SquareTextToIndex(), "e5".SquareTextToIndex());
                var actual = pieceCanMoveToDestinationValidator.Validate(board, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual, "Pawn can move 2 squares from the opening rank.");
            }

            [Test]
            public void ShouldReturnNoError_WhenWhitePawnAttacksEnemyOccupiedSquare()
            {
                var bi1 = fenTextToBoard.Translate("rnbqkbnr/ppp1p1pp/3P1P2/8/8/3p1p2/PP2PPPP/RNBQKBNR w KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(12, 21);
                var actual = pieceCanMoveToDestinationValidator.Validate(bi1, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual,
                    "Expected no error. Pawns can capture an enemy-occupied square to the NE.");
                move = MoveHelpers.GenerateMove(12, 19);
                actual = pieceCanMoveToDestinationValidator.Validate(bi1, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual,
                    "Expected no error. Pawns can capture an enemy-occupied square to the NW.");
            }

            [Test]
            public void ShouldReturnNoError_WhenBlackPawnAttacksEnemyOccupiedSquare()
            {
                var bi1 = fenTextToBoard.Translate("rnbqkbnr/ppp1p1pp/3P1P2/8/8/3p1p2/PP2PPPP/RNBQKBNR b KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(52, 43);
                var actual = pieceCanMoveToDestinationValidator.Validate(bi1, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual,
                    "Expected no error. Pawns can capture an enemy-occupied square to the SE.");
                move = MoveHelpers.GenerateMove(52, 45);
                actual = pieceCanMoveToDestinationValidator.Validate(bi1, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual,
                    "Expected no error. Pawns can capture an enemy-occupied square to the SW.");
            }

            #endregion
        }
    }
}