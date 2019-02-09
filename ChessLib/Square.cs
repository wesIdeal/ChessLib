using System;

namespace ChessLib
{

    public class Square : IEquatable<Square>
    {
        public Square()
        {
        }
        public Square(File f, int arrayRank)
        {
            File = f;
            Rank = arrayRank;
        }

        public Square(string strSquareRepresentation)
        {
            if (strSquareRepresentation.Length != 2)
            {
                throw new ArgumentException("String representation must have 2 characters, first for file, second for rank. I.e. e4.");
            }
            var strFile = strSquareRepresentation[0];
            var strRank = strSquareRepresentation[1];
            if (!Char.IsLetter(strFile) || !IsFileCharacterInRange(strFile))
            {
                throw new ArgumentException("First letter in square representation must be a letter, from a-h");
            }
            var file = (File)Enum.Parse(typeof(File), strFile.ToString(), true);
            var rank = int.Parse(strRank.ToString());
            if (rank < 1 || rank > 8)
            {
                throw new ArgumentException("Rank must be between 1 and 8, inclusively.");
            }
            File = file;
            Rank = Utilities.RealRankToArrayRank(rank);
        }

        public Square(PieceOfColor pieceOfColor, File file, int rank)
            : this(file, rank)
        {
            PieceOfColor = pieceOfColor;
        }

        public PieceOfColor PieceOfColor { get; set; }

        public static Square FromRealRank(File f, int realRank) => new Square(f, Utilities.RealRankToArrayRank(realRank));

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
        //protected void SetRankWithArrayIndex(int arrayIndex) { Rank = arrayIndex; }
        //protected void SetRankWithRealRank(int realRank) { Rank = Utilities.RealRankToArrayRank(realRank); }
        public override string ToString()
        {
            return File.ToString().ToLower() + Utilities.ArrayRankToRealRank(Rank).ToString();
        }
        public bool Equals(Square other)
        {
            if ((object)other == null) return false;
            return other.File == File && other.Rank == Rank;
        }
        public static bool operator ==(Square s1, Square s2)
        {
            if ((object)s1 == null)
                return ((object)s2 == null);

            return s1.Equals(s2);
        }
        public static bool operator !=(Square s1, Square s2)
        {
            return !(s1 == s2);
        }

        public Square Copy()
        {
            return new Square(File, Rank);
        }
    }
}
