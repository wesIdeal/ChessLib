using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib
{
    public static class BoardFileHelpers
    {
        public static int AsInt(this File f) => (int)f;
        public static int Increase(this File f) => (int)f + 1;
        public static int Decrease(this File f) => (int)f - 1;
    }
}
