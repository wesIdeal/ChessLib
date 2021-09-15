using System;
using System.Collections.Generic;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using EnumsNET;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.MoveValidation.CastlingRules.TestData
{
    internal class CastlingMoveIsAvailableValidatorTestData
    {
        public static IEnumerable<TestCaseData> GetCastlingMoveTestCases(Color color)
        {
            var castlingMovesForColor = color.GetCastlingMoves();

            foreach (var move in castlingMovesForColor)
            {
                var dict = GetCastingBoards(color, move);
                yield return new TestCaseData(dict[true], move).SetName("Castling Availability - Good")
                    .Returns(MoveError.NoneSet);
                yield return new TestCaseData(dict[false], move).SetName("Castling Availability - Not Available")
                    .Returns(MoveError.CastleUnavailable);
            }
        }

        public static IEnumerable<TestCaseData> GetExceptionTestCase()
        {
            var translator = new FenTextToBoard();
            var whiteBoard = translator.Translate("r3k2r/8/8/8/8/8/8/R3K2R w - - 0 1");
            var blackBoard = translator.Translate("r3k2r/8/8/8/8/8/8/R3K2R b - - 0 1");
            yield return new TestCaseData(whiteBoard, MoveHelpers.GenerateMove("e2".ToBoardIndex(), "e4".ToBoardIndex(),
                MoveType.Castle)).SetName("Castling Availability [Exception, White]");
            yield return new TestCaseData(blackBoard, MoveHelpers.GenerateMove("e7".ToBoardIndex(), "e5".ToBoardIndex(),
                MoveType.Castle)).SetName("Castling Availability [Exception, Black]");
        }


        private static CastlingAvailability GetCastlingAvailabilityFromMove(Move move)
        {
            if (move.Equals(MoveHelpers.WhiteCastleKingSide))
            {
                return CastlingAvailability.WhiteKingside;
            }

            if (move.Equals(MoveHelpers.BlackCastleKingSide))
            {
                return CastlingAvailability.BlackKingside;
            }

            if (move.Equals(MoveHelpers.WhiteCastleQueenSide))
            {
                return CastlingAvailability.WhiteQueenside;
            }

            if (move.Equals(MoveHelpers.BlackCastleQueenSide))
            {
                return CastlingAvailability.BlackQueenside;
            }

            throw new ArgumentException("Move is not castling move.");
        }

        private static Dictionary<bool, Board> GetCastingBoards(Color color, Move move)
        {
            var fenToBoard = new FenTextToBoard();
            const string emptyFenWhiteToMove = "4k3/8/8/8/8/8/8/4K3 w - - 0 1";
            const string emptyFenBlackToMove = "4k3/8/8/8/8/8/8/4K3 b - - 0 1";
            var dictCastlingBoards = new Dictionary<bool, Board>();
            var emptyBoard = color == Color.Black
                ? fenToBoard.Translate(emptyFenBlackToMove)
                : fenToBoard.Translate(emptyFenWhiteToMove);
            var castlingAvailability = GetCastlingAvailabilityFromMove(move);
            var allCastles = CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackKingside |
                             CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackQueenside;
            var castleUnavailable = allCastles.RemoveFlags(castlingAvailability);
            var castleAvailable = castlingAvailability;
            var canCastle = new Board(emptyBoard.Occupancy, emptyBoard.HalfMoveClock, emptyBoard.EnPassantIndex,
                emptyBoard.PieceCaptured,
                castleAvailable, emptyBoard.ActivePlayer, emptyBoard.FullMoveCounter, true);

            var cannotCastle = new Board(emptyBoard.Occupancy, emptyBoard.HalfMoveClock, emptyBoard.EnPassantIndex,
                emptyBoard.PieceCaptured,
                castleUnavailable, emptyBoard.ActivePlayer, emptyBoard.FullMoveCounter, true);
            dictCastlingBoards.Add(true, canCastle);
            dictCastlingBoards.Add(false, cannotCastle);
            return dictCastlingBoards;
        }
    }
}