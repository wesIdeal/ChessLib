﻿using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ChessLib.Data;
using ChessLib.Data.Boards;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Interfaces;
using ChessLib.Parse.PGN.Parser;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLib.Parse.PGN
{
    public class ParsePgn
    {


        public ParsePgn()
        {
        }

        public TimeSpan TotalValidationTime;


        public IEnumerable<Game<IMoveText>> GetGameTexts(AntlrInputStream inputStream)
        {
            var listener = new PGNListener();
            PGNLexer lexer = new PGNLexer(inputStream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new PGNParser(tokens);
            var parseTree = parser.parse();
            var walker = new ParseTreeWalker();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            walker.Walk(listener, parseTree);
            sw.Stop();
            Debug.WriteLine($"Parsed {listener.Games.Count()} games in {sw.ElapsedMilliseconds} ms, ({sw.ElapsedMilliseconds / 1000} seconds.)");
            return listener.Games;
        }

        /// <summary>
        /// Gets MoveExt objects from PGN and validates moves while parsing.
        /// </summary>
        /// <returns>Validated Moves</returns>
        public List<Game<MoveStorage>> ParseAndValidateGames(string strGameDatabase, int? MaxGames = null)
        {
            AntlrInputStream stream = new AntlrInputStream(strGameDatabase);
            var games = GetGameTexts(stream);
            return ValidateGames(games);
        }

        /// <summary>
        /// Gets MoveExt objects from PGN and validates moves while parsing.
        /// </summary>
        /// <returns>Validated Moves</returns>
        public List<Game<MoveStorage>> ParseAndValidateGames(FileStream fs)
        {
            AntlrInputStream stream = new AntlrInputStream(fs);
            var games = GetGameTexts(stream);
            return ValidateGames(games);
        }

        /// <summary>
        /// Gets MoveExt objects from PGN and validates moves while parsing.
        /// </summary>
        /// <returns>Validated Moves</returns>
        private List<Game<MoveStorage>> ValidateGames(IEnumerable<Game<IMoveText>> games)
        {
            var rv = new List<Game<MoveStorage>>();
            var sw = new Stopwatch();
            
            sw.Start();
            var gameArray = games as Game<IMoveText>[] ?? games.ToArray();
            Parallel.ForEach(gameArray, (game) =>
             {
                 var fen = game.TagSection.FENStart;
                 var moveTree = ValidateGame(game.MoveSection, fen);
                 rv.Add(new Game<MoveStorage>() { TagSection = game.TagSection, MoveSection = moveTree });
             });
            sw.Stop();
            TotalValidationTime = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
            Debug.WriteLine($"Validated {gameArray.Count()} games in {sw.ElapsedMilliseconds} ms, ({sw.ElapsedMilliseconds / 1000} seconds.)");
            return rv;
        }

        private MoveTree<MoveStorage> ValidateGame(MoveTree<IMoveText> sanGame, string initialFEN)
        {
            var boardInfo = new BoardInfo(initialFEN);
            foreach (var move in sanGame.GetNodeEnumerator())
            {
                var currentFen = boardInfo.CurrentFEN;
                boardInfo.ApplySANMove(move.MoveData.SAN);
                if (move.Variations.Any())
                {
                    var node = boardInfo.MoveTree.LastMove;
                    foreach (var variation in move.Variations)
                    {
                        var validatedVariation = ValidateGame(variation, currentFen);
                        node.AddVariation(validatedVariation);
                    }
                }
            }
            return boardInfo.MoveTree;
        }

    }
}
