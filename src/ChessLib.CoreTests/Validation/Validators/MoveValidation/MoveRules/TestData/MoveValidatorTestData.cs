using System;
using System.Linq.Expressions;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using Moq;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.MoveRules
{
    internal class MoveDestinationValidatorTestParams
    {
        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();




        /// <summary>
        ///     A move from an unoccupied source square
        /// </summary>
        internal static readonly Move EmptySourceMove =
            MoveHelpers.GenerateMove("h3".SquareTextToIndex(), "h4".SquareTextToIndex());

        internal static readonly Board EmptySourceBoard = new Board();




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

    internal class MoveValidatorTestData
    {
        protected static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();
    }
}