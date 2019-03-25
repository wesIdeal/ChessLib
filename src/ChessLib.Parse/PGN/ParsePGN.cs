﻿using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN.Parser;
using ChessLib.Parse.PGN.Parser.BaseClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ChessLib.Parse.PGN
{
    public class Game<T> where T : IEquatable<T>
    {
        public Tags TagSection = new Tags();
        public MoveTree<T> MoveSection = new MoveTree<T>(null);
    }
    public class ParsePGN
    {
        public string PgnDatabase;
        AntlrInputStream inputStream;
        public const string GameRegEx = @"(?<pgnGame>\s*(?:\[\s*(?<tagName>\w+)\s*""(?<tagValue>[^""]*)""\s*\]\s*)+(?:(?<moveNumber>\d+)(?<moveMarker>\.|\.{3})\s*(?<moveValue>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?)(?:\s*(?<moveValue2>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?))?\s*(?:\s*\(\s*(?<variation>(?:(?<varMoveNumber>\d+)(?<varMoveMarker>\.|\.{3})\s*(?<varMoveValue>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?)(?:\s*(?<varMoveValue2>(?:[PNBRQK]?[a-h]?[1-8]?x?[a-h][1-8](?:\=[PNBRQK])?|O(-?O){1,2})[\+#]?(\s*[\!\?]+)?))?\s*(?:\((?<varVariation>.*)\s*\)\s*)?(?:\{(?<varComment>[^\}]*?)\}\s*)?)*)\s*\)\s*)*(?:\{(?<comment>[^\}]*?)\}\s*)?)*(?<endMarker>1\-?0|0\-?1|1/2\-?1/2|\*)?\s*)";
        public ParsePGN(string pgnPath)
        {
            PgnDatabase = File.ReadAllText(pgnPath);
            SanitizePgnFile();

        }

        private void SanitizePgnFile()
        {
            var whiteSpace = "(\\s)+";
            var openParen = "(\\()";
            var closeParn = "(\\))+";
            var regOpenParen = $"{whiteSpace}(?<var>{openParen})";
            var regCloseParn = $"{whiteSpace}(?<var>{closeParn}){whiteSpace}";
            PgnDatabase = Regex.Replace(PgnDatabase, @"((\r\n)|\n){2,}", "##BREAK##");
            PgnDatabase = PgnDatabase.Replace(Environment.NewLine, " ");
            PgnDatabase = PgnDatabase.Replace("##BREAK##", Environment.NewLine);
            PgnDatabase = Regex.Replace(PgnDatabase, @"(\ {2,})", " ");
            PgnDatabase = Regex.Replace(PgnDatabase, @"(?<b>\S)(?<p>\))", @"${b} ${p}");
        }


        public void GetMovesFromPGN()
        {
            var games = GetGameObjects();
        }

        private List<Game<MoveText>> GetGameObjects()
        {
            var splitPgn = Regex.Matches(PgnDatabase, GameRegEx);
            var games = new List<Game<MoveText>>();
            foreach (Match game in splitPgn)
            {
                var listener = new PGNListener();
                inputStream = new AntlrInputStream(game.Value);
                PGNLexer lexer = new PGNLexer(inputStream);
                var tokens = new CommonTokenStream(lexer);
                var parser = new PGNParser(tokens);
                var parseTree = parser.parse();
                var walker = new ParseTreeWalker();
                walker.Walk(listener, parseTree);
                games.Add(listener.Game);
            }
            return games;
        }


    }
}