﻿using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using NUnit.Framework;
using System.Linq;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Data.Validators.MoveValidation;
using ChessLib.Data.Boards;

namespace ChessLib.Data.Validators.MoveValidation.MoveRules.Tests
{
    [TestFixture]
    internal class PieceCanMoveToDestination : Data.Validators.MoveValidation.MoveRules.PieceCanMoveToDestination
    {
        protected static readonly ulong[][] _postMoveBoard = new ulong[2][];

        [TestCase("r4rk1/1bqn2bp/pp1pp1p1/2p5/P1N1nB2/2N4P/1PP1BPP1/R2Q1RK1 w - - 0 15", "f4d6", MoveError.NoneSet)]
        public void TestPreviouslyIncorrectValidation(string fen, string lanMove, MoveError expectedError)
        {
            var bi = new BoardInfo(fen);
            var moveTranslator = new MoveTranslatorService(bi);
            var move = moveTranslator.FromLongAlgebraicNotation(lanMove);
            var moveValidator = new MoveValidator(bi, move);
            var actual = moveValidator.Validate();
            Assert.AreEqual(expectedError, actual);
        }

        [TestCase("r1b1k2r/1pq2ppp/p1p1p3/3nP3/5P2/2P2Q2/P1PB2PP/R3KB1R w KQkq - 0 12", 10, 26, "Pawn can't jump over doubled pawn on c3 to get to c4. :(")]
        [TestCase("rnbqkbnr/pp1ppppp/8/8/2p5/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 10, 26, "Pawn can't move to c4, as it is occuplied.")]
        public void ShouldReturnErrorWhenPawnBlocked(string fen, int from, int to, string errorMsg = "")
        {
            var bi = new BoardInfo(fen);
            var moveValidator = new MoveValidator(bi, MoveHelpers.GenerateMove((ushort)from, (ushort)to));
            var actual = moveValidator.Validate();
            Assert.AreEqual(MoveError.BadDestination, actual, errorMsg);
        }

        [Test]
        public void ShouldReturnNull_OnValidMove_EmptySquare_Edge()
        {
            var move = MoveHelpers.GenerateMove(0, 56);
            var BoardInfo = new BoardInfo("4k3/8/8/8/8/8/8/R3K3 w - - 0 1");
            Assert.AreEqual(MoveError.NoneSet, Validate(BoardInfo, _postMoveBoard, move), "Should return null when moving to empty square on a8.");
        }

        [Test]
        public void ShouldReturnNull_OnValidMove_EmptySquare_Middle()
        {
            var move = MoveHelpers.GenerateMove(2, 26);
            var BoardInfo = new BoardInfo("4k3/8/8/8/8/8/8/2R1K3 w - - 0 1");
            Assert.AreEqual(MoveError.NoneSet, Validate(BoardInfo, _postMoveBoard, move), "Should return null when moving to empty square on c4.");
        }

        [Test]
        public void ShouldReturnNull_OnValidMove_OpponentPieceAttacked_Edge()
        {
            var move = MoveHelpers.GenerateMove(2, 58);
            var BoardInfo = new BoardInfo("2q1k3/8/8/8/8/8/8/2R1K3 w - - 0 1");
            Assert.AreEqual(MoveError.NoneSet, Validate(BoardInfo, _postMoveBoard, move), "Should return null when capturing on c8.");

        }

        [Test]
        public void ShouldReturnNull_OnValidMove_OpponentPieceAttacked_Middle()
        {
            var move = MoveHelpers.GenerateMove(2, 26);
            var BoardInfo = new BoardInfo("4k3/8/8/8/2q5/8/8/2R1K3 w - - 0 1");
            Assert.AreEqual(MoveError.NoneSet, Validate(BoardInfo, _postMoveBoard, move), "Should return null when capturing on c4.");
        }

        [Test]
        public void ShouldReturnProperError_WhenActivePlayerHasNoPieceAtSource()
        {
            var move = MoveHelpers.GenerateMove(0, 56);
            var BoardInfo = new BoardInfo("4k3/8/8/8/2q5/8/8/2R1K3 w - - 0 1");
            Assert.AreEqual(MoveError.ActivePlayerHasNoPieceOnSourceSquare, Validate(BoardInfo, _postMoveBoard, move), "Should return error when active color has no piece at source.");
        }

        [Test]
        public void ShouldReturnProperError_WhenSlidingPieceBlocked()
        {
            var move = MoveHelpers.GenerateMove(2, 26);
            var BoardInfo = new BoardInfo("4k3/8/8/8/2q5/2p5/8/2R1K3 w - - 0 1");
            Assert.AreEqual(MoveError.BadDestination, Validate(BoardInfo, _postMoveBoard, move), "Should return error when capturing on c4 but is blocked.");
        }

        [Test]
        public void ShouldReturnProperError_WhenDestinationIsInvalidForPiece()
        {
            var move = MoveHelpers.GenerateMove(2, 11);
            var BoardInfo = new BoardInfo("4k3/8/8/8/2q5/8/8/2R1K3 w - - 0 1");
            Assert.AreEqual(MoveError.BadDestination, Validate(BoardInfo, _postMoveBoard, move), "Should return error when the destination is invalid.");
        }

        class PawnMoves : Data.Validators.MoveValidation.MoveRules.PieceCanMoveToDestination
        {
            private const string InitialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            private BoardInfo _BoardInfo;

            [SetUp]
            public void Setup()
            {
                _BoardInfo = new BoardInfo(InitialFEN);
            }
            #region Validation Errors
            [Test]
            public void ShouldReturnProperError_WhenWhitePawnMovesBackwards()
            {
                var bi = new BoardInfo("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(28, 12);

                var actual = Validate(bi, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Pawns can't move backwards.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackPawnMovesBackwards()
            {
                var bi = new BoardInfo("rnbqkbnr/pppp1ppp/8/4p3/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(36, 52);

                var actual = Validate(bi, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Pawns can't move backwards.");
            }

            [Test]
            public void ShouldReturnProperError_WhenWhitePawnAttacksEmptySquare()
            {
                var move = MoveHelpers.GenerateMove(12, 21);
                var actual = Validate(_BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Pawns can't capture an empty square to the NE.");
                move = MoveHelpers.GenerateMove(12, 19);
                actual = Validate(_BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Pawns can't capture an empty square to the NW.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackPawnAttacksEmptySquare()
            {
                _BoardInfo.ActivePlayer = Color.Black;
                var move = MoveHelpers.GenerateMove(52, 43);
                var actual = Validate(_BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Pawns can't capture an empty square to the SE.");
                move = MoveHelpers.GenerateMove(52, 45);
                actual = Validate(_BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Pawns can't capture an empty square to the SW.");
            }

            [Test]
            public void ShouldReturnProperError_WhenWhitePawnAttacksSquareOccupiedByWhite()
            {
                var bi = new BoardInfo("rnbqkbnr/pp1ppp1p/3p1p2/8/8/3P1P2/PP1PPP1P/RNBQKBNR w KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(12, 21);
                var actual = Validate(bi, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Pawns can't capture a friendly-occupied square to the NE.");
                move = MoveHelpers.GenerateMove(12, 19);
                actual = Validate(bi, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Pawns can't capture a friendly-occupied square to the NW.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackPawnAttacksSquareOccupiedByWhite()
            {
                var bi = new BoardInfo("rnbqkbnr/pp1ppp1p/3p1p2/8/8/3P1P2/PP1PPP1P/RNBQKBNR b KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(52, 43);
                var actual = Validate(bi, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Pawns can't capture a friendly-occupied square to the SE.");
                move = MoveHelpers.GenerateMove(52, 45);
                actual = Validate(bi, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Pawns can't capture a friendly-occupied square to the SW.");
            }

            [Test]
            public void ShouldReturnProperError_WhenWhitePawnIsBlockedFromInitial()
            {
                var bi1 = new BoardInfo("rnbqkbnr/pppp1ppp/8/8/8/4p3/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
                var bi2 = new BoardInfo("rnbqkbnr/pppp1ppp/8/8/4p3/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
                var move1 = MoveHelpers.GenerateMove(12, 20);
                var move2 = MoveHelpers.GenerateMove(12, 28);
                var actual = Validate(bi1, _postMoveBoard, move1);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Cannot move pawn when piece blocks its mobility.");
                actual = Validate(bi2, _postMoveBoard, move2);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Cannot move pawn 2 squares in opening when piece blocks its mobility.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackPawnIsBlockedFromInitial()
            {
                var bi1 = new BoardInfo("rnbqkbnr/pppppppp/4P3/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1");
                var bi2 = new BoardInfo("rnbqkbnr/pppppppp/8/4P3/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1");
                var move1 = MoveHelpers.GenerateMove(52, 44);
                var move2 = MoveHelpers.GenerateMove(52, 36);
                var actual = Validate(bi1, _postMoveBoard, move1);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Cannot move pawn when piece blocks its mobility.");
                actual = Validate(bi2, _postMoveBoard, move2);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Cannot move pawn 2 squares in opening when piece blocks its mobility.");
            }

            [Test]
            public void ShouldReturnProperError_WhenWhitePawnMoves2From3rdRank()
            {
                var bi = new BoardInfo("rnbqkbnr/pppp1ppp/4p3/8/8/4P3/PPPP1PPP/RNBQKBNR w KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(20, 36);
                var actual = Validate(bi, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, "Pawn cannot move 2 squares if not on the opening rank.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackPawnMoves2From6thRank()
            {
                var bi = new BoardInfo("rnbqkbnr/pppp1ppp/4p3/8/8/4P3/PPPP1PPP/RNBQKBNR b KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(44, 28);
                var actual = Validate(bi, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, "Pawn cannot move 2 squares if not on the opening rank.");
            }

            [Test]
            public void ShouldReturnProperError_WhenWhitePawnIsBlockedInMiddle()
            {
                var bi1 = new BoardInfo("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1");
                var move1 = MoveHelpers.GenerateMove(28, 36);

                var actual = Validate(bi1, _postMoveBoard, move1);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Cannot move pawn when piece blocks its mobility.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackPawnIsBlockedInMiddle()
            {
                var bi1 = new BoardInfo("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1");

                var move1 = MoveHelpers.GenerateMove(36, 28);

                var actual = Validate(bi1, _postMoveBoard, move1);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Cannot move pawn when piece blocks its mobility.");
            }

            [Test]
            public void ShouldReturnProperError_WhenWhiteTryingToAttackAcrossBoard()
            {
                var bi1 = new BoardInfo("rnbqkbnr/ppppppp1/8/8/8/8/PPPPPPPp/RNBQKBNR w KQkq - 0 1");

                var move1 = MoveHelpers.GenerateMove(8, 15);

                var actual = Validate(bi1, _postMoveBoard, move1);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Cannot capture across board.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackTryingToAttackAcrossBoard()
            {
                var bi1 = new BoardInfo("rnbqkbnr/pppppppp/8/7P/8/8/PPPPPPP1/RNBQKBNR b KQkq - 0 1");

                var move1 = MoveHelpers.GenerateMove(48, 39);

                var actual = Validate(bi1, _postMoveBoard, move1);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Cannot capture across board.");
            }

            [Test]
            public void ShouldReturnProperError_WhenWhitePawnMoves3()
            {
                var move = MoveHelpers.GenerateMove(12, 36);
                var actual = Validate(_BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Pawns can't move 3 squares.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackPawnMoves3()
            {
                _BoardInfo.ActivePlayer = Color.Black;
                var move = MoveHelpers.GenerateMove(52, 28);
                var actual = Validate(_BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, "Expected validation error. Pawns can't move 3 squares.");
            }
            #endregion

            #region Validation Non-errors
            [Test]
            public void ShouldReturnNoError_WhenWhitePawnMovesForward1()
            {
                var move = MoveHelpers.GenerateMove(12, 20);
                var actual = Validate(_BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual, "Expected no error. Pawns can move 1 forward.");
            }

            [Test]
            public void ShouldReturnNoError_WhenBlackPawnMovesForward1()
            {
                _BoardInfo.ActivePlayer = Color.Black;
                var move = MoveHelpers.GenerateMove(52, 44);
                var actual = Validate(_BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual, "Expected no error. Pawns can move 1 forward.");
            }
            [Test]
            public void ShouldReturnNoError_WhenWhitePawnMoves2From2ndRank()
            {
                var move = MoveHelpers.GenerateMove(12, 28);
                var actual = Validate(_BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual, "Pawn can move 2 squares from the opening rank.");
            }

            [Test]
            public void ShouldReturnNoError_WhenBlackPawnMoves2From7thRank()
            {
                _BoardInfo.ActivePlayer = Color.Black;
                var move = MoveHelpers.GenerateMove(52, 36);
                var actual = Validate(_BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual, "Pawn can move 2 squares from the opening rank.");
            }
            [Test]
            public void ShouldReturnNoError_WhenWhitePawnAttacksEnemyOccupiedSquare()
            {
                var bi1 = new BoardInfo("rnbqkbnr/ppp1p1pp/3P1P2/8/8/3p1p2/PP2PPPP/RNBQKBNR w KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(12, 21);
                var actual = Validate(bi1, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual, "Expected no error. Pawns can capture an enemy-occupied square to the NE.");
                move = MoveHelpers.GenerateMove(12, 19);
                actual = Validate(bi1, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual, "Expected no error. Pawns can capture an enemy-occupied square to the NW.");
            }

            [Test]
            public void ShouldReturnNoError_WhenBlackPawnAttacksEnemyOccupiedSquare()
            {
                var bi1 = new BoardInfo("rnbqkbnr/ppp1p1pp/3P1P2/8/8/3p1p2/PP2PPPP/RNBQKBNR b KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(52, 43);
                var actual = Validate(bi1, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual, "Expected no error. Pawns can capture an enemy-occupied square to the SE.");
                move = MoveHelpers.GenerateMove(52, 45);
                actual = Validate(bi1, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual, "Expected no error. Pawns can capture an enemy-occupied square to the SW.");
            }

            #endregion

        }

        class KnightMoves : Data.Validators.MoveValidation.MoveRules.PieceCanMoveToDestination
        {
            private static readonly MoveExt Ne5Tof3 = MoveHelpers.GenerateMove(36, 21);
            private static readonly MoveExt Ne5Tod3 = MoveHelpers.GenerateMove(36, 19);
            private static readonly MoveExt Ne5Toc4 = MoveHelpers.GenerateMove(36, 26);
            private static readonly MoveExt Ne5Toc6 = MoveHelpers.GenerateMove(36, 42);
            private static readonly MoveExt Ne5Tod7 = MoveHelpers.GenerateMove(36, 51);
            private static readonly MoveExt Ne5Tof7 = MoveHelpers.GenerateMove(36, 53);
            private static readonly MoveExt Ne5Tog6 = MoveHelpers.GenerateMove(36, 46);
            private static readonly MoveExt Ne5Tog4 = MoveHelpers.GenerateMove(36, 30);



            private readonly MoveExt[] _movesFromE5 = new[] { Ne5Tof3, Ne5Tod3, Ne5Toc4, Ne5Toc6, Ne5Tod7, Ne5Tof7, Ne5Tog6, Ne5Tog4 };

            private readonly MoveExt[] _movesFromCorners = new[]
            {
                MoveHelpers.GenerateMove(0, 17),
                MoveHelpers.GenerateMove(0, 10),
                MoveHelpers.GenerateMove(7, 13),
                MoveHelpers.GenerateMove(7, 22),
                MoveHelpers.GenerateMove(56, 41),
                MoveHelpers.GenerateMove(56, 50),
                MoveHelpers.GenerateMove(63, 53),
                MoveHelpers.GenerateMove(63, 46)

            };


            private readonly string _fenAllEnemyOccupied = "rnbqkbnr/3p1p2/2p3p1/4N3/2p3p1/3p1p2/PPPPPPPP/RNBQKB1R w KQkq - 0 1";
            private readonly string _fenAllFriendlyOccupied = "rk1q1bnr/pppPpPpp/2P3P1/4N3/2P3P1/3P1P2/8/RNBQKB1R w KQ - 0 1";
            private readonly string _fenAllEmpty = "rnbqkbnr/ppp1p1pp/8/4N3/8/8/PPPPPPPP/RNBQKB1R w KQkq - 0 1";
            private readonly string _fenKnightInCorners = "NnbqkbnN/pp2p1pp/8/8/8/8/PP1PP1PP/N1BQKB1N w - - 0 1";
            private readonly string _fenBare = "4k3/8/8/4N3/8/8/8/4K3 w - - 0 1";

            private readonly MoveExt[] _illegalMoves = new MoveExt[56];
            [OneTimeSetUp]
            public void OneTimeSetUp()
            {
                var arrIndex = 0;
                var legalDestinationIndexes = _movesFromE5.Select(x => x.DestinationIndex).ToArray();
                for (ushort i = 0; i < 64; i++)
                {
                    if (!legalDestinationIndexes.Contains(i))
                    {
                        _illegalMoves[arrIndex] = MoveHelpers.GenerateMove(36, i);
                        arrIndex++;
                    }
                }
            }

            [Test]
            public void ShouldReturnError_WhenMoveIsToIllegalSquare()
            {
                var board = new BoardInfo(_fenBare);
                foreach (var move in _illegalMoves)
                {
                    var actual = Validate(board, _postMoveBoard, move);
                    Assert.AreEqual(MoveError.BadDestination, actual, $"Knight should not be able to move to illegal square. Move was from {move.SourceIndex.IndexToSquareDisplay()} to {move.DestinationIndex.IndexToSquareDisplay()}");
                }
            }

            [Test]
            public void ShouldReturnNoError_WhenKnightMovesFromCorner()
            {
                var board = new BoardInfo(_fenKnightInCorners);
                foreach (var move in _movesFromCorners)
                {
                    var actual = Validate(board, _postMoveBoard, move);
                    Assert.AreEqual(MoveError.NoneSet, actual, $"Knight should be able to move to legal empty square from corner. Move was from {move.SourceIndex.IndexToSquareDisplay()} to {move.DestinationIndex.IndexToSquareDisplay()}");
                }

            }

            [Test]
            public void ShouldReturnNoError_WhenAllTargetSquaresAreEmpty()
            {
                var board = new BoardInfo(_fenAllEmpty);
                foreach (var move in _movesFromE5)
                {
                    var actual = Validate(board, _postMoveBoard, move);
                    Assert.AreEqual(MoveError.NoneSet, actual, $"Knight should be able to move to legal empty square. Move was from {move.SourceIndex.IndexToSquareDisplay()} to {move.DestinationIndex.IndexToSquareDisplay()}");
                }

            }

            [Test]
            public void ShouldReturnNoError_WhenAllTargetSquaresAreOccupiedByOpponent()
            {
                var board = new BoardInfo(_fenAllEnemyOccupied);
                foreach (var move in _movesFromE5)
                {
                    var actual = Validate(board, _postMoveBoard, move);
                    Assert.AreEqual(MoveError.NoneSet, actual, $"Knight should be able to move to legal square occupied by opponent. Move was from {move.SourceIndex.IndexToSquareDisplay()} to {move.DestinationIndex.IndexToSquareDisplay()}");
                }

            }

            [Test]
            public void ShouldReturnError_WhenAllTargetSquaresAreOccupiedByActiveSide()
            {
                var board = new BoardInfo(_fenAllFriendlyOccupied);
                foreach (var move in _movesFromE5)
                {
                    var actual = Validate(board, _postMoveBoard, move);
                    Assert.AreEqual(MoveError.BadDestination, actual, $"Knight should not be able to move to legal square occupied by side to move. Move was from {move.SourceIndex.IndexToSquareDisplay()} to {move.DestinationIndex.IndexToSquareDisplay()}");
                }

            }
        }

        /// <summary>
        /// Test Bishop Moves- should be minimal, as sliding piece attacks were tested by calculated shifts
        /// </summary>
        class BishopQueenMoves : Data.Validators.MoveValidation.MoveRules.PieceCanMoveToDestination
        {

            private readonly SlidingPieceMoveAndBoard _bAttacksEnemyOnE5 =
                new SlidingPieceMoveAndBoard(18, 36, "4k3/8/8/4r3/8/2B5/8/4K3 w - - 0 1");
            private readonly SlidingPieceMoveAndBoard _bAttacksFriendlyOnE5 = new SlidingPieceMoveAndBoard(18, 36, "5k2/8/8/4R3/8/2B5/8/4K3 w - - 0 1");
            private readonly SlidingPieceMoveAndBoard _qAttacksEnemyOnE5 =
                new SlidingPieceMoveAndBoard(18, 36, "4k3/8/8/4r3/8/2Q5/8/4K3 w - - 0 1");
            private readonly SlidingPieceMoveAndBoard _qAttacksFriendlyOnE5 = new SlidingPieceMoveAndBoard(18, 36, "5k2/8/8/4R3/8/2Q5/8/4K3 w - - 0 1");

            [Test]
            public void ShouldReturnNoError_WhenBishopCapturesEnemy()
            {
                var actual = Validate(_bAttacksEnemyOnE5.BoardInfo, _postMoveBoard, _bAttacksEnemyOnE5.Move);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Bishop should be able to capture enemy.  {_bAttacksEnemyOnE5.Move.SourceIndex.IndexToSquareDisplay()} to {_bAttacksEnemyOnE5.Move.DestinationIndex.IndexToSquareDisplay()}");
                actual = Validate(_qAttacksEnemyOnE5.BoardInfo, _postMoveBoard, _qAttacksEnemyOnE5.Move);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Queen should be able to capture enemy.  {_qAttacksEnemyOnE5.Move.SourceIndex.IndexToSquareDisplay()} to {_qAttacksEnemyOnE5.Move.DestinationIndex.IndexToSquareDisplay()}");
            }

            [Test]
            public void ShouldReturnNoError_WhenBishopMovesToFriendlySquare()
            {
                var actual = Validate(_bAttacksFriendlyOnE5.BoardInfo, _postMoveBoard, _bAttacksFriendlyOnE5.Move);
                Assert.AreEqual(MoveError.BadDestination, actual, $"Bishop should not be able to capture friendly piece.  {_bAttacksFriendlyOnE5.Move.SourceIndex.IndexToSquareDisplay()} to {_bAttacksFriendlyOnE5.Move.DestinationIndex.IndexToSquareDisplay()}");
                actual = Validate(_qAttacksFriendlyOnE5.BoardInfo, _postMoveBoard, _qAttacksFriendlyOnE5.Move);
                Assert.AreEqual(MoveError.BadDestination, actual, $"Queen should not be able to capture friendly piece.  {_qAttacksFriendlyOnE5.Move.SourceIndex.IndexToSquareDisplay()} to {_qAttacksFriendlyOnE5.Move.DestinationIndex.IndexToSquareDisplay()}");
            }

            [Test]
            public void ShouldReturnNoError_WhenBishopMovesShortOfCapturingEnemy()
            {
                var move = MoveHelpers.GenerateMove(18, 27);
                var actual = Validate(_bAttacksEnemyOnE5.BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Bishop should be able to move short of enemy piece.  {move.SourceIndex.IndexToSquareDisplay()} to {move.DestinationIndex.IndexToSquareDisplay()}");

                actual = Validate(_qAttacksEnemyOnE5.BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Queen should be able to move short of enemy piece.  {move.SourceIndex.IndexToSquareDisplay()} to {move.DestinationIndex.IndexToSquareDisplay()}");
            }
            [Test]
            public void ShouldReturnNoError_WhenBishopMovesAwayFromEnemy()
            {
                var moveToB4 = MoveHelpers.GenerateMove(18, 25);
                var moveToA5 = MoveHelpers.GenerateMove(18, 32);
                var moveToD2 = MoveHelpers.GenerateMove(18, 11);
                var moveToB2 = MoveHelpers.GenerateMove(18, 9);
                var moveToA1 = MoveHelpers.GenerateMove(18, 0);
                var actual = Validate(_bAttacksEnemyOnE5.BoardInfo, _postMoveBoard, moveToB4);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Bishop should be able to move short of enemy piece.  {moveToB4.SourceIndex.IndexToSquareDisplay()} to {moveToB4.DestinationIndex.IndexToSquareDisplay()}");
                actual = Validate(_bAttacksEnemyOnE5.BoardInfo, _postMoveBoard, moveToA5);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Bishop should be able to move short of enemy piece.  {moveToA5.SourceIndex.IndexToSquareDisplay()} to {moveToA5.DestinationIndex.IndexToSquareDisplay()}");
                actual = Validate(_bAttacksEnemyOnE5.BoardInfo, _postMoveBoard, moveToD2);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Bishop should be able to move short of enemy piece.  {moveToD2.SourceIndex.IndexToSquareDisplay()} to {moveToD2.DestinationIndex.IndexToSquareDisplay()}");
                actual = Validate(_bAttacksEnemyOnE5.BoardInfo, _postMoveBoard, moveToB2);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Bishop should be able to move short of enemy piece.  {moveToB2.SourceIndex.IndexToSquareDisplay()} to {moveToB2.DestinationIndex.IndexToSquareDisplay()}");
                actual = Validate(_bAttacksEnemyOnE5.BoardInfo, _postMoveBoard, moveToA1);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Bishop should be able to move short of enemy piece.  {moveToA1.SourceIndex.IndexToSquareDisplay()} to {moveToA1.DestinationIndex.IndexToSquareDisplay()}");

                actual = Validate(_qAttacksEnemyOnE5.BoardInfo, _postMoveBoard, moveToB4);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Queen should be able to move short of enemy piece.  {moveToB4.SourceIndex.IndexToSquareDisplay()} to {moveToB4.DestinationIndex.IndexToSquareDisplay()}");
                actual = Validate(_qAttacksEnemyOnE5.BoardInfo, _postMoveBoard, moveToA5);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Queen should be able to move short of enemy piece.  {moveToA5.SourceIndex.IndexToSquareDisplay()} to {moveToA5.DestinationIndex.IndexToSquareDisplay()}");
                actual = Validate(_qAttacksEnemyOnE5.BoardInfo, _postMoveBoard, moveToD2);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Queen should be able to move short of enemy piece.  {moveToD2.SourceIndex.IndexToSquareDisplay()} to {moveToD2.DestinationIndex.IndexToSquareDisplay()}");
                actual = Validate(_qAttacksEnemyOnE5.BoardInfo, _postMoveBoard, moveToB2);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Queen should be able to move short of enemy piece.  {moveToB2.SourceIndex.IndexToSquareDisplay()} to {moveToB2.DestinationIndex.IndexToSquareDisplay()}");
                actual = Validate(_qAttacksEnemyOnE5.BoardInfo, _postMoveBoard, moveToA1);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Queen should be able to move short of enemy piece.  {moveToA1.SourceIndex.IndexToSquareDisplay()} to {moveToA1.DestinationIndex.IndexToSquareDisplay()}");
            }

            [Test]
            public void ShouldReturnError_WhenBishopMovesPastEnemy()
            {
                var move = MoveHelpers.GenerateMove(18, 54);
                var actual = Validate(_bAttacksEnemyOnE5.BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, $"Bishop should not be able to move past enemy piece.  {move.SourceIndex.IndexToSquareDisplay()} to {move.DestinationIndex.IndexToSquareDisplay()}");
                actual = Validate(_qAttacksEnemyOnE5.BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, $"Queen should not be able to move past enemy piece.  {move.SourceIndex.IndexToSquareDisplay()} to {move.DestinationIndex.IndexToSquareDisplay()}");

            }

            [Test]
            public void ShouldReturnError_WhenBishopMovesPastFriendlyBlocker()
            {
                var move = _bAttacksFriendlyOnE5.Move;
                var board = _bAttacksFriendlyOnE5.BoardInfo;
                var actual = Validate(board, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, $"Bishop should not be able to move past enemy piece.  {move.SourceIndex.IndexToSquareDisplay()} to {move.DestinationIndex.IndexToSquareDisplay()}");

                move = _qAttacksFriendlyOnE5.Move;
                board = _qAttacksFriendlyOnE5.BoardInfo;
                actual = Validate(board, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, $"Queen should not be able to move past enemy piece.  {move.SourceIndex.IndexToSquareDisplay()} to {move.DestinationIndex.IndexToSquareDisplay()}");
            }
        }

        class RookQueenMoves : Data.Validators.MoveValidation.MoveRules.PieceCanMoveToDestination
        {
            private readonly SlidingPieceMoveAndBoard _rEnemyOnC7 = new SlidingPieceMoveAndBoard(2, 50, "2r1k3/2r5/8/8/8/8/8/n1R1K3 w - - 0 1");
            private readonly SlidingPieceMoveAndBoard _rEnemyOnC8 = new SlidingPieceMoveAndBoard(2, 58, "2r1k3/2r5/8/8/8/8/8/n1R1K3 w - - 0 1");
            private readonly SlidingPieceMoveAndBoard _rEnemyOnA1 = new SlidingPieceMoveAndBoard(2, 00, "2r1k3/2r5/8/8/8/8/8/n1R1K3 w - - 0 1");
            private readonly SlidingPieceMoveAndBoard _qEnemyOnC7 = new SlidingPieceMoveAndBoard(2, 50, "2r1k3/2r5/8/8/8/8/8/n1Q1K3 w - - 0 1");
            private readonly SlidingPieceMoveAndBoard _qEnemyOnC8 = new SlidingPieceMoveAndBoard(2, 58, "2r1k3/2r5/8/8/8/8/8/n1Q1K3 w - - 0 1");
            private readonly SlidingPieceMoveAndBoard _qEnemyOnA1 = new SlidingPieceMoveAndBoard(2, 00, "2r1k3/2r5/8/8/8/8/8/n1Q1K3 w - - 0 1");

            [Test]
            public void ShouldReturnNull_WhenMoveCapturesEnemyOnC7()
            {
                var actual = Validate(_rEnemyOnC7.BoardInfo, _postMoveBoard, _rEnemyOnC7.Move);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Should be able to capture. {_rEnemyOnC7.ToString()}");

                actual = Validate(_qEnemyOnC7.BoardInfo, _postMoveBoard, _qEnemyOnC7.Move);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Should be able to capture. {_qEnemyOnC7.ToString()}");
            }

            [Test]
            public void ShouldReturnNull_WhenMoveCapturesEnemyOnA1()
            {
                var actual = Validate(_rEnemyOnA1.BoardInfo, _postMoveBoard, _rEnemyOnA1.Move);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Should be able to capture. {_rEnemyOnA1.ToString()}");

                actual = Validate(_qEnemyOnA1.BoardInfo, _postMoveBoard, _qEnemyOnA1.Move);
                Assert.AreEqual(MoveError.NoneSet, actual, $"Should be able to capture. {_qEnemyOnA1.ToString()}");
            }

            

            [Test]
            public void ShouldReturnNull_WhenMovingToNonBlockedSquares()
            {
                MoveExt move;
                for (ushort dest = 10; dest < 50; dest += 8)
                {
                    move = MoveHelpers.GenerateMove(2, dest);
                    Assert.AreEqual(MoveError.NoneSet, Validate(_rEnemyOnC7.BoardInfo, _postMoveBoard, move), $"Rook from {move} should be legal.");
                    Assert.AreEqual(MoveError.NoneSet, Validate(_qEnemyOnC7.BoardInfo, _postMoveBoard, move), $"Queen from {move} should be legal.");
                }

                move = MoveHelpers.GenerateMove(2, 1);
                Assert.AreEqual(MoveError.NoneSet, Validate(_rEnemyOnC7.BoardInfo, _postMoveBoard, move), $"Rook from {move} should be legal.");
                Assert.AreEqual(MoveError.NoneSet, Validate(_qEnemyOnC7.BoardInfo, _postMoveBoard, move), $"Queen from {move} should be legal.");
            }

            [Test]
            public void ShouldReturnError_WhenCapturingBehindBlockedPiece()
            {
                var actual = Validate(_rEnemyOnC8.BoardInfo, _postMoveBoard, _rEnemyOnC8.Move);
                Assert.AreEqual(MoveError.BadDestination, actual, $"Should not be able to capture. {_rEnemyOnC7.ToString()}");

                actual = Validate(_qEnemyOnC8.BoardInfo, _postMoveBoard, _qEnemyOnC8.Move);
                Assert.AreEqual(MoveError.BadDestination, actual, $"Should not be able to capture. {_qEnemyOnC7.ToString()}");
            }
            [Test]
            public void ShouldReturnError_WhenMovingToWrongSquare()
            {
                var move = MoveHelpers.GenerateMove(2, 11);
                var actual = Validate(_rEnemyOnC8.BoardInfo, _postMoveBoard, move);
                Assert.AreEqual(MoveError.BadDestination, actual, $"Should not be able to move from {_rEnemyOnC7.ToString()}");
            }
        }

        class KingMoves : Data.Validators.MoveValidation.MoveRules.PieceCanMoveToDestination
        {
            readonly BoardInfo _bi = new BoardInfo("4k3/8/8/8/8/8/1K6/8 w - - 0 1");

            private readonly MoveExt[] moves = new MoveExt[]
            {
                MoveHelpers.GenerateMove(9, 0), MoveHelpers.GenerateMove(9, 1), MoveHelpers.GenerateMove(9, 2),
                MoveHelpers.GenerateMove(9, 10), MoveHelpers.GenerateMove(9, 16), MoveHelpers.GenerateMove(9, 17),
                MoveHelpers.GenerateMove(9, 18), MoveHelpers.GenerateMove(9, 8)
            };
            private readonly MoveExt[] badMoves = new MoveExt[]
            {

                MoveHelpers.GenerateMove(9, 24), MoveHelpers.GenerateMove(9, 25), MoveHelpers.GenerateMove(9, 26),
                MoveHelpers.GenerateMove(9, 19), MoveHelpers.GenerateMove(9, 3)
            };

            [Test]
            public void ShouldReturnNull_WhenMoveIsLegal()
            {
                foreach (var move in moves)
                {
                    Assert.AreEqual(MoveError.NoneSet, Validate(_bi, _postMoveBoard, move), $"Should be able to move king to {move.DestinationIndex.IndexToSquareDisplay()}");
                }
            }

            [Test]
            public void ShouldReturnError_WhenMoveIsNotLegal()
            {
                foreach (var badMove in badMoves)
                {
                    Assert.AreEqual(MoveError.BadDestination, Validate(_bi, _postMoveBoard, badMove), $"Should not be able to move king to {badMove.DestinationIndex.IndexToSquareDisplay()}");
                }
            }
        }

        internal struct SlidingPieceMoveAndBoard
        {
            public SlidingPieceMoveAndBoard(ushort source, ushort dest, string fen)
            {
                Move = MoveHelpers.GenerateMove(source, dest);
                BoardInfo = new BoardInfo(fen);
            }
            public readonly MoveExt Move;
            public readonly BoardInfo BoardInfo;
            public override string ToString()
            {
                return $"{Move.SourceIndex.IndexToSquareDisplay()} to {Move.DestinationIndex.IndexToSquareDisplay()}";
            }
        }
    }
}