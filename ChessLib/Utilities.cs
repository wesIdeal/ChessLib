using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessLib
{
    enum FENPieces { Placement = 0, ActiveColor, Casting, EnPassant, HalfmoveClock, FullmoveNumber }

    public static class FileHelpers
    {
        public static int ToInt(this File f) => (int)f;

    }
    public static class Utilities
    {
        public static bool IsInBoardsRange(this int i)
        {
            return i < 8 && i >= 0;
        }
        delegate char VaryCaseByColor(char c);
        public static char? GetCharFromPiece(this PieceOfColor p)
        {
            VaryCaseByColor changeCase;
            if (p.Color == Color.White) { changeCase = char.ToUpper; }
            else { changeCase = char.ToLower; }

            switch (p.Piece)
            {
                case Piece.NULL: return null;
                case Piece.Pawn: return changeCase('p');
                case Piece.Knight: return changeCase('n');
                case Piece.Bishop: return changeCase('b');
                case Piece.Rook: return changeCase('r');
                case Piece.Queen: return changeCase('q');
                case Piece.King: return changeCase('k');
                default: throw new ArgumentException("Could not determine piece's character.");
            }
        }
        public static Piece GetPieceFromChar(char? piece)
        {
            if (piece == null)
            {
                return Piece.NULL;
            }
            switch (char.ToUpper((char)piece))
            {
                case 'P': return Piece.Pawn;
                case 'N': return Piece.Knight;
                case 'B': return Piece.Knight;
                case 'R': return Piece.Rook;
                case 'Q': return Piece.Queen;
                case 'K': return Piece.King;
                default: throw new Exception($"Invalid piece passed to Utilities.GetPieceFromChar: '{piece}'");
            }
        }


        public static int ArrayRankToRealRank(int arrayRank)
        {
            if (arrayRank > 7 || arrayRank < 0) throw new ArgumentException("Array Rank must be between 0 and 7.", "arrayRank");
            return Math.Abs(8 - arrayRank);
        }

        public static int RealRankToArrayRank(int realRank)
        {
            if (realRank > 8 || realRank < 1) throw new ArgumentException("Real Rank must be between 1 and 8.", "realRank");
            return Math.Abs(8 - realRank);
        }

        public static Square[,] BoardFromFEN(string fen, out BoardProperties bp)
        {
            Square[,] rv = new Square[8, 8];
            var cBoard = CharBoardFromFEN(fen, out bp);
            for (int rank = 0; rank < 8; rank++)
            {
                for (File f = File.a; f <= File.h; f++)
                {
                    rv[rank, f.ToInt()] = new Square(PieceOfColor.GetPieceColorFromChar(cBoard[rank, f.ToInt()]), f, rank);
                }
            }
            return rv;
        }

        private static char?[,] CharBoardFromFEN(string fen, out BoardProperties bp)
        {
            var FENArray = fen.Split(' ');
            var boardSetup = FENArray[0];
            var ranks = boardSetup.Split('/');
            var rv = new char?[8, 8];
            var rIndex = 0;

            foreach (var rank in ranks)
            {
                int fIndex = 0;
                foreach (var p in rank)
                {
                    if (Char.IsDigit(p))
                    {
                        var emptySquaresToAdd = short.Parse(p.ToString());
                        if (emptySquaresToAdd > 8 || emptySquaresToAdd + fIndex > 8) { throw new FENException($"Invalid rank passed from FEN on rank {ArrayRankToRealRank(rIndex)}."); }
                        for (var i = 0; i < emptySquaresToAdd; i++)
                        {
                            rv[rIndex, fIndex] = null;
                            fIndex++;
                        }
                    }
                    else
                    {
                        rv[rIndex, fIndex] = p;
                        fIndex++;
                    }
                }
                rIndex++;
            }

            bp = GetBoardPropertiesFromFEN(fen);

            return rv;
        }

        public static BoardProperties GetBoardPropertiesFromFEN(this string fen)
        {
            var FENArray = fen.Split(' ');
            if (FENArray.Length != 6)
            {
                throw new ArgumentException("Could not parse FEN. Not enough pieces in record. See https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation");
            }
            var bp = new BoardProperties();
            var enPassant = FENArray[(int)FENPieces.EnPassant];
            var castling = FENArray[(int)FENPieces.Casting];
            var halfMove = FENArray[(int)FENPieces.HalfmoveClock];
            var fullMove = FENArray[(int)FENPieces.FullmoveNumber];
            var activeColor = FENArray[(int)FENPieces.ActiveColor];
            InterpretCastlingString(castling, ref bp);

            if (activeColor != "w" && activeColor != "b")
            {
                throw new ArgumentException("Active Color portion of FEN must either be 'w' or 'b'.");
            }

            bp.ActiveColor = (activeColor == "w" ? Color.White : Color.Black);

            if (enPassant != "-")
            {
                bp.EnPassentSquare = new Square(enPassant);
            }
            if (int.TryParse(halfMove, out int hmClock))
            {
                bp.HalfmoveClock = hmClock;
            }
            else
            {
                throw new ArgumentException("Could not parse FEN's Halfmove Clock piece.");
            }
            if (int.TryParse(fullMove, out int fMove))
            {
                bp.FullMoveNumber = fMove;
            }
            else
            {
                throw new ArgumentException("Could not parse FEN's Move Number piece.");
            }
            return bp;
        }

        private static void InterpretCastlingString(string input, ref BoardProperties bp)
        {
            bp.CanWhiteCastleQueenSide = input.Contains("Q");
            bp.CanWhiteCastleKingSide = input.Contains("K");
            bp.CanBlackCastleQueenSide = input.Contains("q");
            bp.CanBlackCastleKingSide = input.Contains("k");
        }

        public static IEnumerable<Square> GetTargetSquaresForKnight(Square sq)
        {
            var possibleSquares = new List<Tuple<int, int>>();
            possibleSquares.Add(new Tuple<int, int>(sq.Rank + 2, (int)sq.File + 1));
            possibleSquares.Add(new Tuple<int, int>(sq.Rank + 2, (int)sq.File - 1));
            possibleSquares.Add(new Tuple<int, int>(sq.Rank + 1, (int)sq.File + 2));
            possibleSquares.Add(new Tuple<int, int>(sq.Rank + 1, (int)sq.File - 2));
            possibleSquares.Add(new Tuple<int, int>(sq.Rank - 2, (int)sq.File + 1));
            possibleSquares.Add(new Tuple<int, int>(sq.Rank - 2, (int)sq.File - 1));
            possibleSquares.Add(new Tuple<int, int>(sq.Rank - 1, (int)sq.File + 2));
            possibleSquares.Add(new Tuple<int, int>(sq.Rank - 1, (int)sq.File - 2));
            possibleSquares = possibleSquares.Where(x => x.Item1.IsInBoardsRange() && x.Item2.IsInBoardsRange()).ToList();
            return possibleSquares.Select(x => new Square((File)x.Item2, x.Item1));
        }



    }
}
