using System;
using System.Security.Cryptography;
using System.Text;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types;

namespace ChessLib.Data.MoveRepresentation
{

    public class MoveHashStorage : MoveStorage, IEquatable<MoveHashStorage>
    {
        public MoveHashStorage(MoveExt move, Piece pieceMoving, Color colorMoving, string fen, string textRepresentation)
        : base(move, pieceMoving, colorMoving)
        {
            FENHelpers.ValidateFENString(fen);
            FEN = fen;
            BoardStateHash = GetHashString(FEN.Split(' ')[0]);
            SAN = textRepresentation;
        }

        public string FEN { get; }
        public string BoardStateHash { get; }
        public string SAN { get; }

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
        public bool Equals(MoveHashStorage other)
        {
            return base.Equals(other) && other.FEN == FEN;
        }
    }
}

