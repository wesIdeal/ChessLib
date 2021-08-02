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

namespace ChessLib.Core
{

    public class Game : MoveNodeIterator, IEquatable<Game>
    {
        /// <summary>
        /// Constructs an empty game, starting in the traditional chess starting position, defined <see cref="BoardConstants.FenStartingPosition"/>
        /// </summary> 
        public Game() : this(BoardConstants.FenStartingPosition)
        {
            CurrentBoard = this;
        }
        /// <summary>
        /// Copy constructor for type <see cref="Game"/>
        /// </summary>
        /// <param name="game"></param>
        public Game(Game game) : base(game)
        {
            CurrentBoard = this;
            TagSection = new Tags(game.TagSection);
            _parsingLog = new List<PgnParsingLog>();
            if (CurrentBoard.Fen != BoardConstants.FenStartingPosition)
            {
                TagSection.SetFen(CurrentBoard.Fen);
            }
        }


        public Game(string fen) : this(FenTextToBoard.Translate(fen))
        {

        }

        /// <summary>
        ///     Construct game from <paramref name="fen" /> and <paramref name="tags" />
        /// </summary>
        /// <param name="fen">Specify a starting position. Defaults to initial position if null.</param>
        /// <param name="tags">Specify tags providing information about the game.</param>
        public Game(string fen, Tags tags) : this(fen)
        {
            TagSection = new Tags(tags);
            if (fen != BoardConstants.FenStartingPosition)
            {
                TagSection.SetFen(fen);
            }
        }

        /// <summary>
        ///     Constructs a game's initial board, setting <paramref name="initialBoard" /> as the game's starting position
        /// </summary>
        /// <param name="initialBoard">Initial board's value, stored as <see cref="StartingBoard" /></param>
        /// <remarks>
        ///     Sets the <see cref="MoveNode.MoveValue" /> as <see cref="Move.NullMove" /> as the indication that this board
        ///     had no moves applied.
        /// </remarks>
        private Game(Board initialBoard) : base(initialBoard, Move.NullMove)
        {
            _parsingLog = new List<PgnParsingLog>();
            StartingBoard = this;
            CurrentBoard = this;
            TagSection ??= new Tags();
            if (initialBoard.CurrentFEN != BoardConstants.FenStartingPosition)
            {
                TagSection.SetFen(initialBoard.CurrentFEN);
            }
        }

        /// <summary>
        ///     Contains the game's initial starting board
        /// </summary>
        public MoveNodeIterator StartingBoard { get; }

        /// <summary>
        ///     Contains the current
        /// </summary>
        public MoveNodeIterator CurrentBoard { get; private set; }

        public IEnumerable<PgnParsingLog> ParsingLog => _parsingLog;

        public Tags TagSection { get; }

        /// <summary>
        ///     PGN string of the game's result.
        /// </summary>
        public string Result
        {
            get
            {
                switch (GameResult)
                {
                    case GameResult.None:
                        return "*";
                    case GameResult.WhiteWins:
                        return "1-0";
                    case GameResult.BlackWins:
                        return "0-1";
                    case GameResult.Draw:
                        return "1/2-1/2";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (value.Replace(" ", ""))
                {
                    case "0-1":
                        GameResult = GameResult.BlackWins;
                        break;
                    case "1-0":
                        GameResult = GameResult.WhiteWins;
                        break;
                    case "1/2-1/2":
                        GameResult = GameResult.Draw;
                        break;
                    default:
                        GameResult = GameResult.None;
                        break;
                }
            }
        }

        /// <summary>
        ///     The result, provided as a <see cref="GameResult" />
        /// </summary>
        public GameResult GameResult
        {
            get => _gameResult;
            set
            {
                _gameResult = value;
                TagSection["Result"] = Result;
            }
        }

        public int PlyCount => MainLine.Count();
        private static readonly SanToMove SanToMove = new SanToMove();
        private static readonly FenTextToBoard FenTextToBoard = new FenTextToBoard();
        private readonly List<PgnParsingLog> _parsingLog;
        private GameResult _gameResult;

        public new string Fen => CurrentBoard.Fen;
        public bool Equals(Game other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            var resultEq = _gameResult == other._gameResult;
            var tagSectionEq = TagSection.SequenceEqual(other.TagSection);
            var baseEq = base.Equals(other);
            return resultEq && tagSectionEq && baseEq;
        }

        protected static Board BoardFromFen(string fen)
        {
            return FenTextToBoard.Translate(fen);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Game)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)_gameResult;
                hashCode = (hashCode * 397) ^ (_parsingLog != null ? _parsingLog.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TagSection != null ? TagSection.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Game left, Game right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Game left, Game right)
        {
            return !Equals(left, right);
        }

        public void ClearParsingLog()
        {
            _parsingLog.Clear();
        }

