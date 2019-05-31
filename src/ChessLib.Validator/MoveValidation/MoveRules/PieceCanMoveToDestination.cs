
using System.Collections.Generic;
using System.Linq;
using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Types;
using ChessLib.Types.Interfaces;

namespace ChessLib.Validators.MoveValidation.MoveRules
{
    public class PieceCanMoveToDestination : IMoveRule
    {
        public MoveExceptionType? Validate(in IBoard boardInfo, in ulong[][] postMoveBoard, in IMoveExt move)
        {

            var piece = boardInfo.PiecePlacement.GetTypeOfPieceAtIndex(move.SourceIndex);
            if (piece == null)
            {
                return MoveExceptionType.ActivePlayerHasNoPieceOnSourceSquare;
            }

            var moves = new List<MoveExt>();
            var attackedSquares = Bitboard.GetPseudoLegalMoves(piece.Value, move.SourceIndex,
                boardInfo.PiecePlacement.ColorOccupancy(boardInfo.ActivePlayer),
                boardInfo.PiecePlacement.ColorOccupancy(boardInfo.OpponentColor()),
                boardInfo.ActivePlayer, boardInfo.EnPassantSquare, boardInfo.CastlingAvailability, out moves);
            return moves.Contains(move) ? (MoveExceptionType?)null : MoveExceptionType.BadDestination;
        }


    }
}
