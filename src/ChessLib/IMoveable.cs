namespace ChessLib
{
    public interface IMovable
    {
        Square MoveE(Square sq);
        Square MoveN(Square sq);
        Square MoveNE(Square sq);
        Square MoveNW(Square sq);
        Square MoveS(Square sq);
        Square MoveSE(Square sq);
        Square MoveSW(Square sq);
        Square MoveW(Square sq);
    }

    public class Movable : IMovable
    {
        public readonly int BoardWidth;
        public readonly int BoardHeight;

        public Movable()
        {
            BoardHeight = 8;
            BoardWidth = 8;
        }

        public Movable(int boardWidth, int boardHeight)
        {
            BoardWidth = boardWidth - 1;
            BoardHeight = boardHeight - 1;
        }

        public Square MoveN(Square sq)
        {
            if (sq.Rank - 1 < 0) { return null; }
            return new Square(sq.File, sq.Rank - 1);
        }

        public Square MoveNE(Square sq)
        {
            if ((sq.File.Increase() >= BoardWidth) || (sq.Rank - 1 < 0)) { return null; }
            return new Square((File)(sq.File.Increase()), sq.Rank - 1);
        }

        public Square MoveE(Square sq)
        {
            if ((int)sq.File + 1 >= BoardWidth) { return null; }
            return new Square((File)((int)sq.File + 1), sq.Rank);
        }

        public Square MoveSE(Square sq)
        {
            if (((int)sq.File + 1 >= BoardWidth) || (sq.Rank + 1 >= BoardHeight)) { return null; }
            return new Square((File)((int)sq.File + 1), sq.Rank + 1);
        }

        public Square MoveS(Square sq)
        {
            if (sq.Rank + 1 >= BoardHeight) { return null; }
            return new Square(sq.File, sq.Rank + 1);
        }

        public Square MoveSW(Square sq)
        {
            if (((int)sq.File - 1 < 0) || (sq.Rank + 1 >= BoardHeight)) { return null; }
            return new Square((File)((int)sq.File - 1), sq.Rank + 1);
        }

        public Square MoveW(Square sq)
        {
            if ((int)sq.File - 1 < 0) { return null; }
            return new Square((File)((int)sq.File - 1), sq.Rank);
        }

        public Square MoveNW(Square sq)
        {
            if (((int)sq.File - 1 < 0) || (sq.Rank - 1 < 0)) { return null; }
            return new Square((File)((int)sq.File - 1), sq.Rank - 1);
        }


    }
}