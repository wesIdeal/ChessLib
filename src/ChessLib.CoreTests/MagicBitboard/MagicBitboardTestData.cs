using System;
using System.Collections;
using System.Collections.Generic;
using ChessLib.Core.Helpers;
using ChessLib.Core.Tests.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using NUnit.Framework;

// ReSharper disable StringLiteralTypo

namespace ChessLib.Core.Tests.MagicBitboard
{
    public class MagicBitboardTestData
    {
        private static readonly FenTextToBoard FenReader = new FenTextToBoard();

        public static IEnumerable<TestCase<ulong, Board>> GetPiecesAttackingSquareTestCases_SpecificColor()
        {
            var description = "Specific Color";
            yield return new TestCase<ulong, Board>(0x4000ul,
                FenReader.Translate("2r5/3r2k1/p3b3/1p3p2/2pPpP2/P1N1P3/1P4RP/2R3K1 b - - 1 34"),
                $"{description} White Rook attacks (Black attacks from Queenside)", (ushort)54, Color.White);
            yield return new TestCase<ulong, Board>(0x800000000,
                FenReader.Translate("4k3/8/8/2pP4/8/8/8/4K3 w - c6 0 1"),
                $"{description} White Pawn on d4 attacks the c6 square.", (ushort)42, Color.White);
            yield return new TestCase<ulong, Board>(0x200000000,
                FenReader.Translate("4k3/8/8/1p6/8/3P4/8/4K3 w - - 0 1"),
                $"{description} Black Pawn on b5 attacks the c4 square.", (ushort)26, Color.Black);
            yield return new TestCase<ulong, Board>(0x80000, FenReader.Translate("4k3/8/8/1p6/8/3P4/8/4K3 w - - 0 1"),
                $"{description} White Pawn on d3 attacks the c4 square.", (ushort)26, Color.White);
            yield return new TestCase<ulong, Board>(0x00, FenReader.Translate("4k3/8/8/2pP4/8/8/8/4K3 w - c6 0 1"),
                $"{description} Nothing attacking d6", (ushort)43, Color.White);
            yield return new TestCase<ulong, Board>(0x400000000,
                FenReader.Translate("4k3/8/8/2pP4/8/8/8/4K3 w - c6 0 1"),
                $"{description} Black Pawn attacks d4", (ushort)27, Color.Black);
            yield return new TestCase<ulong, Board>(0x8000140000000000,
                FenReader.Translate("4k2Q/8/2B1R3/8/8/8/8/4K3 b - - 0 1"),
                $"{description} 3 White Pieces attack Black King", (ushort)60, Color.White);
            yield return new TestCase<ulong, Board>(
                0x140000000000,
                FenReader.Translate("4k2q/8/2B1R3/8/8/8/8/4K3 b - - 0 1"),
                $"{description} Two White pieces attack the Black Queen, one Black Queen flanks from the Kingside.",
                (ushort)60,
                Color.White);
           
            var board = FenReader.Translate("8/1k6/8/3pP3/8/5K2/8/8 w - d6 0 4");
            yield return new TestCase<ulong, Board>(
                "e5".ToBoardIndex().GetBoardValueOfIndex(),
                board,
                "En Passant available from pawn on d6, after d7-d5",
                "d6".ToBoardIndex(),
                Color.White);
           
            board = FenReader.Translate("4k3/8/8/2p5/3P4/8/8/4K3 w - - 0 1");
            yield return new TestCase<ulong, Board>("d4".ToBoardIndex().GetBoardValueOfIndex(),
                board,
                "Normal pawn attack- White pawn on d4 attacks c5",
                "c5".ToBoardIndex(),
                Color.White);
        }


        public static IEnumerable<TestCase<ulong, Board>> GetPiecesAttackingSquareTestCases_AllColors()
        {
            var description = "All Colors";
            yield return new TestCase<ulong, Board>(0x8000000004000ul,
                FenReader.Translate("2r5/3r2k1/p3b3/1p3p2/2pPpP2/P1N1P3/1P4RP/2R3K1 b - - 1 34"),
                $"{description} Black and White Rook Attack g7", (ushort)54, null);
            yield return new TestCase<ulong, Board>(0x800000000,
                FenReader.Translate("4k3/8/8/2pP4/8/8/8/4K3 w - c6 0 1"),
                $"{description} White Pawn on d5 attacks the c6 square.", (ushort)42, null);
            yield return new TestCase<ulong, Board>(0x00, FenReader.Translate("4k3/8/8/2pP4/8/8/8/4K3 w - c6 0 1"),
                $"{description} Nothing attacking d6", (ushort)43, null);
            yield return new TestCase<ulong, Board>(0x200080000,
                FenReader.Translate("4k3/8/8/1p6/8/3P4/8/4K3 w - - 0 1"),
                $"{description} White Pawn on d3 / Black Pawn on b5 both attack the c4 square.", (ushort)26, null);
            yield return new TestCase<ulong, Board>(0x400000000,
                FenReader.Translate("4k3/8/8/2pP4/8/8/8/4K3 w - c6 0 1"),
                $"{description} Black Pawn attacks d4", (ushort)27, null);
            yield return new TestCase<ulong, Board>(0x8000140000000000,
                FenReader.Translate("4k2Q/8/2B1R3/8/8/8/8/4K3 b - - 0 1"),
                $"{description} 3 White Pieces attack Black King", (ushort)60, null);
            yield return new TestCase<ulong, Board>(0x8000140000000000,
                FenReader.Translate("4k2q/8/2B1R3/8/8/8/8/4K3 b - - 0 1"),
                $"{description} Two White pieces attack the Black Queen, one Black Queen flanks from the Kingside.",
                (ushort)60,
                null);
        }

