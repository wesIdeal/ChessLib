using System;
using System.Security.Cryptography;
using System.Text;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types;

namespace ChessLib.Data.MoveRepresentation
{
    public class MoveHashStorage : IEquatable<MoveHashStorage>
    {
        public MoveHashStorage(MoveExt move, Piece pieceMoving, Color colorMoving, string fen)
        {
            FENHelpers.ValidateFENString(fen);
            Move = move.Move;
            FEN = fen;
            PieceMoving = pieceMoving;
            ColorMoving = colorMoving;
            BoardStateHash = GetHashString(FEN.Split(' ')[0]);
        }

        public Color ColorMoving { get; set; }
        public ushort Move { get; }
        public string FEN { get; }
        public string BoardStateHash { get; }
        public Piece PieceMoving { get; set; }

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
            return other != null && other.Move == Move && other.FEN == FEN;
        }
    }
}

