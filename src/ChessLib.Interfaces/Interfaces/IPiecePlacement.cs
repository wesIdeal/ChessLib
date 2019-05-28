using ChessLib.Types.Enums;

namespace ChessLib.Types.Interfaces
{
    public interface IPiecePlacement
    {

        ulong[][] GetPiecePlacementArray();
        ulong BlackOccupancy { get; }
        ulong WhiteOccupancy { get; }
        ulong ColorOccupancy(Color c);
        void ApplyMove(in IMoveExt move);
        ulong BlackPiecesOfType(Piece p);
        ulong[][] GetBoardPostMove(in IMoveExt move);
        ulong PieceOfColorOccupancy(Color color, Piece piece);
        ulong WhitePiecesOfType(Piece p);
    }
}