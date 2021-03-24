using System;
using System.Linq;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Graphics.TextDisplay;
using EnumsNET;

namespace ChessLib.MagicBitboard.Tests.MovingPieces
{
    public struct MoveTestCase
    {
        public ulong OpponentObstructions;
        public ulong Expected;
        public ulong PlayerObstructions;
        public ushort SquareIndex;
        public Color Color;

        public MoveTestCase(ushort squareIndex, Color color, ulong playerObstructions, ulong opponentObstructions, ulong expected)
        {
            SquareIndex = squareIndex;
            Color = color;
            PlayerObstructions = playerObstructions;
            OpponentObstructions = opponentObstructions;
            Expected = expected;
        }

        public ulong Occupancy
        {
            get
            {
                return OpponentObstructions | PlayerObstructions;
            }
        }

        public override string ToString()
        {
            var pieceFile = MovingPieceService.FileFromIdx(SquareIndex);
            var pawnDescription = $"{Color.AsString()}";
            var pieceDescription = $"Piece";
            var pawnPlacement = $"on {DisplayService.IndexToSquareDisplay(SquareIndex)} ";
            var expectedIndexes = MovingPieceService.GetSetBits(Expected);
            var playerOccupancy = MovingPieceService.GetSetBits(PlayerObstructions);
            var opponentOccupancy = MovingPieceService.GetSetBits(OpponentObstructions);
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