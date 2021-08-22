using System;
using System.IO;
using System.Text;
using ChessLib.Core.Types.Enums;

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

        [Flags]
        public enum ContinuationType
        {
            NULL = 0,
            FirstMove,
            MainLine,
            Variation = 4,
            EndVariation = 8
        }

        public PgnWriter(PGNFormatterOptions options)
        {
            Options = options;
            IndentLevel = 0;
            bufferLength = 0;
            NewLine = Options.NewLineIndicator;
        }

        private int _indentLevel;
        private int bufferLength;

        private bool firstMove = true;
        private string indentation = "";

        public void WriteTag(string tag)
        {
            WriteLine(tag);
        }

        public void WriteMoveObject(PgnMoveInformation move)
        {
            var sb = new StringBuilder();
            BuildMove(sb, move);
            WriteMove(sb.ToString());
            firstMove &= false;
        }


        private bool IsBufferOverfilledAfterMove(in string move)
        {
            var newBufferLength = bufferLength + move.Length;
            if (newBufferLength >= Options.MaxCharsPerLine) //Equal because of separating char (newline or space)
            {
                return true;
            }

            return false;
        }

        private StringBuilder GetMovePrefix(StringBuilder builder, PgnMoveInformation pgnMoveInformation)
        {
            var newLine = pgnMoveInformation.ActiveColor == Color.White && Options.NewlineAfterBlackMove ? NewLine : "";
            BuildMove(builder, pgnMoveInformation);

            return builder;
        }

        private void BuildMove(StringBuilder builder, PgnMoveInformation pgnMoveInformation)
        {
            BuildMoveSeparator(builder, pgnMoveInformation);
            BuildMoveValue(builder, pgnMoveInformation);
            BuildEndOfVariation(builder, pgnMoveInformation);
        }

        private void BuildEndOfVariation(StringBuilder builder, PgnMoveInformation pgnMoveInformation)
        {
            if (IndentLevel != 0 && (pgnMoveInformation.ContinuationType & ContinuationType.EndVariation) != 0)
            {
                builder.Append(Options.GetVariationPadding()).Append(")");
                if (Options.IndentVariations)
                {
                    builder.Append(NewLine);
                }
                else
                {
                    builder.Append(Options.WhitespaceSeparator);
                }
            }
        }

        private void BuildMoveSeparator(StringBuilder builder, PgnMoveInformation pgnMoveInformation)
        {
            if (Options.NewlineAfterBlackMove && pgnMoveInformation.ActiveColor == Color.White)
            {
                builder.Append(NewLine);
            }
            else
            {
                BuildSeparator(builder, pgnMoveInformation);
            }
        }

        private void BuildMoveValue(StringBuilder builder, PgnMoveInformation pgnMoveInformation)
        {
            builder.Append(pgnMoveInformation.MoveNumber)
                .Append(pgnMoveInformation.MoveNumberDecimalMarker)
                .Append(pgnMoveInformation.PostMoveNumberSpace)
                .Append(pgnMoveInformation.MoveSan);
        }

        private void BuildSeparator(StringBuilder builder, PgnMoveInformation pgnMoveInformation)
        {
            if (pgnMoveInformation.ContinuationType == ContinuationType.Variation)
            {
                builder.Append("(").Append(Options.GetVariationPadding());
            }
            else
            {
                if (!firstMove)
                {
                    builder.Append(Options.WhitespaceSeparator);
                }
            }
        }


        /// <summary>
        ///     Writes PGN move, using buffer to limit line width using <see cref="PGNFormatterOptions.MaxCharsPerLine" />.
        /// </summary>
        /// <param name="move">The move to write.</param>
        /// <param name="continuationType">Used to write variation markers</param>
        public void WriteMove(string move)
        {
            if (move.StartsWith(NewLine) || IsBufferOverfilledAfterMove(move))
            {
                bufferLength = 0;
            }

            if (move.EndsWith(NewLine))
            {
                bufferLength = 0;
                Write(move);
                WriteIndentation();
            }
            else
            {
                bufferLength += move.Length;
                Write(move);
            }


            move = $"{move}{Options.WhitespaceSeparator}";
        }

        private void WriteIndentation()
        {
            bufferLength = indentation.Length;
            Write(indentation);
        }

        private static string GetVariationStartText(ContinuationType continuationType)
        {
            var variationText = continuationType == ContinuationType.Variation ? "(" : "";
            return variationText;
        }

        private string GetVariationEndText(ContinuationType continuationType)
        {
            var variationText = (continuationType & ContinuationType.EndVariation) != 0 ? ")" : "";
            variationText += Options.IndentVariations ? NewLine : "";
            return variationText;
        }


        public void WriteResult(string result)
        {
            var preResultWhitespace = Options.ResultOnNewLine
                ? Options.NewLineIndicator
                : Options.WhitespaceSeparator.ToString();
            WriteLine($"{preResultWhitespace}{result}{NewLine}");
        }


        public void WriteNewLine()
        {
            WriteLine();
        }

        public readonly struct PgnMoveInformation
        {
            public string MoveNumber { get; }
            public string MoveNumberDecimalMarker { get; }
            public string PostMoveNumberSpace { get; }
            public string Comments { get; }
            public string NAG { get; }
            public string MoveSan { get; }
            public Color ActiveColor { get; }
            public ContinuationType ContinuationType { get; }

            /// <summary>
            ///     Creates object to hold PGN move information.
            /// </summary>
            /// <param name="activeColor">The side/color that made the move.</param>
            /// <param name="moveSan">The string representation of the move.</param>
            /// <param name="continuationType">Sets the type of move this is. This is so that variations get written properly.</param>
            /// <param name="moveNumber">
            ///     Move number in game
            ///     <remarks>Only set this if the number needs to be written.</remarks>
            /// </param>
            /// <param name="moveNumberDecimalMarker">
            ///     <remarks>Not written if <paramref name="moveNumber" /> is null or whitespace.</remarks>
            ///     In standard PGN, this will either be '...' (Black move) or '.' (White move).
            /// </param>
            /// <param name="postMoveNumberSpace">
            ///     <remarks>Not written if <paramref name="moveNumber" /> is null or whitespace.</remarks>
            ///     Indicates any characters written between the move number and move. (eg. Where 'this' is: 21.[this]Bxf7# )
            /// </param>
            /// <param name="comments">Post move comments.</param>
            /// <param name="nag">The move's Numeric Annotation Glyph</param>
            /// <remarks>All parameters except <paramref name="postMoveNumberSpace" /> are trimmed.</remarks>
            public PgnMoveInformation(Color activeColor, string moveSan, ContinuationType continuationType,
                string moveNumber, string moveNumberDecimalMarker,
                string postMoveNumberSpace, string comments = "", string nag = "")

            {
                ActiveColor = activeColor;
                ContinuationType = continuationType;
                if (!string.IsNullOrWhiteSpace(moveNumber))
                {
                    MoveNumber = moveNumber.Trim();
                    MoveNumberDecimalMarker = moveNumberDecimalMarker.Trim();
                    PostMoveNumberSpace = postMoveNumberSpace;
                }
                else
                {
                    MoveNumber = MoveNumberDecimalMarker = PostMoveNumberSpace = string.Empty;
                }

                Comments = comments.Trim();
                NAG = nag.Trim();
                MoveSan = moveSan.Trim();
            }
        }
    }
}