namespace ChessLib.Parse.PGN
{
    public class PGNParserOptions
    {
        public PGNParserOptions()
        {
            UpdateFrequency = 100;
        }
        /// <summary>
        /// How many rows to process before an update event is sent.
        /// </summary>
        public int UpdateFrequency { get; set; }
    }
}