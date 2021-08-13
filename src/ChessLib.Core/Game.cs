using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.GameTree;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core
{
    public static class GameHelpers
    {
        public static IEnumerable<INode<PostMoveState>> MainLine(this Game game)
        {
            var mainLine = game.InitialNode.Node.Continuations.MainLine();
            return mainLine;
        }

        private static IEnumerable<INode<PostMoveState>> MainLine(this IEnumerable<INode<PostMoveState>> continuations,
            List<INode<PostMoveState>> aggregator = null)
        {
            aggregator ??= new List<INode<PostMoveState>>();
            var nextMove = continuations.FirstOrDefault();
            if (nextMove == null)
            {
                return aggregator;
            }

            aggregator.Add(nextMove);
            return nextMove.Continuations.MainLine(aggregator);
        }
    }

    public class Game : GameBuilder
    {
        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();

        public BoardNode InitialNode { get; }

        public virtual PostMoveState[] NextMoves => Current?.Node.Continuations.Select(x => x.Value).ToArray() ??
                                                    throw new Exception("Current can never be null.");

        public GameResult GameResult { get; set; }
        public string Result { get; set; }
        public int PlyCount => this.MainLine()?.Count() ?? 0;

        public Tags Tags { get; }
        public List<PgnParsingLog> ParsingLogs { get; set; }

        public Game() : this(BoardConstants.FenStartingPosition, new Tags())
        {
        }

        public Game(string fen, Tags tags = null)
        {
            var board = fenTextToBoard.Translate(fen);
            InitialNode = new BoardNode(board);
            Current = InitialNode;
            Tags = new Tags(fen, tags);
            ParsingLogs = new List<PgnParsingLog>();
        }


        public override void Reset()
        {
            Current = InitialNode;
        }


        public void AddParsingLogItem(ParsingErrorLevel errorLevel, string Message, string inputFromParser = "")
        {
            AddParsingLogItem(new PgnParsingLog(errorLevel, Message, inputFromParser));
        }

        public virtual void AddParsingLogItem(PgnParsingLog logEntry)
        {
            ParsingLogs.Add(logEntry);
        }


        public void AddNag(NumericAnnotation nag)
        {
            Current.Node.Annotation.ApplyNag(nag);
        }

        /// <summary>
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public virtual bool MoveNext(ushort move)
        {
            var index = FindMoveIndexInContinuations(move);
            return index >= 0 && MoveNext(index);
        }

        internal virtual int FindMoveIndexInContinuations(ushort move)
        {
            int i;
            for (i = 0; i < NextMoves.Length; i++)
            {
                if (NextMoves[i].MoveValue == move)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        ///     Exits the current variation, traversing to it's parent variation. If node is on the main line, the current board is
        ///     set to the initial.
        /// </summary>
        public void ExitVariation()
        {
            var currentMoveValue = Current.Node.Value.MoveValue;

            while (MovePrevious() && (FindMoveIndexInContinuations(currentMoveValue)) < 1)
            {
                currentMoveValue = Current.Node.Value.MoveValue;
            }
        }


        public void SetComment(string comment)
        {
            Current.Node.Comment = comment;
        }
    }
}