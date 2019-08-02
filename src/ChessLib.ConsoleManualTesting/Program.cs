using ChessLib.Data;
using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Graphics;
using ChessLib.Parse.PGN;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Bitboard.Tests.ConsoleApp
{
    class Program
    {


        static void Main(string[] args)
        {
            var parsePgn = new ParsePgn();

            using (var fs = new FileStream(".\\PGN\\validGames.pgn", FileMode.Open, FileAccess.Read))
            {
                var games = parsePgn.ParseAndValidateGames(fs);
                Console.WriteLine($"Parsed {games.Count} games in {parsePgn.TotalValidationTime.TotalMilliseconds} ms, ({parsePgn.TotalValidationTime.TotalMilliseconds / 1000} seconds.)");
                EvalPosition(games[0]);
            }
            // Console.ReadKey();
        }

        private static void EvalPosition(Game<MoveStorage> game)
        {
            // var engineRunner = new EngineRunner();
        }

        private static void MakeGifs(List<Game<MoveStorage>> games)
        {
            var graphics = new Imaging();
            var game = games[0];
            var botvinnik = games.Where(x =>
                    (x.TagSection["Black"].Contains("Botvinnik") || x.TagSection["White"].Contains("Botvinnik"))
                    && x.TagSection["Date"].Contains("1960"))
                .Where(x => x.TagSection["Round"] == "2");
            game = botvinnik.FirstOrDefault() ?? game;

            var bi = new BoardInfo();
            var round = game.TagSection["Round"];
            var black = game.TagSection["Black"];
            var white = game.TagSection["White"];

            var fnP = $"{white}-{black}par.gif";

            var nonPTime = 0L;
            var pTime = 0L;
            using (var fsParallel = new FileStream($".\\GameGifs\\{fnP}", FileMode.Create, FileAccess.Write))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                sw.Stop();
                nonPTime = sw.ElapsedMilliseconds;
                sw.Reset();
                sw.Start();
                graphics.MakeAnimationFromMoveTree(fsParallel, game, 1, new ImageOptions() { SquareSize = 80 });
                sw.Stop();
                pTime = sw.ElapsedMilliseconds;
                Console.WriteLine($"Created and wrote {fnP} in {pTime}ms.");
            }
        }
    }
}
