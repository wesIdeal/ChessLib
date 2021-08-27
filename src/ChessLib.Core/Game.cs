using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.GameTree;
using EnumsNET;

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

        public GameResult GameResult
        {
            get => _gameResult;
            set
            {
                _gameResult = value;
                Tags.GameResult = _gameResult;
            }
        }

        public string Result
        {
            get => GameResult.AsString(EnumFormat.Description);
            set
            {
                var result = Regex.Replace(value, @"\s+", "");
                var resultDictionary = Enums
                    .GetMembers(typeof(GameResult)).SingleOrDefault(x => x.AsString(EnumFormat.Description) == result);
                GameResult = (GameResult)resultDictionary.Value;
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

        private GameResult _gameResult = GameResult.None;

        /// <summary>
        ///     Uses copy constructor to clone a deep copy of <see cref="GameResult" />
        /// </summary>
        /// <returns>An object of type Game containing a clone of the current game.</returns>
        public object Clone()
        {
            return new Game(this);
        }

        public override string ToString()
        {
            var str = CurrentMove.Equals(Move.NullMove) ? "Initial Board" : CurrentSan;
            return str;
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