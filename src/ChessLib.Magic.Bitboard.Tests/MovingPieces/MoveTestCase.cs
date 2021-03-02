using System;
using System.Linq;
using ChessLib.Data.Types.Enums;
using ChessLib.Graphics.TextDisplay;
using ChessLib.MagicBitboard.Bitwise;
using EnumsNET;

namespace ChessLib.MagicBitboard.Tests.MovingPieces
{
    public struct MoveTestCase
    {
        public ulong OpponentBlocker;
        public ulong Expected;
        public ulong PlayerBlocker;
        public ushort SquareIndex;
        public Color Color;

        public MoveTestCase(ushort squareIndex, Color color, ulong playerBlocker, ulong opponentBlocker, ulong expected)
        {
            SquareIndex = squareIndex;
            Color = color;
            PlayerBlocker = playerBlocker;
            OpponentBlocker = opponentBlocker;
            Expected = expected;
        }

        public ulong Occupancy
        {
            get
            {
                return OpponentBlocker | PlayerBlocker;
            }
        }

        public override string ToString()
        {
            var pieceFile = MovingPieceService.FileFromIdx(SquareIndex);
            var pawnDescription = $"{Color.AsString()}";
            var pieceDescription = $"Piece";
            var pawnPlacement = $"on {DisplayService.IndexToSquareDisplay(SquareIndex)} ";
            var expectedIndexes = MovingPieceService.GetSetBits(Expected);
            var playerOccupancy = MovingPieceService.GetSetBits(PlayerBlocker);
            var opponentOccupancy = MovingPieceService.GetSetBits(OpponentBlocker);
            var strMoves =
                expectedIndexes.Any()
                    ? $"move to {string.Join(", ", expectedIndexes.Select(DisplayService.IndexToSquareDisplay))}"
                    : "[no moves expected]";
            var strPlayerOccupancy =
                playerOccupancy.Any()
                    ? string.Join(", ", playerOccupancy.Select(DisplayService.IndexToSquareDisplay))
                    : "[no player pieces]";
            var strOpponentOccupancy =
                opponentOccupancy.Any()
                    ? string.Join(", ", opponentOccupancy.Select(DisplayService.IndexToSquareDisplay))
                    : "[no opponent pieces]";
            var attackedOpponentPieces = opponentOccupancy.Where(x => MovingPieceService.FileFromIdx(x) != pieceFile);
            var strAttack = attackedOpponentPieces.Any()
                ? $"- Attack pieces on {string.Join(", ", attackedOpponentPieces.Select(DisplayService.IndexToSquareDisplay))}"
                : "[no attacked pieces]";

            return $"{pawnDescription} {pieceDescription} on {pawnPlacement} should be able to:{Environment.NewLine}" +
                   $"{strMoves}{Environment.NewLine}" +
                   $"{strAttack}{Environment.NewLine}" +
                   $"when players pieces are at {strPlayerOccupancy} and opponent pieces are at {strOpponentOccupancy}.";
        }
    }
}