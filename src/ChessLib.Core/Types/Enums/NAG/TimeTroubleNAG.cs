using System.ComponentModel;

namespace ChessLib.Core.Types.Enums.NAG
{
    public enum TimeTroubleNAG
    {
        Null,

        [Symbol("")] [Description("White has moderate time control pressure")]
        WhiteModerateTimePressure = 136,

        [Symbol("")] [Description("Black has moderate time control pressure")]
        BlackModerateTimePressure = 137,

        [Symbol("")] [Description("White has severe time control pressure")]
        WhiteSevereTimePressure = 138,

        [Symbol("")] [Description("Black has severe time control pressure")]
        BlackSevereTimePressure = 139
    }
}