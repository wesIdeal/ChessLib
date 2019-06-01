namespace ChessLib.Types.Interfaces
{
    public interface IBoardInformationService<T> where T : IMoveStorage
    {
        /// <summary>
        /// Applies a move from SAN text to a board information service object
        /// </summary>
        /// <param name="moveText">SAN Move <example>e4, Nxf5, O-O</example></param>
        void ApplyMove(string moveText);

        /// <summary>
        /// Where moves are stored
        /// </summary>
        IMoveTree<T> MoveTree { get; set; }
        IMoveTree<T> Moves { get; }
    }
}
