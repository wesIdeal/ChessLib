﻿using System;
using System.Collections.Generic;
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
    public class BoardSnapshot : Move, IEquatable<BoardSnapshot>
    {
        public NumericAnnotation Annotation;
        public List<MoveTree> Variations = new List<MoveTree>();


        /// <summary>
        ///     Makes a NULL move node for head of move list
        /// </summary>
        internal BoardSnapshot() : base(NullMoveValue)
        {
        }

        /// <summary>
        /// Establishes a <see cref="BoardSnapshot"/> as a null-move board, meaning it represents the beginning of the game or the beginning variation, with no prior moves applied
        /// </summary>
        /// <param name="nullMoveBoard"></param>
        public BoardSnapshot(Board nullMoveBoard):this()
        {
            Board = new Board(nullMoveBoard);
        }


        public BoardSnapshot(Board boardInfo, Move move)
            : base(move.MoveValue)
        {
            Board = new Board(boardInfo);
            BoardStateHash = PolyglotHelpers.GetBoardStateHash(boardInfo);
            Id = Guid.NewGuid();
        }

        public BoardSnapshot(BoardSnapshot node) : base(node)
        {
            if (node.Board != null)
            {
                Board = new Board(node.Board);
            }

            Variations = new List<MoveTree>(node.Variations);
            Annotation = node.Annotation;
            Comment = node.Comment;
            Id = node.Id;
            Validated = node.Validated;
        }

        public Guid Id { get; }


        public Board Board { get; }

        public ulong BoardStateHash { get; }

        public string Comment { get; set; }
        public bool Validated { get; set; }


        public bool Equals(BoardSnapshot other)
        {
            if (other == null)
            {
                return false;
            }

            var baseEq = base.Equals(other);
            var boardEq = Board.Equals(other.Board);
            return baseEq && boardEq;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return obj is BoardSnapshot other && Board.Equals(other.Board) &&
                   MoveValue.Equals(other.MoveValue);
        }


        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }


        public override int GetHashCode()
        {
            if (Board != null)
            {
                return base.GetHashCode() ^ Board.GetHashCode();
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


        public LinkedListNode<BoardSnapshot> AddVariation(LinkedListNode<BoardSnapshot> boardNode, Move move)
        {
            var moveTree = new MoveTree(boardNode, move);
            this.Variations.Add(moveTree);
            return moveTree.Last;
        }
    }
}