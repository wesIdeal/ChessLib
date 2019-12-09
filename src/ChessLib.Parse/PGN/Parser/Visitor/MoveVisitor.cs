using System;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.MoveRepresentation.NAG;
using ChessLib.Parse.PGN.Parser.BaseClasses;

namespace ChessLib.Parse.PGN.Parser.Visitor
{
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
}