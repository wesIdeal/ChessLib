using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChessLib.Graphics.TextDisplay
{
    public interface IDisplayService
    {
        string ToHexDisplay(ulong u, bool appendHexNotation = true, bool pad = false, int padSize = 64);
        string PrintBoard(ulong u, string header = "", char replaceOnesWith = '1');
    }
}
