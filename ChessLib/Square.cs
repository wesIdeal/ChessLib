using System;

namespace ChessLib
{
    public class Square : IEquatable<Square>
    {
        public Square(File f, int arrayRank)
        {
            File = f;
            Rank = arrayRank;
        }
        public static Square FromArrayRank(File f, int arrayRank) => new Square(f, arrayRank);
        public static Square FromRealRank(File f, int realRank) => new Square(f, Utilities.RealRankToArrayRank(realRank));
        public static Square FromString(string square)
        {
            if (square.Length != 2) throw new ArgumentException("String representation must have 2 characters, first for file, second for rank. I.e. e4.");
            var file = square[0];
            var rank = square[1];
            if (!Char.IsLetter(file) || !IsFileCharacterInRange(file))
                throw new ArgumentException("First letter in square representation must be a letter, from a-h");
            return FromRealRank((File)Enum.Parse(typeof(File), file.ToString(), true), int.Parse(rank.ToString()));
        }

        private static bool IsFileCharacterInRange(char file)
        {
            var f = Char.ToUpper(file);
            return f >= 'A' && f <= 'H';
        }

        private int _rank;
        public File File { get; set; }
        public int Rank
        {
            get => _rank;
            set
            {
                if (value < 0 || value > 7) throw new Exception("Rank cannot be less than zero or greater than 7.");
                _rank = value;
            }
        }
        protected void SetRankWithArrayIndex(int arrayIndex) { Rank = arrayIndex; }
        protected void SetRankWithRealRank(int realRank) { Rank = Utilities.RealRankToArrayRank(realRank); }
        public override string ToString()
        {
            return File.ToString().ToLower() + Utilities.ArrayRankToRealRank(Rank).ToString();
        }
        public bool Equals(Square other)
        {
            return other.File == File && other.Rank == Rank;
        }

        public Square Copy()
        {
            return new Square(File, Rank);
        }
    }
}
