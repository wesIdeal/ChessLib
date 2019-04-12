using ChessLib.Data.Exceptions;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using ChessLib.MagicBitboard.MoveValidation.CastlingRules;
using ChessLib.MagicBitboard.MoveValidation.MoveRules;
using MagicBitboard;
using System;
using System.Collections.Generic;
using ChessLib.Data.Helpers;
using ChessLib.MagicBitboard.MoveValidation.EnPassantRules;

namespace ChessLib.MagicBitboard.MoveValidation
{
    public class MoveValidator
    {
        readonly List<IMoveRule> _rules = new List<IMoveRule>();
        public readonly ulong[][] PostMoveBoard;
        private readonly BoardInfo _boardInfo;
        private readonly MoveExt _move;
        public MoveValidator(in BoardInfo board, in MoveExt move)
        {
            PostMoveBoard = BoardHelpers.GetBoardPostMove(board.PiecesOnBoard, board.ActivePlayerColor, move);
            _boardInfo = board;
            _move = move;
            _rules.Add(new PieceMovingIsActiveColor());
            _rules.Add(new KingNotInCheckAfterMove());
            if (move.MoveType == MoveType.Normal)
            {
                _rules.Add(new PieceCanMoveToDestination());
                _rules.Add(new DestinationNotOccupiedByActiveColor());
            }
            else if (move.MoveType == MoveType.Promotion)
            {
                _rules.Add(new PieceCanMoveToDestination());
                _rules.Add(new SourceIsPawn());
            }
            else if (move.MoveType == MoveType.EnPassant)
            {
                _rules.Add(new EnPassantIsPossible());
                _rules.Add(new SourceIsPawn());
                _rules.Add(new SourceIsCorrectRank());
                _rules.Add(new EnPassantSquareIsAttackedBySource());
            }
            else if (move.MoveType == MoveType.Castle)
            {
                _rules.Add(new HasValidDestinationSquare());
                _rules.Add(new HasCastlingAvailability());
                _rules.Add(new KingNotInCheckBeforeMove());
                _rules.Add(new CastlingSquaresNotAttacked());
                _rules.Add(new CastlingHasNoPiecesBlocking());
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public MoveExceptionType? Validate()
        {
            foreach (var rule in _rules)
            {
                var moveIssue = rule.Validate(_boardInfo, PostMoveBoard, _move);
                if (moveIssue.HasValue)
                {
                    return moveIssue;
                }
            }

            return null;
        }
    }


}
