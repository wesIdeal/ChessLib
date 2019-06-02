using System;
using ChessLib.Data;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using System.Collections.Generic;
using System.Text;
using ChessLib.Validators.BoardValidators.Rules;
using EnumsNET;

namespace ChessLib.Validators.BoardValidators
{
    public class BoardValidator
    {
        private readonly IBoard _board;

        readonly List<IBoardRule> _rules = new List<IBoardRule>();
        private readonly bool _throwExc;

        /// <summary>
        /// BoardValidator constructor
        /// </summary>
        /// <param name="board"></param>
        
        public BoardValidator(in IBoard board)
        {
            _board = board;
            _rules.Add(new PawnCountRule());
            _rules.Add(new PieceCountRule());
            _rules.Add(new EnPassantSquareRule());
            _rules.Add(new OpponentInCheckRule());
            _rules.Add(new EndOfGameRule());
        }

        public BoardException Validate(bool throwValidationExceptionOnError)
        {
            BoardException rv = BoardException.None;
            foreach (var rule in _rules)
            {
                var moveIssue = rule.Validate(_board);

                rv |= moveIssue;
            }
            if(throwValidationExceptionOnError)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var validationIssue in rv.GetFlags())
                {
                    sb.AppendLine($"* {validationIssue.AsString(EnumFormat.Description)}");
                }
                throw new Exception(sb.ToString());
            }
            return rv;
        }
    }
}

