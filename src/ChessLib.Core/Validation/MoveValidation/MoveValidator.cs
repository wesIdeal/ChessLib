using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.MoveValidation.CastlingRules;
using ChessLib.Core.Validation.MoveValidation.EnPassantRules;
using ChessLib.Core.Validation.MoveValidation.MoveRules;
using ChessLib.Core.Validation.MoveValidation.PromotionRules;

namespace ChessLib.Core.Validation.MoveValidation
{
    public class MoveValidator
    {
        public MoveValidator()
        {

        }

        internal virtual List<IMoveRule> rules
        {
            get;
            set;
        }

        private static readonly NotInCheckAfterMoveValidator notInCheckAfterMoveValidator =
            new NotInCheckAfterMoveValidator();

        internal virtual IEnumerable<IMoveRule> CompileRules(MoveType moveType)
        {
            switch (moveType)
            {
                case MoveType.Normal:
                    break;

                case MoveType.EnPassant:
                    yield return new EnPassantDestinationValidator();
                    break;
                case MoveType.Promotion:
                    yield return new SourceIsPawnValidator();
                    break;
                case MoveType.Castle:
                    yield return new CastlingMoveIsAvailableValidator();
                    yield return new KingDestinationValidator();
                    yield return new NotInCheckBeforeMoveValidator();
                    yield return new AttackNotBlockingMoveValidator();
                    yield return new NoPieceBlocksCastlingMoveValidator();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null);
            }

            yield return new ActiveColorValidator();
            yield return new MoveDestinationValidator();
            yield return notInCheckAfterMoveValidator;
        }


        public MoveError Validate(Board board, Move move, out ulong[][] postMoveBoard)
        {
            rules = CompileRules(move.MoveType).ToList();
            foreach (var rule in rules)
            {
                var moveIssue = rule.Validate(board, move);
                if (moveIssue != MoveError.NoneSet)
                {
                    postMoveBoard = null;
                    return moveIssue;
                }
            }

            postMoveBoard = notInCheckAfterMoveValidator.PostMoveBoard;
            return MoveError.NoneSet;
        }
    }
}