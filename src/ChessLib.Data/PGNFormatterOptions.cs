namespace ChessLib.Data
{
    public class PGNFormatterOptions
    {
        public PGNFormatterOptions()
        {
            IndentVariations = true;
        }

        /// <summary>
        /// Sets the option for the Export Format Standard for PGN. Overrides all other options.
        /// </summary>
        public bool ExportFormat { get; set; }
        public bool NewlineEachMove { get; set; }
        public bool SpaceAfterMoveNumber { get; set; }
        public bool IndentVariations { get; set; }
    }
}