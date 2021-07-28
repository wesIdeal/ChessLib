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
        public const string EnglishTabiyaNextMoveSan = "g3";
        public static readonly Move EnglishTabiyaNextMoveAlternate = MoveHelpers.GenerateMove(11, 29);
        public const string EnglishTabiyaPostMove =
            "r1bqkb1r/pppp1ppp/2n2n2/4p3/2P5/2N2NP1/PP1PPP1P/R1BQKB1R b KQkq - 0 4";

        public const string EnglishTabiyaPostMoveAlternate = "r1bqkb1r/pppp1ppp/2n2n2/4p3/2PP4/2N2N2/PP2PPPP/R1BQKB1R b KQkq - 0 4";
        public const ulong EnglishTabiyaBoardStateHash = 0x90901935d014a8dc;
    }
}
