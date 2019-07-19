using System;
using System.Security.Cryptography;
using System.Text;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;

namespace ChessLib.Data.MoveRepresentation
{
    /// <summary>
    /// A class that stores move information in a way that can easily be hashed for quick lookup
    /// </summary>
    public class MoveStorage : MoveExt, IEquatable<MoveStorage>, IContainsSAN
    {
        public static MoveStorage Root(Color c, string fen) => new MoveStorage(fen, null, Piece.King, c, "ROOT");

        public MoveStorage(string premoveFen, IMoveExt move, Piece pieceMoving, Color colorMoving, string textRepresentation)
        : this(move, pieceMoving, colorMoving)
        {
            PremoveFEN = premoveFen;
            SAN = textRepresentation;
            BoardStateHash = GetHashString(premoveFen);
        }
        protected MoveStorage(ushort move, Piece pieceMoving, Color colorMoving)
            : base(move)
        {
            PieceMoving = pieceMoving;
            ColorMoving = colorMoving;
        }

        protected MoveStorage(IMoveExt move, Piece pieceMoving, Color colorMoving) : this(move.Move, pieceMoving, colorMoving) { }

        //public MoveStorage(BoardInfo boardInfo, MoveExt move)
        //{

        //}

        public virtual Color ColorMoving { get; }

        public virtual Piece PieceMoving { get; }

        public void SetPostMoveFEN(string fen)
        {
            PostmoveFEN = fen;
        }
        public string PremoveFEN { get; protected set; }
        public string PostmoveFEN { get; private set; }
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

    public interface IContainsSAN
    {
        string SAN { get; set; }
    }
}