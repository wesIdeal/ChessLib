using MagicBitboard;
using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using MagicBitboard.SlidingPieces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Bitboard.Tests.ConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {
            var dt = DateTime.Now;
            //Console.WriteLine(PieceAttackPatternHelper.KnightAttackMask[Rank.R1.ToInt(), File.A.ToInt()].PrintBoard("a1 knight Attack", Rank.R1, File.A));
            //Console.WriteLine(PieceAttackPatternHelper.KnightAttackMask[Rank.R4.ToInt(), File.E.ToInt()].PrintBoard("e4 knight Attack", '*'));
            //WritePawnMovesAndAttacks();
            //WriteKingAttacks();
            //WriteKnightAttacks();
            //WriteBishopAttacks();
            //WriteRookAttacks();
            var gameInfo = FENHelpers.BoardInfoFromFen(FENHelpers.InitialFEN);
            var fen = gameInfo.FEN;
            Console.WriteLine($"{gameInfo.PiecesOnBoard[0][Piece.Queen.ToInt()].GetDisplayBits()}");
            Console.WriteLine($"Finished in {DateTime.Now.Subtract(dt).TotalMilliseconds} ms.");
            //Console.ReadKey();
        }

        private static void WriteBishopAttacks()
        {
            const string message = "Bishop Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            var bishop = new BishopPatterns();
            var regMs = new List<double>();
            var arrayMs = new List<double>();
            for (var i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                var attack = PieceAttackPatternHelper.BishopAttackMask[rank.ToInt(), file.ToInt()];

                sb.AppendLine(attack.MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", BoardHelpers.HtmlPieceRepresentations[Color.White][Piece.Bishop], "&#9670;"));
                for (int occupancyIndex = 0; occupancyIndex < bishop.OccupancyAndMoveBoards[i].Length; occupancyIndex++)
                {
                    var occupancy = bishop.OccupancyAndMoveBoards[i][occupancyIndex].Occupancy;
                    var legalMovesForOccupancy = bishop.OccupancyAndMoveBoards[i][occupancyIndex].MoveBoard;
                    var dtReg = DateTime.Now;
                    var ob = bishop.GetLegalMoves((uint)i, bishop.OccupancyAndMoveBoards[i][occupancyIndex].Occupancy);
                    regMs.Add(DateTime.Now.Subtract(dtReg).TotalMilliseconds);
                    dtReg = DateTime.Now;
                    var obFromQuery = bishop.OccupancyAndMoveBoards[i].FirstOrDefault(x => x.Occupancy == occupancy).MoveBoard;
                    arrayMs.Add(DateTime.Now.Subtract(dtReg).TotalMilliseconds);
                    Debug.Assert(bishop.OccupancyAndMoveBoards[i][occupancyIndex].MoveBoard == ob);
                }
            }
            Debug.WriteLine($"Avg time to get legal moves for bishop from magics: {regMs.Average()}");
            Debug.WriteLine($"Avg time to get legal moves for bishop from linq query: {arrayMs.Average()}");

            var html = BoardHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("BishopMoves.html", html);
        }

        private static void WriteRookAttacks()
        {
            const string message = "Rook Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");

            var masks = new List<ulong>();
            var dtStart = DateTime.Now;
            var rook = new RookPatterns();
            var totalMS = (DateTime.Now - dtStart).TotalMilliseconds;
            var regMs = new List<double>();
            var arrayMs = new List<double>();
            for (var i = 0; i < 64; i++)
            {
                ulong attackMask = rook[i];
                for (int occupancyIndex = 0; occupancyIndex < rook.OccupancyAndMoveBoards[i].Length; occupancyIndex++)
                {
                    var occupancy = rook.OccupancyAndMoveBoards[i][occupancyIndex].Occupancy;
                    var legalMovesForOccupancy = rook.OccupancyAndMoveBoards[i][occupancyIndex].MoveBoard;
                    var dtReg = DateTime.Now;
                    var ob = rook.GetLegalMoves((uint)i, occupancy);
                    regMs.Add(DateTime.Now.Subtract(dtReg).TotalMilliseconds);
                    dtReg = DateTime.Now;
                    var obFromQuery = rook.OccupancyAndMoveBoards[i].FirstOrDefault(x => x.Occupancy == occupancy).MoveBoard;
                    arrayMs.Add(DateTime.Now.Subtract(dtReg).TotalMilliseconds);
                    Debug.Assert(legalMovesForOccupancy == ob);
                }
                sb.AppendLine(rook[i].MakeBoardTable(i, $"{i.IndexToSquareDisplay()} {message}", BoardHelpers.HtmlPieceRepresentations[Color.White][Piece.Rook], "&#9670;"));
            }
            var regAvg = regMs.Average();
            Debug.WriteLine($"Avg time to get legal moves for rook from magics: {regAvg}");
            Debug.WriteLine($"Avg time to get legal moves for rook from linq query: {arrayMs.Average()}");

            var html = BoardHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("RookMoves.html", html);
        }




        private static void WriteKnightAttacks()
        {
            const string message = "Knight Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            for (var i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                sb.AppendLine(PieceAttackPatternHelper.KnightAttackMask[rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", BoardHelpers.HtmlPieceRepresentations[Color.White][Piece.Knight], "&#9670;"));
            }
            var html = BoardHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("KnightMoves.html", html);
        }


        private static void WriteKingAttacks()
        {
            const string message = "King Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            for (var i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                sb.AppendLine(PieceAttackPatternHelper.KingMoveMask[rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", BoardHelpers.HtmlPieceRepresentations[Color.White][Piece.King], "&#9670;"));
            }
            var html = BoardHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("KingMoves.html", html);
        }

        private static void WritePawnMovesAndAttacks()
        {
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(PieceAttackPatternHelper.PawnAttackMask[Color.White.ToInt(), rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} White Pawn Attack", BoardHelpers.HtmlPieceRepresentations[Color.White][Piece.Pawn], "&#9670;"));
            }
            var html = BoardHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("WhitePawnAttack.html", html);

            sb.Clear();
            for (var i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(PieceAttackPatternHelper.PawnAttackMask[Color.Black.ToInt(), rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} Black Pawn Attack", BoardHelpers.HtmlPieceRepresentations[Color.Black][Piece.Pawn], "&#9670;"));
            }
            html = BoardHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("BlackPawnAttack.html", html);
            sb.Clear();
            for (var i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(PieceAttackPatternHelper.PawnMoveMask[Color.White.ToInt(), rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} White Pawn Move", BoardHelpers.HtmlPieceRepresentations[Color.White][Piece.Pawn], "&#9678;"));
            }
            html = BoardHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("WhitePawnMove.html", html);
            sb.Clear();
            for (var i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(PieceAttackPatternHelper.PawnMoveMask[Color.Black.ToInt(), rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} Black Pawn Move", BoardHelpers.HtmlPieceRepresentations[Color.Black][Piece.Pawn], "&#9678;"));
            }
            html = BoardHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("BlackPawnMove.html", html);
        }
    }
}
