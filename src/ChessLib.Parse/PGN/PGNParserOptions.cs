using System.Diagnostics;
using ChessLib.Core;
using ChessLib.Core.Types.Helpers;
using ChessLib.Data;
using ChessLib.Data.Helpers;

namespace ChessLib.Parse.PGN
{
    public class PGNParserOptions
    {
        private string _fenFilter;
        private FEN _fenObject;
        private bool _ignoreVariations;

        /// <summary>
        ///     Constructs new PGNParserOptions class
        /// </summary>
        /// <param name="updateProgressFrequency">Parse this number of games before calling update event</param>
        /// <param name="maxGameCount">Parse at the maximum this many games</param>
        /// <param name="maxPlyCountPerGame">Only parse this many ply (half-moves) of each game</param>
        /// <param name="ignoreVariations">true to not parse variations</param>
        public PGNParserOptions(int updateProgressFrequency = 100, int? maxGameCount = null,
            int? maxPlyCountPerGame = null,
            bool ignoreVariations = false)
        {
            UpdateFrequency = updateProgressFrequency;
            MaxGameCount = maxGameCount;
            MaximumPlyPerGame = maxPlyCountPerGame;
            IgnoreVariations = ignoreVariations;
        }

        /// <summary>
        ///     To construct options to filter games by FEN.
        /// </summary>
        /// <param name="fenPositionFilter">Complete FEN to filter on</param>
        /// <param name="plySearchLimit">
        ///     Number of ply (half-moves) to search before determining game does not meet filter
        ///     specifications. If null, this will be set using the full-move count in the FEN
        /// </param>
        /// <param name="updateProgressFrequency">Parse this number of games before calling update event</param>
        /// <param name="maxGameCount">Parse at the maximum this many games</param>
        /// <param name="maxPlyCount">Only parse this many ply (half-moves) of each game</param>
        /// <param name="ignoreVariations">true to not parse variations</param>
        public PGNParserOptions(string fenPositionFilter, int? plySearchLimit = null, int updateProgressFrequency = 100,
            int? maxGameCount = null, int? maxPlyCount = null, bool ignoreVariations = false)
            : this(updateProgressFrequency, maxGameCount, maxPlyCount, ignoreVariations)
        {
            FenFilter = fenPositionFilter;
            FilterPlyLimit = plySearchLimit ?? _fenObject.TotalPlies;
            BoardStateHash = PolyglotHelpers.GetBoardStateHash(fenPositionFilter);
        }

        public int? FilterPlyLimit { get; protected set; }

        public ulong? BoardStateHash { get; protected set; }

        /// <summary>
        ///     Return only games that reach this position
        /// </summary>
        public string FenFilter
        {
            get => _fenFilter;
            set
            {
                _fenFilter = value.Trim();
                if (!string.IsNullOrWhiteSpace(_fenFilter))
                {
                    _fenObject = FENHelpers.GetFenObject(_fenFilter);
                    Debug.Assert(ShouldUseFenFilter);
                }
            }
        }

        public bool ShouldFilterDuringParsing => ShouldUseFenFilter || ShouldLimitPlyCount;

        public bool ShouldUseFenFilter => _fenObject != null;

        /// <summary>
        ///     How many rows to process before an update event is sent. Higher numbers should give marginally better performance.
        /// </summary>
        public int UpdateFrequency { get; set; }

        /// <summary>
        ///     Sets maximum games to parse. Set to MaximumParseValue to parse all available games.
        /// </summary>
        public int? MaxGameCount { get; set; }

        /// <summary>
        ///     Returns true if game count should be limited.
        /// </summary>
        public bool ShouldLimitGameCount => MaxGameCount.HasValue;

        /// <summary>
        ///     Limit ply parsing to number. Set to MaximumParseValue to parse all available moves. Setting this as 0 will get tags
        ///     only. Will ignore variations if true.
        /// </summary>
        public int? MaximumPlyPerGame { get; set; }

        /// <summary>
        ///     Returns true if total ply parsed from game is limited
        /// </summary>
        public bool ShouldLimitPlyCount => MaximumPlyPerGame.HasValue;

        /// <summary>
        ///     If true, does not parse Recursive Annotation Variations in game.
        /// </summary>
        public bool IgnoreVariations
        {
            get => _ignoreVariations || ShouldLimitPlyCount;
            set => _ignoreVariations = value;
        }
    }
}