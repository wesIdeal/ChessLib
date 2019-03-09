using MagicBitboard;
using MagicBitboard.Enums;
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
            var bb = new MagicBitboard.Bitboard();


            //Console.WriteLine(bb.KnightAttackMask[Rank.R1.ToInt(), File.A.ToInt()].PrintBoard("a1 knight Attack", Rank.R1, File.A));
            //Console.WriteLine(bb.KnightAttackMask[Rank.R4.ToInt(), File.E.ToInt()].PrintBoard("e4 knight Attack", '*'));
            WritePawnMovesAndAttacks(bb);
            WriteKingAttacks(bb);
            WriteQueenAttacks(bb);
            WriteKnightAttacks(bb);
            WriteBishopAttacks(bb);
            WriteRookAttacks(bb);
            var br = new BoardRepresentation("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            var board = br[Color.White, Piece.Knight].MakeBoardTable("Initial White Knight Position", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Knight]);
            var html = MoveHelpers.PrintBoardHtml(board);
            System.IO.File.WriteAllText("InitialWhiteKnights.html", html);

            var fullBoard = br.MakeBoardTable();
            html = MoveHelpers.PrintBoardHtml(fullBoard);
            System.IO.File.WriteAllText("initialBoard.html", html);

            board = br[Color.Black, Piece.Pawn].MakeBoardTable("Initial Black Pawns Position", MoveHelpers.HtmlPieceRepresentations[Color.Black][Piece.Pawn]);
            html = MoveHelpers.PrintBoardHtml(board);
            System.IO.File.WriteAllText("InitialBlackPawns.html", html);
            //Console.ReadKey();
        }

        private static void WriteBishopAttacks(MagicBitboard.Bitboard bb)
        {
            const string message = "Bishop Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.GetRank(i);
                var attack = bb.BishopAttackMask[rank.ToInt(), file.ToInt()];
                sb.AppendLine(attack.MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Bishop], "&#9670;"));
                //var n = bb.BishopAttackMask[rank.ToInt(), file.ToInt()].GetAllPermutations();
                //if (i == 27)
                //{
                //    Debug.WriteLine($"{i.IndexToSquareDisplay()} mask");
                //    Debug.Write(bb.BishopAttackMask[rank.ToInt(), file.ToInt()].GetDisplayBits());
                //    var ob = OccupancyBoards.GetBishopBoards(bb.BishopAttackMask[rank.ToInt(), file.ToInt()], n, i);
                //    string str = $"Blocker Boards Bishop on {i.IndexToSquareDisplay()} \r\n" + bb.BishopAttackMask[rank.ToInt(), file.ToInt()].GetDisplayBits() + "\r\n";
                //    StringBuilder sbBlock = new StringBuilder();
                //    for (int i1 = 0; i1 < ob.BlockerBoards.Length; i1++)
                //    {
                //        if (i1 <= 10)
                //        {
                //            sbBlock.AppendLine(ob.BlockerBoards[i1].BlockerBoard.MakeBoardTable(i, $"{i.IndexToSquareDisplay()} Blocker", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Bishop], "&#9670;"));
                //            sbBlock.AppendLine(ob.BlockerBoards[i1].MoveBoard.MakeBoardTable(i, $"{i.IndexToSquareDisplay()} Move", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Bishop], "&#9670;")+"<br/><br/>");
                //        }
                //        str += ob.ToString(i1);
                //    }
                //    var blockHtml = MoveHelpers.PrintBoardHtml(sbBlock.ToString());
                //    System.IO.File.WriteAllText("BishopBlockerBoards.d4.txt", str);
                //    System.IO.File.WriteAllText("BishopBlockerBoards.d4.html", blockHtml);
                //}
            }
            var html = MoveHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("BishopMoves.html", html);
        }

        private static void WriteRookAttacks(MagicBitboard.Bitboard bb)
        {
            const string message = "Rook Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            var milliseconds = (double)0;
            var masks = new List<ulong>();
            var dtStart = DateTime.Now;
            var Rook = new RookPatterns();
            var totalMS = (DateTime.Now - dtStart).TotalMilliseconds;
            var regMs = new List<double>();
            var hashMs = new List<double>();
            for (var i = 0; i < 64; i++)
            {

                ulong attackMask = Rook[i];



                for (int occupancyIndex = 0; occupancyIndex < Rook.OccupancyAndMoveBoards[i].Length; occupancyIndex++)
                {
                    var dtReg = DateTime.Now;
                    var ob = Rook.GetLegalMoves((uint)i, Rook.OccupancyAndMoveBoards[i][occupancyIndex].Occupancy);
                    regMs.Add(DateTime.Now.Subtract(dtReg).TotalMilliseconds);
                    var dtHash = DateTime.Now;
                    var obHashed = Rook.GetHashedLegalMoves((uint)i, Rook.OccupancyAndMoveBoards[i][occupancyIndex].Occupancy);
                    hashMs.Add(DateTime.Now.Subtract(dtHash).TotalMilliseconds);
                    Debug.Assert(obHashed == ob);
                    Debug.Assert(Rook.OccupancyAndMoveBoards[i][occupancyIndex].MoveBoard == obHashed);
                }

                //System.IO.File.WriteAllText("RookBlockerBoardsE4.txt", str);


                //var any = n.Any(x => x == 4503668447514624);
                sb.AppendLine(Rook[i].MakeBoardTable(i, $"{i.IndexToSquareDisplay()} {message}", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Rook], "&#9670;"));
            }
            var regAvg = regMs.Average();
            var hashAvg = hashMs.Average();
            Debug.WriteLine($"Regular Avg: {regAvg}\r\nHashed Avg: {hashAvg}");
            Debug.WriteLine($"{(regAvg > hashAvg ? "Hashed" : "Regular")} was faster by {Math.Abs(regAvg - hashAvg)} ms.");
            var html = MoveHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("RookMoves.html", html);
        }
        private static void WriteKnightAttacks(MagicBitboard.Bitboard bb)
        {
            const string message = "Knight Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.GetRank(i);
                sb.AppendLine(bb.KnightAttackMask[rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Knight], "&#9670;"));
            }
            var html = MoveHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("KnightMoves.html", html);
        }

        private static void WriteQueenAttacks(MagicBitboard.Bitboard bb)
        {
            const string message = "Queen Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.GetRank(i);
                sb.AppendLine(bb.QueenAttackMask[rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Queen], "&#9670;"));
            }
            var html = MoveHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("QueenMoves.html", html);
        }

        private static void WriteKingAttacks(MagicBitboard.Bitboard bb)
        {
            const string message = "King Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.GetRank(i);
                sb.AppendLine(bb.KingMoveMask[rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.King], "&#9670;"));
            }
            var html = MoveHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("KingMoves.html", html);
        }

        private static void WritePawnMovesAndAttacks(MagicBitboard.Bitboard bb)
        {
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(bb.PawnAttackMask[Color.White.ToInt(), rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} White Pawn Attack", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Pawn], "&#9670;"));
            }
            var html = MoveHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("WhitePawnAttack.html", html);

            sb.Clear();
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(bb.PawnAttackMask[Color.Black.ToInt(), rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} Black Pawn Attack", MoveHelpers.HtmlPieceRepresentations[Color.Black][Piece.Pawn], "&#9670;"));
            }
            html = MoveHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("BlackPawnAttack.html", html);
            sb.Clear();
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(bb.PawnMoveMask[Color.White.ToInt(), rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} White Pawn Move", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Pawn], "&#9678;"));
            }
            html = MoveHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("WhitePawnMove.html", html);
            sb.Clear();
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(bb.PawnMoveMask[Color.Black.ToInt(), rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} Black Pawn Move", MoveHelpers.HtmlPieceRepresentations[Color.Black][Piece.Pawn], "&#9678;"));
            }
            html = MoveHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("BlackPawnMove.html", html);
        }
    }
}
