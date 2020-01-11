using System;
using System.Collections.Generic;
using System.Text;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.Data
{
    public static class GameHelpers
    {
        public static MoveTree SplitFromMoveToEnd(string fen, IEnumerable<MoveStorage> firstMove, bool copyVariations = false)
        {
            var mt = new MoveTree(null, fen);
            foreach (var mv in firstMove)
            {
                mt.AddMove(mv);
            }

            return mt;
        }
    }
}
