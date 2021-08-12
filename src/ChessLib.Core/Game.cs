using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Tree;

namespace ChessLib.Core
{
    public class InitialPosition : TreeRoot<PostMoveState>
    {
        public Board Board { get; }
        public InitialPosition(Board initialBoard) : base(new PostMoveState(initialBoard.BoardStateStorage))
        {
            Board = initialBoard;
        }
    }

    public class CurrentPosition : InitialPosition
    {
        public CurrentPosition(Board initialBoard) : base(initialBoard)
        {
        }
    }

  
    public class Game
    {
        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();
        private Tags tags;

        public CurrentPosition CurrentState { get; private set; }


        public InitialPosition InitialPosition { get; }

        public PostMoveState[] NextMoves => CurrentState.Continuations.Select(x => x.Value).ToArray();
        public void AddParsingLogItem(ParsingErrorLevel errorLevel, string Message, string inputFromParser)
        {
            throw new NotImplementedException();
        }

        public void AddNag(NumericAnnotation nag)
        {
            throw new NotImplementedException();
        }

        public int PlyCount => throw new NotImplementedException();
        public Game(string fen, Tags tags = null)
        {
            var board = fenTextToBoard.Translate(fen);
            InitialPosition = new InitialPosition(board);
            CurrentState = (CurrentPosition)InitialPosition;
            Tags = new Tags(fen, tags);
        }

        public Tags Tags { get; }

        /// <summary>
        /// Move to the next continuation. 0 represents the main line continuation.
        /// </summary>
        /// <param name="index">index of variation to move to. [default: 0]</param>
        /// <returns>An object with the current position's information. <see cref="CurrentState"/></returns>
        /// <exception cref="GameException"><see cref="GameError.NoContinuation"/> thrown when already at last move.</exception>
        public CurrentPosition MoveNext(int index = 0)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Move to the previous board state.
        /// </summary>
        /// <returns>An object with the parent (previous) position's information. <see cref="CurrentState"/></returns>
        /// <exception cref="GameException"><see cref="GameError.AtInitialState"/> thrown when already at first move.</exception>
        public Board MovePrevious()
        {
            throw new NotImplementedException();
        }

        public Board ApplyMove(string san, MoveApplicationStrategy moveApplicationStrategy)
        {
            throw new NotImplementedException();
        }





        public void SetComment(string comment)
        {
            throw new NotImplementedException();
        }
    }
}