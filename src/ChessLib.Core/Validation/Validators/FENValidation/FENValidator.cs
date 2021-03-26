using System.Collections.Generic;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.FENValidation.Rules;

namespace ChessLib.Core.Validation.Validators.FENValidation
{
    public class FENValidator
    {
        private readonly string _fen;
        private readonly List<IFENRule> _rules = new List<IFENRule>();
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
            FENError rv;
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
