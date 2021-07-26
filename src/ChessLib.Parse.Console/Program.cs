using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using ChessLib.Core;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Parse.PGN;
using ChessLib.Parse.Tests;

namespace ChessLib.Parse.Console
{
    [SuppressMessage("ReSharper", "LocalizableElement")]
    internal class Program
    {
        public enum GameDatabase
        {
            WithFENSetup,
            TalSmall,
            TalLarge,
            TalMedium,
            GameWithVariation,
            PregameComment,
            SymbolNag
        }

        private static int CursorTop => System.Console.CursorTop;

        private static void Main(string[] args)
        {
            var games = TestParsing(GameDatabase.TalLarge);

        }

        public static Game[] TestParsing(GameDatabase db)
        {
            var parser = new PGNParser();
            parser.UpdateProgress += UpdateProgress;
            var dbToUse = GetDbFromEnum(db);
            var timer = new Stopwatch();
            timer.Start();
            var games = parser.GetGamesFromPGNAsync(dbToUse).Result.ToArray();
            timer.Stop();
            System.Console.WriteLine($"Parsing Finished {games.Length} games in {timer.ElapsedMilliseconds} ms.");
            return games;
        }

        public static void TestSpeed()
        {
            var sw = new Stopwatch();
            var pgn = GetDbFromEnum(GameDatabase.TalLarge);
            var oldParsingTimes = new List<long>();
            var numberOfTimes = 1;

            System.Console.WriteLine($"OLD PARSING{Environment.NewLine}");
            for (var i = 0; i < numberOfTimes; i++)
            {
                sw.Restart();
                ParseOldWay(pgn);
                sw.Stop();
                oldParsingTimes.Add(sw.ElapsedMilliseconds);
                System.Console.Write($"\r{i}  /  {numberOfTimes}\t{sw.ElapsedMilliseconds} ms       ");
            }

            System.Console.WriteLine(
                $"{Environment.NewLine}Old:\tTotal:{Math.Round((double)(oldParsingTimes.Sum() / 1000), 2)} secs\t{oldParsingTimes.Average()} avg ms");
        }

        public static void TestParsingNagSymbols()
        {
            var parser = new PGNParser();
            var game = parser.GetGamesFromPGNAsync(PGNResources.MoveNagSymbol).Result;
            var formatter = new PgnFormatter<Move>(new PGNFormatterOptions());
            formatter.BuildPgn(game.First());
        }

        private static GameDatabase GetDatabaseFromArg(string s)
        {
            if (string.IsNullOrWhiteSpace(s) ||
                !Enum.TryParse(typeof(GameDatabase), s, true, out var rv))
            {
                System.Console.WriteLine($"Cannot match {s}. Using {GameDatabase.TalSmall}");
                return GameDatabase.TalSmall;
            }

            return (GameDatabase)rv;
        }

        private static GameDatabase GetDatabaseFromUser()
        {
            var answer = -1;
            var max = 10;
            var min = 0;
            while (answer > max || answer < min)
            {
                System.Console.Clear();
                System.Console.WriteLine("Choose database:");
                var count = 0;
                foreach (var db in (GameDatabase[])Enum.GetValues(typeof(GameDatabase)))
                {
                    System.Console.WriteLine($"{count}\t{db}");
                    max = count++;
                }

                var response = System.Console.ReadLine();
                if (!int.TryParse(response?.Trim(), out answer))
                {
                    answer = -1;
                }
            }

            return (GameDatabase)answer;
        }

        private static GameDatabase GetDatabaseToParse(string[] args)
        {
            GameDatabase database;
            database = args.Length != 0 ? GetDatabaseFromArg(args[0]) : GetDatabaseFromUser();
            System.Console.WriteLine($"Using {database}");
            return database;
        }

        private static string GetDbFromEnum(GameDatabase db)
        {
            byte[] byteArray;
            switch (db)
            {
                case GameDatabase.TalLarge:
                    byteArray = PGNResources.talLarge;
                    break;
                case GameDatabase.TalSmall:
                    byteArray = PGNResources.tal;
                    break;
                case GameDatabase.TalMedium:
                    byteArray = PGNResources.talMedium;
                    break;
                case GameDatabase.GameWithVariation:
                    return PGNResources.GameWithVars;
                case GameDatabase.WithFENSetup:
                    return PGNResources.WithFENSetup;
                case GameDatabase.PregameComment:
                    return PGNResources.PregameComment;
                default:
                    throw new ArgumentOutOfRangeException(nameof(db), db, null);
            }

            return Encoding.UTF8.GetString(byteArray);
        }

        private static void ParseOldWay(string pgn)
        {
            var parser = new PGNParser();
            var game = parser.GetGamesFromPGNAsync(pgn).Result.ToArray();
            System.Console.WriteLine($"Parsed {game.Length} games.");
        }

        private static void UpdateProgress(object sender, ParsingUpdateEventArgs e)
        {
            System.Console.SetCursorPosition(0, CursorTop);
            System.Console.Write(e.Label);
        }

        private static void WriteGame(Game[] games, int i)
        {
            if (i >= games.Length)
            {
                System.Console.WriteLine($"Requested game at index {i} not found. Length is {games.Length}.");
                return;
            }

            var game = games[i];
            var pgnFormatter = new PgnFormatter<Move>(new PGNFormatterOptions
            {
                ExportFormat = true
            });
            var pgn = pgnFormatter.BuildPgn(game);
            System.Console.WriteLine(pgn);
            File.WriteAllText("C:\\temp\\test.pgn", pgn);
        }

        private static void WritePolyglotInfo(Game[] games, int i)
        {
            if (i >= games.Length)
            {
                System.Console.WriteLine($"Requested game at index {i} not found. Length is {games.Length}.");
                return;
            }

            var game = games[0];

            System.Console.WriteLine("Starting Position");


            foreach (var move in game.MainMoveTree.Skip(1).Take(5))
            {
                var hash = Convert.ToString((long)PolyglotHelpers.GetBoardStateHash(game.Board), 16);
                System.Console.WriteLine($"\tHash:{hash}");
                game.TraverseForward();
                var pgMove = Convert.ToString(PolyglotMove.GetEncodedMove(move), 16).PadLeft(4, '0');
                System.Console.WriteLine(move.SAN);

                System.Console.WriteLine($"\tHash:{hash}");
                System.Console.WriteLine($"\tMove:{pgMove}");
            }
        }
    }
}