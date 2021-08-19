using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.GameTree;

// ReSharper disable PossibleNullReferenceException
[assembly: InternalsVisibleTo("ChessLib.Parse.Tests")]

namespace ChessLib.Core
{
    public class Game : GameEnumerator, ICloneable
    {
        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();

        public BoardNode InitialNode { get; }

        public virtual PostMoveState[] NextMoves => Current?.Node.Continuations.Select(x => x.Value).ToArray() ??
                                                    throw new Exception("Current can never be null.");

        public GameResult GameResult { get; set; } = GameResult.None;
        public override string ToString()
        {
            var str = CurrentMove == Move.NullMove ? "Initial Board" : CurrentSan;
            return str;
        }

        public string Result
        {
            get
            {
                string rv;
                switch (GameResult)
                {
                    case GameResult.WhiteWins:
                        rv = "1-0";
                        break;
                    case GameResult.BlackWins:
                        rv = "0-1";
                        break;
                    case GameResult.Draw:
                        rv = "1/2-1/2";
                        break;
                    default:
                        rv = "*";
                        break;
                }

                return rv;
            }
            set
            {
                switch (value.Trim())
                {
                    case "1-0":
                        GameResult = GameResult.WhiteWins;
                        break;
                    case "0-1":
                        GameResult = GameResult.BlackWins;
                        break;
                    case "1/2-1/2":
                        GameResult = GameResult.Draw;
                        break;
                    default:
                        GameResult = GameResult.None;
                        break;
                }

                this.Tags.Result = Result;
            }
        }

        public int PlyCount => this.MainLine()?.Count() ?? 0;

        public Tags Tags { get; }
        public List<PgnParsingLog> ParsingLogs { get; set; }

        public NumericAnnotation CurrentAnnotation => Current.Node.Annotation;

        public string CurrentComment
        {
            get => Current.Node.Comment;
            set => Current.Node.Comment = value;
        }

        public Move CurrentMove => Current.Node.Value.MoveValue;
        public string CurrentSan => Current.Node.Value.San;


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

        /// <summary>
        ///     Used to clone <paramref name="game" /> to a new <see cref="Game" />
        /// </summary>
        /// <param name="game">Game to clone</param>
        /// <remarks>Creates a deep copy of <paramref name="game" /> into a new game.</remarks>
        public Game(Game game)
        {
            Tags = new Tags(game.Tags);
            GameResult = game.GameResult;
            ParsingLogs = game.ParsingLogs.Select(p => new PgnParsingLog(p.ParsingErrorLevel, p.Message, p.ParseInput))
                .ToList();
            Result = game.Result;
            InitialNode = (BoardNode)game.InitialNode.Clone();
            Current = InitialNode;
        }

        /// <summary>
        ///     Uses copy constructor to clone a deep copy of <see cref="GameResult" />
        /// </summary>
        /// <returns>An object of type Game containing a clone of the current game.</returns>
        public object Clone()
        {
            return new Game(this);
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
            CurrentAnnotation.ApplyNag(nag);
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
            var currentMoveValue = CurrentMove;

            while (MovePrevious() && FindMoveIndexInContinuations(currentMoveValue) < 1)
            {
                currentMoveValue = CurrentMove;
            }
        }


        public void SetComment(string comment)
        {
            CurrentComment = comment;
        }
    }
}