using System.ComponentModel;

namespace ChessLib.EngineInterface.Commands.FromEngine
{
    public enum OptionType
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
