using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN.Parser.BaseClasses;

namespace ChessLib.Parse.PGN.Parser.Visitor
{
    using GameContext = BaseClasses.PGNParser.PgnGameContext;
    internal class GameVisitor : PGNBaseVisitor<Game<MoveStorage>>
    {
        public Game<MoveStorage> VisitGame(GameContext context, in PGNParserOptions parserOptions)
        {
            var tagVisitor = new TagVisitor();
            var moveVisitor = new MoveVisitor();
            var tags = tagVisitor.GetTagSection(context.tagSection());
            var game = new Game<MoveStorage>(tags);
            moveVisitor.VisitMoveSections(context.moveSection(), parserOptions, ref game);
            game?.GoToInitialState();
            return game;
        }
    }
}