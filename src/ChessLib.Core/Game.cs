using System;
using System.Collections.Generic;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;

namespace ChessLib.Core
{
    public class Game : MoveTraversalService, ICloneable
    {
        private static readonly SanToMove sanToMove = new SanToMove();
        private GameResult _gameResult;

        public Game() : base(BoardConstants.FenStartingPosition)
        {
            ParsingLog = new List<PgnParsingLog>();
            TagSection = new Tags();
            TagSection.SetFen(BoardConstants.FenStartingPosition);
        }

        public Game(Tags tags) : base(tags?.FENStart)
        {
            ParsingLog = new List<PgnParsingLog>();
            TagSection = tags ?? new Tags();
        }

        public Game(string fen) : base(fen)
        {
            TagSection = new Tags();
            TagSection.SetFen(fen);
            ParsingLog = new List<PgnParsingLog>();
        }

        public List<PgnParsingLog> ParsingLog { get; protected set; }

        public Tags TagSection { get; set; }

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

        public GameResult GameResult
        {
            get => _gameResult;
            set
            {
                _gameResult = value;
                TagSection["Result"] = Result;
            }
        }

        public object Clone()
        {
            var clonedGame = new Game(TagSection);
            foreach (var node in MainMoveTree)
            {
                clonedGame.MainMoveTree.AddLast(node);
            }

            return clonedGame;
        }

        public LinkedListNode<BoardSnapshot> ApplySanMove(string moveText,
            MoveApplicationStrategy moveApplicationStrategy)
        {
            if (moveApplicationStrategy == MoveApplicationStrategy.Variation)
            {
                return ApplySanVariationMove(moveText);
            }

            var move = sanToMove.GetMoveFromSAN(Board, moveText);
            move.SAN = moveText;
            return ApplyMove(move);
        }

        protected LinkedListNode<BoardSnapshot> ApplySanVariationMove(string moveText)
        {
            TraverseBackward();
            var move = sanToMove.GetMoveFromSAN(Board, moveText);
            move.SAN = moveText;
            ValidateMove(move);
            return ApplyValidatedMoveVariation(move);
        }

        public void AddParsingLogItem(ParsingErrorLevel errorLevel, string message, string parseInput = "")
        {
            ParsingLog.Add(new PgnParsingLog(errorLevel, message, parseInput));
        }

        public void AddParsingLogItem(PgnParsingLog logItem)
        {
            ParsingLog.Add(logItem);
        }

        public string GetPgn()
        {
            var formatter = new PgnFormatter<Move>(new PGNFormatterOptions());
            return formatter.BuildPgn(this);
        }

        public bool IsEqualTo(Game otherGame, bool includeVariations = false)
        {
            if (otherGame.PlyCount != PlyCount)
            {
                return false;
            }

            if (otherGame.InitialFen != InitialFen)
            {
                return false;
            }

            return ParseTreesForEquality(MainMoveTree.First, otherGame.MainMoveTree.First, includeVariations);
        }


        public Game SplitFromCurrentPosition(bool copyVariations = false)
        {
            return SplitFromMove(CurrentMoveNode);
        }

        public Game SplitFromMove(LinkedListNode<BoardSnapshot> move, bool copyVariations = false)
        {
            var currentFen = CurrentFEN;
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
                g.ApplyValidatedMove(moveStorage);
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