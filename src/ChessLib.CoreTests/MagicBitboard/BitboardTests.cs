using System;
using System.Linq;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Tests.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;
using NUnit.Framework;

namespace ChessLib.Core.Tests.MagicBitboard
{
    [TestFixture(Category = "Bitboard")]
    public class BitboardTests
    {
        [TestCaseSource(typeof(MagicBitboardTestData),
            nameof(MagicBitboardTestData.GetPiecesAttackingSquareTestCases_SpecificColor))]
        [TestCaseSource(typeof(MagicBitboardTestData),
            nameof(MagicBitboardTestData.GetPiecesAttackingSquareTestCases_AllColors))]
        public void PiecesAttackingSquareTest(TestCase<ulong, Board> testCase)
        {
            var square = (ushort)testCase.AdditionalInputs[0];
            var attackerColor = (Color?)testCase.AdditionalInputs[1];
            var actual =
                Bitboard.Instance.PiecesAttackingSquareByColor(testCase.TestMethodInputValue.Occupancy,
                    square, attackerColor);
            Assert.AreEqual(testCase.ExpectedValue, actual);
        }


        [TestCaseSource(typeof(MagicBitboardTestData), nameof(MagicBitboardTestData.GetPseudoLegalMoveTestCases))]
        public void GetPseudoLegalMoves_ShouldReturnArrayOfMoves(Board board, Move[] expectedMoves,
            ushort fromSquareIndex)
        {
            var legalMovesCollection =
                Bitboard.Instance.GetLegalMoves(fromSquareIndex, board.Occupancy, board.EnPassantIndex, board.CastlingAvailability).ToArray();
             Assert.NotNull(legalMovesCollection);
            foreach (var move in expectedMoves)
            {
                var movesCollection = (IMove[])legalMovesCollection;
                Assert.Contains(move, movesCollection.ToList());
            }
        }

        [Test(TestOf = typeof(Bitboard), Description = "Test that an empty array is returned when no piece at square.")]
        public void GetLegal_IfNoPieceAtIndex_ReturnsEmptyArray()
        {
            var fenToBoard = new FenTextToBoard();
            var board = fenToBoard.Translate("2kr1b1r/pppqpppp/3pb3/3N4/2P1P3/6P1/PPQBPP1P/R3KB1R w KQ - 1 12");

            var legalMoves = Bitboard.Instance.GetLegalMoves(34, board.Occupancy, null,
                CastlingAvailability.WhiteKingside | CastlingAvailability.WhiteQueenside);
            Assert.IsEmpty(legalMoves);
        }

    }
}