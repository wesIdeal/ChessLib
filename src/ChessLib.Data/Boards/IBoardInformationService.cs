using System.Collections.Generic;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;

namespace ChessLib.Data.Boards
{
    public interface IBoardInformationService<T> where T : MoveStorage
    {
        /// <summary>
        /// Applies a move from SAN text to a board information service object
        /// </summary>
        /// <param name="moveText">SAN Move <example>e4, Nxf5, O-O</example></param>
        void ApplyMove(string moveText);

        /// <summary>
        /// Where moves are stored
        /// </summary>
        MoveTree<T> MoveTree { get; }

        Color ActivePlayerColor { get; set; }
        CastlingAvailability CastlingAvailability { get; set; }
        ushort? EnPassantIndex { get; }
        string FEN { get; }
        uint HalfmoveClock { get; set; }
        uint MoveCounter { get; set; }
        MoveTree<T> Moves { get; }
        bool IsAttackedBy(Color color, ushort squareIndex);
    }
}
