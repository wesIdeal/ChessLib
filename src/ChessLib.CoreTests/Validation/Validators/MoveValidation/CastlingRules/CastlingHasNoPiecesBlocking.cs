using System.Collections;
using System.Diagnostics.CodeAnalysis;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.CastlingRules
{
    /// <summary>
    ///     Tests for castling through occupied and non-occupied squares between castling King and Rook
    /// </summary>
    [TestFixture(Description =
        "Tests for castling through occupied and non-occupied squares between castling King and Rook")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    internal class
        CastlingHasNoPiecesBlocking : Core.Validation.Validators.MoveValidation.CastlingRules.
            CastlingHasNoPiecesBlocking
    {
        public static IEnumerable GetCastlingTestCases()
        {
            yield return new TestCaseData("r1bqk2r/pppp1ppp/2n2n2/4p3/1bP5/2N2NP1/PP1PPPBP/R1BQK2R b KQkq - 2 5",
                    MoveHelpers.BlackCastleKingSide)
                .SetName("Castling Validation")
                .SetDescription("No error set Black O-O")
                .Returns(MoveError.NoneSet);

            yield return new TestCaseData("r1bq1rk1/pppp1ppp/2n2n2/4p3/1bP5/2N2NP1/PP1PPPBP/R1BQK2R w KQ - 3 6",
                    MoveHelpers.WhiteCastleKingSide)
                .SetName("Castling Validation")
                .SetDescription("No error set White O-O")
                .Returns(MoveError.NoneSet);

            yield return new TestCaseData("r3k2r/ppp1qppp/2npbn2/4p3/2P5/2P2NP1/PBQPPPBP/R3K2R w KQkq - 4 9",
                    MoveHelpers.WhiteCastleQueenSide)
                .SetName("Castling Validation")
                .SetDescription("No error set White O-O-O")
                .Returns(MoveError.NoneSet);

            yield return new TestCaseData("r3k2r/ppp1qppp/2npbn2/4p3/2P5/2P2NP1/PBQPPPBP/2KR3R b kq - 5 9",
                    MoveHelpers.BlackCastleQueenSide)
                .SetName("Castling Validation")
                .SetDescription("No error set Black O-O-O")
                .Returns(MoveError.NoneSet);


            yield return new TestCaseData("r1bq1rk1/pppp1ppp/2n2n2/4p3/1bP5/2N2NP1/PP1PPPBP/R1BQK2R w KQ - 3 6",
                    MoveHelpers.GetRookMoveForCastleMove(MoveHelpers.WhiteCastleKingSide))
                .SetName("Castling Validation - Bad Destination")
                .SetDescription("Bad Destination set - White")
                .Returns(MoveError.CastleBadDestinationSquare);

            yield return new TestCaseData("r3k2r/ppp1qppp/2npbn2/4p3/2P5/2P2NP1/PBQPPPBP/R3K2R w KQkq - 4 9",
                    MoveHelpers.GetRookMoveForCastleMove(MoveHelpers.WhiteCastleQueenSide))
                .SetName("Castling Validation - Bad Destination")
                .SetDescription("Bad Destination set - White")
                .Returns(MoveError.CastleBadDestinationSquare);

            yield return new TestCaseData("r3k2r/ppp1qppp/2npbn2/4p3/2P5/2P2NP1/PBQPPPBP/2KR3R b kq - 5 9",
                    MoveHelpers.GetRookMoveForCastleMove(MoveHelpers.BlackCastleQueenSide))
                .SetName("Castling Validation - Bad Destination")
                .SetDescription("Bad Destination set - Black")
                .Returns(MoveError.CastleBadDestinationSquare);

            yield return new TestCaseData("r1bqk2r/pppp1ppp/2n2n2/4p3/1bP5/2N2NP1/PP1PPPBP/R1BQK2R b KQkq - 2 5",
                    MoveHelpers.GetRookMoveForCastleMove(MoveHelpers.BlackCastleKingSide))
                .SetName("Castling Validation - Bad Destination")
                .SetDescription("Bad Destination set - Black")
                .Returns(MoveError.CastleBadDestinationSquare);


            yield return new TestCaseData("r1bqk2r/pppp1ppp/2n2n2/4p3/1bP5/1BN2NP1/PPQPPP1P/R3KB1R w KQkq - 1 5",
                    MoveHelpers.WhiteCastleKingSide)
                .SetName("Castling Validation")
                .SetDescription("Pieces in between R + K - White O-O")
                .Returns(MoveError.CastleOccupancyBetween);

            yield return new TestCaseData("r2qk2r/ppp2ppp/2npbn2/4p3/2P5/2P2NP1/PB1PPPBP/R2QK2R w KQkq - 2 8",
                    MoveHelpers.WhiteCastleQueenSide)
                .SetName("Castling Validation - Pieces Between")
                .SetDescription("Pieces in between R + K - White O-O-O")
                .Returns(MoveError.CastleOccupancyBetween);

            yield return new TestCaseData("r1bqkb1r/pppp1ppp/2n2n2/4p3/2P5/2N2NP1/PP1PPP1P/R1BQKB1R b KQkq - 0 4",
                    MoveHelpers.BlackCastleKingSide)
                .SetName("Castling Validation - Pieces Between")
                .SetDescription("Pieces in between R + K - Black O-O")
                .Returns(MoveError.CastleOccupancyBetween);

            yield return new TestCaseData("r2qk2r/ppp2ppp/2npbn2/4p3/2P5/2P2NP1/PBQPPPBP/R3K2R b KQkq - 3 8",
                    MoveHelpers.BlackCastleQueenSide)
                .SetName("Castling Validation - Pieces Between")
                .SetDescription("Pieces in between R + K - Black O-O-O")
                .Returns(MoveError.CastleOccupancyBetween);

        }

        private readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();

        [TestCaseSource(nameof(GetCastlingTestCases))]
        public MoveError TestWhenPiecesBlockCastling(string fen, Move move)
        {
            var preMoveBoard = fenTextToBoard.Translate(fen);
            var postMoveBoard = preMoveBoard.ApplyMoveToBoard(move);
            var validation = Validate(preMoveBoard, postMoveBoard.Occupancy, move);
            return validation;
        }
    }
}