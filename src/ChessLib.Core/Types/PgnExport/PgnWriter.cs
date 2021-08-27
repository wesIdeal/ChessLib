using System;
using System.IO;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.GameTree.Traversal;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core.Types.PgnExport
{
    public sealed class PgnWriter : StringWriter
    {
        public PGNFormatterOptions Options { get; }

        public int IndentLevel
        {
            get => _indentLevel;
            set
            {
                _indentLevel = value;
                indentation = new string(' ', _indentLevel * 4);
            }
        }

        public PgnWriter(PGNFormatterOptions options)
        {
            Options = options;
            IndentLevel = 0;
            bufferLength = 0;
            NewLine = Options.NewLineIndicator;
            pgnMoveBuilder = new PgnMoveBuilder(Options);
        }

        private readonly PgnMoveBuilder pgnMoveBuilder;

        private int _indentLevel;
        private int bufferLength;

        private string indentation = "";


        public void WriteTag(string tag)
        {
            WriteLine(tag);
        }

        private bool IsBufferOverfilledAfterMove(in string move)
        {
            var newBufferLength = bufferLength + move.Length;
            if (newBufferLength >= Options.MaxCharsPerLine)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        ///     Writes PGN move, using buffer to limit line width using <see cref="PGNFormatterOptions.MaxCharsPerLine" />.
        /// </summary>
        /// <param name="move">The move to write.</param>
        public void WriteMove(string move)
        {
            if (move.EndsWith(NewLine))
            {
                bufferLength = 0;
                Write(move);
                WriteIndentation();
            }

            if (move.StartsWith(NewLine))
            {
                bufferLength = 0;
                move = move.Replace(NewLine, $"{NewLine}{indentation}");
            }
            else if (IsBufferOverfilledAfterMove(move))
            {
                var trimmedMove = move.TrimStart();
                move = NewLine + trimmedMove;
                bufferLength = trimmedMove.Length;
            }
            else
            {
                bufferLength += move.Length;
            }

            Write(move);
        }

        private void WriteIndentation()
        {
            bufferLength = indentation.Length;
            Write(indentation);
        }


        public void WriteResult(string result)
        {
            var preResultWhitespace = Options.ResultOnNewLine
                ? Options.NewLineIndicator
                : Options.WhitespaceSeparator.ToString();
            var formattedResult = $"{preResultWhitespace}{result}{NewLine}";
            WriteLine(formattedResult);
        }


        public void WriteNewLine()
        {
            WriteLine();
        }

        public void WriteMove(MovePair move)
        {
            var whiteMove = string.Empty;
            var blackMove = string.Empty;
            if (!move.WhiteNode.HasValue && !move.BlackNode.HasValue)
            {
                throw new ArgumentNullException(nameof(move), "Both white and black's move cannot be null.");
            }

            if (move.WhiteNode.HasValue)
            {
                whiteMove = pgnMoveBuilder.BuildMove(move.WhiteNode.Value, true);
            }

            if (move.BlackNode.HasValue)
            {
                blackMove = pgnMoveBuilder.BuildMove(move.BlackNode.Value, !move.WhiteNode.HasValue);
            }


            var pgnMove = whiteMove + blackMove;
            WriteMove(pgnMove);
        }
    }

    public readonly struct PgnMoveInformation : IHasColorMakingMove
    {
        private readonly int _variationDepth;
        public string MoveNumber { get; }
        public string Comments { get; }
        public string NAG { get; }
        public string MoveSan { get; }


        /// <summary>
        ///     Creates object to hold PGN move information.
        /// </summary>
        /// <param name="activeColor">The side/color that made the move.</param>
        /// <param name="moveSan">The string representation of the move.</param>
        /// <param name="moveNumber">Move number in game</param>
        /// <param name="isLastMove">Is the last move in a continuation</param>
        /// <param name="variationDepth">Depth from 0 of variation</param>
        /// <param name="comments">Post move comments.</param>
        /// <param name="nag">The move's Numeric Annotation Glyph</param>
        /// <param name="isFirstGameMove">Is the first move in the game</param>
        /// <remarks>All parameters are trimmed.</remarks>
        public PgnMoveInformation(Color activeColor, string moveSan,
            string moveNumber, bool isFirstGameMove, bool isLastMove, int variationDepth, bool firstMoveInVariation, string comments = "", string nag = "")

        {
            ColorMakingMove = activeColor;
            IsFirstGameMove = isFirstGameMove;
            IsLastMove = isLastMove;
            this._variationDepth = variationDepth;
            FirstMoveInVariation = firstMoveInVariation;
            MoveNumber = moveNumber.Trim();

            Comments = comments.Trim();
            NAG = nag.Trim();
            MoveSan = moveSan.Trim();
        }

        public Color ColorMakingMove { get; }
        public bool IsFirstGameMove { get; }
        public bool IsLastMove { get;  }
        public bool IsLastMoveInVariation => IsLastMove && _variationDepth > 0;
        public bool FirstMoveInVariation { get; }

        public override string ToString()
        {
            var decimalSeparator = ColorMakingMove == Color.Black ? "..." : ".";
            return $"{MoveNumber}{decimalSeparator}{MoveSan}";
        }
    }
}