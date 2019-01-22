using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ChessLib
{
    public class ChessBoard
    {
        public const string InitialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public ChessBoard()
        {
            var p = new BoardProperties();
            Board = Utilities.BoardFromFEN(InitialFEN, out p);
            Properties = p;
        }

        public ChessBoard(string fen)
        {
            var p = new BoardProperties();
            Board = Utilities.BoardFromFEN(fen, out p);
            Properties = p;
        }
        //indexed by rank,file. a8 = [0, File.a]
        public char?[,] Board { get; set; }
        public char? PieceOnSquare(Square sq) => Board[sq.Rank, (int)sq.File];
        public BoardProperties Properties { get; set; }

        public IEnumerable<Square> GetTargetSquares(Square sq)
        {
            var charPiece = Board[sq.Rank, (int)sq.File];
            if (charPiece == null) return new List<Square>();
            var pieceOfColor = Utilities.GetPieceColorFromChar((char)charPiece);
            switch (pieceOfColor.Piece)
            {
                case Piece.Knight:
                    return GetTargetSquaresForKnight(sq, pieceOfColor.Color);
                case Piece.Bishop:
                    return GetTargetSquaresForBishop(sq, pieceOfColor.Color);
                case Piece.Rook:
                    return GetTargetSquaresForRook(sq, pieceOfColor.Color);
                case Piece.Pawn:
                    return GetTargetSquaresForPawn(sq, pieceOfColor.Color);
                case Piece.Queen:
                    return GetTargetSquaresForQueen(sq, pieceOfColor.Color);
            }
            return GetTargetSquaresForKnight(sq, pieceOfColor.Color);
        }

        private IEnumerable<Square> GetTargetSquaresForQueen(Square sq, Color color)
        {
            var squares = new List<Square>();
            squares.AddRange(GetTargetSquaresForBishop(sq, color));
            squares.AddRange(GetTargetSquaresForRook(sq, color));
            return squares.Distinct();
        }

        delegate Square Move(Square sq);


        private void GetPawnMoveDelegates(Color color, out Move forward, out Move diag1, out Move diag2)
        {
            if (color == Color.White)
            {
                forward = MoveN;
                diag1 = MoveNE;
                diag2 = MoveNW;
            }
            else
            {
                forward = MoveS;
                diag1 = MoveSE;
                diag2 = MoveSW;
            }
        }

        private bool IsWhitePiece(char piece) => Char.IsUpper(piece);
        private bool IsBlackPiece(char piece) => Char.IsLower(piece);
        private IEnumerable<Square> GetTargetSquaresForPawn(Square sq, Color color)
        {
            var tmpSquare = sq.Copy();
            var squares = new List<Square>();
            var startingRankForPawns = color == Color.White ? 2 : 7;
            GetPawnMoveDelegates(color, out Move moveForward, out Move moveDiag1, out Move moveDiag2);
            tmpSquare = moveForward(sq);
            if (PieceOnSquare(tmpSquare) == null)
            {
                squares.Add(tmpSquare);
                if (sq.Rank == Utilities.RealRankToArrayRank(startingRankForPawns))
                {
                    tmpSquare = moveForward(tmpSquare);
                    if (PieceOnSquare(tmpSquare) == null)
                        squares.Add(tmpSquare);
                }
            }
            var diag1 = moveDiag1(sq);
            var diag2 = moveDiag2(sq);
            var pieceOnSquare1 = PieceOnSquare(diag1);
            var pieceOnSquare2 = PieceOnSquare(diag2);
            if (pieceOnSquare1 != null && !IsOccupiedWithPieceOfSameColor(diag1, color))
            {
                squares.Add(diag1);
            }
            if (pieceOnSquare2 != null && !IsOccupiedWithPieceOfSameColor(diag2, color))
            {
                squares.Add(diag2);
            }
            return squares;
        }

        private bool PlacePieceInTargetList(Square sq, Color color, ref List<Square> squares)
        {
            return PlacePieceInTargetList((int)sq.File, sq.Rank, color, ref squares);
        }

        private bool PlacePieceInTargetList(int file, int rank, Color color, ref List<Square> squares)
        {
            var piece = Board[rank, file];
            var square = new Square((File)file, rank);
            var sameColor = IsOccupiedWithPieceOfSameColor(square, color);
            if (piece == null || !sameColor)
            {
                squares.Add(square);
                if (piece == null) return true;
                return false;
            }
            return false;
        }

        private IEnumerable<Square> GetTargetSquaresForRook(Square sq, Color color)
        {
            var squares = new List<Square>();
            //up
            Square tmpSq = sq.Copy();
            while ((tmpSq = MoveN(tmpSq)) != null)
            {
                if (!PlacePieceInTargetList(tmpSq, color, ref squares))
                    break;
            }
            //down
            tmpSq = sq.Copy();
            while ((tmpSq = MoveS(tmpSq)) != null)
            {
                if (!PlacePieceInTargetList(tmpSq, color, ref squares))
                    break;
            }
            //left
            tmpSq = sq.Copy();
            while ((tmpSq = MoveW(tmpSq)) != null)
            {
                if (!PlacePieceInTargetList(tmpSq, color, ref squares))
                    break;
            }
            //right
            tmpSq = sq.Copy();
            while ((tmpSq = MoveE(tmpSq)) != null)
            {
                if (!PlacePieceInTargetList(tmpSq, color, ref squares))
                    break;
            }
            return squares;
        }

        private IEnumerable<Square> GetTargetSquaresForKnight(Square sq, Color color)
        {
            var allPossibleSquares = Utilities.GetTargetSquaresForKnight(sq);
            allPossibleSquares = allPossibleSquares.Where(s => !IsOccupiedWithPieceOfSameColor(s, color));
            return allPossibleSquares;
        }

        public IEnumerable<Square> GetTargetSquaresForBishop(Square sq, Color color)
        {
            var squares = new List<Square>();
            //up and to the left
            Square tmpSq = sq.Copy();
            while ((tmpSq = MoveNW(tmpSq)) != null)
            {
                if (!PlacePieceInTargetList(tmpSq, color, ref squares))
                    break;
            }
            //up and to the right
            tmpSq = sq.Copy();
            while ((tmpSq = MoveNE(tmpSq)) != null)
            {
                if (!PlacePieceInTargetList(tmpSq, color, ref squares))
                    break;
            }
            //down and to the right
            tmpSq = sq.Copy();
            while ((tmpSq = MoveSE(tmpSq)) != null)
            {
                if (!PlacePieceInTargetList(tmpSq, color, ref squares))
                    break;
            }
            //down and to the left
            tmpSq = sq.Copy();
            while ((tmpSq = MoveSW(tmpSq)) != null)
            {
                if (!PlacePieceInTargetList(tmpSq, color, ref squares))
                    break;
            }
            return squares;
        }

        private bool IsOccupiedWithPieceOfSameColor(Square sq, Color color)
        {
            var p = Board[sq.Rank, (int)sq.File];
            if (p != null)
            {
                return (color == Color.White ? Char.IsUpper((char)p) : Char.IsLower((char)p));
            }
            return false;
        }

        public static Square MoveN(Square sq)
        {
            if (sq.Rank - 1 < 0) { return null; }
            return new Square(sq.File, sq.Rank - 1);
        }

        public static Square MoveNE(Square sq)
        {
            if (((int)sq.File + 1 > 7) || (sq.Rank - 1 < 0)) { return null; }
            return new Square((File)((int)sq.File + 1), sq.Rank - 1);
        }

        public static Square MoveE(Square sq)
        {
            if ((int)sq.File + 1 > 7) { return null; }
            return new Square((File)((int)sq.File + 1), sq.Rank);
        }

        public static Square MoveSE(Square sq)
        {
            if (((int)sq.File + 1 > 7) || (sq.Rank + 1 > 7)) { return null; }
            return new Square((File)((int)sq.File + 1), sq.Rank + 1);
        }

        public static Square MoveS(Square sq)
        {
            if (sq.Rank + 1 > 7) { return null; }
            return new Square(sq.File, sq.Rank + 1);
        }

        public static Square MoveSW(Square sq)
        {
            if (((int)sq.File - 1 < 0) || (sq.Rank + 1 > 7)) { return null; }
            return new Square((File)((int)sq.File - 1), sq.Rank + 1);
        }

        public static Square MoveW(Square sq)
        {
            if ((int)sq.File - 1 < 0) { return null; }
            return new Square((File)((int)sq.File - 1), sq.Rank);
        }

        public static Square MoveNW(Square sq)
        {
            if (((int)sq.File - 1 < 0) || (sq.Rank - 1 < 0)) { return null; }
            return new Square((File)((int)sq.File - 1), sq.Rank - 1);
        }






    }
}
