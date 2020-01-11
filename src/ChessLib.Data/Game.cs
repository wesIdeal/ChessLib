using System;
using System.Collections.Generic;
using System.Linq;
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
            ParsingLog = new List<PgnParsingLog>();
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
            ParsingLog = new List<PgnParsingLog>();
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

        /// <summary>
        /// Strategy for merging games
        /// </summary>
        public enum MergeGameStrategy
        {
            /// <summary>
            /// Merge as ordered - if second is longer than first, add as variation
            /// </summary>
            Ordered,
            /// <summary>
            /// Use longer game as mainline
            /// </summary>
            LongestIsMainLine
        }

        public static Game<TMove> MergeGames(Game<TMove> g1, Game<TMove> g2)
        {
            var moveTree = MergeTrees(g1.MainMoveTree, g2.MainMoveTree);
            return new Game<TMove>()
            {
                MainMoveTree = moveTree
            };
        }

        private static MoveTree MergeTrees(MoveTree to, MoveTree from)
        {
            var rv = new MoveTree(null, to.StartingFEN);
            var nullMoveOffset = to.First.Value.IsNullMove ? 1 : 0;
            var arrTo = to
                .Where(x => !x.IsNullMove).ToList();
            var arrFrom = from
                .Where(x => !x.IsNullMove).ToList();

            var tCount = arrTo.Count;
            var fCount = arrFrom.Count;
            var game = new Game<TMove>(to.StartingFEN) { MainMoveTree = to };
            if (to.StartingFEN == from.StartingFEN)
            {
                var index = 0;
                for (index = 0;
                    index < arrTo.Count && index < tCount && index < fCount;
                    index++)
                {
                    var toMove = arrTo[index];
                    var fromMove = arrFrom[index];
                    if (toMove.Equals(fromMove))
                    {
                        continue;
                    }

                    var foundVariation = false;
                    for (var i = 0; i < toMove.Variations.Count; i++)
                    {
                        var variation = toMove.Variations[i];
                        if (variation.First().Equals(fromMove))
                        {
                            foundVariation = true;
                            var arr = arrFrom.Skip(index);
                            var split = GameHelpers.SplitFromMoveToEnd(variation.StartingFEN, arr);
                            arrTo[index].Variations[i] = MergeTrees(variation, split);
                            break;
                        }
                    }

                    if (!foundVariation)
                    {
                        var split = GameHelpers.SplitFromMoveToEnd(game.CurrentFEN,
                            arrFrom.Skip(index));
                        toMove.Variations.Add(new MoveTree(game.CurrentMoveNode, game.CurrentFEN));
                        var variation = toMove.Variations.Last();
                        foreach (var move in split.Where(x => !x.IsNullMove))
                        {
                            variation.AddMove(move);
                        }
                    }

                    game.TraverseForward();
                }

                if (fCount > tCount)
                {
                    arrTo.AddRange(arrFrom.Skip(index));
                }
                foreach (var moveStorage in arrTo.Where(x => !x.IsNullMove))
                {
                    rv.AddMove(moveStorage);
                }
            }

            return rv;
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