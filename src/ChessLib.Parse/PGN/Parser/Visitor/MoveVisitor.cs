using System;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.MoveRepresentation.NAG;
using ChessLib.Parse.PGN.Parser.BaseClasses;

namespace ChessLib.Parse.PGN.Parser.Visitor
{
    using ElementSequenceContext = Parser.BaseClasses.PGNParser.Element_sequenceContext;
    using TerminationContext = BaseClasses.PGNParser.Game_terminationContext;
    using VariationContext = BaseClasses.PGNParser.Recursive_variationContext;
    internal class MoveVisitor : PGNBaseVisitor<Game<MoveStorage>>
    {
        private bool _nextMoveVariation;
        private int _moveCount;
        private int _plyCount;

        public void Visit(BaseClasses.PGNParser.Movetext_sectionContext context, PGNParserOptions parserOptions,
            ref Game<MoveStorage> game)
        {
            if (game == null) { throw new ArgumentNullException("Must pass in non-null game to MoveVisitor"); }

            foreach (var child in context.children)
            {
                if (child is ElementSequenceContext sequenceContext)
                {
                    VisitMoveSequence(sequenceContext, parserOptions, ref game);
                }
                else if (child is TerminationContext terminationContext)
                {
                    var terminationString = terminationContext.GetText();
                    if (!string.IsNullOrWhiteSpace(terminationString))
                    {
                        game.Result = terminationString;
                    }
                }
                if (parserOptions.LimitPlyCount && _plyCount >= parserOptions.MaximumPlyPerGame)
                {
                    break;
                }
            }

        }
        protected void VisitMoveSequence(ElementSequenceContext ctx, in PGNParserOptions parserOptions,
            ref Game<MoveStorage> game)
        {
            foreach (var child in ctx.children)
            {
                if (child is Parser.BaseClasses.PGNParser.ElementContext elementContext)
                {
                    VisitElement(elementContext, ref game);
                }
                if (child is VariationContext variationContext && !parserOptions.IgnoreVariations)
                {
                    _nextMoveVariation = true;
                    VisitMoveSequence(variationContext.element_sequence(), parserOptions, ref game);
                    game.ExitVariation();
                }

                if (parserOptions.LimitPlyCount && _plyCount >= parserOptions.MaximumPlyPerGame)
                {
                    break;
                }
            }
        }

        protected void VisitElement(BaseClasses.PGNParser.ElementContext context, ref Game<MoveStorage> game)
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
                _plyCount++;
            }

            if (context.nag() != null)
            {
                game.CurrentMoveNode.Value.Annotation = new NumericAnnotation(context.nag().GetText());
            }

            if (context.comment() != null)
            {
                game.AddComment(context.comment().GetText().Trim('{', '}').Trim());
            }
        }
    }
}