using ChessLib.Data.Helpers;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ChessLib.Data.MoveRepresentation
{

    public class MoveHashStorage : MoveStorage, IEquatable<MoveHashStorage>
    {
        public MoveHashStorage(string fen, IMoveExt move, Piece pieceMoving, Color colorMoving, string textRepresentation)
        : base(move, pieceMoving, colorMoving)
        {
            
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

