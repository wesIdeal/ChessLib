using ChessLib.Types.Enums;
using ChessLib.Validators.FENValidation.Rules;
using System.Collections.Generic;
using ChessLib.Data.Exceptions;

namespace ChessLib.Validators.FENValidation
{
    public class FENValidator
    {
        private readonly string _fen;
        List<IFENRule> _rules = new List<IFENRule>();
        public FENValidator(string fen)
        {
            _fen = fen;
            _rules.Add(new PiecePlacementRule());
            _rules.Add(new ActiveColorRule());
            _rules.Add(new CastlingAvailabilityRule());
            _rules.Add(new EnPassantRule());
            _rules.Add(new HalfmoveClockRule());
            _rules.Add(new FullMoveCountRule());
        }

        public FENError Validate(bool exceptionOnError = true)
        {
            FENError rv = FENError.None;
            if ((rv = (new FENStructureRule()).Validate(_fen)) != FENError.None) return rv;
            _rules.ForEach(rule => { rv |= rule.Validate(_fen); });
            if (exceptionOnError && rv != FENError.None)
            {
                throw new FENException(_fen, rv);
            }
            return rv;
        }
    }
}
