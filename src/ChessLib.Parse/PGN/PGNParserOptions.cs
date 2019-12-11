namespace ChessLib.Parse.PGN
{
    public class PGNParserOptions
    {
        private bool _ignoreVariations;

        public const int MaximumParseValue = -1;
        public PGNParserOptions()
        {
            UpdateFrequency = 100;
            GameCountToParse = MaximumParseValue;
            MaximumPlyPerGame = MaximumParseValue;
            IgnoreVariations = false;
        }
        /// <summary>
        /// How many rows to process before an update event is sent. Higher numbers should give marginally better performance.
        /// </summary>
        public int UpdateFrequency { get; set; }

        /// <summary>
        /// Sets maximum games to parse. Set to MaximumParseValue to parse all available games.
        /// </summary>
        public int GameCountToParse { get; set; }

        /// <summary>
        /// Returns true if game count should be limited. 
        /// </summary>
        public bool LimitGameCount => GameCountToParse != MaximumParseValue;

        /// <summary>
        /// Limit ply parsing to number. Set to MaximumParseValue to parse all available moves. Setting this as 0 will get tags only. Will ignore variations.
        /// </summary>
        public int MaximumPlyPerGame { get; set; }

        public bool LimitPlyCount => MaximumPlyPerGame != MaximumParseValue;

        /// <summary>
        /// If true, does not parse Recursive Annotation Variations in game.
        /// </summary>
        public bool IgnoreVariations
        {
            get => _ignoreVariations || LimitPlyCount;
            set => _ignoreVariations = value;
        }
    }
}