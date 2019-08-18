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
using ChessLib.Data;

namespace ChessLib.Parse.Console
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var fStream = File.OpenRead(".\\PGN\\tal.pgn"))
            {
                var parsePGN = new ParsePgn();

                var games = parsePGN.GetGamesFromPGN(fStream);
                System.Console.WriteLine($@"Found {games.Count()} games.");
            }
        }
    }
}
