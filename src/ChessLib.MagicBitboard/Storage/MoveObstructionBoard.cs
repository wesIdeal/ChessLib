namespace ChessLib.MagicBitboard.Storage
{
    public class MoveObstructionBoard
    {
        public ulong Occupancy { get; }
        public ulong MoveBoard { get; }

        
        public MoveObstructionBoard(ulong occupancyBoard, ulong moveBoard)
        {
            Occupancy = occupancyBoard;
            MoveBoard = moveBoard;
        }
       
    }
}
