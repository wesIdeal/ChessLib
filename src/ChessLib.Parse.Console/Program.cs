using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN;
using ChessLib.Parse.PGN.Parser;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ChessLib.Data;
using ChessLib.Parse.Tests;

namespace ChessLib.Parse.Console
{
    class Program
    {
        public enum GAMEDB { WithFENSetup, TalSmall, TalLarge, TalMedium, GameWithVariation }
        static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var listenerGames = TestParsingWithListener(GAMEDB.WithFENSetup);
            System.Console.WriteLine($"Listener: Finished {listenerGames.Length} games in {timer.ElapsedMilliseconds} ms.");
            timer.Reset();
            var games = TestParsingWithVisitor(GAMEDB.WithFENSetup);
            timer.Stop();
            System.Console.WriteLine($"Visitor: Finished {games.Length} games in {timer.ElapsedMilliseconds} ms.");

            WriteGame(games, 0);
        }

        private static void WriteGame(Game<MoveStorage>[] games, int i)
        {
            if (i >= games.Length)
            {
                System.Console.WriteLine($"Requested game at index {i} not found. Length is {games.Length}.");
                return;
            }
            var game = games[i];
            var pgnFormatter = new PGNFormatter<MoveStorage>(new PGNFormatterOptions()
            {
                ExportFormat = true
            });
            System.Console.WriteLine(pgnFormatter.BuildPGN(game));
        }

        public static Game<MoveStorage>[] TestParsingWithVisitor(GAMEDB db)
        {
            var dbToUse = GetDbFromEnum(db);
            var _parser = new GameDatabaseVisitor();
            return _parser.Visit(dbToUse).ToArray();
        }
        public static Game<MoveStorage>[] TestParsingWithListener(GAMEDB db)
        {
            PGNParser _parser = new PGNParser();
            _parser.ProgressUpdate += UpdateProgress;
            var dbToUse = GetDbFromEnum(db);
            Debug.WriteLine($"Starting to parse db of size {dbToUse.Length} bytes.");

            return _parser.GetGamesFromPGN(dbToUse).ToArray();
        }

        private static void UpdateProgress(object sender, ParsingUpdateEventArgs e)
        {
            Debug.WriteLine($"\tProgress update: {e.NumberComplete} completed.");
        }

        private static string GetDbFromEnum(GAMEDB db)
        {
            byte[] byteArray = null;
            switch (db)
            {
                case GAMEDB.TalLarge:
                    byteArray = PGNResources.talLarge;
                    break;
                case GAMEDB.TalSmall:
                    byteArray = PGNResources.tal;
                    break;
                case GAMEDB.TalMedium:
                    byteArray = PGNResources.talMedium;
                    break;
                case GAMEDB.GameWithVariation:
                    return PGNResources.GameWithVars;
                case GAMEDB.WithFENSetup:
                    return PGNResources.WithFENSetup;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(db), db, null);
            }

            return Encoding.UTF8.GetString(byteArray);
        }
    }
}
