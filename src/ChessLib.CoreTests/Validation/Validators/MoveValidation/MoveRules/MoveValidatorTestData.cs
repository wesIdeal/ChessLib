using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    internal class MoveDestinationValidatorTestParams
    {
        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();


        internal static readonly Board pseudoLegalMoveBoard =
            fenTextToBoard.Translate("r1bqkbnr/pp1ppppp/2n5/2pN4/8/8/PPPPPPPP/R1BQKBNR w KQkq - 1 3");

        /// <summary>
        ///     A board where a pawn is attacking an empty square
        /// </summary>
        internal static readonly Board pawnAttackingEmptySquareBoard =
            fenTextToBoard.Translate("r1bqkbnr/pp1ppppp/8/n1pP4/1N6/8/PPP1PPPP/R1BQKBNR w KQkq - 1 3");

        /// <summary>
        ///     A board where a pawn is attacking an enemy-occupied square
        /// </summary>
        internal static readonly Board pawnAttackingOccupiedSquareBoard =
            fenTextToBoard.Translate("r1b1kbnr/pp1ppppp/2q5/n1pP4/1N6/8/PPP1PPPP/R1BQKBNR w KQkq - 1 3");

        internal static readonly Board pawnAttackingEnPassantSquareBoard =
            fenTextToBoard.Translate("rnbqkbnr/1p1ppppp/8/p1pP4/8/8/PPP1PPPP/RNBQKBNR w KQkq c6 0 3");

        /// <summary>
        ///     Pseudolegal move, to test that move's destination square's presence in the Pseudolegal Move list
        /// </summary>
        internal static readonly Move pseudoLegalMove =
            MoveHelpers.GenerateMove("d5".SquareTextToIndex(), "c6".SquareTextToIndex());

        internal static readonly Move enPassantCapture =
            MoveHelpers.GenerateMove("d5".SquareTextToIndex(), "c6".SquareTextToIndex(), MoveType.EnPassant);

        /// <summary>
        ///     A move from an unoccupied source square
        /// </summary>
        internal static readonly Move EmptySourceMove =
            MoveHelpers.GenerateMove("h3".SquareTextToIndex(), "h4".SquareTextToIndex());

        internal static readonly Board EmptySourceBoard = new Board();

        internal static readonly ulong PseudoLegalMovesIncludingDestinationSquare =
            "c6".SquareValue() | "d6".SquareValue() | "e6".SquareValue();

        internal static readonly ulong PseudoLegalMovesNotIncludingDestinationSquare = ~"c6".SquareValue();

        private static Expression<Func<IBitboard, ulong>> MakePseudoLegalMoveSetupExpression(Move move, Board board)
        {
            var occupancy = board.Occupancy.Occupancy();
            var sourcePieceAndColor = board.Occupancy.GetPieceOfColorAtIndex(move.SourceIndex);
            var piece = sourcePieceAndColor?.Piece;
            var color = sourcePieceAndColor?.Color;
            return mock => mock.GetPseudoLegalMoves(It.Is<ushort>(x => x == move.SourceIndex),
                It.Is<Piece>(x => x == piece),
                It.Is<Color>(x => x == color),
                It.Is<ulong>(x => x == occupancy));
        }

        internal static Expression<Func<IBitboard, ulong>> SetupPseudoLegalMovesMock(Board board, Move move,
            ulong pseudoLegalMovesReturnValue,
            Mock<IBitboard> bitboardMock)
        {
            var setupExpression = MakePseudoLegalMoveSetupExpression(move, board);
            bitboardMock.Setup(setupExpression)
                .Returns(pseudoLegalMovesReturnValue)
                .Verifiable();
            return setupExpression;
        }

        public static IEnumerable<TestCaseData> GetNoPieceAtSourceTestCases()
        {
            yield return new TestCaseData(EmptySourceBoard, EmptySourceMove, PseudoLegalMovesIncludingDestinationSquare)
                .SetName("Validate Pawn Move Source - should return error when no piece is at source.")
                .SetDescription("Validates an error is returned when nothing is on source")
                .Returns(MoveError.ActivePlayerHasNoPieceOnSourceSquare);
        }

        public static IEnumerable<TestCaseData> GetPawnAttackTestCases()
        {
            const string name = "Pawn Attacks";
            yield return new TestCaseData(pawnAttackingEmptySquareBoard, pseudoLegalMove,
                    PseudoLegalMovesIncludingDestinationSquare)
                .SetName(
                    $"{name} - Pawn attacks empty square")
                .SetDescription(
                    $"Should return {MoveError.BadDestination} when destination is unoccupied in pawn attack.")
                .Returns(MoveError.BadDestination);

            yield return new TestCaseData(pawnAttackingOccupiedSquareBoard, pseudoLegalMove,
                    PseudoLegalMovesIncludingDestinationSquare)
                .SetName(
                   $"{name} - Pawn attacks enemy-occupied square")
                .SetDescription(
                    $"Should return {MoveError.NoneSet} when destination is occupied by enemy piece in pawn attack.")
                .Returns(MoveError.NoneSet);

            yield return new TestCaseData(pawnAttackingEnPassantSquareBoard, enPassantCapture,
                    PseudoLegalMovesIncludingDestinationSquare)
                .SetName($"{name} - Pawn attacks en passant square")
                .Returns(MoveError.NoneSet);

            yield return new TestCaseData(pawnAttackingEnPassantSquareBoard, pseudoLegalMove,
                    PseudoLegalMovesIncludingDestinationSquare)
                .SetName($"{name} - Pawn attacks en passant square but move isn't marked correctly")
                .Returns(MoveError.EnPassantNotMarked);
        }

        public static IEnumerable<TestCaseData> GetPseudoLegalMoveTestCases()
        {
            yield return new TestCaseData(pseudoLegalMoveBoard, pseudoLegalMove,
                    PseudoLegalMovesIncludingDestinationSquare)
                .SetName("Validate Move Destination - Move Destination is in Pseudolegal Move set")
                .SetDescription("Should call to get pseudolegal moves")
                .Returns(MoveError.NoneSet);

            yield return new TestCaseData(pseudoLegalMoveBoard, pseudoLegalMove,
                    PseudoLegalMovesNotIncludingDestinationSquare)
                .SetName(
                    "Validate Move Destination - Move Destination is not in Pseudolegal Move set")
                .SetDescription("Should call to get pseudolegal moves")
                .Returns(MoveError.BadDestination);
        }
    }



    public class MoveValidatorTestData
    {
        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();
        public static IEnumerable<TestCaseData> GetMoveDestinationValidatorTestCases()
        {
            const string pseudoTestCaseName = "Validator.Move.MoveDestinationValidator";
            foreach (var testCase in MoveDestinationValidatorTestParams.GetPseudoLegalMoveTestCases())
            {
                yield return testCase.SetName(pseudoTestCaseName + ".PresentInMoveList")
                    .SetDescription("Proposed move should be in pseudolegal move list.");
            }

            foreach (var testCase in MoveDestinationValidatorTestParams.GetPawnAttackTestCases())
            {
                yield return testCase
                    
                    .SetDescription("Pawn attack-move is not legal if an enemy doesn't occupy square");
            }

            foreach (var testCase in MoveDestinationValidatorTestParams.GetNoPieceAtSourceTestCases())
            {
                yield return testCase
                    .SetName(pseudoTestCaseName + " - Move is not valid if piece does not exist at source.");
            }

        }

       

        /// <summary>
        /// Get the test cases for testing <see cref="KingNotInCheckAfterMoveValidator"/>
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestCaseData> GetKingNotInCheckAfterMoveValidatorTests()
        {
            const string pseudoTestCaseName = "Validator.Move.KingNotInCheckAfterMoveValidator";
            var board = fenTextToBoard.Translate("4k3/8/8/8/8/8/8/4K3 w - - 0 1");
            var postMoveBoard = fenTextToBoard.Translate("3k4/8/8/8/8/8/8/3K4 w - - 0 1");
            var move = MoveHelpers.GenerateMove("e2".SquareTextToIndex(), "e4".SquareTextToIndex());

            yield return
                new TestCaseData(board, postMoveBoard, move, true)
                .SetName(pseudoTestCaseName + " - In Check")
                .SetDescription("Proposed move leaves King in check.")
                .Returns(MoveError.MoveLeavesKingInCheck);

            yield return
                new TestCaseData(board, postMoveBoard, move, false)
                    .SetName(pseudoTestCaseName + " - Not In Check")
                    .SetDescription("Proposed move does not leave King in check.")
                    .Returns(MoveError.NoneSet);


        }
    }
}