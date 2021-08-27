using System;
using System.IO;
using System.Text;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.GameTree.Traversal;

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
            var whiteComment = string.Empty;
            var blackMove = string.Empty;
            var blackComment = string.Empty;
            if (!move.WhiteNode.HasValue && !move.BlackNode.HasValue)
            {
                throw new ArgumentNullException(nameof(move), "Both white and black's move cannot be null.");
            }

            if (move.WhiteNode.HasValue)
            {
                whiteMove = pgnMoveBuilder.BuildMove(move.WhiteNode.Value, true);
                whiteComment = move.WhiteNode.Value.Comments ?? "";
            }

            if (move.BlackNode.HasValue)
            {
                blackMove = pgnMoveBuilder.BuildMove(move.BlackNode.Value, !move.WhiteNode.HasValue);
                blackComment = move.BlackNode.Value.Comments ?? "";
            }

            if (!string.IsNullOrEmpty(whiteComment))
            {
                WriteMove(whiteMove);
                WriteComment(whiteComment);
                WriteMove(blackMove);
            }
            else
            {
                WriteMove(whiteMove + blackMove);
                WriteComment(blackComment);
            }
        }

        private void WriteComment(string comment)
        {
            comment = comment.Trim();
            if (string.IsNullOrWhiteSpace(comment))
            {
                return;
            }

            var words = ("{" + comment + "}").Split(" ");

            var sb = new StringBuilder();
            foreach (var commentWord in words)
            {
                if (string.IsNullOrWhiteSpace(commentWord))
                {
                    sb.Append(commentWord);
                }
                else
                {
                    sb.Append(" " + commentWord);
                    WriteMove(sb.ToString());
                    sb.Clear();
                }
            }
        }
    }

    public readonly struct PgnMoveInformation
    {
        /// <summary>
        ///     Creates object to hold PGN move information.
        /// </summary>
        /// <param name="activeColor">The side/color that made the move.</param>
        /// <param name="moveSan">The string representation of the move.</param>
        /// <param name="moveNumber">Move number in game</param>
        /// <param name="isLastMove">Is the last move in a continuation</param>
        /// <param name="depthDifferenceFromPrevious">Depth from 0 of variation</param>
        /// <param name="depthDifferenceFromNext">
        ///     Positive if the next move further away from the main branch, Negative if the next
        ///     node is closer to main branch
        /// </param>
        /// <param name="comments">Post move comments.</param>
        /// <param name="nag">The move's Numeric Annotation Glyph</param>
        /// <param name="isFirstGameMove">Is the first move in the game</param>
        /// <remarks>All parameters are trimmed.</remarks>
        public PgnMoveInformation(Color activeColor, string moveSan,
            uint moveNumber, bool isFirstGameMove, bool isLastMove, int depthDifferenceFromPrevious,
            int depthDifferenceFromNext, string comments = "", NumericAnnotation nag = null)

        {
            ColorMakingMove = activeColor;
            IsFirstGameMove = isFirstGameMove;
            IsLastMove = isLastMove;
            VariationDepthFromPrevious = depthDifferenceFromPrevious;
            VariationDepthFromNext = depthDifferenceFromNext;
            MoveNumber = moveNumber.ToString();

            Comments = comments.Trim();
            NAG = nag;
            MoveSan = moveSan.Trim();
        }

        public int VariationDepthFromPrevious { get; }

        public string MoveNumber { get; }
        public string Comments { get; }
        public NumericAnnotation NAG { get; }
        public string MoveSan { get; }


        public int VariationDepthFromNext { get; }

        public Color ColorMakingMove { get; }
        public bool IsFirstGameMove { get; }
        public bool IsLastMove { get; }
        public bool FirstMoveInVariation => VariationDepthFromPrevious > 0;

        public override string ToString()
        {
            var decimalSeparator = ColorMakingMove == Color.Black ? "..." : ".";
            return $"{MoveNumber}{decimalSeparator}{MoveSan}";
        }
    }
}