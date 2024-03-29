﻿//using ChessLib.Data.Helpers;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using ChessLib.Core;
//using ChessLib.Core.Types;
//using ChessLib.Core.Types.Enums;
//using ChessLib.Core.Types.Helpers;

//namespace ChessLib.Data.Tests.Magic
//{
//    [TestFixture]
//    public class Bitboard
//    {
//        [TestCase("rnbqkbnr/pppp1ppp/8/3Pp3/8/8/PPP1PPPP/RNBQKBNR w KQkq e6 0 1", 35, 44,
//            "En passant should be available at e6.")]
//        [TestCase("rnbqkbnr/pp1ppppp/8/2pP4/8/8/PPP1PPPP/RNBQKBNR w KQkq c6 0 1", 35, 42,
//            "En passant should be available at c6.")]
//        public void EnPassantShouldBeALegalMove(string fen, int pieceSourceIndex, int epSquare, string errorMessage = "")
//        {
//            try
//            {
//                var board = new Board(fen);
//                var moves = Data.Magic.Bitboard.GetPseudoLegalMoves(Piece.Pawn, (ushort) pieceSourceIndex,
//                    board.Occupancy.Occupancy(board.ActivePlayer),
//                    board.Occupancy.Occupancy(board.OpponentColor), board.ActivePlayer, board.EnPassantSquare, board.CastlingAvailability, out _);
//                var epSquareIncluded = moves.IsBitSet((ushort)epSquare);
//                Assert.IsTrue(epSquareIncluded, errorMessage);
//            }
//            catch (Exception e)
//            {
//                var exc = e;
//            }
//        }

//        [TestCase("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1", 4, 6, "White should be able to castle short.")]
//        [TestCase("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1", 4, 2, "White should be able to castle long.")]
//        [TestCase("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R b KQkq - 0 1", 60, 62,
//            "Black should be able to castle short.")]
//        [TestCase("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R b KQkq - 0 1", 60, 58, "Black should be able to castle long.")]
//        public void TestThatCastlingIsAPseudoLegalMove(string fen, int pieceSourceIndex, int destIndex,
//            string errorMessage = "")
//        {
//            var board = new Board(fen);
//            var moves = ChessLib.Data.Magic.Bitboard.GetPseudoLegalMoves(Piece.King, (ushort)pieceSourceIndex, board.Occupancy.Occupancy(board.ActivePlayer),
//                board.Occupancy.Occupancy(board.OpponentColor), board.ActivePlayer, board.EnPassantSquare, board.CastlingAvailability, out List<Move> arrMoves);
//            var epSquareIncluded = moves.IsBitSet((ushort)destIndex);
//            Assert.IsTrue(epSquareIncluded, errorMessage);
//            var move = MoveHelpers.GenerateMove((ushort)pieceSourceIndex, (ushort)destIndex, MoveType.Castle);
//            Assert.Contains(move, arrMoves);
//        }
//    }
//}