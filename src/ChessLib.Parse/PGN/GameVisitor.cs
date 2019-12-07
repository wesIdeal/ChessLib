using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.MoveRepresentation.NAG;
using ChessLib.Parse.PGN.Parser.BaseClasses;

namespace ChessLib.Parse.PGN
{
    public class GameDatabaseVisitor
    {
        public GameDatabaseVisitor()
        {

        }
        public Game<MoveStorage>[] Visit(string strChessDb)
        {
            var rv = new List<Game<MoveStorage>>();
            var context = GetContext(new AntlrInputStream(strChessDb));
            var gameVisitor = new GameVisitor();
            var gameCount = context.pgn_game().Length;
            Stopwatch sw = new Stopwatch();
            Debug.WriteLine($"Parsing {gameCount} games.");
            foreach (var pgnGameContext in context.pgn_game())
            {
                sw.Reset();
                sw.Start();
                var game = gameVisitor.VisitGame(pgnGameContext);
                rv.Add(game);
                sw.Stop();
                Debug.WriteLine($"\tParsed/validated game {rv.Count} of {gameCount} in {sw.ElapsedMilliseconds} ms.");
            }

            return rv.ToArray();
        }

        private Parser.BaseClasses.PGNParser.Pgn_databaseContext GetContext(AntlrInputStream gameStream)
        {
            var lexer = new PGNLexer(gameStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new Parser.BaseClasses.PGNParser(commonTokenStream);
            return parser.pgn_database();
        }
    }

    internal class GameVisitor : PGNBaseVisitor<Game<MoveStorage>>
    {
        public Game<MoveStorage> VisitGame(Parser.BaseClasses.PGNParser.Pgn_gameContext context)
        {
            var tagVisitor = new TagVisitor();
            var moveVisitor = new MoveVisitor();
            var tags = tagVisitor.VisitTagSection(context.tag_section());
            var game = new Game<MoveStorage>(tags);
            moveVisitor.Visit(context.movetext_section(), ref game);
            game.GoToInitialState();
            return game;
        }
    }

    internal class TagVisitor : PGNBaseVisitor<Tags>
    {


        public Tags VisitTagSection(Parser.BaseClasses.PGNParser.Tag_sectionContext ctx)
        {
            var tagSection = new Tags();
            var tagPairCtxs = ctx.tag_pair();
            var strVisitor = new StrVisitor();
            foreach (var tagPairContext in tagPairCtxs)
            {
                var tagPair = strVisitor.VisitTagPair(tagPairContext);
                tagSection.Add(tagPair.Key, tagPair.Value);
            }
            return tagSection;
        }
    }

    internal class MoveVisitor : PGNBaseVisitor<Game<MoveStorage>>
    {
        private bool _nextMoveVariation;
        public void Visit(Parser.BaseClasses.PGNParser.Movetext_sectionContext context, ref Game<MoveStorage> game)
        {
            if (game == null) { throw new ArgumentNullException("Must pass in non-null game to MoveVisitor"); }

            foreach (var child in context.children)
            {
                if (child is Parser.BaseClasses.PGNParser.Element_sequenceContext sequenceContext)
                {
                    VisitMoveSequence(sequenceContext, ref game);
                }
            }

        }
        protected void VisitMoveSequence(Parser.BaseClasses.PGNParser.Element_sequenceContext ctx, ref Game<MoveStorage> game)
        {
            foreach (var child in ctx.children)
            {
                if (child is Parser.BaseClasses.PGNParser.ElementContext elementContext)
                {
                    VisitElement(elementContext, ref game);
                }
                if (child is Parser.BaseClasses.PGNParser.Recursive_variationContext variationContext)
                {
                    _nextMoveVariation = true;
                    VisitMoveSequence(variationContext.element_sequence(), ref game);
                    game.ExitVariation();
                }

            }
        }

        protected void VisitElement(Parser.BaseClasses.PGNParser.ElementContext context, ref Game<MoveStorage> game)
        {
            if (context.san_move() != null)
            {
                var applicationStrategy = MoveApplicationStrategy.ContinueMainLine;
                if (_nextMoveVariation)
                {
                    _nextMoveVariation = false;
                    applicationStrategy = MoveApplicationStrategy.Variation;
                }
                game.ApplySanMove(context.san_move().GetText(),
                    applicationStrategy);
            }

            if (context.nag() != null)
            {
                game.CurrentMoveNode.Value.Annotation = new NumericAnnotation(context.nag().GetText());
            }

            if (context.comment() != null)
            {
                game.CurrentMoveNode.Value.Comment = context.comment().GetText();
            }
        }
    }

    internal class StrVisitor : PGNBaseVisitor<string>
    {

        public KeyValuePair<string, string> VisitTagPair(Parser.BaseClasses.PGNParser.Tag_pairContext context)
        {
            var tagKey = VisitTagKey(context.tag_name());
            var tagVal = VisitTagValue(context.tag_value());
            return new KeyValuePair<string, string>(tagKey, tagVal);
        }

        public string VisitTagKey(Parser.BaseClasses.PGNParser.Tag_nameContext context)
        {
            return context.GetText().Replace("\"", "");
        }

        public string VisitTagValue(Parser.BaseClasses.PGNParser.Tag_valueContext context)
        {
            return context.GetText().Replace("\"", "");
        }
    }
}
