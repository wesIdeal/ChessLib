using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ChessLib.Core.Helpers;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.Interfaces;

namespace ChessLib.Core
{
    /// <summary>
    ///     A class that stores move information in a way that can easily be hashed for quick lookup
    /// </summary>
    public class BoardSnapshot : Move, IEquatable<BoardSnapshot>
    {
        public NumericAnnotation Annotation;
        public List<MoveTree> Variations = new List<MoveTree>();

        /// <summary>
        ///     Makes a NULL move node for head of move list
        /// </summary>
        public BoardSnapshot()
        {
        }


        public BoardSnapshot(Board boardInfo, Move move)
            : base(move.MoveValue)
        {
            BoardState = (Board)boardInfo.Clone();
            BoardStateHash = PolyglotHelpers.GetBoardStateHash(boardInfo);
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public bool IsEndOfGame =>
            new[] { GameState.Checkmate, GameState.StaleMate }.Contains(BoardState.GameState);

        public BoardState BoardState { get; }

        public ulong BoardStateHash { get; }

        public string Comment { get; set; }
        public bool Validated { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return obj is BoardSnapshot other && BoardState.Equals(other.BoardState) && MoveValue.Equals(other.MoveValue);
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

        public LinkedListNode<BoardSnapshot> AddVariation(LinkedListNode<BoardSnapshot> currentMoveNode, BoardSnapshot move,
            string variationParentFEN)
        {
            var variation = new MoveTree(currentMoveNode, variationParentFEN);
            variation.AddFirst(move);
            Variations.Add(variation);
            var addedVariationIdx = Variations.IndexOf(variation);
            return Variations.ElementAt(addedVariationIdx).First;
        }

        public bool Equals(BoardSnapshot other)
        {
            return other != null &&
                   base.Equals(other) &&
                   BoardState.Equals(other.BoardState);
        }
    }



}