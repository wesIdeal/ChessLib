using System.ComponentModel;

namespace ChessLib.UCI
{
    public enum UCIOptionType
    {
        Null,
        [Description("spin")]
        Spin,
        [Description("check")]
        Check,
        [Description("combo")]
        Combo,
        [Description("button")]
        Button,
        [Description("string")]
        String
    }
}
