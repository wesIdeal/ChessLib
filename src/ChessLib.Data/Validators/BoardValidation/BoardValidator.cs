using System;
using System.Collections.Generic;
using System.Text;
using ChessLib.Data.Boards;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Types.Interfaces;
using ChessLib.Data.Validators.BoardValidation.Rules;
using EnumsNET;

namespace ChessLib.Data.Validators.BoardValidation
{
    public class BoardValidator
    {
        private readonly IBoard _board;

        readonly List<IBoardRule> _rules = new List<IBoardRule>();
        
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

        public BoardExceptionType Validate(bool throwValidationExceptionOnError)
        {
            BoardExceptionType rv = BoardExceptionType.None;
            foreach (var rule in _rules)
            {
                var moveIssue = rule.Validate(_board);

                rv |= moveIssue;
            }

            if (rv.HasFlag(BoardExceptionType.WhiteTooManyPawns) && rv.HasFlag(BoardExceptionType.WhiteTooManyPieces))
            {
                rv = rv ^ BoardExceptionType.WhiteTooManyPieces;
            }
            if (rv.HasFlag(BoardExceptionType.BlackTooManyPawns) && rv.HasFlag(BoardExceptionType.BlackTooManyPieces))
            {
                rv = rv ^ BoardExceptionType.BlackTooManyPieces;
            }
            if (throwValidationExceptionOnError && rv != BoardExceptionType.None)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var validationIssue in rv.GetFlags())
                {
                    sb.AppendLine($"* {validationIssue.AsString(EnumFormat.Description)}");
                }
                throw BoardException.MakeBoardException(rv);
            }
            return rv;
        }
    }
}