        public static IEnumerable GetPseudoLegalMoveTestCases()
        {
            ushort whiteKingIndex = 4;
            ushort blackKingIndex = 60;

            var board = FenReader.Translate("r1bqk2r/ppp1bppp/2np1n2/4p3/2P5/2N2NP1/PP1PPPBP/R1BQK2R w KQkq - 2 6");
            var kf1 = MoveHelpers.GenerateMove("e1".ToBoardIndex(), "f1".ToBoardIndex());
            var castleShort = MoveHelpers.WhiteCastleKingSide;
            var desc = "White should have Kf1 and O-O";
            var name = "Legal Moves - White King (O-O)";
            yield return new TestCaseData(board, new[] { kf1, castleShort }, whiteKingIndex)
                .SetName(name)
                .SetDescription(desc);

            board = FenReader.Translate("r1bqk2r/ppp1bppp/3p4/8/2Ppn3/2NP2P1/PPQBPP1P/R3KB1R w KQkq - 2 9");
            var kd1 = MoveHelpers.GenerateMove("e1".ToBoardIndex(), "d1".ToBoardIndex());
            var castleLong = MoveHelpers.WhiteCastleQueenSide;
            desc = "White should have Kd1 and O-O-O";
            name = "Legal Moves - White King (O-O-O)";
            yield return new TestCaseData(board, new[] { kd1, castleLong }, whiteKingIndex).SetName(name)
                .SetDescription(desc);


            board = FenReader.Translate("r1bqk2r/ppp1bppp/3p4/8/2Ppn3/2NP2P1/PPQBPP1P/R3KB1R b KQkq - 2 9");
            var kf8 = MoveHelpers.GenerateMove("e8".ToBoardIndex(), "f8".ToBoardIndex());
            castleShort = MoveHelpers.BlackCastleKingSide;
            desc = "Black should have Kf8 and O-O";
            name = "Legal Moves - Black King (O-O)";
            yield return new TestCaseData(board, new[] { kf8, castleShort }, blackKingIndex)
                .SetName(name)
                .SetDescription(desc);

            board = FenReader.Translate("r3kb1r/pppqpppp/3pb3/3N4/2P1P3/6P1/PPQBPP1P/R3KB1R b KQkq - 0 11");
            var kd8 = MoveHelpers.GenerateMove("e8".ToBoardIndex(), "d8".ToBoardIndex());
            castleLong = MoveHelpers.BlackCastleQueenSide;
            desc = "Black should have Kd8 and O-O-O";
            name = "Legal Moves - Black King (O-O-O)";
            yield return new TestCaseData(board, new[] { kd8, castleLong }, blackKingIndex).SetName(name)
                .SetDescription(desc);

            board = FenReader.Translate("2kr1b1r/pppqpppp/3pb3/3N4/2P1P3/6P1/PPQBPP1P/R3KB1R w KQ - 1 12");
            desc = "No moves should be returned if source square is unoccupied";
            name = "Legal Moves - [null]";
            yield return new TestCaseData(board, Array.Empty<Move>(), (ushort)34)
                .SetDescription(desc)
                .SetName(name);

            board = FenReader.Translate("rnbqkbnr/ppp2ppp/8/2Ppp3/8/8/PP1PPPPP/RNBQKBNR w KQkq d6 0 3");
            desc = "En Passant available on d6";
            name = "Legal Moves - White En Passant";
            var normalMove = MoveHelpers.GenerateMove("c5".ToBoardIndex(), "c6".ToBoardIndex());
            var epMove =
                MoveHelpers.GenerateMove("c5".ToBoardIndex(), "d6".ToBoardIndex(), MoveType.EnPassant);
            yield return new TestCaseData(board, new[] { epMove, normalMove }, "c5".ToBoardIndex())
                .SetDescription(desc)
                .SetName(name);
        }
    }
}