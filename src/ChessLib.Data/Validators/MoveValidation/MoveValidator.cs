using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using ChessLib.Validators.MoveValidation.CastlingRules;
using ChessLib.Validators.MoveValidation.EnPassantRules;
using ChessLib.Validators.MoveValidation.MoveRules;
using System;
using System.Collections.Generic;

namespace ChessLib.Validators.MoveValidation
{
    public class MoveValidator
    {

        readonly List<IMoveRule> _rules = new List<IMoveRule>();
        public readonly ulong[][] PostMoveBoard;
        private readonly IBoard _board;
        private readonly MoveExt _move;

        public MoveValidator(in IBoard board, in MoveExt move)
        {
            _board = board;
            _move = move;
            PostMoveBoard = BoardHelpers.GetBoardPostMove(board.GetPiecePlacement(), board.ActivePlayer, move);
            _rules.Add(new PieceMovingIsActiveColor());
            _rules.Add(new KingNotInCheckAfterMove());
            switch (move.MoveType)
            {
                case MoveType.Normal:
                    _rules.Add(new PieceCanMoveToDestination());
                    _rules.Add(new DestinationNotOccupiedByActiveColor());
                    break;
                case MoveType.Promotion:
                    _rules.Add(new PieceCanMoveToDestination());
                    _rules.Add(new SourceIsPawn());
                    break;
                case MoveType.EnPassant:
                    _rules.Add(new EnPassantIsPossible());
                    _rules.Add(new SourceIsPawn());
                    _rules.Add(new SourceIsCorrectRank());
                    _rules.Add(new EnPassantSquareIsAttackedBySource());
                    break;
                case MoveType.Castle:
                    _rules.Add(new HasValidDestinationSquare());
                    _rules.Add(new HasCastlingAvailability());
                    _rules.Add(new KingNotInCheckBeforeMove());
                    _rules.Add(new CastlingSquaresNotAttacked());
                    _rules.Add(new CastlingHasNoPiecesBlocking());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public MoveError? Validate()
        {
            foreach (var rule in _rules)
            {
                var moveIssue = rule.Validate(this._board, PostMoveBoard, _move);
                if (moveIssue.HasValue)
                {
                    return moveIssue;
                }
            }

            return null;
        }
    }


}
