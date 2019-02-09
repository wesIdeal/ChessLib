using MagicBitboard;
using MagicBitboard.Enums;
using System;
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
            //Console.ReadKey();
        }

        private static void WriteBishopAttacks(MagicBitboard.Bitboard bb)
        {
            const string message = "Bishop Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.Rank(i);
                sb.AppendLine(bb.BishopAttackMask[rank.ToInt(), file.ToInt()].MakeBoardTable(rank, file, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", "&#9815;", "&#9670;"));
            }
            var html = MoveHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("BishopMoves.html", html);
        }

        private static void WriteRookAttacks(MagicBitboard.Bitboard bb)
        {
            const string message = "Rook Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.Rank(i);
                sb.AppendLine(bb.RookAttackMask[rank.ToInt(), file.ToInt()].MakeBoardTable(rank, file, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", "&#9814;", "&#9670;"));
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
                var rank = MoveHelpers.Rank(i);
                sb.AppendLine(bb.KnightAttackMask[rank.ToInt(), file.ToInt()].MakeBoardTable(rank, file,$"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", "&#9816;", "&#9670;"));
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
                var rank = MoveHelpers.Rank(i);
                sb.AppendLine(bb.QueenAttackMask[rank.ToInt(), file.ToInt()].PrintBoard($"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", '*'));
            }
            System.IO.File.WriteAllText("QueenMoves.txt", sb.ToString());
        }

        private static void WriteKingAttacks(MagicBitboard.Bitboard bb)
        {
            const string message = "King Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.Rank(i);
                sb.AppendLine(bb.KingMoveMask[rank.ToInt(), file.ToInt()].PrintBoard($"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", '*'));
            }
            System.IO.File.WriteAllText("KingMoves.txt", sb.ToString());
        }

        private static void WritePawnMovesAndAttacks(MagicBitboard.Bitboard bb)
        {
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.Rank(i);
                sb.AppendLine(bb.PawnAttackMask[Color.White.ToInt(), rank.ToInt(), file.ToInt()].PrintBoard($"{file.ToString().ToLower()}{rank.ToString()[1]} White Pawn Attack", '*'));
            }
            System.IO.File.WriteAllText("WhitePawnAttack.txt", sb.ToString());
            sb.Clear();
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.Rank(i);
                sb.AppendLine(bb.PawnAttackMask[Color.Black.ToInt(), rank.ToInt(), file.ToInt()].PrintBoard($"{file.ToString().ToLower()}{rank.ToString()[1]} Black Pawn Attack", '*'));
            }
            System.IO.File.WriteAllText("BlackPawnAttack.txt", sb.ToString());
            sb.Clear();
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.Rank(i);
                sb.AppendLine(bb.PawnMoveMask[Color.White.ToInt(), rank.ToInt(), file.ToInt()].PrintBoard($"{file.ToString().ToLower()}{rank.ToString()[1]} White Pawn Attack", '*'));
            }
            System.IO.File.WriteAllText("WhitePawnMove.txt", sb.ToString());
            sb.Clear();
            for (var i = 0; i < 64; i++)
            {
                var file = MoveHelpers.GetFile(i);
                var rank = MoveHelpers.Rank(i);
                sb.AppendLine(bb.PawnMoveMask[Color.Black.ToInt(), rank.ToInt(), file.ToInt()].PrintBoard($"{file.ToString().ToLower()}{rank.ToString()[1]} Black Pawn Attack", '*'));
            }
            System.IO.File.WriteAllText("BlackPawnMove.txt", sb.ToString());
        }
    }
}
