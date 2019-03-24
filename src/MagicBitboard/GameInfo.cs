using ChessLib.Data.Helpers;

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
            BoardInfo = BoardInfo.BoardInfoFromFen(fen);
        }

        private string _fen;


    }
}
