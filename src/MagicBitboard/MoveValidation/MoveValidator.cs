using ChessLib.Data.MoveRepresentation;
using ChessLib.MagicBitboard.MoveValidation.MoveRules;
using MagicBitboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.MagicBitboard.MoveValidation
{
    public class MoveValidator
    {
        List<IMoveRule> _rules = new List<IMoveRule>();
        public ulong[][] PostMoveBoard { get; }
        private readonly BoardInfo _boardInfo;
        private readonly MoveExt _move;
        public MoveValidator(in BoardInfo board, in MoveExt move)
        {
            PostMoveBoard = board.GetBoardPostMove(move);
            _rules.Add(new KingInCheckAfterMove());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            var results = new List<ValidationResult>();
            foreach (var rule in _rules)
            {
                var vr = rule.Validate(_boardInfo, PostMoveBoard, _move);
                if (vr.Severity != ValidationSeverity.None)
                {
                    results.Add(vr);
                    if (vr.Severity == ValidationSeverity.Error)
                    {
                        break;
                    }
                }
            }
            return results;
        }
    }
}
