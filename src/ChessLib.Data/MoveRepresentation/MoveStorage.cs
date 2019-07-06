using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ChessLib.Data.MoveRepresentation
{
    /// <summary>
    /// A class that stores move information in a way that can easily be hashed for quick lookup
    /// </summary>
    public class MoveStorage : MoveExt, IEquatable<MoveStorage>
    {
        public MoveStorage(string fen, IMoveExt move, Piece pieceMoving, Color colorMoving, string textRepresentation)
        : this(move, pieceMoving, colorMoving)
        {
            FEN = fen;
            SAN = textRepresentation;
            BoardStateHash = GetHashString(fen);

        }
        protected MoveStorage(ushort move, Piece pieceMoving, Color colorMoving)
            : base(move)
        {
            PieceMoving = pieceMoving;
            ColorMoving = colorMoving;
        }

        protected MoveStorage(MoveExt move, Piece pieceMoving, Color colorMoving)
            : this(move.Move, pieceMoving, colorMoving) { }

        protected MoveStorage(IMoveExt move, Piece pieceMoving, Color colorMoving) : this(move.Move, pieceMoving, colorMoving) { }

        public virtual Color ColorMoving { get; private set; }

        public virtual Piece PieceMoving { get; private set; }

        public string FEN { get; protected set; }

        public string BoardStateHash { get; }

        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
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