using ChessLib.Core.Parse;

namespace ChessLib.Core.Parse
{
    public class FenTextToBoard : ChessDTO<string, Board>
    {
        private static readonly FenToObject FenReader = new FenToObject();

        public override Board Translate(string from)
        {
            var fenObj = FenReader.Translate(from);
            return fenObj.AsBoard;
        }
    }
}