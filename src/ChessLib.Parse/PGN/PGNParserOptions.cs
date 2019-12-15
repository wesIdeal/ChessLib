using System.Diagnostics;
using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types.Enums;

namespace ChessLib.Parse.PGN
{
    public class PGNParserOptions
    {
        public const int MaximumParseValue = -1;
        private string _fenPositionFilter;
        private bool _ignoreVariations;

        public PGNParserOptions()
        {
            UpdateFrequency = 100;
            GameCountToParse = MaximumParseValue;
            MaximumPlyPerGame = MaximumParseValue;
            IgnoreVariations = false;
            FenPositionFilter = "";
            FenPositionMoveCount = 0;
        }

        public FEN FENObject { get; protected set; }

        public bool FilteringApplied => UseFenFilter || LimitPlyCount;

        public int FenPlyMoveLimit =>
            UseFenFilter
                ? FenFilterFullMoveLimit * 2 - (FENObject.ActiveColor == Color.Black ? 1 : 0)
                : 0;

        private int FenFilterFullMoveLimit =>
            UseFenFilter && FenPositionMoveCount == 0 ? FENObject.FullmoveClock : FenPositionMoveCount;

        public bool UseFenFilter => !string.IsNullOrEmpty(FenPositionFilter);

        public ulong? BoardStateSearchHash { get; private set; }

        /// <summary>
        ///     Return only games that reach this position
        /// </summary>
        public string FenPositionFilter
        {
            get => _fenPositionFilter;
            protected set
            {
                _fenPositionFilter = value.Trim();
                if (!string.IsNullOrWhiteSpace(_fenPositionFilter))
                {
                    FENObject = FENHelpers.GetFenObject(_fenPositionFilter);
                    Debug.Assert(UseFenFilter);
                }
            }
        }

        /// <summary>
        ///     Expands the moves in which to search <see cref="FenPositionFilter" />
        /// </summary>
        protected int FenPositionMoveCount { get; set; }

        /// <summary>
        ///     How many rows to process before an update event is sent. Higher numbers should give marginally better performance.
        /// </summary>
        public int UpdateFrequency { get; set; }

        /// <summary>
        ///     Sets maximum games to parse. Set to MaximumParseValue to parse all available games.
        /// </summary>
        public int GameCountToParse { get; set; }

        /// <summary>
        ///     Returns true if game count should be limited.
        /// </summary>
        public bool LimitGameCount => GameCountToParse != MaximumParseValue;

        /// <summary>
        ///     Limit ply parsing to number. Set to MaximumParseValue to parse all available moves. Setting this as 0 will get tags
        ///     only. Will ignore variations.
        /// </summary>
        public int MaximumPlyPerGame { get; set; }

        public bool LimitPlyCount => MaximumPlyPerGame != MaximumParseValue;

        /// <summary>
        ///     If true, does not parse Recursive Annotation Variations in game.
        /// </summary>
        public bool IgnoreVariations
        {
            get => _ignoreVariations || LimitPlyCount;
            set => _ignoreVariations = value;
        }

        public void SetFenFiltering(string fen, int fullMovesToMatch = 0)
        {
            FenPositionFilter = fen;
            FenPositionMoveCount = fullMovesToMatch == 0 ? FENObject.FullmoveClock : fullMovesToMatch;
            BoardStateSearchHash = PolyglotHelpers.GetBoardStateHash(fen);
        }
    }
}