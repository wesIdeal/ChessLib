using System;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Types
{
    public class PGNFormatterOptions
    {

        // Defines options available to control exporting tags.
        #region Tag Options
        /// <summary>
        ///     Keeps tags with values equal to '?' if set to true. Otherwise discard them.
        ///     <remarks>This should be left untouched if exporting PGN for other applications.</remarks>
        /// </summary>
        public bool KeepOnlyRequiredTags { get; set; } = false;

        public char KeyValueWhitespaceSeparator { get; set; } = ' ';
        #endregion Tag Options
        /// <summary>
        ///     Alternative character to use for whitespace.
        /// </summary>
        /// <remarks>
        ///     Default is
        ///     <value>" "</value>
        /// </remarks>
        public char WhitespaceSeparator { get; set; } = ' ';

        public AnnotationStyle AnnotationStyle { get; set; }

        /// <summary>
        ///     Used to override the export format's designated new line
        ///     <value>\n</value>
        ///     .
        /// </summary>
        public string NewLineIndicator { get; set; } = Environment.NewLine;



        public bool NewlineEachMove { get; set; }
        public bool SpaceAfterMoveNumber { get; set; }
        public bool IndentVariations { get; set; }


        public bool KeepComments { get; set; }
        public bool ResultOnNewLine { get; set; } = false;
        public bool NewlineAfterBlackMove { get; set; }

        /// <summary>
        ///     Adds a whitespace padding to variations.
        /// </summary>
        /// <remarks>
        ///     When
        ///     <value>true</value>
        ///     , variations will be formatted with spaces: "( ...variation... )", otherwise no padding is added between variation
        ///     and parenthesis.
        /// </remarks>
        public bool IsVariationPadded { get; set; }


        /// <summary>
        ///     Gets and sets ExportFormat, basically resetting all options.
        /// </summary>
        /// <remarks>Practically, the set only affects the options when true. Setting all properties to true would make little sense.</remarks>
        public bool ExportFormat
        {
            get =>
                NewlineEachMove == false &&
                SpaceAfterMoveNumber == false &&
                IndentVariations == false &&
                ResultOnNewLine == false &&
                NewlineAfterBlackMove == false &&
                IsVariationPadded == false &&
                WhitespaceSeparator == ' ';
            set
            {
                if (!value)
                {
                    // do nothing if false
                    return;
                }
                NewlineEachMove = false;
                SpaceAfterMoveNumber = false;
                IndentVariations = false;
                ResultOnNewLine = false;
                NewlineAfterBlackMove = false;
                IsVariationPadded = false;
                WhitespaceSeparator = ' ';
            }
        }


        public int SpacesPerTab { get; set; } = 4;

        public bool IndentComments { get; set; }

        /// <summary>
        /// Defines the maximum width of each line in PGN export
        /// </summary>
        public int MaxCharsPerLine { get; set; } = 80;



        public PGNFormatterOptions()
        {
            this.ResetToExportFormat();
        }


    }

    internal static class PgnFormatterOptionsHelper
    {
        /// <summary>
        ///     Gets the padding (if any) for the variation's first and last moves.
        /// </summary>
        public static string GetVariationPadding(this PGNFormatterOptions options)
        {
            return options.IsVariationPadded ? options.WhitespaceSeparator.ToString() : string.Empty;
        }

        /// <summary>
        /// Resets all options to the default format, PGN Export Specs.
        /// </summary>
        /// <param name="options"></param>
        public static void ResetToExportFormat(this PGNFormatterOptions options)
        {
            options.NewlineEachMove = false;
            options.SpaceAfterMoveNumber = false;
            options.IndentVariations = false;
            options.ResultOnNewLine = false;
            options.NewlineAfterBlackMove = false;
            options.IsVariationPadded = false;
            options.WhitespaceSeparator = ' ';
        }
        /// <summary>
        ///     Gets an indentation string, based on current indentation level / variation depth.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="indent"></param>
        /// <returns>An indentation string, depending on options, to offset a variation or comment.</returns>
        /// <remarks>
        ///     If
        ///     <value>'\t'</value>
        ///     is received, that character is repeated <paramref name="indent" /> times. Otherwise, it is repeated
        ///     <see cref="PGNFormatterOptions.SpacesPerTab" /> * <paramref name="indent" /> times.
        /// </remarks>
        public static string GetIndentation(this PGNFormatterOptions options, int indent)
        {
            if (!options.IndentVariations && !options.IndentComments)
            {
                return "";
            }

            var repetitionCount = indent * options.SpacesPerTab;
            if (options.WhitespaceSeparator == '\t')
            {
                repetitionCount = indent;
            }

            return new string(options.WhitespaceSeparator, repetitionCount);
        }




        /// <summary>
        ///     Gets the Move Separator string that is used to divide move objects in PGN.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="activeColor">Side which made the move.</param>
        /// <returns>
        ///     A <see cref="PGNFormatterOptions.NewLineIndicator" /> or <see cref="PGNFormatterOptions.WhitespaceSeparator" />,
        ///     depending on the options.
        /// </returns>
        public static string GetPostMoveString(this PGNFormatterOptions options, Color activeColor)
        {
            if (options.NewlineAfterBlackMove && activeColor == Color.Black)
            {
                return options.NewLineIndicator;
            }

            return options.WhitespaceSeparator.ToString();
        }


    }
}