using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Parse.PGN.Parser.BaseClasses;

namespace ChessLib.Parse.PGN.Parser.Visitor
{
    using GameContext = BaseClasses.PGNParser.Pgn_gameContext;
    internal class GameVisitor : PGNBaseVisitor<Game<MoveStorage>>
    {
        public Game<MoveStorage> VisitGame(GameContext context)
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
}