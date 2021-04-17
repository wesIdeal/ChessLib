using System.Collections.Generic;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.FENValidation.Rules;

namespace ChessLib.Core.Validation.Validators.FENValidation
{
    public class FENValidator
    {
        private readonly string _fen;
        private readonly List<IFENRule> _rules = new List<IFENRule>();

        public FENValidator()
            : this(new PiecePlacementRule(),
                new ActiveColorRule(),
                new CastlingAvailabilityRule(),
                new EnPassantRule(),
                new HalfmoveClockRule(),
                new FullMoveCountRule())
        {
            _rules.Insert(0, new FENStructureRule());
        }

        internal FENValidator(params IFENRule[] fenRules)
        {
            _rules.AddRange(fenRules);
        }
        public FENError Validate(string fen)
        {
            fen = FENHelpers.SanitizeFenString(fen);
            foreach (var rule in _rules)
            {
                var rv = rule.Validate(fen);
                if (rv != FENError.None)
                {
                    throw new FENException(_fen, rv);
                }
            }
            return FENError.None;
        }
    }
}
