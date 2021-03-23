using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.MagicBitboard.Bitwise;
using ChessLib.Types.Enums;

namespace ChessLib.MagicBitboard.MovingPieces
{
    internal interface IMovingPiece
    {
        ulong GetPseudoLegalMoves(ushort square, Color playerColor, ulong occupancy);
    }

    internal abstract class MovingPiece : IMovingPiece
    {
        public readonly ulong[] MagicKey = new ulong[64];
        protected ulong[] AttackMask;
        protected ulong[] MoveMask;
        protected MovingPieceService MovingPieceSvc;

        protected IEnumerable<ushort> AllSquares => Enumerable.Range(0, 64).Select(x=>(ushort)x);

        protected MovingPiece()
        {
            MovingPieceSvc = new MovingPieceService();
        }

        private ulong[] MovesAndAttacks
        {
            get
            {
                Debug.Assert(MoveMask != null && AttackMask != null, "Move and Attack masks need initialization.");
                Debug.Assert(MoveMask.Length == AttackMask.Length,
                    "Move and Attack masks should be of the same length.");
                Debug.Assert(MoveMask.Length == 64, "Move and Attack mask should be of size 64.");
                return MoveMask.Select((movesFromPosition, index) => movesFromPosition | AttackMask[index])
                    .ToArray();
            }
        }

        public abstract ulong GetPseudoLegalMoves(ushort square, Color playerColor, ulong occupancy);

        public abstract void Initialize();

        public ulong GetMovesFromSquare(ushort squareIndex)
        {
            return MoveMask[squareIndex];
        }
        
    }
}