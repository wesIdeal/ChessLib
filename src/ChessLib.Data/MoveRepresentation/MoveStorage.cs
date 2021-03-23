using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ChessLib.Data.Boards;
using ChessLib.Data.MoveRepresentation.NAG;
using ChessLib.Types;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;

namespace ChessLib.Data.MoveRepresentation
{
    /// <summary>
    ///     A class that stores move information in a way that can easily be hashed for quick lookup
    /// </summary>
    public class MoveStorage : MoveExt, IEquatable<MoveStorage>, IContainsSAN
    {
        public NumericAnnotation Annotation;

        public List<MoveTree> Variations = new List<MoveTree>();

        /// <summary>
        ///     Makes a NULL move node for head of move list
        /// </summary>
        public MoveStorage()
        {
        }

        public MoveStorage(IMoveExt move, string textRepresentation)
            : this(move.Move)
        {
            SAN = textRepresentation;
        }

        public MoveStorage(IMoveExt move)
            : base(move.Move)
        {
            SAN = null;
        }

        public MoveStorage(ushort move)
            : base(move)
        {
            SAN = null;
        }

        protected MoveStorage(IMoveExt move, Piece pieceMoving, Color colorMoving) : this(move.Move)
        {
        }

        public MoveStorage(Board boardInfo, MoveExt move, Piece? capturedPiece)
            : base(move)
        {
            var boardState = (Board)boardInfo.Clone();
            BoardState = boardState;
            BoardStateHash = PolyglotHelpers.GetBoardStateHash(boardInfo);
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public bool IsEndOfGame =>
            new[] {GameState.Checkmate, GameState.StaleMate}.Contains(BoardState.GetGameState());

        public BoardState BoardState { get; }

        public ulong BoardStateHash { get; }

        public string Comment { get; set; }
        public bool Validated { get; internal set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var other = obj as MoveStorage;
            return other != null && BoardState.Equals(other.BoardState) && Move.Equals(other.Move);
        }


        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }


        public override int GetHashCode()
        {
            if (BoardState != null)
            {
                return base.GetHashCode() ^ BoardState.GetHashCode();
            }

            return -1;
        }

        public static string GetHashString(string inputString)
        {
            var sb = new StringBuilder();
            foreach (var b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        internal LinkedListNode<MoveStorage> AddVariation(LinkedListNode<MoveStorage> currentMoveNode, MoveStorage move,
            string variationParentFEN)
        {
            var variation = new MoveTree(currentMoveNode, variationParentFEN);
            variation.AddFirst(move);
            Variations.Add(variation);
            var addedVariationIdx = Variations.IndexOf(variation);
            return Variations.ElementAt(addedVariationIdx).First;
        }

        public bool Equals(MoveStorage other)
        {
            return other != null &&
                   base.Equals(other) &&
                   BoardState.Equals(other.BoardState);
        }
    }


    public interface IContainsSAN
    {
        string SAN { get; set; }
    }
}