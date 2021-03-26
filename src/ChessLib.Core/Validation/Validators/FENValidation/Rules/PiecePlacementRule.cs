using System;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Validation.Validators.FENValidation.Rules
{
    public class PiecePlacementRule : IFENRule
    {
        private static readonly char[] ValidFENChars = { '/', 'p', 'P', 'n', 'N', 'b', 'B', 'r', 'R', 'q', 'Q', 'k', 'K', '1', '2', '3', '4', '5', '6', '7', '8' };
        private string _piecePlacement = "";
        public FENError Validate(in string fen)
        {
            _piecePlacement = fen.GetFENPiece(FENPieces.PiecePlacement);
            FENError fenError = FENError.None;
            fenError |= ValidatePiecePlacementRanks(_piecePlacement);
            fenError |= ValidatePiecePlacementCharacters(_piecePlacement);
            return fenError;
        }

        private static int GetStringRepForRank(string rank)
        {
            var rv = 0;
            foreach (var c in rank)
            {
                if (char.IsDigit(c))
                {
                    rv += UInt16.Parse(c.ToString());
                }
                else rv++;
            }
            return rv;
        }

        private static FENError ValidatePiecePlacementRanks(string piecePlacement)
        {
            FENError fenError = FENError.None;
            var ranks = piecePlacement.Split('/').Reverse().ToArray();
            if (ranks.Length != 8)
            {
                fenError |= FENError.PiecePlacementRankCount;
            }
            var ranksValidation = ranks.Select((r, idx) => new { Rank = idx + 1, Count = GetStringRepForRank(r) });
            var badRanks = ranksValidation.Where(x => x.Count != 8);

            if (badRanks.Any())
            {
                fenError |= FENError.PiecePlacementPieceCountInRank;
            }
            return fenError;
        }

        private static FENError ValidatePiecePlacementCharacters(string piecePlacement)
        {
            if ((piecePlacement.Select(x => x).Where(x => !FENHelpers.ValidFENChars.Contains(x)).Select(x => x.ToString()).ToArray()).Any())
            {
                return FENError.PiecePlacementInvalidChars;
            }
            return FENError.None;
        }
    }
}

