using System;
using System.Collections;
using MagicBitboard.Helpers;

namespace MagicBitboard
{

    public class GameInfo
    {

        public readonly BoardInfo BoardInfo;



        public GameInfo() : this(FENHelpers.InitialFEN)
        {

        }

        public GameInfo(string fen)
        {
            _fen = fen;
            BoardInfo = FENHelpers.BoardInfoFromFen(fen);
        }

        private string _fen;

        
    }
}
