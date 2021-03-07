#region

using System.Collections.Generic;
using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Exceptions;
using NUnit.Framework;

#endregion

namespace ChessLib.Data.Tests.Validators.MoveValidation.CastlingRules
{
    /// <summary>
    ///     Tests for castling through occupied and non-occupied squares between castling King and Rook
    /// </summary>
    [TestFixture(Description =
        "Tests for castling through occupied and non-occupied squares between castling King and Rook")]
    internal class
        CastlingHasNoPiecesBlocking : Data.Validators.MoveValidation.CastlingRules.CastlingHasNoPiecesBlocking
    {
        public static IEnumerable<CastlingTestCase> GetCastlingTestCases()
        {
            yield return new CastlingTestCase("r1bqk2r/pppp1ppp/2n2n2/4p3/1bP5/2N2NP1/PP1PPPBP/R1BQK2R b KQkq - 2 5",
                MoveError.NoneSet, MoveHelpers.BlackCastleKingSide);
            yield return new CastlingTestCase("r1bq1rk1/pppp1ppp/2n2n2/4p3/1bP5/2N2NP1/PP1PPPBP/R1BQK2R w KQ - 3 6",
                MoveError.NoneSet, MoveHelpers.WhiteCastleKingSide);
            yield return new CastlingTestCase("r3k2r/ppp1qppp/2npbn2/4p3/2P5/2P2NP1/PBQPPPBP/R3K2R w KQkq - 4 9",
                MoveError.NoneSet, MoveHelpers.WhiteCastleQueenSide);
            yield return new CastlingTestCase("r3k2r/ppp1qppp/2npbn2/4p3/2P5/2P2NP1/PBQPPPBP/2KR3R b kq - 5 9",
                MoveError.NoneSet, MoveHelpers.BlackCastleQueenSide);

            yield return new CastlingTestCase("r1bqk2r/pppp1ppp/2n2n2/4p3/1bP5/2N2NP1/PP1PPPBP/R1BQK2R b KQkq - 2 5",
                MoveError.CastleBadDestinationSquare,
                MoveHelpers.GetRookMoveForCastleMove(MoveHelpers.BlackCastleKingSide));
            yield return new CastlingTestCase("r1bq1rk1/pppp1ppp/2n2n2/4p3/1bP5/2N2NP1/PP1PPPBP/R1BQK2R w KQ - 3 6",
                MoveError.CastleBadDestinationSquare,
                MoveHelpers.GetRookMoveForCastleMove(MoveHelpers.WhiteCastleKingSide));
            yield return new CastlingTestCase("r3k2r/ppp1qppp/2npbn2/4p3/2P5/2P2NP1/PBQPPPBP/R3K2R w KQkq - 4 9",
                MoveError.CastleBadDestinationSquare,
                MoveHelpers.GetRookMoveForCastleMove(MoveHelpers.WhiteCastleQueenSide));
            yield return new CastlingTestCase("r3k2r/ppp1qppp/2npbn2/4p3/2P5/2P2NP1/PBQPPPBP/2KR3R b kq - 5 9",
                MoveError.CastleBadDestinationSquare,
                MoveHelpers.GetRookMoveForCastleMove(MoveHelpers.BlackCastleQueenSide));

            yield return new CastlingTestCase("r2qk2r/ppp2ppp/2npbn2/4p3/2P5/2P2NP1/PBQPPPBP/R3K2R b KQkq - 3 8",
                MoveError.CastleOccupancyBetween, MoveHelpers.BlackCastleQueenSide);
            yield return new CastlingTestCase("r1bqkb1r/pppp1ppp/2n2n2/4p3/2P5/2N2NP1/PP1PPP1P/R1BQKB1R b KQkq - 0 4",
                MoveError.CastleOccupancyBetween, MoveHelpers.BlackCastleKingSide);
            yield return new CastlingTestCase("r2qk2r/ppp2ppp/2npbn2/4p3/2P5/2P2NP1/PB1PPPBP/R2QK2R w KQkq - 2 8",
                MoveError.CastleOccupancyBetween, MoveHelpers.WhiteCastleQueenSide);
            yield return new CastlingTestCase("r1bqk2r/pppp1ppp/2n2n2/4p3/1bP5/1BN2NP1/PPQPPP1P/R3KB1R w KQkq - 1 5",
                MoveError.CastleOccupancyBetween, MoveHelpers.WhiteCastleKingSide);
        }

        [TestCaseSource(nameof(GetCastlingTestCases))]
        public void TestWhenPiecesBlockCastling(CastlingTestCase testCase)
        {
            var preMoveBoard = new Board(testCase.Fen);
            var postMoveBoard = preMoveBoard.ApplyMoveToBoard(testCase.CastlingMove);
            var validation = Validate(preMoveBoard, postMoveBoard.Occupancy, testCase.CastlingMove);
            Assert.AreEqual(testCase.ExpectedError, validation, testCase.ToString());
        }
    }
}