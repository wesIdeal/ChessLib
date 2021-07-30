using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;

namespace ChessLib.Core
{
    public class Game : MoveTraversalService, IEquatable<Game>
    {
        public bool Equals(Game other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            var resultEq = _gameResult == other._gameResult;
            var parsingLogEq = ParsingLog.SequenceEqual(other.ParsingLog);
            var tagSectionEq = TagSection.SequenceEqual(other.TagSection);
            var baseEq = base.Equals(other);
            return resultEq && parsingLogEq && tagSectionEq && baseEq;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Game) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) _gameResult;
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

        private static readonly SanToMove SanToMove = new SanToMove();
        private GameResult _gameResult;
        private readonly List<PgnParsingLog> _parsingLog;


        /// <summary>
        /// Copies <paramref name="game"/>'s objects into a new Game object.
        /// </summary>
        /// <param name="game"></param>
        public Game(Game game) : base(game)
        {
            TagSection = new Tags(game.TagSection);
            _parsingLog = new List<PgnParsingLog>(game.ParsingLog);
        }

        private Game(string fen) : base(fen)
        {
            if (fen != BoardConstants.FenStartingPosition)
            {
                TagSection.SetFen(fen);
            }
        }
        /// <summary>
        /// Construct game from <paramref name="fen"/> and <paramref name="tags"/>
        /// </summary>
        /// <param name="fen">Specify a starting position. Defaults to initial position if null.</param>
        /// <param name="tags">Specify tags providing information about the game.</param>
        public Game(string fen = null, Tags tags = null) : base(fen)
        {
            TagSection = tags ?? new Tags();
            
            _parsingLog = new List<PgnParsingLog>();
        }

        public IEnumerable<PgnParsingLog> ParsingLog => _parsingLog;

        public void ClearParsingLog()
        {
            _parsingLog.Clear();
        }

        public Tags TagSection { get; }

        /// <summary>
        /// PGN string of the game's result.
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
        /// The result, provided as a <see cref="GameResult"/>
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


        /// <summary>
        /// Applies a short/standard algebraic notation move to the <see cref="MoveTraversalService.CurrentBoard"/>"/>
        /// </summary>
        /// <param name="moveText"></param>
        /// <param name="moveApplicationStrategy"></param>
        /// <returns></returns>
        public LinkedListNode<BoardSnapshot> ApplySanMove(string moveText, MoveApplicationStrategy moveApplicationStrategy)
        {
            if (moveApplicationStrategy == MoveApplicationStrategy.Variation)
            {
                return ApplySanVariationMove(moveText);
            }

            var move = SanToMove.GetMoveFromSAN(CurrentBoard, moveText);

            return AddMove(move);
        }

        /// <summary>
        /// Used by <see cref="ApplySanMove"/> to apply a short/standard algebraic notation move as a variation to the <see cref="MoveTraversalService.CurrentBoard"/>
        /// </summary>
        /// <param name="moveText"></param>
        /// <returns></returns>
        protected LinkedListNode<BoardSnapshot> ApplySanVariationMove(string moveText)
        {
            var move = SanToMove.GetMoveFromSAN(CurrentBoard, moveText);
            move.SAN = moveText;
            return AddMove(move, MoveApplicationStrategy.Variation);
        }

        /// <summary>
        /// Adds an item to the <see cref="ParsingLog"/> to help detail parsing events / exceptions.
        /// </summary>
        /// <param name="errorLevel"></param>
        /// <param name="message"></param>
        /// <param name="parseInput"></param>
        public void AddParsingLogItem(ParsingErrorLevel errorLevel, string message, string parseInput = "")
        {
            _parsingLog.Add(new PgnParsingLog(errorLevel, message, parseInput));
        }

        /// <summary>
        /// Adds an item to the <see cref="ParsingLog"/> to help detail parsing events / exceptions.
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

        public bool IsEqualTo(Game other, bool includeVariations = false)
        {
            var otherGame = new Game(other);
            var thisGame = new Game(this);
            if (otherGame.PlyCount != thisGame.PlyCount)
            {
                return false;
            }

            if (otherGame.InitialFen != thisGame.InitialFen)
            {
                return false;
            }

            return ParseTreesForEquality(thisGame.MainMoveTree.First, otherGame.MainMoveTree.First, includeVariations);
        }


        public Game SplitFromCurrentPosition(bool copyVariations = false)
        {
            return SplitFromMove(CurrentMoveNode);
        }

        public Game SplitFromMove(LinkedListNode<BoardSnapshot> move, bool copyVariations = false)
        {
            var moveStack = new Stack<BoardSnapshot>();
            var currentMove = move;
            moveStack.Push(currentMove.Value);
            while (!(currentMove = GetPreviousNode(currentMove)).Value.IsNullMove)
            {
                moveStack.Push(currentMove.Value);
            }

            var g = new Game(InitialFen);

            while (moveStack.Count > 0)
            {
                var moveStorage = moveStack.Pop();
                g.AddMove(moveStorage);
            }

            g.GoToInitialState();
            return g;
        }

        public override string ToString()
        {
            var formatter = new PgnFormatter<Move>(PGNFormatterOptions.ExportFormatOptions);
            return formatter.BuildPgn(this);
        }

        private bool ParseTreesForEquality(LinkedListNode<BoardSnapshot> gNode, LinkedListNode<BoardSnapshot> otherNode,
            bool includeVariations)
        {
            var areEqual = true;
            if (gNode.Value.MoveValue != otherNode.Value.MoveValue)
            {
                return false;
            }

            var moveNode = gNode.Next;
            var otherMoveNode = otherNode.Next;
            while (moveNode != null)
            {
                if (otherMoveNode == null)
                {
                    areEqual = false;
                }
                else
                {
                    areEqual &= moveNode.Value.MoveValue == otherMoveNode.Value.MoveValue;
                }

                if (!areEqual)
                {
                    break;
                }

                if (includeVariations)
                {
                    if (moveNode.Value.Variations.Count != otherMoveNode.Value.Variations.Count)
                    {
                        areEqual = false;
                    }
                    else
                    {
                        for (var variationIndex = 0; variationIndex < moveNode.Value.Variations.Count; variationIndex++)
                        {
                            var variation = moveNode.Value.Variations[variationIndex];
                            var otherVariation = otherMoveNode.Value.Variations[variationIndex];
                            areEqual &= ParseTreesForEquality(variation.First, otherVariation.First,
                                true);
                        }
                    }
                }

                moveNode = moveNode.Next;
                otherMoveNode = otherMoveNode.Next;
            }

            return areEqual;
        }

        public void ApplyNAG(int nag)
        {
            CurrentTree.Last.Value.Annotation = new NumericAnnotation(nag);
        }
    }
}