using System;
using System.Linq.Expressions;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using Moq;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.MoveRules.TestData
{
    internal class MoveDestinationValidatorTestParams
    {
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
    }

   
}