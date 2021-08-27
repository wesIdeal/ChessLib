using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ChessLib.Core;
using ChessLib.Core.Types;
using ChessLib.Graphics;
using ChessLib.Parse.PGN;
using ChessLib.Parse.Tests;

namespace ChessLib.ConsoleManualTesting
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var parsePgn = new PGNParser();
            //parsePgn.ProgressUpdate += OnProgressUpdated;
            var sw = new Stopwatch();
            var pgnDb = Encoding.UTF8.GetString(PGNResources.talLarge);
            sw.Start();
            var games = parsePgn.GetGamesFromPGNAsync(pgnDb).Result;
            sw.Stop();
            Console.WriteLine($"Parsed {games.Count()} games in {sw.ElapsedMilliseconds} ms.");
        }

        private static void OnProgressUpdated(object sender, double e)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($@"Progress {e * 100}%                ");
        }

        private static void EvalPosition(Game game)
        {
            // var engineRunner = new EngineRunner();
        }

        private static void MakeGifs(List<Game> games)
        {
            var graphics = new Imaging();
            var game = games[0];
            var botvinnik = games.Where(x =>
                    (x.Tags["Black"].Contains("Botvinnik") || x.Tags["White"].Contains("Botvinnik"))
                    && x.Tags["Date"].Contains("1960"))
                .Where(x => x.Tags["Round"] == "2");
            game = botvinnik.FirstOrDefault() ?? game;

            var bi = new Board();
            var round = game.Tags["Round"];
            var black = game.Tags["Black"];
            var white = game.Tags["White"];

            var fnP = $"{white}-{black}par.gif";

            var nonPTime = 0L;
            var pTime = 0L;
            using (var fsParallel = new FileStream($".\\GameGifs\\{fnP}", FileMode.Create, FileAccess.Write))
            {
                var sw = new Stopwatch();
                sw.Start();
                sw.Stop();
                nonPTime = sw.ElapsedMilliseconds;
                sw.Reset();
                sw.Start();
                graphics.MakeAnimationFromMoveTree(fsParallel, game, 1, new ImageOptions {SquareSize = 80});
                sw.Stop();
                pTime = sw.ElapsedMilliseconds;
                Console.WriteLine($"Created and wrote {fnP} in {pTime}ms.");
            }
        }
    }
}