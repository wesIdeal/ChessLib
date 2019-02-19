using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicBitboard
{
    public class OccupencyBoards
    {
        public ulong AttackMask { get; private set; }
        public BlockerAndMoveBoards[] BlockerBoards { get; private set; }
        public string ToString(int index)
        {
            var str = "";
            str += BlockerBoards[index].ToString();
            return str;
        }

        public static OccupencyBoards GetRookBoards(ulong attackMask, IEnumerable<ulong> n, int piecePositionIndex)
        {
            var rookGenerator = new RookMovesGenerator();
            var rv = new OccupencyBoards();
            rv.AttackMask = attackMask;
            var boardCombos = new List<BlockerAndMoveBoards>();
            var startTime = DateTime.Now;
            foreach (var board in n)
            {
                boardCombos.Add(new BlockerAndMoveBoards(board, rookGenerator.CalculateMovesFromPosition(piecePositionIndex, board)));
            }
            var totalMS = DateTime.Now.Subtract(startTime).TotalMilliseconds;
            rv.BlockerBoards = boardCombos.ToArray();
            return rv;
        }
    }

    public class BlockerAndMoveBoards
    {
        public BlockerAndMoveBoards(ulong blockerBoard, ulong moveBoard)
        {
            BlockerBoard = blockerBoard;
            MoveBoard = moveBoard;
        }

        public ulong BlockerBoard { get; private set; }
        public ulong MoveBoard { get; private set; }
        public override string ToString()
        {
            var bb = Convert.ToString((long)BlockerBoard, 2).PadLeft(64, '0').ToCharArray();
            var mb = Convert.ToString((long)MoveBoard, 2).PadLeft(64, '0').ToCharArray();
            var bbHeader = "Blocker".PadRight(16 + "\t\t".Length);
            var mbHeader = "Move Board";
            StringBuilder sb = new StringBuilder($"{bbHeader}\t\t{mbHeader}\r\n");
            for (int i = 0; i < 8; i++)
            {
                sb.AppendLine($"{string.Join(" ", (bb.Skip(8 * i).Take(8).ToArray()))}\t\t{string.Join(" ", mb.Skip(8 * i).Take(8).ToArray())}");
            }
            return sb.ToString();
        }
    }

    public class RookMovesGenerator
    {
        public ulong CalculateMovesFromPosition(int positionIndex, ulong occupancyBoard)
        {
            var rv = (ulong)0;
            var startingValue = (ulong)1 << positionIndex;
            //N
            var positionalValue = startingValue;
            while ((positionalValue = positionalValue.ShiftN()) != 0 && (occupancyBoard & positionalValue) != positionalValue)
            {

                rv |= positionalValue;
            }
            //E
            positionalValue = startingValue;
            while ((positionalValue = positionalValue.ShiftE()) != 0 && (occupancyBoard & positionalValue) != positionalValue)
            {
                rv |= positionalValue;
            }
            //S
            positionalValue = startingValue;
            while ((positionalValue = positionalValue.ShiftS()) != 0 && (occupancyBoard & positionalValue) != positionalValue)
            {
                rv |= positionalValue;
            }
            //W
            positionalValue = startingValue;
            while ((positionalValue = positionalValue.ShiftW()) != 0 && (occupancyBoard & positionalValue) != positionalValue)
            {
                rv |= positionalValue;
            }
            return rv;
        }
    }
}
