using System;
using System.IO;
using ChessLib.Core.Types.Enums;
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
        /// <param name="continuationType">Used to write variation markers</param>
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
                move = NewLine + move.TrimStart();
                bufferLength = 0;
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
        PgnMoveBuilder pgnMoveBuilder;
        private bool isBlackMoveNeeded = false;

        public void WriteMove(MovePair<PgnMoveInformation?> move)
        {
            string whiteMove = string.Empty;
            string blackMove = string.Empty;
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
                blackMove = pgnMoveBuilder.BuildMove(move.BlackNode.Value, whiteMove == null);
            }


            var pgnMove = whiteMove + blackMove;
            WriteMove(pgnMove);
        }
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
        public GameMoveFlags ContinuationType { get; }

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
        public PgnMoveInformation(Color activeColor, string moveSan, GameMoveFlags continuationType,
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

    /// <summary>
    ///     Flags the move details as they relate to a game.
    /// </summary>
    [Flags]
    public enum GameMoveFlags
    {
        NullMove = 0,
        MainLine = 1 << 0,
        LastMoveOfContinuation = 1 << 2,
        InitialMove = 1 << 4,
        Variation = 1 << 5,
        BeginVariation = Variation | InitialMove,
        FirstMoveInGame = InitialMove | MainLine,
        LastMoveInVariation = LastMoveOfContinuation | Variation

    }
}