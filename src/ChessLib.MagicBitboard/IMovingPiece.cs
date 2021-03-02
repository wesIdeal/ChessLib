using ChessLib.Data.Types.Enums;

namespace ChessLib.MagicBitboard
{
    internal interface IMovingPiece
    {
        ulong GetPseudoLegalMoves(ushort square, Color playerColor, ulong occupancy);
        void Initialize();
    }
}