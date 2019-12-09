using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN;
using ChessLib.Parse.PGN.Parser.Visitor;
using ChessLib.Parse.Tests;

namespace ChessLib.Parse.Console
{
    internal class Program
    {
        public enum GameDatabases
        {
            WithFENSetup,
            TalSmall,
            TalLarge,
            TalMedium,
            GameWithVariation
        }

        private static int _cursorTop = System.Console.CursorTop;
        private static void Main(string[] args)
        {
            var database = GameDatabases.TalLarge;
            var vGames = TestParsingVisitor(database);
            var listenerGames = TestParsing(database);
            WriteGame(listenerGames, 0);
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
            System.Console.WriteLine(pgnFormatter.BuildPGN(game));
        }

        public const string DebugCategory = "Game Parsing/Validation";

        public static Game<MoveStorage>[] TestParsing(GameDatabases db)
        {
            var _parser = new PGNParser();
            var dbToUse = GetDbFromEnum(db);
            var timer = new Stopwatch();
            timer.Start();
            var games = _parser.GetGamesFromPGNAsync(dbToUse).Result.ToArray();
            timer.Stop();
            System.Console.WriteLine($"Listener: Finished {games.Length} games in {timer.ElapsedMilliseconds} ms.");
            return games;
        }
        public static Game<MoveStorage>[] TestParsingVisitor(GameDatabases db)
        {
            var dbToUse = GetDbFromEnum(db);
            var dbVisitor = new GameDatabaseVisitor(dbToUse);
            var timer = new Stopwatch();
            timer.Start();
            var games = dbVisitor.GetAllGames().Result.ToArray();
            timer.Stop();
            System.Console.WriteLine($"Visitor: Finished {games.Length} games in {timer.ElapsedMilliseconds} ms.");
            return games;
        }
        private static void UpdateProgress(object sender, ParsingUpdateEventArgs e)
        {
            System.Console.SetCursorPosition(0, _cursorTop);
            System.Console.Write(e.Label);
        }

        private static string GetDbFromEnum(GameDatabases db)
        {
            byte[] byteArray = null;
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
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(db), db, null);
            }

            return Encoding.UTF8.GetString(byteArray);
        }
    }
}