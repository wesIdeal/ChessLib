using System.Collections.Generic;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.MoveValidation.MoveRules.TestData
{
    internal class MoveDestinationValidatorTestData
    {
        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();

        internal static readonly ulong PseudoLegalMovesIncludingDestinationSquare =
            "c6".ToBoardValue() | "d6".ToBoardValue() | "e6".ToBoardValue();


        internal static readonly ulong PseudoLegalMovesNotIncludingDestinationSquare = ~"c6".ToBoardValue();

        private static readonly ushort d5 = "d5".ToBoardIndex();
        private static readonly ushort c6 = "c6".ToBoardIndex();

        /// <summary>
        /// Pseudolegal move, to test that move's destination square's presence in the Pseudolegal Move list
        /// </summary>
        private static readonly Move pseudoLegalMove =
            MoveHelpers.GenerateMove(d5, c6);

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

            foreach (var testCase in GetFriendlyPieceOccupyingDestinationTestCases())
            {
                yield return testCase;
            }
        }

        private static IEnumerable<TestCaseData> GetFriendlyPieceOccupyingDestinationTestCases()
        {
            const string name = "Destination Occupation";
            const string whiteOccupiesDestination = "rnbqkbnr/ppp2ppp/8/3pp3/3P4/4P3/PPP2PPP/RNBQKBNR w KQkq e6 0 3";
            const string blackOccupiesDestination = "r1bqk1nr/pppp1ppp/2n5/3Np3/1bP5/P7/1P1PPPPP/R1BQKBNR b KQkq - 0 4";
            var boardWhite = fenTextToBoard.Translate(whiteOccupiesDestination);
            var boardBlack = fenTextToBoard.Translate(blackOccupiesDestination);
            var d4 = "d4".ToBoardIndex();
            var b4 = "b4".ToBoardIndex();
            var whiteMove = MoveHelpers.GenerateMove("e3".ToBoardIndex(), d4);
            var blackMove = MoveHelpers.GenerateMove("c6".ToBoardIndex(), b4);
            yield return new TestCaseData(boardWhite, whiteMove, d4.GetBoardValueOfIndex())
                .SetName($"{name} - Active color occupies destination")
                .SetDescription("Check that active color does not occupy destination(White, Pe3 x Pe4)")
                .Returns(MoveError.ActiveColorPieceAtDestination);

            yield return new TestCaseData(boardBlack, blackMove, b4.GetBoardValueOfIndex())
                .SetName($"{name} - Active color occupies destination (Black, Nb6 x Bb4")
                .SetDescription("Check that active color does not occupy destination")
                .Returns(MoveError.ActiveColorPieceAtDestination);
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


            foreach (var testCaseData in GetNormalPawnAttackTestCases(name))
            {
                yield return testCaseData;
            }

            foreach (var testCaseData1 in GetEnPassantTestCases(name))
            {
                yield return testCaseData1;
            }
        }

        private static IEnumerable<TestCaseData> GetEnPassantTestCases(string name)
        {
            const string enPassantAvailableFen = "rnbqkbnr/1p1ppppp/8/p1pP4/8/8/PPP1PPPP/RNBQKBNR w KQkq c6 0 3";
            var pawnAttackingEnPassantSquareBoard = fenTextToBoard.Translate(enPassantAvailableFen);

            var enPassantCapture = MoveHelpers.GenerateMove(d5, c6, MoveType.EnPassant);

            var occupancy = PseudoLegalMovesIncludingDestinationSquare;
            yield return new TestCaseData(pawnAttackingEnPassantSquareBoard, enPassantCapture,
                    occupancy)
                .SetName($"{name} - Pawn attacks en passant square")
                .Returns(MoveError.NoneSet);


            yield return new TestCaseData(pawnAttackingEnPassantSquareBoard, pseudoLegalMove,
                    occupancy)
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


            //Pseudolegal move, to test that move's destination square's presence in the Pseudolegal Move list

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