using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Graphics;
using ChessLib.MagicBitboard;
using ChessLib.Parse.PGN;
using ChessLib.Types.Enums;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Bitboard.Tests.ConsoleApp
{
    class Program
    {
        private const string pgn = @"[Event ""Riga""]
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

        private const string _pgn2 = @"[Event ""Lakeway Spring Open: Michal_Kucera_(1648)_vs_Steve_Watson_(1409)""]
[Site ""https://lichess.org/study/HT6kSiru""]
[Date """"]
[Round """"]
[White """"]
[Black """"]
[Result ""1-0""]
[Annotator ""https://lichess.org/@/MKucera""]
[ECO ""D13""]
[Opening ""English Opening: Caro-Kann Defensive System""]
[UTCDate ""2019.04.15""]
[UTCTime ""18:31:32""]
[Variant ""Standard""]

1. c4 c6 2. Nc3 d5 3. cxd5 cxd5 4. d4 Nf6 5. Nf3 Bf5 6. Qb3 Bc8 7. Bf4 a6 8. Ne5
Nc6 9. Qa4 Bd7 10. Nxd7 Qxd7 11. e3 Rc8 12. Bd3 e6 13. a3 Be7 14. O-O O-O 15.
Qd1 Na5 16. Na4 Rc6 17. Nc5 Bxc5 18. dxc5 Rfc8 19. b4 Nc4 20. Qe2 b5 21. Rfd1
Qb7 22. f3 h6 23. e4 Re8 24. e5 Nh5 25. g3 f6 26. exf6 Nxf6 27. g4 Qf7 28. Bg3
e5 29. Bxc4 dxc4 30. Bxe5 Rce6 31. f4 Nd7 32. Rd6 Qxf4 33. Bxf4 Rxe2 34. Rxd7
Rf8 35. Bg3 c3 36. Rc1 Rf3 37. c6 c2 38. c7 Rc3 39. Rd8+  1-0";

        private const string epPgn = @"[Event ""?""]
[Site ""?""]
[Date ""????.??.??""]
[Round ""?""]
[White ""Idell""]
[Black ""Treadway""]
[Result ""*""]

1. e4 e5 2. Nf3 Nc6 3. Bc4 d6 4. Nc3 Bg4
5. Nxe5 Bxd1 6. Bxf7+ Ke7 7. Nd5# 1-0
";

        static void Main(string[] args)
        {
            var graphics = new Imaging();
            //var parsePgn = ParsePgn.FromFilePath(".\\PGN\\talLarge.pgn");
            var parsePgn = ParsePgn.FromText(pgn);
            //var games = parsePgn.GetGameTexts();
            var games = parsePgn.GetGames<BoardInfo, MoveHashStorage>();
            var game = games[0];
            Console.WriteLine($"Avg time per move:\t{parsePgn.AvgTimePerMove} ms");
            Console.WriteLine($"Avg time per game:\t{parsePgn.AvgTimePerGame} ms");
            Console.WriteLine($"Avg validation time per game:\t{parsePgn.AvgValidationTimePerGame} ms");
            Console.WriteLine($"Total validation time:\t{parsePgn.TotalValidationTime} ms");
            Console.WriteLine($"Total Time:\t{parsePgn.TotalTime} seconds");
            var botvinnik = games.Where(x =>
                (x.TagSection["Black"].Contains("Botvinnik") || x.TagSection["White"].Contains("Botvinnik"))
                && x.TagSection["Date"].Contains("1960"))
                .Where(x => x.TagSection["Round"] == "2");
            game = botvinnik.FirstOrDefault() ?? game;
            //foreach (var game in botvinnik)
            //{
            var bi = new BoardInfo();
            var round = game.TagSection["Round"];
            var black = game.TagSection["Black"];
            var white = game.TagSection["White"];
            var fileName = $"{white}-{black}.gif";
            var fnP = $"{white}-{black}par.gif";


            var nonPTime = 0L;
            var pTime = 0L;
            using (var fsNonParallel = new FileStream($".\\GameGifs\\{fileName}", FileMode.Create, FileAccess.Write))
            using (var fsParallel = new FileStream($".\\GameGifs\\{fnP}", FileMode.Create, FileAccess.Write))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                graphics.MakeAnimationFromMoveTree(fsNonParallel, game, 1, new ImageOptions() { SquareSize = 45 });
                sw.Stop();
                nonPTime = sw.ElapsedMilliseconds;
                sw.Reset();
                sw.Start();
                graphics.MakeAnimationFromMoveTreeParallel(fsParallel, game, 1, new ImageOptions() { SquareSize = 45 });
                sw.Stop();
                pTime = sw.ElapsedMilliseconds;
                Console.WriteLine($"Created and wrote {fileName} in {nonPTime}ms.");
                Console.WriteLine($"Created and wrote {fnP} in {pTime}ms.");
            }
            Console.ReadKey();
        }

        private static void WriteBishopAttacks()
        {
            const string message = "Bishop Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");
            var bishop = new BishopPatterns();

            for (ushort i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                var attack = PieceAttackPatternHelper.BishopAttackMask[rank, file];

                sb.AppendLine(attack.MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", DisplayHelpers.HtmlPieceRepresentations[Color.White][Piece.Bishop], "&#9670;"));
                for (int occupancyIndex = 0; occupancyIndex < bishop.OccupancyAndMoveBoards[i].Length; occupancyIndex++)
                {
                    var occupancy = bishop.OccupancyAndMoveBoards[i][occupancyIndex].Occupancy;
                    var legalMovesForOccupancy = bishop.OccupancyAndMoveBoards[i][occupancyIndex].MoveBoard;
                    var ob = bishop.GetLegalMoves((uint)i, bishop.OccupancyAndMoveBoards[i][occupancyIndex].Occupancy);
                    Debug.Assert(bishop.OccupancyAndMoveBoards[i][occupancyIndex].MoveBoard == ob);
                }
            }
            var html = DisplayHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("BishopMoves.html", html);
        }

        private static void WriteRookAttacks()
        {
            const string message = "Rook Moves/Attacks";
            StringBuilder sb = new StringBuilder(message + "\r\n");

            var rook = new RookPatterns();
            for (ushort i = 0; i < 64; i++)
            {
                ulong attackMask = rook[i];
                for (int occupancyIndex = 0; occupancyIndex < rook.OccupancyAndMoveBoards[i].Length; occupancyIndex++)
                {
                    var occupancy = rook.OccupancyAndMoveBoards[i][occupancyIndex].Occupancy;
                    var legalMovesForOccupancy = rook.OccupancyAndMoveBoards[i][occupancyIndex].MoveBoard;
                    var dtReg = DateTime.Now;
                    var ob = rook.GetLegalMoves((uint)i, occupancy);
                    dtReg = DateTime.Now;
                    var obFromQuery = rook.OccupancyAndMoveBoards[i].FirstOrDefault(x => x.Occupancy == occupancy).MoveBoard;

                    Debug.Assert(legalMovesForOccupancy == ob);
                }
                sb.AppendLine(rook[i].MakeBoardTable(i, $"{i.IndexToSquareDisplay()} {message}", DisplayHelpers.HtmlPieceRepresentations[Color.White][Piece.Rook], "&#9670;"));
            }


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
                sb.AppendLine(PieceAttackPatternHelper.KnightAttackMask[rank, file].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", DisplayHelpers.HtmlPieceRepresentations[Color.White][Piece.Knight], "&#9670;"));
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
                sb.AppendLine(PieceAttackPatternHelper.KingMoveMask[rank, file].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} {message}", DisplayHelpers.HtmlPieceRepresentations[Color.White][Piece.King], "&#9670;"));
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
                if (rank == 0 || rank == 7) continue;
                sb.AppendLine(PieceAttackPatternHelper.PawnAttackMask[Color.White.ToInt()][i].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} White Pawn Attack", DisplayHelpers.HtmlPieceRepresentations[Color.White][Piece.Pawn], "&#9670;"));
            }
            var html = DisplayHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("WhitePawnAttack.html", html);

            sb.Clear();
            for (ushort i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                if (rank == 0 || rank == 7) continue;
                sb.AppendLine(PieceAttackPatternHelper.PawnAttackMask[Color.Black.ToInt()][i].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} Black Pawn Attack", DisplayHelpers.HtmlPieceRepresentations[Color.Black][Piece.Pawn], "&#9670;"));
            }
            html = DisplayHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("BlackPawnAttack.html", html);
            sb.Clear();
            for (ushort i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                if (rank == 0 || rank == 7) continue;
                sb.AppendLine(PieceAttackPatternHelper.PawnMoveMask[Color.White.ToInt()][i].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} White Pawn Move", DisplayHelpers.HtmlPieceRepresentations[Color.White][Piece.Pawn], "&#9678;"));
            }
            html = DisplayHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("WhitePawnMove.html", html);
            sb.Clear();
            for (ushort i = 0; i < 64; i++)
            {
                var file = BoardHelpers.GetFile(i);
                var rank = BoardHelpers.GetRank(i);
                if (rank == 0 || rank == 7) continue;
                sb.AppendLine(PieceAttackPatternHelper.PawnMoveMask[Color.Black.ToInt()][i].MakeBoardTable(i, $"{file.ToString().ToLower()}{rank.ToString()[1]} Black Pawn Move", DisplayHelpers.HtmlPieceRepresentations[Color.Black][Piece.Pawn], "&#9678;"));
            }
            html = DisplayHelpers.PrintBoardHtml(sb.ToString());
            System.IO.File.WriteAllText("BlackPawnMove.html", html);
        }
    }
}
