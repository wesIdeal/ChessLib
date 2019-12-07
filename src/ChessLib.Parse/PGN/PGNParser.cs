﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
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

        private ParsingUpdateEventArgs _parsingUpdateArgs = new ParsingUpdateEventArgs();
        private int _totalGamesProcessed;
        public TimeSpan TotalValidationTime;
        public event EventHandler<ParsingUpdateEventArgs> ProgressUpdate;


        public IEnumerable<Game<MoveStorage>> GetGamesFromPGN(string pgn)
        {
            return ParseAndValidateGames(new PGNGroup(0, pgn), CancellationToken.None)?.Games ??
                   new List<Game<MoveStorage>>();
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

        private PGNGroup ParseAndValidateGames(PGNGroup gameGroup, CancellationToken cancellationToken)
        {
            var parseTree = InitializeParsing(new AntlrInputStream(gameGroup.PGNData), out var walker);
            var listener = new PGNGameDetailListener(cancellationToken);
            listener.BatchParsed += OnDetailBatchProcessed;
            walker.Walk(listener, parseTree);
            listener.Games.ForEach(game => game.EndGameInitialization());
            return new PGNGroup(gameGroup.Index, gameGroup.PGNData) { Games = listener.Games };
        }

        private IEnumerable<PGNGroup> SplitGamesFromDatabase(AntlrInputStream inputStream,
            CancellationToken cancellationToken, int groupSize)
        {
            var parseTree = InitializeParsing(inputStream, out var walker);
            var listener = new PGNGameListener(inputStream, cancellationToken);
            _parsingUpdateArgs = SendInitialUpdate("Splitting Games From Db", true);
            listener.BatchProcessed += OnGeneralBatchProcessed;
            walker.Walk(listener, parseTree);
            var games = listener.Games;
            listener.BatchProcessed -= OnGeneralBatchProcessed;
            _totalGamesProcessed = games.Count;
            return SplitListOfGameData(games, groupSize);
        }

        private IEnumerable<PGNGroup> SplitListOfGameData(IEnumerable<string> gameData, int groupSize)
        {
            var grouped = gameData
                .Select((x, i) => new { Item = x, Index = i })
                .GroupBy(x => x.Index / groupSize, x => x.Item).ToList();
            return grouped.Select((x, i) => new PGNGroup(i, string.Join("", x)));
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
            parser.Interpreter.PredictionMode = PredictionMode.SLL;
            var parseTree = parser.parse();
            walker = new ParseTreeWalker();
            return parseTree;
        }


        protected List<Game<MoveStorage>> GetValidatedGames(AntlrInputStream inputStream,
            CancellationToken cancellationToken)
        {
            try
            {
                var gameGroups = SplitGamesFromDatabase(inputStream, cancellationToken, 100).ToArray();
                _parsingUpdateArgs = SendInitialUpdate($"Parsing / Validating {_totalGamesProcessed} games", false,
                    _totalGamesProcessed);
                var parallelLoopOptions = new ParallelOptions
                { CancellationToken = cancellationToken, MaxDegreeOfParallelism = 5 };
                Parallel.ForEach(gameGroups, parallelLoopOptions,
                    (group, state) =>
                    {
                        try
                        {
                            var games = ParseAndValidateGames(group, cancellationToken);
                            group.Games = games.Games;
                        }
                        catch (ParseCanceledException)
                        {
                            state.Stop();
                        }
                    });
                return gameGroups.OrderBy(x => x.Index).SelectMany(x => x.Games).ToList();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Operation cancelled.");
                throw;
            }

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
            _parsingUpdateArgs = new ParsingUpdateEventArgs();
            if (ProgressUpdate != null)
            {
                _parsingUpdateArgs.NumberComplete = _gamesProcessedSoFar;
                SendUpdate(_parsingUpdateArgs);
            }
        }
    }
}