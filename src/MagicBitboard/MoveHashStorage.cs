using System.Text;
using ChessLib.Data.MoveRepresentation;
using System.Security.Cryptography;
using System;

namespace MagicBitboard
{
    public class MoveHashStorage : IEquatable<MoveHashStorage>
    {
        public MoveHashStorage(MoveExt move, string fEN)
        {
            Move = move.Move;
            FEN = fEN;
            BoardStateHash = GetHashString(FEN.Split(' ')[0]);
        }

        public ushort Move { get; private set; }
        public string FEN { get; private set; }
        public string BoardStateHash { get; private set; }

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
            return other.Move == Move && other.FEN == other.FEN;
        }
    }
}

