using System;
using System.Collections.Generic;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Types
{
    public class PGNFormatterOptions
    {
        public static PGNFormatterOptions ExportFormatOptions => new PGNFormatterOptions { ExportFormat = true };

        /// <summary>
        ///     Keeps tags with values equal to '?' if set to true. Otherwise discard them.
        ///     <remarks>This should be left untouched if exporting PGN for other applications.</remarks>
        /// </summary>
        public bool KeepTagsWithUnknownValues { get; set; }

        public AnnotationStyle AnnotationStyle { get; set; }

        /// <summary>
        /// Used to override the export format's designated new line <value>\n</value>.
        /// </summary>
        public string ExportFormatNewLineOverride { get; set; } = "\n";

        /// <summary>
        ///     Sets the option for the Export Format Standard for PGN. Overrides all other options.
        /// </summary>
        public bool ExportFormat
        {
            get
            {
                return NewlineEachMove == false &&
                       SpaceAfterMoveNumber == false &&
                       IndentVariations == false &&
                       ResultOnNewLine == false &&
                       NewlineAfterBlackMove == false;
            }
            set
            {
                NewlineEachMove = false;
                SpaceAfterMoveNumber = false;
                IndentVariations = false;
                ResultOnNewLine = false;
                NewlineAfterBlackMove = false;
            }
        }

        public bool NewlineEachMove { get; set; }
        public bool SpaceAfterMoveNumber { get; set; }
        public bool IndentVariations { get; set; }

        /// <summary>
        ///     Specify which tags to keep when making output PGN.
        ///     Defaults to <see cref="TagKeys.All">TagKeys.All</see>
        /// </summary>
        public TagKeys TagsToKeep { get; set; }

        /// <summary>
        ///     Specify tags other than the standard PGN tags to keep in export. Will be overridden with setting TagsToKeep to
        ///     TagKeys.All
        /// </summary>
        public List<string> OtherTagsToKeep { get; set; }

        public bool KeepAllTags => TagsToKeep.HasFlag(TagKeys.All);
        public bool KeepComments { get; set; }
        public bool ResultOnNewLine { get; set; }
        public bool NewlineAfterBlackMove { get; set; }

        public string NewLine => ExportFormat ? ExportFormatNewLineOverride : Environment.NewLine;
        public PGNFormatterOptions()
        {
            IndentVariations = true;
            KeepComments = true;
            TagsToKeep = TagKeys.All;
            OtherTagsToKeep = new List<string>();
            KeepTagsWithUnknownValues = true;
            AnnotationStyle = AnnotationStyle.PGNSpec;
        }
    }
}