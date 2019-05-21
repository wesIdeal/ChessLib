namespace ChessLib.Data.MoveRepresentation
{
    /// <summary>
    /// This is for move storage, long term.
    /// </summary>
    /// <remarks>
    /// Structure:
    /// bits 0-5: DestinationIndex (0-63)
    /// bits 6-11: OriginIndex (0-63)
    /// bits 12-14: Promotion Piece Type (Knight, Bishop, Rook, Queen)
    /// bits 14-16: MoveType
    /// </remarks>
    //public class MoveExt : System.IEquatable<MoveExt>, MoveExtBase
    //{

    //    public MoveExt(ushort move) { Move = move; }



    //    public List<MoveExt> Variations;

    //    public bool Equals(MoveExt other) => Move == other.Move;
    //    public bool Equals(ushort other) => Move == other;


    //}
}
