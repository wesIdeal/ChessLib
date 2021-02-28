using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Bitwise;
using File = System.IO.File;

namespace ChessLib.MagicBitboard.MovingPieces
{
    internal class Bishop : SlidingPiece
    {
        public Bishop()
        {
            base.Initialize();
        }

        protected override Func<ulong, ulong>[] DirectionalMethods => new Func<ulong, ulong>[]
        {
            MovingPieceService.ShiftNE,
            MovingPieceService.ShiftNW,
            MovingPieceService.ShiftSE,
            MovingPieceService.ShiftSW
        };

        //ToDo: remove because this is test code.
        public void ShowBlockersFromSquare(ushort squareIndex)
        {
            var moveHeader =
                $"Move board for Bishop on {squareIndex.IndexToSquareDisplay()}\r\n{MoveMask[squareIndex]}";
            var blockingBoards = BlockerBoards[squareIndex].ToList();
            var sb = new StringBuilder(moveHeader);
            sb.Append(MoveMask[squareIndex].MakeBoardTable(moveHeader, "B"));

            foreach (var bb in BlockerBoards[squareIndex])
            {
                var occupancy = bb.Occupancy;
                var moveBoard = MagicBitboard[squareIndex].GetAttacks(occupancy);
                sb.AppendLine(bb.Occupancy.MakeBoardTable($"Occupancy Board ( {bb.Occupancy} )"));
                sb.AppendLine(moveBoard.MakeBoardTable($"Move Board ( {moveBoard} )"));
                sb.AppendLine("<hr />");
            }

            var html = DisplayHelpers.PrintBoardHtml(sb.ToString());
            var filePath = Path.GetTempFileName() + ".html";
            File.WriteAllText(filePath, html);
            Process.Start(new ProcessStartInfo(filePath) {UseShellExecute = true}); // Works ok on windows
        }

        public override ulong GetPsuedoLegalMoves(ushort square, Color playerColor, ulong playerOccupancy,
            ulong opponentOccupancy)
        {
            throw new NotImplementedException();
        }
    }
}