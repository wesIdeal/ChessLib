using ChessLib.Data.MoveRepresentation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.UCI.Commands.FromEngine
{
    public class BestMoveResponse : IEngineResponse
    {
        public BestMoveResponse(MoveExt bestMove, MoveExt ponder)
        {
            BestMove = bestMove;
            Ponder = ponder;
        }

        public MoveExt BestMove { get; set; }
        public MoveExt Ponder { get; set; }
    }
}
