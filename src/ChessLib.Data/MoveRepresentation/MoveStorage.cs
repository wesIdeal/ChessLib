using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using System;

namespace ChessLib.Data.MoveRepresentation
{
    public abstract class MoveStorage : IMove, IMoveStorage, IEquatable<MoveStorage>
    {
        public MoveStorage(ushort move, Piece pieceMoving, Color colorMoving)
        {
            Move = move;
            PieceMoving = pieceMoving;
            ColorMoving = colorMoving;
        }

        public MoveStorage(IMoveExt move, Piece pieceMoving, Color colorMoving) : this(move.Move, pieceMoving, colorMoving) { }

        public virtual Color ColorMoving { get; private set; }
        public virtual ushort Move { get; }
        public virtual Piece PieceMoving { get; private set; }
        public bool Equals(IMoveStorage other)
        {
            return other != null &&
                   ColorMoving == other.ColorMoving &&
                   Move == other.Move &&
                   PieceMoving == other.PieceMoving;
        }

        public bool Equals(MoveStorage other)
        {
            return other != null &&
                   ColorMoving == other.ColorMoving &&
                   Move == other.Move &&
                   PieceMoving == other.PieceMoving;
        }
    }
}