using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Validation.Validators.BoardValidation.Rules;

namespace ChessLib.Core.Validation.Validators.BoardValidation
{
    public interface IBoardValidator
    {
        BoardExceptionType Validate(in Board board);
    }

    public class BoardValidator : IBoardValidator
    {
        /// <summary>
        ///     BoardValidator constructor
        /// </summary>
        public BoardValidator() :
            this(new PawnCountRule(), new PieceCountRule(), new EnPassantPositionRule(),
                new OpponentInCheckRule(), new EndOfGameRule())
        {
        }

        internal BoardValidator(params IBoardRule[] boardRules)
        {
            _rules.AddRange(boardRules.Where(x => x != null));
        }

        private readonly List<IBoardRule> _rules = new List<IBoardRule>();

        public BoardExceptionType Validate(in Board board)
        {
            var rv = BoardExceptionType.None;
            foreach (var rule in _rules)
            {
                var moveIssue = rule.Validate(board);
                rv |= moveIssue;
            }

            if (rv.HasFlag(BoardExceptionType.WhiteTooManyPawns) && rv.HasFlag(BoardExceptionType.WhiteTooManyPieces))
            {
                rv ^= BoardExceptionType.WhiteTooManyPieces;
            }

            if (rv.HasFlag(BoardExceptionType.BlackTooManyPawns) && rv.HasFlag(BoardExceptionType.BlackTooManyPieces))
            {
                rv ^= BoardExceptionType.BlackTooManyPieces;
            }

            return rv;
        }
    }
}