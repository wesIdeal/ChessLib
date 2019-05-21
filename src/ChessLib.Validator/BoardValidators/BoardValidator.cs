using ChessLib.Data;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using System.Collections.Generic;

namespace ChessLib.Validators.BoardValidators
{
    public class BoardValidator
    {
        private readonly IBoard _board;

        readonly List<IBoardRule> _rules = new List<IBoardRule>();


        public BoardValidator(in BoardFENInfo board)
        {
            _board = board;

        }

        public BoardException Validate()
        {
            BoardException rv = BoardException.None;
            foreach (var rule in _rules)
            {
                var moveIssue = rule.Validate(_board);

                rv |= moveIssue;
            }

            return rv;
        }
    }
}

