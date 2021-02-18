using ChessLib.Graphics.TextDisplay;
using ChessLib.MagicBitboard.Bitwise;
using System;
using System.Linq;

namespace ChessLib.MagicBitboard.App
{
    class Program
    {
        static MovingPieceService movingPieceService = new MovingPieceService();
        static char entry;
        static DisplayService ds = new DisplayService();
        static void Main(string[] args)
        {
            StartInterface();
        }

        private static void StartInterface()
        {
            var startingIndex = (ushort)26;
            var file = movingPieceService.GetFileFromIndex(startingIndex);
            Console.WriteLine($"File Coordinates");
            var str = string.Join(", ", movingPieceService.GetSetBits(file).Select(DisplayService.IndexToSquareDisplay));
            Console.WriteLine(str);
            var rotatedRight = movingPieceService.RotateClockwise(file);
            var rotatedLeft = movingPieceService.RotateAntiClockwise(file);

            var strRotatedRight = string.Join(", ", movingPieceService.GetSetBits(rotatedRight).Select(DisplayService.IndexToSquareDisplay));
            var strRotatedLeft = string.Join(", ", movingPieceService.GetSetBits(rotatedLeft).Select(DisplayService.IndexToSquareDisplay));
            Console.WriteLine($"Rotated Right is:{Environment.NewLine}{strRotatedRight}");

            Console.WriteLine($"Rotated Left is:{Environment.NewLine}{strRotatedLeft}");

            //ShowPawnFunctions();
            //while ((entry = ShowMainMenu()) != 'q')
            //{
            //    switch (entry)
            //    {
            //        case '1':
            //            ShowPawnFunctions();
            //            break;
            //        default: break;
            //    }
            //}
        }

        private static void ShowPawnFunctions()
        {
            var pawnTests = new PawnTests();
            pawnTests.ShowPawnMainMenu();
        }





        private static char ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("1\tTest Pawn Methods");
            Console.WriteLine("[q to quit]");
            return char.ToLower(Console.ReadKey().KeyChar);
        }
    }
}
