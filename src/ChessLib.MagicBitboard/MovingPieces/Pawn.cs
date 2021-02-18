using ChessLib.Data.Types.Enums;
using ChessLib.MagicBitboard.Bitwise;
using ChessLib.MagicBitboard.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ChessLib.MagicBitboard.MovingPieces
{
    class Pawn : MovingPiece
    {
        private const ulong NotFileAMask = ~(ulong)0x101010101010101;
        private const ulong NotFileHMask = ~(ulong)0x8080808080808080;
        private const ulong Rank2Mask = (ulong)0xff00;
        private ulong[] moveMask;
        private ulong[] attackMask;
        public ulong[][] MovesFromLocation = new ulong[64][];
        private IMovingPieceService _movingPieceService;
        private MoveObstructionBoard[][] blockerBoards = new MoveObstructionBoard[64][];
        public readonly ulong[] MagicKey = new ulong[64];
        public Pawn(IMovingPieceService movingPieceService)
        {
            _movingPieceService = movingPieceService;
            Initialize();
        }

        public override ulong GetPsuedoLegalMoves(ushort square, Color playerColor, ulong playerOccupancy, ulong opponentOccupancy)
        {
            var fullOccupancy = playerOccupancy | opponentOccupancy;
            return 0;
        }

        public override void Initialize()
        {
            SetMoveAndAttackBoards();
            InitializeMagicBitBoard();
        }

        private ulong[] MovesAndAttacks
        {
            get
            {
                Debug.Assert(moveMask != null && attackMask != null, "Move and Attack masks need initialization.");
                Debug.Assert(moveMask.Length == attackMask.Length, "Move and Attack masks should be of the same length.");
                Debug.Assert(moveMask.Length == 64, "Move and Attack mask should be of size 64.");
                return moveMask.Select((movesFromPosition, index) => movesFromPosition | attackMask[index])
                               .ToArray();
            }
        }

        private void InitializeMagicBitBoard()
        {
            var movesAndAttacks = MovesAndAttacks;
     
            for (ushort index = 8; index < 56; index++)
            {
                var attackBoard = movesAndAttacks[index];
                ulong[] attackArray = new ulong[0];
                
                if (attackBoard != 0)
                {
                    var setBitCount = 12;
                    attackMask[index] = movesAndAttacks[index];
                    var occupancyPermutations = _movingPieceService.GetAllPermutations(attackBoard);
                    var permutations = 
                        _movingPieceService.GetAllPermutationsForAttackMask(index, moveMask[index], attackMask[index], occupancyPermutations)
                        .ToArray();
                    blockerBoards[index] = permutations;
                    MagicKey[index] = GenerateMagicKey(blockerBoards[index], setBitCount,
                        out attackArray);
                }
                MovesFromLocation[index] = attackArray;
            }
            //);

        }

        private void SetMoveAndAttackBoards()
        {
            moveMask = new ulong[64];
            attackMask = new ulong[64];
            for (int square = 8; square < 56; square++)
            {
                ulong squareValue = (ulong)1 << square;
                moveMask[square] = (squareValue << 8) | ((squareValue & Rank2Mask) << 16);
                attackMask[square] |=
                   ((squareValue & NotFileAMask) >> 7) |
                   ((squareValue & NotFileHMask) >> 9);
            }
        }

        private ulong GetPawnAttacksFromPosition(int positionIndex, ulong occupancyBoard)
        {
            var attacks = attackMask[positionIndex];
            return attacks & occupancyBoard;
        }

        private ulong GetPawnMovesFromPosition(ushort boardSquare, ulong occupancyBoard)
        {
            ulong rv = 0;
            var potentialMoves = attackMask[boardSquare];
            var setBits = _movingPieceService.GetSetBits(potentialMoves);
            foreach (var potentialMoveSq in setBits)
            {
                var moveSqValue = _movingPieceService.GetBoardValueOfIndex(potentialMoveSq);
                var inBetween = _movingPieceService.GetInBetweenSquares(boardSquare, potentialMoveSq);
                var checkSquares = inBetween | _movingPieceService.GetBoardValueOfIndex(potentialMoveSq);
                if (_movingPieceService.GetSetBits(checkSquares)
                    .All(ib => (_movingPieceService.GetBoardValueOfIndex(ib) & occupancyBoard) == 0))
                {
                    rv |= moveSqValue;
                }
            }
            return rv;
        }
    }
}
