using ChessLib.Data.Helpers;
using ChessLib.Data.Types;
using MagicBitboard;
using MagicBitboard.SlidingPieces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ChessLib.Graphics;
using ChessLib.Parse.PGN;

namespace Bitboard.Tests.ConsoleApp
{
    class Program
    {
        private static string pgn = @"[Event ""Riga""]
[Site ""Riga""]
[Date ""1949.??.??""]
[EventDate ""?""]
[Round ""?""]
[Result ""1-0""]
[White ""Mikhail Tal""]
[Black ""Josif Israel Zilber""]
[ECO ""C07""]
[WhiteElo ""?""]
[BlackElo ""?""]
[PlyCount ""65""]

1.e4 e6 2.d4 d5 3.Nd2 c5 4.exd5 Qxd5 5.Ngf3 Nc6 6.Bc4 Qh5
7.dxc5 Bxc5 8.Ne4 Nge7 9.Bg5 Qg4 10.Qd3 b6 11.O-O-O O-O 12.Bf6
Qf4+ 13.Kb1 gxf6 14.g3 Qh6 15.g4 Qf4 16.g5 fxg5 17.Nfxg5 Ng6
18.h4 Nb4 19.Qh3 e5 20.Qg2 Bf5 21.h5 Kg7 22.hxg6 h6 23.Bxf7
Rxf7 24.gxf7 hxg5 25.Nxg5 Qxf2 26.Ne6+ Kxf7 27.Qg7+ Kxe6
28.Rh6+ Bg6 29.Qxg6+ Ke7 30.Rh7+ Kf8 31.Qg7+ Ke8 32.Qd7+ Kf8
33.Rh8# 1-0";

        private static string pgn2 = @"[Event ""LAT-ch""]
[Site ""Riga LAT""]
[Date ""1953.??.??""]
[EventDate ""?""]
[Round ""?""]
[Result ""1-0""]
[White ""Mikhail Tal""]
[Black ""Mark Pasman""]
[ECO ""B93""]
[WhiteElo ""?""]
[BlackElo ""?""]
[PlyCount ""80""]

1.e4 c5 2.Nf3 d6 3.d4 cxd4 4.Nxd4 Nf6 5.Nc3 a6 6.f4 e5 7.Nf3
Nbd7 8.Bd3 Be7 9.O-O O-O 10.Kh1 b5 11.a3 Qc7 12.fxe5 dxe5
13.Nh4 Nc5 14.Bg5 Qd8 15.Nf5 Bxf5 16.Rxf5 Nfd7 17.Bxe7 Qxe7
18.Nd5 Qd6 19.Qg4 g6 20.Raf1 f6 21.h4 Kh8 22.R5f3 f5 23.exf5
Qxd5 24.fxg6 Rxf3 25.g7 Kg8 26.Bxh7+ Kxh7 27.Rxf3 Ne4 28.h5
Ndf6 29.Qg6+ Kg8 30.h6 Ra7 31.Kh2 Re7 32.Rh3 Nh7 33.Rd3 Qa8
34.Qxe4 Qxe4 35.Rd8+ Kf7 36.g8=Q+ Kf6 37.Rd6+ Kf5 38.Qg6+ Kf4
39.g3+ Ke3 40.Rd3+ Qxd3 1-0";

        private static string epPgn = @"[Event ""?""]
[Site ""?""]
[Date ""????.??.??""]
[Round ""?""]
[White ""?""]
[Black ""?""]
[Result ""*""]

1.e4 b5 2.f4 b4 3.c4 bxc3 *
";
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
            var graphics = new FENToImage();
            //graphics.SaveBoardBaseImage("boardBase.png");
            //graphics.SaveBoardFromFen("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 2", "InitialBoard.png");
            var parsePgn = ParsePGN.MakeParser(epPgn);
            var m = parsePgn.GetGameObjects();
            var bi = BoardInfo.BoardInfoFromFen(FENHelpers.FENInitial);
            var counter = 0;
            foreach (var move in m[0].MoveSection)
            {
                counter++;
                bi.ApplyMove(move.Move.MoveSAN);
               
            }

