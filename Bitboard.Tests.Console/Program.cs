using MagicBitboard;
using MagicBitboard.Enums;
using System;
using System.Collections.Generic;
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
                sb.AppendLine(bb.BishopAttackMask[rank.ToInt(), file.ToInt()].MakeBoardTable(rank, file, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Bishop], "&#9670;"));
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
            for (var i = 0; i < 64; i++)
            {

                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.GetRank(i);
                var dtStart = DateTime.Now;
                var n = bb.RookAttackMask[rank.ToInt(), file.ToInt()].GetAllPermutations();
                var totalMS = (DateTime.Now - dtStart).TotalMilliseconds;
                var occupancyPerms = n.ToList();
                milliseconds += totalMS;
                if (i == 28)
                {
                    masks.AddRange(occupancyPerms);
                    StringBuilder stringBuilder = new StringBuilder();
                    masks.ForEach(x => { stringBuilder.AppendLine(x.MakeBoardTable("Mask for Rook at e4")); });
                    var h = MoveHelpers.PrintBoardHtml(stringBuilder.ToString());
                    System.IO.File.WriteAllText("RookBlocke4.html", h);
                }
                //var any = n.Any(x => x == 4503668447514624);
                sb.AppendLine(bb.RookAttackMask[rank.ToInt(), file.ToInt()].MakeBoardTable(rank, file, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Rook], "&#9670;"));
            }

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
                sb.AppendLine(bb.KnightAttackMask[rank.ToInt(), file.ToInt()].MakeBoardTable(rank, file, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Knight], "&#9670;"));
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
                sb.AppendLine(bb.QueenAttackMask[rank.ToInt(), file.ToInt()].MakeBoardTable(rank, file, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Queen], "&#9670;"));
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
                sb.AppendLine(bb.KingMoveMask[rank.ToInt(), file.ToInt()].MakeBoardTable(rank, file, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.King], "&#9670;"));
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
                sb.AppendLine(bb.PawnAttackMask[Color.White.ToInt(), rank.ToInt(), file.ToInt()].MakeBoardTable(rank, file, $"{file.ToString().ToLower()}{rank.ToString()[1]} White Pawn Attack", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Pawn], "&#9670;"));
            }
            var html = MoveHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("WhitePawnAttack.html", html);

            sb.Clear();
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(bb.PawnAttackMask[Color.Black.ToInt(), rank.ToInt(), file.ToInt()].MakeBoardTable(rank, file, $"{file.ToString().ToLower()}{rank.ToString()[1]} Black Pawn Attack", MoveHelpers.HtmlPieceRepresentations[Color.Black][Piece.Pawn], "&#9670;"));
            }
            html = MoveHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("BlackPawnAttack.html", html);
            sb.Clear();
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(bb.PawnMoveMask[Color.White.ToInt(), rank.ToInt(), file.ToInt()].MakeBoardTable(rank, file, $"{file.ToString().ToLower()}{rank.ToString()[1]} White Pawn Move", MoveHelpers.HtmlPieceRepresentations[Color.White][Piece.Pawn], "&#9678;"));
            }
            html = MoveHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("WhitePawnMove.html", html);
            sb.Clear();
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(bb.PawnMoveMask[Color.Black.ToInt(), rank.ToInt(), file.ToInt()].MakeBoardTable(rank, file, $"{file.ToString().ToLower()}{rank.ToString()[1]} Black Pawn Move", MoveHelpers.HtmlPieceRepresentations[Color.Black][Piece.Pawn], "&#9678;"));
            }
            html = MoveHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("BlackPawnMove.html", html);
        }
    }
}
