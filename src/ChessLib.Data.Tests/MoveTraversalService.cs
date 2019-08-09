﻿using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Parse.PGN;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ChessLib.Data.Tests
{
    [TestFixture]
    class MoveTraversalServiceTest
    {
        private MoveTraversalService _svc;

        private static readonly object[] UnApplyTestCases = new object[]
           {
                //Ruy
                new object[]
                {
                    FENHelpers.FENInitial,
                    new string[] {"e4", "e5", "Nf3", "Nc6", "Bb5", "a6", "Bxc6"},
                    "Ruy Lopez"

                },
                new object[]
                {
                    "rnbqkbnr/1pp1pppp/8/p2pP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3",
                    new string[]{"exd6"},
                    "En Passant"
                },
                new object[]
                {
                "r3k2r/qpp1pppp/2nP1n2/pb3b2/2BQ4/B1N2N2/PPPP1PPP/R3K2R w KQkq - 0 3",
                new string[]{"O-O", "O-O"},
                "Castling Kingside"
                },
                new object[]
                {
                    "r3k2r/qpp1pppp/2nP1n2/pb3b2/2BQ4/B1N2N2/PPPP1PPP/R3K2R w KQkq - 0 3",
                    new string[]{"O-O-O", "O-O-O"},
                    "Castling Queenside"
                },
                new object[]
                {
                    "1q6/2P5/8/8/5k1K/8/4p3/3Q4 w - - 0 1",
                    new string[]{"c8=N", "e1=N"},
                    "Promotion to Knight"
                },
                new object[]
                {
                    "1q6/2P5/8/8/5k1K/8/4p3/3Q4 w - - 0 1",
                    new string[]{"c8=N", "e1=N"},
                    "Promotion to Bishop"
                },
                new object[]
                {
                    "1q6/2P5/8/8/5k1K/8/4p3/3Q4 w - - 0 1",
                    new string[]{"c8=R", "e1=R"},
                    "Promotion to Rook"
                },
                new object[]
                {
                    "1q6/2P5/8/8/5k1K/8/4p3/3Q4 w - - 0 1",
                    new string[]{"c8=Q", "e1=Q"},
                    "Promotion to Queen"
                },
                new object[]
                {
                "1q6/2P5/6k1/8/7K/8/4p3/3Q4 w - - 0 1",
                new string[]{"cxb8=Q", "exd1=Q"},
                "Promotion to Queen after capture"
                }
           };

        [Test]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "e4", new ushort[] { 12, 28 })]
        [TestCase("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", "d5", new ushort[] { 51, 35 })]
        [TestCase("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 2", "exd5", new ushort[] { 35, 28 })]
        [TestCase("rnbqkbnr/ppp1pppp/8/3P4/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 2", "e5", new ushort[] { 52, 36 })]
        [TestCase("rnbqkbnr/ppp2ppp/8/3Pp3/8/8/PPPP1PPP/RNBQKBNR w KQkq e6 0 3", "dxe6", new ushort[] { 35, 44, 36 })]
        [TestCase("rnbqkbnr/ppp2ppp/4P3/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 3", "Bxe6", new ushort[] { 58, 44 })]
        [TestCase("r3kbnr/ppp1qppp/2n1b3/8/8/5N2/PPPPBPPP/RNBQK2R w KQkq - 4 6", "O-O", new ushort[] { 4, 5, 6, 7 })]
        [TestCase("r3kbnr/ppp1qppp/2n1b3/8/8/5N2/PPPPBPPP/RNBQ1RK1 b kq - 5 6", "O-O-O", new ushort[] { 56, 58, 59, 60 })]
        public void GetSquaresUpdated(string f1, string san, ushort[] expected)
        {
            var translator = new MoveTranslatorService(f1);
            var move = translator.GetMoveFromSAN(san);
            var actual = MoveTraversalService.GetSquaresUpdated(move);
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test(Description = "1. e4 test")]
        public void ApplyMove_ShouldReflectCorrectBoardStatusAfter_e4()
        {
            _svc = new MoveTraversalService();
            var move = MoveHelpers.GenerateMove(12, 28);
            var expectedFEN = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";
            _svc.ApplyMove(move);
            Assert.AreEqual(expectedFEN, _svc.CurrentFEN);
        }

        [Test(Description = "Ruy - applying series of moves")]
        public void ApplyMove_ShouldReflectCorrectBoardStatusAfterSeriesOfMoves()
        {
            var expectedFEN = new[]
            {
                    "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", //1. e4
                    "rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e6 0 2", //1...e5
                    "rnbqkbnr/pppp1ppp/8/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2", //2. Nf3
                    "r1bqkbnr/pppp1ppp/2n5/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 2 3", //2...Nc6
                    "r1bqkbnr/pppp1ppp/2n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3", //3. Bb5
                    "r1bqkbnr/1ppp1ppp/p1n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 0 4", //3...a6
                    "r1bqkbnr/1ppp1ppp/p1n5/4p3/B3P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 4", //4. Ba4
                    "r1bqkb1r/1ppp1ppp/p1n2n2/4p3/B3P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 2 5", // 4...Nf6
                    "r1bqkb1r/1ppp1ppp/p1n2n2/4p3/B3P3/5N2/PPPP1PPP/RNBQ1RK1 b kq - 3 5" //O-O
                };
            var moves = new[]
            {
                    MoveHelpers.GenerateMove(12, 28),
                    MoveHelpers.GenerateMove(52, 36),
                    MoveHelpers.GenerateMove(6, 21),
                    MoveHelpers.GenerateMove(57, 42),
                    MoveHelpers.GenerateMove(5, 33),
                    MoveHelpers.GenerateMove(48, 40),
                    MoveHelpers.GenerateMove(33, 24),
                    MoveHelpers.GenerateMove(62, 45),
                    MoveHelpers.GenerateMove(4, 6, MoveType.Castle)
                };
            Assert.AreEqual(expectedFEN.Length, moves.Length);
            _svc = new MoveTraversalService();
            for (var i = 0; i < moves.Length; i++)
            {
                var expected = expectedFEN[i];
                _svc.ApplyMove(moves[i]);
                Assert.AreEqual(expected, _svc.CurrentFEN);
            }
        }

        [Test, TestCaseSource(nameof(UnApplyTestCases))]
        public void UnapplyMove(string fenStart, string[] moves, string description)
        {
            var moveTree = new MoveTree<MoveStorage>(null);
            _svc = new MoveTraversalService(fenStart, ref moveTree);
            var stateStack = new Stack<string>();
            int index;
            for (index = 0; index < moves.Length; index++)
            {
                var board = _svc.Board;
                var move = moves[index];
                var moveTranslator = new MoveTranslatorService(_svc.CurrentFEN);
                var moveExt = moveTranslator.GetMoveFromSAN(move);
                stateStack.Push(_svc.CurrentFEN);
                _svc.ApplyMove(moveExt);
            }

            string expectedState;
            for (; index > 0; index--)
            {
                expectedState = stateStack.Pop();
                Assert.AreNotEqual(_svc.CurrentFEN, expectedState, $"{description}: expected state should not equal current state.");
                _svc.TraverseBackward();
                Assert.AreEqual(expectedState, _svc.CurrentFEN, $"{description}: current state not equal to the expected state after undoing move {index}.");
            }

        }

        [Test]
        public void FindNextMoves_ShouldReturnVariationsInOrder()
        {
            var game = LoadGameByPGN(PGN.WithVariations);
            _svc = new MoveTraversalService(game.TagSection.FENStart, ref game.MoveSection);
            var appliedMove = _svc.TraverseForward(new MoveStorage(666));
            var moves = _svc.GetNextMoves();
            Assert.AreEqual(4, moves.Count());
            Assert.AreEqual(4013, moves[0].Move);
            Assert.AreEqual(3364, moves[1].Move);
            Assert.AreEqual(3372, moves[2].Move);
            Assert.AreEqual(3234, moves[3].Move);
        }

        [Test]
        public void TraversePGN()
        {
            var sb = new StringBuilder();
            (MoveStorage moveApplied, string premoveFEN) expectedState;
            var game = LoadGameByPGN(PGN.Fischer01);
            _svc = new MoveTraversalService(game.TagSection.FENStart, ref game.MoveSection);
            var stateStack = new Stack<(MoveStorage moveApplied, string premoveFEN)>();
            var moves = game.MoveSection;
            sb.AppendLine($"****APPLYING {game.MoveSection.Count()} MOVES****");
            foreach (var move in moves)
            {
                var premoveFEN = _svc.Board.CurrentFEN;
                stateStack.Push((moveApplied: move, premoveFEN: premoveFEN));
                _svc.TraverseForward(move);
                var postmoveFEN = _svc.Board.CurrentFEN;
                sb.AppendLine($"\t{move}\t{premoveFEN}->{postmoveFEN}");
            }

            sb.AppendLine("****UNAPPLYING MOVES****");
            while (stateStack.TryPop(out expectedState))
            {
                var moveUnapplied = expectedState.moveApplied;
                _svc.TraverseBackward();
                try
                {
                    MoveDisplayService mts = new MoveDisplayService(expectedState.premoveFEN);
                    Assert.AreEqual(expectedState.premoveFEN, _svc.CurrentFEN,
                        $"Current state != to expected after undoing {mts.MoveToSAN(moveUnapplied)}.\r\nMove applied to FEN {expectedState.premoveFEN}");


                }
                catch
                {
                    Console.WriteLine(sb.ToString());
                    throw;
                }
                Debug.WriteLine($"Unapplied {moveUnapplied} successfully.");
            }
            Console.WriteLine(sb.ToString());
        }

        [Test]
        public void FindNextMoves_ShouldReturnEmptyCollectionIfTheEndIsReached()
        {
            var game = LoadGameByPGN(PGN.Fischer01);
            _svc = new MoveTraversalService(game);
            _svc.GoToLastMove();
            var moves = _svc.GetNextMoves();
            Assert.IsEmpty(moves);
        }

        [Test]
        public void TraversingBackwardOnFirstMoveShouldReturnInitialBoard()
        {
            _svc = new MoveTraversalService();
            _svc.TraverseBackward();
            for (int i = 0; i < 25; i++)
            {
                var rv = _svc.TraverseBackward();
                Assert.AreEqual(FENHelpers.FENInitial, _svc.CurrentFEN);
            }
        }

        [Test]
        public void TraversingForwardWithEmptyTreeShouldReturnInitialBoard()
        {
            var game = new Game<MoveStorage>();
            var _svc = new MoveTraversalService(game);
            for (int i = 0; i < 25; i++)
            {
                var rv = _svc.TraverseForward(new MoveStorage(new MoveExt(405), "Nf3"));
                Assert.AreEqual(_svc.InitialFEN, _svc.CurrentFEN);
            }
        }

        [Test]
        public void TraverseBackward_OutOfVariation_ShouldReturnCorrectMove()
        {
            var game = LoadGameByPGN(PGN.ShortVariation);
            _svc = new MoveTraversalService(game);
            MoveStorage[] moves;
            MoveStorage move = null;
            while ((moves = _svc.GetNextMoves()).Count() == 1)
            {
                move = moves[0];
                _svc.TraverseForward(move);
            }
            _svc.TraverseForward(moves[1]);
            _svc.TraverseBackward();
            Assert.AreEqual(move, _svc.CurrentMove.MoveData);
        }

        [TestCase(nameof(PGN.Puzzle), "r1b2rk1/pp1p1pp1/1b1p2B1/n1qQ2p1/8/5N2/P3RPPP/4R1K1 w - - 0 1")]
        [TestCase(nameof(PGN.Fischer01), "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
        public void GoToInitialState_ShouldApplyInitialState(string nameOfPgn, string expected)
        {
            var game = LoadGameIntoBoardByName(nameOfPgn);
            _svc = new MoveTraversalService(game);
            _svc.GoToLastMove();
            _svc.GoToInitialState();
            Assert.AreEqual(_svc.MoveTree.HeadMove, _svc.CurrentMove);
            Assert.AreEqual(expected, _svc.CurrentFEN);
        }

        [TestCase(nameof(PGN.Puzzle), "r1b1Rk2/pp1p2p1/1b1p2B1/n1q3p1/8/5N2/P4PPP/6K1 b - - 1 4")]
        [TestCase(nameof(PGN.Fischer01), "8/2b5/8/p5R1/P1p1P2p/4kP2/1P4K1/8 b - - 0 52")]
        public void GoToLastMove_ShouldApplyLastMove(string nameOfPgn, string expected)
        {
            var game = LoadGameIntoBoardByName(nameOfPgn);
            _svc = new MoveTraversalService(game);
            _svc.GoToLastMove();
            Assert.AreEqual(_svc.MoveTree.LastMove, _svc.CurrentMove);
            Assert.AreEqual(expected, _svc.CurrentFEN);
        }

        private Game<MoveStorage> LoadGameByPGN(string pgn)
        {
            var parser = new ParsePgn();
            var game = parser.ParseAndValidateGames(pgn).First();
            return game;
        }

        private Game<MoveStorage> LoadGameIntoBoardByName(string name)
        {
            var data = PGN.ResourceManager.GetString(name);
            return LoadGameByPGN(data);
        }
    }
}