            counter = 0;
            foreach(var move in bi.MoveTree)
            {
                graphics.SaveBoardFromFen(move.Move.FEN, $".\\Game1\\game2.halfMove{counter++}.png");
                
            }
            //Console.ReadKey();
        }

        private static void WriteBishopAttacks()
        {
            const string message = "Bishop Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            var bishop = new BishopPatterns();
            var regMs = new List<double>();
            var arrayMs = new List<double>();
            for (ushort i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                var attack = PieceAttackPatternHelper.BishopAttackMask[rank.ToInt(), file.ToInt()];

                sb.AppendLine(attack.MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", DisplayHelpers.HtmlPieceRepresentations[Color.White][Piece.Bishop], "&#9670;"));
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

            var html = DisplayHelpers.PrintBoardHtml(sb.ToString());
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
            for (ushort i = 0; i < 64; i++)
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
                sb.AppendLine(rook[i].MakeBoardTable(i, $"{i.IndexToSquareDisplay()} {message}", DisplayHelpers.HtmlPieceRepresentations[Color.White][Piece.Rook], "&#9670;"));
            }
            var regAvg = regMs.Average();
            Debug.WriteLine($"Avg time to get legal moves for rook from magics: {regAvg}");
            Debug.WriteLine($"Avg time to get legal moves for rook from linq query: {arrayMs.Average()}");

            var html = DisplayHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("RookMoves.html", html);
        }




        private static void WriteKnightAttacks()
        {
            const string message = "Knight Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            for (ushort i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                sb.AppendLine(PieceAttackPatternHelper.KnightAttackMask[rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", DisplayHelpers.HtmlPieceRepresentations[Color.White][Piece.Knight], "&#9670;"));
            }
            var html = DisplayHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("KnightMoves.html", html);
        }


        private static void WriteKingAttacks()
        {
            const string message = "King Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            for (ushort i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                sb.AppendLine(PieceAttackPatternHelper.KingMoveMask[rank.ToInt(), file.ToInt()].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", DisplayHelpers.HtmlPieceRepresentations[Color.White][Piece.King], "&#9670;"));
            }
            var html = DisplayHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("KingMoves.html", html);
        }

        private static void WritePawnMovesAndAttacks()
        {
            StringBuilder sb = new StringBuilder();
            for (ushort i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(PieceAttackPatternHelper.PawnAttackMask[Color.White.ToInt()][i].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} White Pawn Attack", DisplayHelpers.HtmlPieceRepresentations[Color.White][Piece.Pawn], "&#9670;"));
            }
            var html = DisplayHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("WhitePawnAttack.html", html);

            sb.Clear();
            for (ushort i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(PieceAttackPatternHelper.PawnAttackMask[Color.Black.ToInt()][i].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} Black Pawn Attack", DisplayHelpers.HtmlPieceRepresentations[Color.Black][Piece.Pawn], "&#9670;"));
            }
            html = DisplayHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("BlackPawnAttack.html", html);
            sb.Clear();
            for (ushort i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(PieceAttackPatternHelper.PawnMoveMask[Color.White.ToInt()][i].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} White Pawn Move", DisplayHelpers.HtmlPieceRepresentations[Color.White][Piece.Pawn], "&#9678;"));
            }
            html = DisplayHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("WhitePawnMove.html", html);
            sb.Clear();
            for (ushort i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                if (rank == Rank.R1 || rank == Rank.R8) continue;
                sb.AppendLine(PieceAttackPatternHelper.PawnMoveMask[Color.Black.ToInt()][i].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} Black Pawn Move", DisplayHelpers.HtmlPieceRepresentations[Color.Black][Piece.Pawn], "&#9678;"));
            }
            html = DisplayHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("BlackPawnMove.html", html);
        }
    }
}
