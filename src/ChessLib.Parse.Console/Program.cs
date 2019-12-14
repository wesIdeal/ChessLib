using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN;
using ChessLib.Parse.PGN.Parser.Visitor;
using ChessLib.Parse.Tests;

namespace ChessLib.Parse.Console
{
    [SuppressMessage("ReSharper", "LocalizableElement")]
    internal class Program
    {
        public enum GameDatabases
        {
            WithFENSetup,
            TalSmall,
            TalLarge,
            TalMedium,
            GameWithVariation,
            PregameComment
        }

        private static int CursorTop => System.Console.CursorTop;

        private static void Main(string[] args)
        {
            var database = GetDatabaseToParse(args);
            var listenerGames = TestParsing(database);
            WritePolyglotInfo(listenerGames, 0);
            //WriteGame(listenerGames, 0);
        }

        private static void WritePolyglotInfo(Game<MoveStorage>[] games, int i)
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

        public static Game<MoveStorage>[] TestParsing(GameDatabases db)
        {
            var parser = new PGNParser();
            parser.UpdateProgress += UpdateProgress;
            var dbToUse = GetDbFromEnum(db);
            var timer = new Stopwatch();
            timer.Start();
            var games = parser.GetGamesFromPGNAsync(dbToUse).Result.ToArray();
            timer.Stop();
            System.Console.WriteLine($"Listener: Finished {games.Length} games in {timer.ElapsedMilliseconds} ms.");
            return games;
        }

        private static GameDatabases GetDatabaseToParse(string[] args)
        {
            GameDatabases database;
            database = args.Length != 0 ? GetDatabaseFromArg(args[0]) : GetDatabaseFromUser();
            System.Console.WriteLine($"Using {database}");
            return database;
        }

        private static GameDatabases GetDatabaseFromUser()
        {
            var answer = -1;
            var max = 10;
            var min = 0;
            while (answer > max || answer < min)
            {
                System.Console.Clear();
                System.Console.WriteLine("Choose database:");
                var count = 0;
                foreach (var db in (GameDatabases[])Enum.GetValues(typeof(GameDatabases)))
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

            return (GameDatabases)answer;
        }

        private static GameDatabases GetDatabaseFromArg(string s)
        {
            if (string.IsNullOrWhiteSpace(s) ||
                !Enum.TryParse(typeof(GameDatabases), s, true, out var rv))
            {
                System.Console.WriteLine($"Cannot match {s}. Using {GameDatabases.TalSmall}");
                return GameDatabases.TalSmall;
            }

            return (GameDatabases)rv;
        }

        private static void WriteGame(Game<MoveStorage>[] games, int i)
        {
            if (i >= games.Length)
            {
                System.Console.WriteLine($"Requested game at index {i} not found. Length is {games.Length}.");
                return;
            }

            var game = games[i];
            var pgnFormatter = new PGNFormatter<MoveStorage>(new PGNFormatterOptions
            {
                ExportFormat = true
            });
            var pgn = pgnFormatter.BuildPGN(game);
            System.Console.WriteLine(pgn);
            File.WriteAllText("C:\\temp\\test.pgn", pgn);
        }

        private static void UpdateProgress(object sender, ParsingUpdateEventArgs e)
        {
            System.Console.SetCursorPosition(0, CursorTop);
            System.Console.Write(e.Label);
        }

        private static string GetDbFromEnum(GameDatabases db)
        {
            byte[] byteArray;
            switch (db)
            {
                case GameDatabases.TalLarge:
                    byteArray = PGNResources.talLarge;
                    break;
                case GameDatabases.TalSmall:
                    byteArray = PGNResources.tal;
                    break;
                case GameDatabases.TalMedium:
                    byteArray = PGNResources.talMedium;
                    break;
                case GameDatabases.GameWithVariation:
                    return PGNResources.GameWithVars;
                case GameDatabases.WithFENSetup:
                    return PGNResources.WithFENSetup;
                case GameDatabases.PregameComment:
                    return PGNResources.PregameComment;
                default:
                    throw new ArgumentOutOfRangeException(nameof(db), db, null);
            }

            return Encoding.UTF8.GetString(byteArray);
        }
    }
}