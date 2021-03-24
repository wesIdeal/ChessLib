#region

using System;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Interfaces;

#endregion

namespace ChessLib.Core
{
    public class Board : BoardState, IEquatable<Board>, IBoard, ICloneable
    {
        private ulong[][] CloneOccupancy()
        {
            var rv = new ulong[2][];
            rv[0] = new ulong[6];
            rv[1] = new ulong[6];
            Array.Copy(Occupancy[0], rv[0], 6);
            Array.Copy(Occupancy[1], rv[1], 6);
            return rv;
        }
        public new object Clone()
        {
            Console.WriteLine($"Total occupancy: {Occupancy.Occupancy()}");
            var clonedOccupancy = CloneOccupancy();
            var activePlayerColor = ActivePlayer;
            return new Board(clonedOccupancy, HalfMoveClock, EnPassantSquare, PieceCaptured, CastlingAvailability, activePlayerColor, FullMoveCounter);
        }
        public Board() : base(FENHelpers.FENInitial)
        {
            Occupancy = new ulong[2][];
            InitializeOccupancy();
        }

        /// <summary>
        ///     Construct Board from FEN
        /// </summary>
        /// <param name="fen">fen string </param>
        public Board(string fen) : base(fen)
        {
            Occupancy = FENHelpers.BoardFromFen(fen);
            
        }

        public Board(ulong[][] occupancy, ushort halfMoveClock, ushort? enPassantIndex, Piece? capturedPiece,
            CastlingAvailability castlingAvailability, Color activePlayer, uint fullMoveCounter)
        : base(occupancy, halfMoveClock, enPassantIndex, capturedPiece, castlingAvailability, activePlayer, fullMoveCounter)
        {
            Occupancy = occupancy;
        }

        public ulong[] BlackOccupancy => Occupancy[(int)Color.Black];
        public ulong[] WhiteOccupancy => Occupancy[(int)Color.White];
        public Color OpponentColor => ActivePlayer.Toggle();

        public string CurrentFEN => this.ToFEN();

        public ulong[][] Occupancy { get; private set; }


        public bool Equals(Board other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(Occupancy, other.Occupancy);
        }

        private void InitializeOccupancy()
        {
            Occupancy[(int)Color.Black] = new ulong[6];
            Occupancy[(int)Color.White] = new ulong[6];
            InitializeBlackOccupancy();
            InitializeWhiteOccupancy();
        }

        private void InitializeBlackOccupancy()
        {
            BlackOccupancy[(int)Piece.Pawn] = 0xff000000000000;
            BlackOccupancy[(int)Piece.Knight] = 0x4200000000000000;
            BlackOccupancy[(int)Piece.Bishop] = 0x2400000000000000;
            BlackOccupancy[(int)Piece.Rook] = 0x8100000000000000;
            BlackOccupancy[(int)Piece.Queen] = 0x800000000000000;
            BlackOccupancy[(int)Piece.King] = 0x1000000000000000;
        }

        private void InitializeWhiteOccupancy()
        {
            WhiteOccupancy[(int)Piece.Pawn] = 0xff00;
            WhiteOccupancy[(int)Piece.Knight] = 0x42;
            WhiteOccupancy[(int)Piece.Bishop] = 0x24;
            WhiteOccupancy[(int)Piece.Rook] = 0x81;
            WhiteOccupancy[(int)Piece.Queen] = 0x8;
            WhiteOccupancy[(int)Piece.King] = 0x10;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Board)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (Occupancy != null ? Occupancy.GetHashCode() : 0);
            }
        }

        public void SetPiecePlacement(ulong[][] occupancy)
        {
            Occupancy = occupancy;
        }

       
    }
 
}