using System.Collections.Generic;
using System.Diagnostics;
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

        private ulong[] MovesAndAttacks
        {
            get
            {
                Debug.Assert(MoveMask != null && AttackMask != null, "MoveValue and Attack masks need initialization.");
                Debug.Assert(MoveMask.Length == AttackMask.Length,
                    "MoveValue and Attack masks should be of the same length.");
                Debug.Assert(MoveMask.Length == 64, "MoveValue and Attack mask should be of size 64.");
                return MoveMask.Select((movesFromPosition, index) => movesFromPosition | AttackMask[index])
                    .ToArray();
            }
        }

        protected MovingPiece()
        {
            MovingPieceSvc = new MovingPieceService();
        }

        public readonly ulong[] MagicKey = new ulong[64];
        protected ulong[] AttackMask;
        protected ulong[] MoveMask;
        protected MovingPieceService MovingPieceSvc;

        public abstract ulong GetPseudoLegalMoves(ushort square, Color playerColor, ulong occupancy);

        public abstract void Initialize();
    }
}