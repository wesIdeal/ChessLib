using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ChessLib
{
    public class ChessBoard : Movable
    {
        public const string InitialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public ChessBoard() : this(InitialFEN)
        {

        }

        public ChessBoard(string fen)
        {
            var p = new BoardProperties();
            Board = Utilities.BoardFromFEN(fen, out p);
            Properties = p;
        }
        //indexed by rank,file. a8 = [0, File.a]
        //public char?[,] Board { get; set; }
        public Square[,] Board { get; private set; }
        public Square PieceOnSquare(Square sq) => Board[sq.Rank, (int)sq.File];
        public BoardProperties Properties { get; set; }

        public IEnumerable<Square> GetTargetSquares(Square sq)
        {
            var charPiece = Board[sq.Rank, (int)sq.File];
            if (charPiece == null) return new List<Square>();
            var pieceOfColor = charPiece.PieceOfColor;
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

        public void MovePiece(string square, Color black)
        {

        }

        public Square GetDestinationFromMoveText(string moveText)
        {
            return new Square(moveText.Substring(moveText.Length - 2, 2));
        }

        public Move InterpretMove(string moveText, Color color)
        {
            bool isPawnMove = false;
            bool isCapture = true;
            Square origin;
            Move move = new Move();
            moveText.Trim();
            if (string.IsNullOrWhiteSpace(moveText) || moveText.Length < 2) { throw new ArgumentException("null, Empty, or illegaly short move passed as text to InterpretMove."); }
            Square destination = GetDestinationFromMoveText(moveText);
            if (moveText.Count() == 2 || Char.IsLower(moveText[0])) // pawn move, or pawn move with capture
            {
                isPawnMove = true;
                isCapture = moveText.Contains('x');
                if (!isCapture) //Then pawn started on same file, moved up a rank.
                {
                    var searchPiece = new PieceOfColor() { Color = color, Piece = Piece.Pawn };
                    //find the pawn on file, closest to rank (in case of doubled pawns)
                    MoveDelegate moveBackToFindSquare = MoveS;
                    if (color == Color.Black)
                    {
                        moveBackToFindSquare = MoveN;
                    }
                    Square tmpSquare = destination.Copy();
                    while ((tmpSquare = moveBackToFindSquare(tmpSquare)) != null)
                    {
                        var piece = PieceOnSquare(tmpSquare);
                        if (piece != null)
                        {
                            if (piece.PieceOfColor.Equals(searchPiece))
                            {
                                //ensure it is a target square, otherwise it is illegal.
                                var targets = GetTargetSquaresForPawn(tmpSquare, color);
                                if (!targets.Any(t => t == destination))
                                    throw new Exception($"Illegal move processed. MoveText = {moveText}");
                                else
                                {
                                    origin = tmpSquare;
                                    return new Move(origin, destination, false, Piece.Pawn);
                                }
                            }
                        }
                    }
                }
                else //pawn came from File -1 or File + 1, Rank - 1
                {

                }
            }
            return null;
        }

        protected IEnumerable<Square> GetTargetSquaresForQueen(Square sq, Color color)
        {
            var squares = new List<Square>();
            squares.AddRange(GetTargetSquaresForBishop(sq, color));
            squares.AddRange(GetTargetSquaresForRook(sq, color));
            return squares.Distinct();
        }

        delegate Square MoveDelegate(Square sq);


        private void GetPawnMoveDelegates(Color color, out MoveDelegate forward, out MoveDelegate diag1, out MoveDelegate diag2)
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

        protected bool IsWhitePiece(char piece) => Char.IsUpper(piece);

        protected bool IsBlackPiece(char piece) => Char.IsLower(piece);

        protected IEnumerable<Square> GetTargetSquaresForPawn(Square sq, Color color)
        {
            var tmpSquare = sq.Copy();
            var squares = new List<Square>();
            var startingRankForPawns = color == Color.White ? 2 : 7;
            GetPawnMoveDelegates(color, out MoveDelegate moveForward, out MoveDelegate moveDiag1, out MoveDelegate moveDiag2);
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

        protected bool PlaceSquareInTargetList(Square sq, Color color, ref List<Square> squares)
        {
            return PlaceSquareInTargetList((int)sq.File, sq.Rank, color, ref squares);
        }

        protected bool PlaceSquareInTargetList(int file, int rank, Color color, ref List<Square> squares)
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

        protected IEnumerable<Square> GetTargetSquaresForRook(Square sq, Color color)
        {
            var squares = new List<Square>();
            //up
            Square tmpSq = sq.Copy();
            while ((tmpSq = MoveN(tmpSq)) != null)
            {
                if (!PlaceSquareInTargetList(tmpSq, color, ref squares))
                    break;
            }
            //down
            tmpSq = sq.Copy();
            while ((tmpSq = MoveS(tmpSq)) != null)
            {
                if (!PlaceSquareInTargetList(tmpSq, color, ref squares))
                    break;
            }
            //left
            tmpSq = sq.Copy();
            while ((tmpSq = MoveW(tmpSq)) != null)
            {
                if (!PlaceSquareInTargetList(tmpSq, color, ref squares))
                    break;
            }
            //right
            tmpSq = sq.Copy();
            while ((tmpSq = MoveE(tmpSq)) != null)
            {
                if (!PlaceSquareInTargetList(tmpSq, color, ref squares))
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
                if (!PlaceSquareInTargetList(tmpSq, color, ref squares))
                    break;
            }
            //up and to the right
            tmpSq = sq.Copy();
            while ((tmpSq = MoveNE(tmpSq)) != null)
            {
                if (!PlaceSquareInTargetList(tmpSq, color, ref squares))
                    break;
            }
            //down and to the right
            tmpSq = sq.Copy();
            while ((tmpSq = MoveSE(tmpSq)) != null)
            {
                if (!PlaceSquareInTargetList(tmpSq, color, ref squares))
                    break;
            }
            //down and to the left
            tmpSq = sq.Copy();
            while ((tmpSq = MoveSW(tmpSq)) != null)
            {
                if (!PlaceSquareInTargetList(tmpSq, color, ref squares))
                    break;
            }
            return squares;
        }

        private bool IsOccupiedWithPieceOfSameColor(Square sq, Color color)
        {
            var p = Board[sq.Rank, (int)sq.File];
            if (p != null)
            {
                return p.PieceOfColor.Color == color;
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("--------------------------------------------------------");
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    var chPiece = Utilities.GetCharFromPiece(Board[r, f].PieceOfColor);
                    sb.Append($" | {(chPiece.HasValue ? chPiece : ' ')} | ");
                }
            }
            sb.Append("--------------------------------------------------------");
            return sb.ToString();
        }




    }
}
