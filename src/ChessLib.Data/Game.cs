﻿using System;
using System.Collections.Generic;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.MoveRepresentation.NAG;

namespace ChessLib.Data
{
    public enum GameResult { None, WhiteWins, BlackWins, Draw }
    public class Game<TMove> : MoveTraversalService, ICloneable
        where TMove : MoveExt, IEquatable<TMove>
    {
        private GameResult _gameResult;


        public Game() : base(FENHelpers.FENInitial)
        {
            TagSection = new Tags();
            TagSection.SetFen(FENHelpers.FENInitial);
        }

        public List<PgnParsingLog> ParsingLog { get; protected set; }

        public void AddParsingLogItem(ParsingErrorLevel errorLevel, string message, string parseInput = "")
        {
            ParsingLog.Add(new PgnParsingLog(errorLevel, message, parseInput));
        }

        public void AddParsingLogItem(PgnParsingLog logItem)
        {
            ParsingLog.Add(logItem);
        }

        public Game(Tags tags) : base(tags.FENStart)
        {
            TagSection = tags;
        }

        public Game(string fen) : base(fen)
        {
            TagSection = new Tags();
            TagSection.SetFen(fen);
            ParsingLog = new List<PgnParsingLog>();
        }

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

        public string GetPGN()
        {
            var formatter = new PGNFormatter<TMove>(new PGNFormatterOptions());
            return formatter.BuildPGN(this);
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

        public bool IsEqualTo(Game<MoveStorage> otherGame, bool includeVariations = false)
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


        public Game<MoveStorage> SplitFromCurrentPosition(bool copyVariations = false)
        {
            return SplitFromMove(CurrentMoveNode);
        }

        public Game<MoveStorage> SplitFromMove(LinkedListNode<MoveStorage> move, bool copyVariations = false)
        {
            var currentFen = CurrentFEN;
            var moveStack = new Stack<MoveStorage>();
            var currentMove = move;
            moveStack.Push(currentMove.Value);
            while (!(currentMove = GetPreviousNode(currentMove)).Value.IsNullMove)
            {
                moveStack.Push(currentMove.Value);
            }

            var g = new Game<MoveStorage>(InitialFen);

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
            var formatter = new PGNFormatter<TMove>(PGNFormatterOptions.ExportFormatOptions);
            return formatter.BuildPGN(this);
        }

        public object Clone()
        {
            var clonedGame = new Game<TMove>(TagSection);
            foreach (var node in MainMoveTree)
            {
                clonedGame.MainMoveTree.AddLast(node);
            }

            return clonedGame;
        }

        private bool ParseTreesForEquality(LinkedListNode<MoveStorage> gNode, LinkedListNode<MoveStorage> otherNode,
            bool includeVariations)
        {
            var areEqual = true;
            if (gNode.Value.Move != otherNode.Value.Move)
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
                    areEqual &= moveNode.Value.Move == otherMoveNode.Value.Move;
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