using MagicBitboard.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicBitboard.Helpers
{
   public static class FENHelpers
    {
        public static Color GetActiveColor(string v)
        {
            switch (v)
            {
                case "w": return Color.White;
                case "b": return Color.Black;
                default: throw new FENException("Invalid active color in FEN. Color character received was " + v + ".");
            }
        }
    }
}
