using System.Collections.Generic;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.MoveRules.TestData
{
    internal class MoveDestinationValidatorTestData : MoveValidatorTestData
    {
        internal static readonly ulong PseudoLegalMovesIncludingDestinationSquare =
            "c6".SquareValue() | "d6".SquareValue() | "e6".SquareValue();


        internal static readonly ulong PseudoLegalMovesNotIncludingDestinationSquare = ~"c6".SquareValue();


        public static IEnumerable<TestCaseData> GetMoveDestinationValidatorTestCases()
        {
            const string pseudoTestCaseName = "Validator.Move.MoveDestinationValidator";
            foreach (var testCase in GetPseudoLegalMoveTestCases())
            {
                yield return testCase.SetName(pseudoTestCaseName + ".PresentInMoveList")
                    .SetDescription("Proposed move should be in pseudolegal move list.");
            }

            foreach (var testCase in GetPawnAttackTestCases())
            {
                yield return testCase
                    .SetDescription("Pawn attack-move is not legal if an enemy doesn't occupy square");
            }

            foreach (var testCase in GetNoPieceAtSourceTestCases())
            {
                yield return testCase
                    .SetName(pseudoTestCaseName + " - Move is not valid if piece does not exist at source.");
            }
        }

        public static IEnumerable<TestCaseData> GetNoPieceAtSourceTestCases()
        {
            const string name = "Source Square";
            var source = "h3".ToBoardIndex();
            var destination = "h4".ToBoardIndex();
            var board = new Board();
            // Double-check that test case is valid
            Assert.AreEqual(0, board.Occupancy.Occupancy(board.ActivePlayer) & source.GetBoardValueOfIndex(), 0,
                "Test is not arranged properly. Piece found at source square.");

            yield return new TestCaseData(board, MoveHelpers.GenerateMove(source, destination),
                    PseudoLegalMovesIncludingDestinationSquare)
                .SetName($"{name} - should return error when no piece is at source.")
                .SetDescription("Validates an error is returned when nothing is on source")
                .Returns(MoveError.ActivePlayerHasNoPieceOnSourceSquare);
        }

        public static IEnumerable<TestCaseData> GetPawnAttackTestCases()
        {
            const string name = "Pawn Attacks";
            var d5 = "d5".ToBoardIndex();
            var c6 = "c6".ToBoardIndex();

            //Pseudolegal move, to test that move's destination square's presence in the Pseudolegal Move list
            var pseudoLegalMove =
                MoveHelpers.GenerateMove(d5, c6);
            foreach (var testCaseData in GetNormalPawnAttackTestCases(name)) yield return testCaseData;

            const string enPassantAvailableFen = "rnbqkbnr/1p1ppppp/8/p1pP4/8/8/PPP1PPPP/RNBQKBNR w KQkq c6 0 3";
            var pawnAttackingEnPassantSquareBoard = fenTextToBoard.Translate(enPassantAvailableFen);

            var enPassantCapture = MoveHelpers.GenerateMove(d5, c6, MoveType.EnPassant);
            yield return new TestCaseData(pawnAttackingEnPassantSquareBoard, enPassantCapture,
                    PseudoLegalMovesIncludingDestinationSquare)
                .SetName($"{name} - Pawn attacks en passant square")
                .Returns(MoveError.NoneSet);

            yield return new TestCaseData(pawnAttackingEnPassantSquareBoard, pseudoLegalMove,
                    PseudoLegalMovesIncludingDestinationSquare)
                .SetName($"{name} - Pawn attacks en passant square but move isn't marked correctly")
                .Returns(MoveError.EnPassantNotMarked);
        }

        private static IEnumerable<TestCaseData> GetNormalPawnAttackTestCases(string name)
        {
            const string pawnAttackingEmptySquareFen =
                "r1bqkbnr/pp1ppppp/8/n1pP4/1N6/8/PPP1PPPP/R1BQKBNR w KQkq - 1 3";
            const string pawnAttackingEnemyOccupiedSquareFen =
                "r1b1kbnr/pp1ppppp/2q5/n1pP4/1N6/8/PPP1PPPP/R1BQKBNR w KQkq - 1 3";

            // Pawn attacks empty square on board
            var pawnAttackingEmptySquareBoard = fenTextToBoard.Translate(pawnAttackingEmptySquareFen);

            var d5 = "d5".ToBoardIndex();
            var c6 = "c6".ToBoardIndex();

            //Pseudolegal move, to test that move's destination square's presence in the Pseudolegal Move list
            var pseudoLegalMove =
                MoveHelpers.GenerateMove(d5, c6);
            yield return new TestCaseData(pawnAttackingEmptySquareBoard, pseudoLegalMove,
                    PseudoLegalMovesIncludingDestinationSquare)
                .SetName(
                    $"{name} - Pawn attacks empty square")
                .SetDescription(
                    $"Should return {MoveError.BadDestination} when destination is unoccupied in pawn attack.")
                .Returns(MoveError.BadDestination);


            // A board where a pawn is attacking an enemy-occupied square
            var pawnAttackingOccupiedSquareBoard =
                fenTextToBoard.Translate(pawnAttackingEnemyOccupiedSquareFen);
            yield return new TestCaseData(pawnAttackingOccupiedSquareBoard, pseudoLegalMove,
                    PseudoLegalMovesIncludingDestinationSquare)
                .SetName(
                    $"{name} - Pawn attacks enemy-occupied square")
                .SetDescription(
                    $"Should return {MoveError.NoneSet} when destination is occupied by enemy piece in pawn attack.")
                .Returns(MoveError.NoneSet);
        }

        public static IEnumerable<TestCaseData> GetPseudoLegalMoveTestCases()
        {
            var d5 = "d5".ToBoardIndex();
            var c6 = "c6".ToBoardIndex();
            var pseudoLegalMove =
                MoveHelpers.GenerateMove(d5, c6);
            var pseudoLegalMoveBoardFen = "r1bqkbnr/pp1ppppp/2n5/2pN4/8/8/PPPPPPPP/R1BQKBNR w KQkq - 1 3";
            var pseudoLegalMoveBoard =
                fenTextToBoard.Translate(pseudoLegalMoveBoardFen);
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
}