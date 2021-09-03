using System;
using System.Linq;
using System.Linq.Expressions;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using Moq;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation
{
    internal static class MoveValidatorTestHelpers
    {
        internal static Expression<Func<IBitboard, ulong>> SetupPseudoLegalMovesMock(this Board board, Move move,
            ulong pseudoLegalMovesReturnValue,
            Mock<IBitboard> bitboardMock)
        {
            var setupExpression = MakePseudoLegalMoveSetupExpression(move, board);
            bitboardMock.Setup(setupExpression)
                .Returns(pseudoLegalMovesReturnValue)
                .Verifiable();
            return setupExpression;
        }

        internal static Expression<Func<IBitboard, bool>> SetupPostMoveCheckMock(this Board board, Board postMoveBoard,
            Move move, bool moveLeavesKingInCheck, Mock<IBitboard> bitboardMock)
        {
            var setupExpression = SetupKingInCheckExpression(board, postMoveBoard);
            bitboardMock.Setup(setupExpression)
                .Returns(moveLeavesKingInCheck)
                .Verifiable();
            return setupExpression;
        }

        internal static Mock<IBitboard> SetupPreMoveCheckMock(this Board board, bool isKingInCheck)
        {
            var kingPosition = board.Occupancy.Occupancy(board.ActivePlayer, Piece.King).GetSetBits()[0];
            var attackingColor = board.OpponentColor;
            var occupancyAll = board.Occupancy.Occupancy();
            var enPassantIndex = board.EnPassantIndex;
            var bitboardMock = MakeSquareInCheckMock(kingPosition, attackingColor, occupancyAll, enPassantIndex,
                isKingInCheck);
            return bitboardMock;
        }

        private static Mock<IBitboard> MakeSquareInCheckMock(ushort kingPosition, Color attackingColor,
            ulong totalOccupancy, ushort? enPassantIndex, bool returnValue)
        {
            var bitboardMock = new Mock<IBitboard>();
            bitboardMock.Setup(x =>
                    x.IsSquareAttackedByColor(
                        It.Is<ushort>(c => c == kingPosition),
                        It.Is<Color>(c => c == attackingColor),
                        It.Is<ulong[][]>(c => c.Occupancy(null, null) == totalOccupancy),
                        It.Is<ushort?>(c => c == enPassantIndex)))
                .Returns(returnValue)
                .Verifiable();
            return bitboardMock;
        }

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


        private static Expression<Func<IBitboard, bool>> SetupKingInCheckExpression(Board board, Board postMoveBoard)
        {
            var activeKingIndex =
                postMoveBoard.Occupancy.Occupancy(board.ActivePlayer, Piece.King).GetSetBits().First();
            var attackerColor = board.OpponentColor;
            var postMoveOccupancy = postMoveBoard.Occupancy.Occupancy();
            return mock =>
                mock.IsSquareAttackedByColor(
                    It.Is<ushort>(x => x == activeKingIndex),
                    It.Is<Color>(x => x == attackerColor),
                    It.Is<ulong[][]>(x => x.Occupancy(null, null) == postMoveOccupancy),
                    It.Is<ushort?>(x => x == board.EnPassantIndex));
        }
    }
}