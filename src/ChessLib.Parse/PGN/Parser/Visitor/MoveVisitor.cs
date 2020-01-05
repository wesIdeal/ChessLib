using System;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.MoveRepresentation.NAG;
using ChessLib.Parse.PGN.Parser.BaseClasses;

namespace ChessLib.Parse.PGN.Parser.Visitor
{
    using ElementSequenceContext = BaseClasses.PGNParser.ElementSequenceContext;
    using TerminationContext = BaseClasses.PGNParser.GameTerminationContext;
    using VariationContext = BaseClasses.PGNParser.RecursiveVariationContext;

    internal class MoveVisitor : PGNBaseVisitor<Game<MoveStorage>>
    {
        private bool _foundGame;
        private bool _nextMoveVariation;
        private int _plyCount;

        public void VisitMoveSections(BaseClasses.PGNParser.MoveSectionContext context, PGNParserOptions parserOptions,
            ref Game<MoveStorage> game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            foreach (var child in context.children)
            {
                if (child is ElementSequenceContext sequenceContext)
                {
                    VisitMoveSequence(sequenceContext, parserOptions, ref game);
                }
                else if (child is TerminationContext terminationContext && game != null)
                {
                    var terminationString = terminationContext.GetText();
                    if (!string.IsNullOrWhiteSpace(terminationString))
                    {
                        game.Result = terminationString;
                    }
                }
            }

            if (parserOptions.UseFenFilter && !_foundGame)
            {
                game = null;
            }
        }

        protected MoveStorage VisitElement(BaseClasses.PGNParser.ElementContext context, PGNParserOptions parserOptions,
            ref Game<MoveStorage> game)
        {
            if (context.sanMove() != null)
            {
                var applicationStrategy = MoveApplicationStrategy.ContinueMainLine;
                if (_nextMoveVariation)
                {
                    _nextMoveVariation = false;
                    applicationStrategy = MoveApplicationStrategy.Variation;
                }
                _plyCount++;
                return game.ApplySanMove(context.sanMove().GetText(),
                    applicationStrategy).Value;
            }

            if (context.nag() != null)
            {
                game.CurrentMoveNode.Value.Annotation = new NumericAnnotation(context.nag().GetText());
            }

            if (context.comment() != null)
            {
                game.AddComment(context.comment().GetText().Trim('{', '}').Trim());
            }

            return null;
        }

        protected MoveStorage VisitMoveSequence(ElementSequenceContext ctx, in PGNParserOptions parserOptions,
            ref Game<MoveStorage> game)
        {
            MoveStorage move = null;
            foreach (var child in ctx.children)
            {
                if (child is BaseClasses.PGNParser.ElementContext elementContext)
                {
                    move = VisitElement(elementContext, parserOptions, ref game);
                    if (move != null && parserOptions.FilteringApplied)
                    {
                        if (parserOptions.UseFenFilter && !ValidatePositionFilter(parserOptions, move))
                        {
                            game = null;
                            break;
                        }

                        if (parserOptions.LimitPlyCount)
                        {
                            if (!ValidatePlyCountLimit(parserOptions))
                            {
                                break;
                            }
                        }
                    }
                }

                if (child is VariationContext variationContext && !parserOptions.IgnoreVariations)
                {
                    _nextMoveVariation = true;
                    VisitMoveSequence(variationContext.elementSequence(), parserOptions, ref game);
                    game.ExitVariation();
                }

                if (parserOptions.LimitPlyCount && _plyCount >= parserOptions.MaximumPlyPerGame)
                {
                    break;
                }
            }

            return move;
        }

        /// <summary>
        /// Validates ply-count limit for parsing
        /// </summary>
        /// <param name="parserOptions"></param>
        /// <returns>true if count hasn't been exceeded</returns>
        private bool ValidatePlyCountLimit(in PGNParserOptions parserOptions)
        {
            return !(_plyCount >= parserOptions.MaximumPlyPerGame);
        }

        private bool ValidatePositionFilter(PGNParserOptions parserOptions, MoveStorage move)
        {
            if (!_foundGame)
            {
                if (move.BoardStateHash == parserOptions.BoardStateSearchHash)
                {
                    _foundGame = true;
                }
                else if (_plyCount >= parserOptions.FenPlyMoveLimit)
                {
                    return false;
                }
            }
            return true;
        }
    }
}