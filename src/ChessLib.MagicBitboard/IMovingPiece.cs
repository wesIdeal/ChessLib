using ChessLib.Data.Types.Enums;

namespace ChessLib.MagicBitboard
{
    internal interface IMovingPiece
    {
        ulong GetPsuedoLegalMoves(ushort square, Color playerColor, ulong playerOccupancy, ulong opponentOccupancy);
        void Initialize();
    }
}