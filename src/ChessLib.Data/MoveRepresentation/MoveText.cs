﻿using System;
using ChessLib.Data.Types.Interfaces;

namespace ChessLib.Data.MoveRepresentation
{
    /// <summary>
    /// Holds move information that was retrieved from text
    /// </summary>
    public class MoveText : IMoveText, IEquatable<MoveText>
    {
        /// <summary>
        /// Constructs a move given the Standard Algebraic Notation (SAN)
        /// </summary>
        /// <param name="san">Standard Algebraic Notation (SAN) representing the given move</param>
        public MoveText(string san)
        {
            SAN = san;
        }

        public int MoveNumber { get; set; }
        public string SAN { get; set; }
        public string Comment { get; set; }
        public string NAG { get; set; }

        public bool Equals(IMoveText other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(SAN, other.SAN) && string.Equals(Comment, other.Comment) && string.Equals(NAG, other.NAG);
        }

        public override string ToString()
        {
            return SAN;
        }

        public bool Equals(MoveText other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return MoveNumber == other.MoveNumber && string.Equals(SAN, other.SAN) && string.Equals(Comment, other.Comment) && string.Equals(NAG, other.NAG);
        }

       

    }
}
