using System;
using System.Collections.Generic;
using System.Text;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types.Enums;

namespace ChessLib.Core.Tests
{
    internal class CoreTestConstants
    {
        public const string InitialFen = BoardConstants.FenStartingPosition;
        public const string EnglishTabiya = "r1bqkb1r/pppp1ppp/2n2n2/4p3/2P5/2N2N2/PP1PPPPP/R1BQKB1R w KQkq - 4 4";
        public static readonly Board EnglishTabiyaBoard = new FenTextToBoard().Translate(EnglishTabiya);
        public static readonly Move EnglishTabiyaNextMove = MoveHelpers.GenerateMove(14, 22);
        public const ulong EnglishTabiyaBoardStateHash = 0x90901935d014a8dc;
    }
}
