using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN.Parser;
using ChessLib.Parse.PGN.Parser.BaseClasses;

namespace ChessLib.Parse.PGN
{
    public class PGNParser
    {
        private int _gamesProcessedSoFar;

        private ParsingUpdateEventArgs _parsingUpdateArgs;
        private int _totalGamesProcessed;
        public TimeSpan TotalValidationTime;
        public event EventHandler<ParsingUpdateEventArgs> ProgressUpdate;


        public IEnumerable<Game<MoveStorage>> GetGamesFromPGN(string pgn)
        {
            return ParseAndValidateGames(pgn, CancellationToken.None, 1);
        }

        public IEnumerable<Game<MoveStorage>> GetGamesFromPGN(Stream stream)
        {
            return GetGamesFromPGNAsync(stream);
        }

        public IEnumerable<Game<MoveStorage>> GetGamesFromPGNAsync(string pgn)
        {
            return GetValidatedGames(new AntlrInputStream(pgn), CancellationToken.None);
        }

        public IEnumerable<Game<MoveStorage>> GetGamesFromPGNAsync(Stream stream)
        {
            return GetValidatedGames(new AntlrInputStream(stream), CancellationToken.None);
        }

        public IEnumerable<Game<MoveStorage>> GetGamesFromPGNAsync(string pgnText, CancellationToken cancelParseToken)
        {
            return GetValidatedGames(new AntlrInputStream(pgnText), cancelParseToken);
        }

        protected List<Game<MoveStorage>> ParseAndValidateGames(string gameGroup, CancellationToken cancellationToken,
            int? index = null)
        {
            var parseTree = InitializeParsing(new AntlrFileStream(gameGroup), out var walker);
            var listener = new PGNGameDetailListener(cancellationToken);
            listener.BatchParsed += OnDetailBatchProcessed;
            walker.Walk(listener, parseTree);
            listener.Games.ForEach(game => game.GoToInitialState());
            return listener.Games;
        }

        private IEnumerable<string> SplitGamesFromDatabase(AntlrInputStream inputStream,
            CancellationToken cancellationToken, int groupSize = 50)
        {
            var parseTree = InitializeParsing(inputStream, out var walker);
            var listener = new PGNGameListener(inputStream, cancellationToken);
            _parsingUpdateArgs = SendInitialUpdate("Splitting Games From Db", true);
            listener.BatchProcessed += OnGeneralBatchProcessed;
            walker.Walk(listener, parseTree);
            var games = listener.Games;
            _totalGamesProcessed = games.Count;
            var tmpGroupSize = _totalGamesProcessed / 20;
            groupSize = tmpGroupSize < 1 ? 1 : tmpGroupSize;
            var grouped = games
                .Select((x, i) => new { Item = x, Index = i })
                .GroupBy(x => x.Index / groupSize, x => x.Item).ToList();
            Debug.WriteLine($"Made {grouped} groups");
            var lines = grouped.Select((x, i) => $"[{i}].Count is {x.Count()}").ToList();
            Debug.WriteLine(string.Join(Environment.NewLine, lines));
            return grouped.Select(x => string.Join("", x));
        }

        private ParsingUpdateEventArgs SendInitialUpdate(string description, bool isIndeterminate, int maxItems = 0)
        {
            var progress = new ParsingUpdateEventArgs
            { IsIndeterminate = isIndeterminate, Label = description, Maximum = maxItems, NumberComplete = 0 };
            SendUpdate(progress);
            _gamesProcessedSoFar = 0;
            return progress;
        }

        private void SendUpdate(ParsingUpdateEventArgs progress)
        {
            ProgressUpdate?.Invoke(this, progress);
        }

        private static Parser.BaseClasses.PGNParser.ParseContext InitializeParsing(AntlrInputStream inputStream,
            out ParseTreeWalker walker)
        {
            var lexer = new PGNLexer(inputStream);
            lexer.RemoveErrorListeners();
            var parser = new Parser.BaseClasses.PGNParser(new CommonTokenStream(lexer));
            var parseTree = parser.parse();
            walker = new ParseTreeWalker();
            return parseTree;
        }


        protected List<Game<MoveStorage>> GetValidatedGames(AntlrInputStream inputStream,
            CancellationToken cancellationToken)
        {
            var rv = new List<Game<MoveStorage>>();
            try
            {
                var gameGroups = SplitGamesFromDatabase(inputStream, cancellationToken);
                _parsingUpdateArgs = SendInitialUpdate($"Parsing / Validating {_totalGamesProcessed} games", false,
                    _totalGamesProcessed);
                var parallelLoopOptions = new ParallelOptions
                { CancellationToken = cancellationToken, MaxDegreeOfParallelism = 5 };
                parallelLoopOptions.CancellationToken.ThrowIfCancellationRequested();
                Parallel.ForEach(gameGroups, parallelLoopOptions,
                    (group, state) =>
                    {
                        try
                        {
                            var games = ParseAndValidateGames(group, cancellationToken);
                            rv.AddRange(games);
                        }
                        catch (ParseCanceledException)
                        {
                            rv.Clear();
                            state.Stop();
                        }
                    });
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Operation cancelled.");
                rv.Clear();
                throw;
            }

            return rv;
        }

        private void OnGeneralBatchProcessed(object sender, int batchSize)
        {
            _gamesProcessedSoFar += batchSize;
            if (ProgressUpdate != null)
            {
                _parsingUpdateArgs.NumberComplete = _gamesProcessedSoFar;
                SendUpdate(_parsingUpdateArgs);
            }
        }

        private void OnDetailBatchProcessed(object sender, int batchSize)
        {
            _gamesProcessedSoFar += batchSize;
            if (ProgressUpdate != null)
            {
                _parsingUpdateArgs.NumberComplete = _gamesProcessedSoFar;
                SendUpdate(_parsingUpdateArgs);
            }
        }
    }
}