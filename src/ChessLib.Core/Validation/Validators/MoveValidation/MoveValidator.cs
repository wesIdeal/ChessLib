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
using ChessLib.Core.Validation.Validators.MoveValidation.PromotionRules;

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
                    _rules.Add(new SourceIsPawnValidator());
                    break;
                case MoveType.EnPassant:
                    _rules.Add(new MoveDestinationValidator());
                    _rules.Add(new EnPassantDestinationValidator());
                    break;
                case MoveType.Castle:
                    _rules.Add(new KingDestinationValidator());
                    _rules.Add(new CastlingMoveIsAvailableValidator());
                    _rules.Add(new NotInCheckBeforeMoveValidator());
                    _rules.Add(new AttackNotBlockingMoveValidator());
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