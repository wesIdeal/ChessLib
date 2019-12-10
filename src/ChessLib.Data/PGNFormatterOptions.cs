using System.Collections.Generic;

namespace ChessLib.Data
{
    public enum AnnotationStyle { PGNSpec, Symbolic }
    public class PGNFormatterOptions
    {
        public PGNFormatterOptions()
        {
            IndentVariations = true;
            KeepComments = true;
            TagsToKeep = TagKeys.All;
            OtherTagsToKeep = new List<string>();
            KeepTagsWithUnknownValues = true;
            AnnotationStyle = AnnotationStyle.PGNSpec;
        }

        /// <summary>
        /// Keeps tags with values equal to '?' if set to true. Otherwise discard them.
        /// <remarks>This should be left untouched if exporting PGN for other applications.</remarks>
        /// </summary>
        public bool KeepTagsWithUnknownValues { get; set; }

        public AnnotationStyle AnnotationStyle { get; set; }

        /// <summary>
        /// Sets the option for the Export Format Standard for PGN. Overrides all other options.
        /// </summary>
        public bool ExportFormat { get; set; }
        public bool NewlineEachMove { get; set; }
        public bool SpaceAfterMoveNumber { get; set; }
        public bool IndentVariations { get; set; }
        /// <summary>
        /// Specify which tags to keep when making output PGN.
        /// Defaults to <see cref="TagKeys.All">TagKeys.All</see>
        /// </summary>
        public TagKeys TagsToKeep { get; set; }
        /// <summary>
        /// Specify tags other than the standard PGN tags to keep in export. Will be overridden with setting TagsToKeep to TagKeys.All
        /// </summary>
        public List<string> OtherTagsToKeep { get; set; }
        public bool KeepAllTags => TagsToKeep.HasFlag(TagKeys.All);
        public bool KeepComments { get; set; }
    }
}