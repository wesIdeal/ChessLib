using ChessLib.Data.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data;
using System.Linq;
using ChessLib.Data.Exceptions;
using ChessLib.Validators.MoveValidation;

namespace ChessLib.Validators.BoardValidators
{
    interface IBoardRule
    {
        BoardException Validate(in IBoard boardInfo);
    }

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

    public class EndOfGameRule : IBoardRule
    {
        public BoardException Validate(in IBoard boardInfo)
        {
            var rv = BoardException.None;

            if (boardInfo.IsCheckmateForColor(boardInfo.ActivePlayer))
            {
                return BoardException.Checkmate;
            }

            if (boardInfo.IsStalemate())
            {
                return BoardException.Stalemate;
            }

            return BoardException.None;
        }





    }
    public class PawnCountRule : IBoardRule
    {

        public BoardException Validate(in IBoard boardInfo)
        {
            var pawn = BoardHelpers.PAWN;
            var white = BoardHelpers.WHITE;
            var black = BoardHelpers.BLACK;
            var rv = BoardException.None;
            rv |= boardInfo.PiecePlacement[white][pawn].CountSetBits() > 8 ? BoardException.WhiteTooManyPawns : BoardException.None;
            rv |= boardInfo.PiecePlacement[black][pawn].CountSetBits() > 8 ? BoardException.BlackTooManyPawns : BoardException.None;
            return rv;
        }

    }
}

