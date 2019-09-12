using System.Collections.Generic;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;

namespace ChessLib.Parse.PGN
{
    /// <summary>
    /// Used to contain a PGN string, stored with index, for predictable ordering
    /// </summary>
    internal class PGNGroup
    {
        public PGNGroup(int index, string pgnData)
        {
            Index = index;
            PGNData = pgnData;
        }

        public int Index { get; set; }
        public string PGNData { get; set; }
        public List<Game<MoveStorage>> Games { get; set; }
    }
}