using ChessLib.Data.Types.Enums;
using ChessLib.Graphics.TextDisplay;
using System;
using System.Linq;

namespace ChessLib.MagicBitboard.App
{
    internal class PawnTests
    {
        private DisplayService _displaySvc;
        private Bitboard bb;
        public PawnTests()
        {
            _displaySvc = new DisplayService();
            bb = Bitboard.Instance;
        }
        char entry;
        internal void ShowPawnMainMenu()
        {
            while ((entry = ShowPawnMenu()) != 'q')
            {
                switch (entry)
                {
                    case '1':
                        ShowPawnMoves();
                        break;
                    case '2':
                        ShowPawnAttacks();
                        break;
                    default: break;
                }
            }
        }

        private void ShowPawnAttacks()
        {
            throw new NotImplementedException();
        }

        private void ShowPawnMoves()
        {
            char[] validEntries = new char[] { 'w', 'b' };
            while (!validEntries.Contains(entry = char.ToLower(Console.ReadKey().KeyChar)))
            {
                Console.Clear();
                Console.WriteLine("Enter pawn color [w | b]");
            }
           
            var squareInput = GetSquareFromUser();
            if (squareInput.HasValue)
            {
                ShowPawnMovesFromSquare(squareInput.Value);
            }

        }

        private ushort? GetSquareFromUser()
        {
            string square;
            do
            {
                Console.WriteLine("Enter square: (q to quit)");
                square = Console.ReadLine().ToLower();
                if (square == "q")
                {
                    return null;
                }
                if (square.Length == 2)
                {
                    var cFile = char.ToLower(square[0]);
                    var cRank = square[1];
                    var file = Math.Abs('a' - cFile);
                    if (file <= 7 && file >= 0)
                    {
                        if (ushort.TryParse(cRank.ToString(), out ushort rank))
                        {
                            if (rank >= 0 && rank <= 7)
                            {
                                return (ushort)((rank * 8) + file);
                            }
                        }
                    }
                    Console.WriteLine("Invalid input.");
                }
            } while (true);

        }

        private void ShowPawnMovesFromSquare(ushort index)
        {

            var moves = bb.GetMoves((ushort)index, Color.White, 0, 0);
            var board = _displaySvc.PrintBoard(moves, $"Moves from {DisplayService.IndexToSquareDisplay(index)}");
            Console.Clear();
            Console.WriteLine(board);
            Console.WriteLine("Any key to continue.");
            Console.ReadKey();
        }



        private char ShowPawnMenu()
        {
            Console.Clear();
            Console.WriteLine("1\tSee Moves from square");
            Console.WriteLine("2\tSee Attacks from square");
            Console.WriteLine("[q to quit]");
            return char.ToLower(Console.ReadKey().KeyChar);
        }
    }
}