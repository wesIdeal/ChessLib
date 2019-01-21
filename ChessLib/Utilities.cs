using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessLib
{
    enum FENPieces { Placement = 0, ActiveColor, Casting, EnPassant, HalfmoveClock, FullmoveNumber }
    public static class Utilities
    {
        public static bool IsInBoardsRange(this int i)
        {
            return i < 8 && i >= 0;
        }
        public static Piece GetPieceFromChar(char piece)
        {
            switch (piece)
            {
                case 'N': return Piece.Knight;
                case 'B': return Piece.Knight;
                case 'R': return Piece.Rook;
                case 'Q': return Piece.Queen;
                case 'K': return Piece.King;
                default: throw new Exception($"Invalid piece passed to Utilities.GetPieceFromChar: '{piece}'");
            }
        }
        public static PieceOfColor GetPieceColorFromChar(char piece)
        {
            var pc = new PieceOfColor();
            pc.Color = Char.IsUpper(piece) ? Color.White : Color.Black;
            piece = Char.ToUpper(piece);
            switch (piece)
            {
                case 'N':
                    pc.Piece = Piece.Knight;
                    break;
                case 'B':
                    pc.Piece = Piece.Bishop;
                    break;
                case 'R':
                    pc.Piece = Piece.Rook;
                    break;
                case 'Q':
                    pc.Piece = Piece.Queen;
                    break;
                case 'K':
                    pc.Piece = Piece.King;
                    break;
                case 'P':
                    pc.Piece = Piece.Pawn;
                    break;
                default: throw new Exception($"Invalid piece passed to Utilities.GetPieceColorFromChar: '{piece}'");
            }
            return pc;
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

        public static char?[,] BoardFromFEN(string fen, out BoardProperties bp)
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
                bp.EnPassentSquare = Square.FromString(enPassant);
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
