using System.ComponentModel;

namespace ChessLib.Core.Types.Enums.NAG
{
    public enum NonStandardNAG
    {
        [Symbol("∆")] [Description("With the idea...")]
        WithTheIdea = 140,

        [Symbol("")] [Description("Aimed against...")]
        AimedAgainst = 141,

        [Symbol("⌓")] [Description("Better is...")]
        BetterIs = 142,

        [Symbol("")] [Description("Worse is...")]
        WorseIs = 143,

        [Symbol("")] [Description("Equivalent is...")]
        EquivalentIs = 144,

        [Symbol("RR")] [Description("Editorial comment")]
        EditorialComment = 145,

        [Symbol("N")] [Description("Novelty")] Novelty = 146,

        [Symbol("D")] [Description("Diagram")] Diagram = 220,

        [Symbol("")] [Description("Diagram (from Black)")]
        DiagramFromBlack = 221,

        [Symbol("")] [Description("Space advantage")]
        SpaceAdvantage = 238,

        [Symbol("⇔")] [Description("File (columns on the chessboard labeled a-h)")]
        File = 239,

        [Symbol("⇗")] [Description("Diagonal")]
        Diagonal = 240,
        [Symbol("")] [Description("Centre")] Centre = 241,

        [Symbol("⟫")] [Description("King-side")]
        King = 242,

        [Symbol("")] [Description("Queen-side")]
        QueenSide = 243,

        [Symbol("")] [Description("Weak point")]
        WeakPoint = 244,
        [Symbol("")] [Description("Ending")] Ending = 245,

        [Symbol("")] [Description("Bishop pair")]
        BishopPair = 246,

        [Symbol("")] [Description("Opposite Bishops")]
        OppositeBishops = 247,

        [Symbol("")] [Description("Same Bishops")]
        SameBishops = 248,

        [Symbol("")] [Description("Connected pawns")]
        ConnectedPawns = 249,

        [Symbol("")] [Description("Isolated pawns")]
        IsolatedPawns = 250,

        [Symbol("")] [Description("Doubled pawns")]
        DoubledPawns = 251,

        [Symbol("")] [Description("Passed pawn")]
        PassedPawn = 252,

        [Symbol("")] [Description("Pawn majority")]
        PawnMajority = 253,

        [Symbol("")] [Description("With")] With = 254,

        [Symbol("")] [Description("Without")] Without = 255
    }
}