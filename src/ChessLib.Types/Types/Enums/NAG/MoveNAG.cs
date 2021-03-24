using System.ComponentModel;

namespace ChessLib.Core.Types.Enums.NAG
{
    public enum MoveNAG
    {
        [Description("null annotation")] [Symbol("")]
        Null = 0,

        [Description("good move")] [Symbol("!")]
        GoodMove = 1,

        [Description("poor move")] [Symbol("?")]
        PoorMove = 2,

        [Description("very good move")] [Symbol("!!")]
        VeryGoodMove = 3,

        [Description("very poor move")] [Symbol("??")]
        VeryPoorMove = 4,

        [Description("speculative move")] [Symbol("!?")]
        SpeculativeMove = 5,

        [Description("questionable move")] [Symbol("?!")]
        QuestionableMove = 6,

        [Description("forced move (all others lose quickly)")] [Symbol("□")]
        ForcedMove = 7,

        [Description("singular move (no reasonable alternatives)")] [Symbol("")]
        SingularMove = 8,

        [Description("worst move")] [Symbol("")]
        WorstMove = 9
    }
}