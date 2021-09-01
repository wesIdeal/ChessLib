using System;
using System.Collections.Generic;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Interfaces;
using ChessLib.Core.Validation.Validators.MoveValidation.CastlingRules;
using ChessLib.Core.Validation.Validators.MoveValidation.EnPassantRules;
using ChessLib.Core.Validation.Validators.MoveValidation.MoveRules;

namespace ChessLib.Core.Validation.Validators.MoveValidation
{
    public class MoveValidator
    {
        public MoveValidator(in Board board, in IMove move)
        {
            _board = board;
            _move = move;
            //PostMoveBoard = board.GetPiecePlacement().GetBoardPostMove(board.ActivePlayer, move);
            PostMoveBoard = BoardHelpers.GetBoardPostMove(_board, move);
            _rules.Add(new ActiveColorValidator());
            _rules.Add(new NotInCheckAfterMoveValidator());
            switch (move.MoveType)
            {
                case MoveType.Normal:
                    _rules.Add(new MoveDestinationValidator());
                    break;
                case MoveType.Promotion:
                    _rules.Add(new MoveDestinationValidator());
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
                    _rules.Add(new NotInCheckBeforeMoveValidator());
                    _rules.Add(new CastlingSquaresNotAttacked());
                    _rules.Add(new NoPieceBlocksCastlingMoveValidator());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private readonly Board _board;
        private readonly IMove _move;

        private readonly List<IMoveRule> _rules = new List<IMoveRule>();
        public readonly ulong[][] PostMoveBoard;

        public MoveError Validate()
        {
            foreach (var rule in _rules)
            {
                var moveIssue = rule.Validate(_board, PostMoveBoard, _move);
                if (moveIssue != MoveError.NoneSet)
                {
                    return moveIssue;
                }
            }

            return MoveError.NoneSet;
        }
    }
}