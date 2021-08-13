using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums.NAG;

namespace ChessLib.Core
{
    /// <summary>
    ///     A move that was applied
    /// </summary>
    public class GameMove : Move, IEquatable<GameMove>, ICloneable
    {
        /// <summary>
        ///     Makes a NULL move node for head of move list
        /// </summary>
        internal GameMove() : base(NullMove)
        {
        }

        /// <summary>
        ///     Establishes a <see cref="GameMove" /> as a null-move board, meaning it represents the beginning of the game or
        ///     the beginning variation, with no prior moves applied
        /// </summary>
        /// <param name="nullMoveBoard"></param>
        public GameMove(Board nullMoveBoard) : this()
        {
            Board = new Board(nullMoveBoard);
        }


        public GameMove(Board boardInfo, Move move)
            : base(move)
        {
            Board = new Board(boardInfo);
            //BoardStateHash = PolyglotHelpers.GetBoardStateHash(boardInfo);
            Id = Guid.NewGuid();
        }

      

        public Guid Id { get; }


        public Board Board { get; }

       

        public string Comment { get; set; }
        public bool Validated { get; set; }
        public NumericAnnotation Annotation;
       
       

        public bool Equals(GameMove other)
        {
            if (other == null)
            {
                return false;
            }

            var baseEq = base.Equals(other.MoveValue);
            var boardEq = Board.Equals(other.Board);
            return baseEq && boardEq;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj is GameMove other && Board.Equals(other.Board) &&
                   MoveValue.Equals(other.MoveValue);
        }


        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }




        public static string GetHashString(string inputString)
        {
            var sb = new StringBuilder();
            foreach (var b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }


       

    }
}