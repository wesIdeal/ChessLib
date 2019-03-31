using ChessLib.Data.Helpers;

namespace MagicBitboard
{

    public class GameInfo
    {

        public readonly BoardInfo BoardInfo;



        public GameInfo() : this(FENHelpers.FENInitial)
        {

        }

        public GameInfo(string fen)
        {
            BoardInfo = BoardInfo.BoardInfoFromFen(fen);
        }
    }
}
