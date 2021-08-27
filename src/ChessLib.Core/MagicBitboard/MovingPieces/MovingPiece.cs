using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.MagicBitboard.MovingPieces
{
    internal interface IMovingPiece
    {
        ulong GetPseudoLegalMoves(ushort square, Color playerColor, ulong occupancy);
    }

    internal abstract class MovingPiece : IMovingPiece
    {
        protected IEnumerable<ushort> AllSquares => Enumerable.Range(0, 64).Select(x => (ushort) x);

        protected MovingPiece()
        {
            MovingPieceSvc = new MovingPieceService();
        }

        protected ulong[] AttackMask;
        protected ulong[] MoveMask;
        protected MovingPieceService MovingPieceSvc;

        public abstract ulong GetPseudoLegalMoves(ushort square, Color playerColor, ulong occupancy);
    }
}