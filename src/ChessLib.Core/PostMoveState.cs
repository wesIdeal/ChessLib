using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums.NAG;

namespace ChessLib.Core
{

    public class MoveTree : List<PostMoveState>
    {
        public MoveTree(int depth)
        {
            NodeDepth = depth;
        }
        public int NodeDepth { get; internal set; }
    }

    
    /// <summary>
    ///     A move that was applied to a board, resulting in <see cref="BoardState"/>
    /// </summary>
    public readonly struct PostMoveState
    {
        public PostMoveState(uint boardState, ushort moveValue = Move.NullMoveValue, string san = null)
        {
            BoardState = boardState;
            MoveValue = moveValue;
            San = san;
        }

        public uint BoardState { get; }
        public ushort MoveValue { get; }
        public string San { get; }

    }
}