        /// <summary>
        ///     Will move to the next move in the sequence, if any. If none, this will stay at the last node.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="MoveNodeIterator" /> for the next node. Will return the last node if no other nodes
        ///     exist.
        /// </returns>
        public MoveNodeIterator MoveNext()
        {
            return CurrentBoard = CurrentBoard.Next;
        }

        public MoveNodeIterator MovePrevious()
        {
            return CurrentBoard = Previous ?? CurrentBoard;
        }

        public new MoveNodeIterator Previous
        {
            get
            {
                var previousNode = CurrentBoard.Previous;
                if (previousNode == null)
                {
                    return StartingBoard;
                }
                var previousBoard = CurrentBoard.BoardState.UnapplyMoveFromBoard(previousNode.BoardState, CurrentBoard.Move);
                return new MoveNodeIterator(previousBoard, previousNode);
            }
        }

        public MoveNodeIterator ExitVariation()
        {
            MoveNodeIterator previousNode;
            while ((previousNode = Previous) != null)
            {
                var currentNode = CurrentBoard;
                CurrentBoard = previousNode;
                if (previousNode.Variations.Select(x => x).Contains(currentNode))
                {
                    return CurrentBoard;
                }
            }

            return CurrentBoard;
        }
        /// <summary>
        ///     Applies a short/standard algebraic notation move to the <see cref="MoveTraversalService.CurrentBoard" />"/>
        /// </summary>
        /// <param name="moveText"></param>
        /// <param name="moveApplicationStrategy"></param>
        /// <returns></returns>
        public MoveNodeIterator ApplySanMove(string moveText, MoveApplicationStrategy moveApplicationStrategy)
        {
            var move = SanToMove.GetMoveFromSAN(CurrentBoard.BoardState, moveText);
            var newBoard = CurrentBoard.AddContinuation(move);
            return CurrentBoard = newBoard;
        }



        /// <summary>
        ///     Adds an item to the <see cref="ParsingLog" /> to help detail parsing events / exceptions.
        /// </summary>
        /// <param name="errorLevel"></param>
        /// <param name="message"></param>
        /// <param name="parseInput"></param>
        public void AddParsingLogItem(ParsingErrorLevel errorLevel, string message, string parseInput = "")
        {
            _parsingLog.Add(new PgnParsingLog(errorLevel, message, parseInput));
        }

        /// <summary>
        ///     Adds an item to the <see cref="ParsingLog" /> to help detail parsing events / exceptions.
        /// </summary>
        /// <param name="logItem">The log item to add.</param>
        public void AddParsingLogItem(PgnParsingLog logItem)
        {
            _parsingLog.Add(logItem);
        }

        public string GetPgn()
        {
            var formatter = new PgnFormatter<Move>(new PGNFormatterOptions());
            return formatter.BuildPgn(this);
        }


        //public Game SplitFromCurrentPosition(bool copyVariations = false)
        //{
        //    return SplitFromMove(CurrentMoveNode);
        //}

        //public Game SplitFromMove(LinkedListNode<GameMove> move, bool copyVariations = false)
        //{
        //    var moveStack = new Stack<GameMove>();
        //    var currentMove = move;
        //    moveStack.Push(currentMove.Value);
        //    while (!(currentMove = GetPreviousNode(currentMove)).Value.IsNullMove)
        //    {
        //        moveStack.Push(currentMove.Value);
        //    }

        //    var g = new Game(InitialFen);

        //    while (moveStack.Count > 0)
        //    {
        //        var moveStorage = moveStack.Pop();
        //        g.AddMove(moveStorage);
        //    }

        //    g.GoToInitialState();
        //    return g;
        //}

        public override string ToString()
        {
            var formatter = new PgnFormatter<Move>(PGNFormatterOptions.ExportFormatOptions);
            return formatter.BuildPgn(this);
        }


        public void ApplyNAG(int nag)
        {
            CurrentBoard.Annotation = new NumericAnnotation(nag);
        }



        public bool IsMainLineEqual(Game game2)
        {
            var tagsEq = this.TagSection.Equals(game2.TagSection);
            var currentMainline = MainLine;
            var otherMainLine = game2.MainLine;
            var mainLine = currentMainline.Zip(otherMainLine,
                (c1, c2) =>
                    new { CurrentMove = c1.Move, OtherMove = c2.Move, AreEqual = (c1.Move.Equals(c2.Move)) });
            var mainLineEq = mainLine.All(x => x.AreEqual);
            Debug.WriteLineIf(!mainLineEq,
                $"Main lines differ, starting here at Game 1's move {mainLine.FirstOrDefault(x => !x.AreEqual)?.CurrentMove}");
            return tagsEq && mainLineEq;
        }

        public void GoToFirstMove()
        {
            CurrentBoard = StartingBoard;
        }
    }
}