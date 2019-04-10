using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using MagicBitboard;
using NUnit.Framework;

namespace ChessLib.MagicBitboard.Tests.MoveValidation.MoveRules
{
    [TestFixture]
    internal class PieceCanMoveToDestination : MagicBitboard.MoveValidation.MoveRules.PieceCanMoveToDestination
    {
        ulong[][] postMoveBoard = new ulong[2][];
        [Test]
        public void ShouldReturnNull_OnValidMove_EmptySquare_Edge()
        {
            var move = MoveHelpers.GenerateMove(0, 56);
            var boardInfo = BoardInfo.BoardInfoFromFen("4k3/8/8/8/8/8/8/R3K3 w - - 0 1");
            Assert.IsNull(Validate(boardInfo, postMoveBoard, move), "Should return null when moving to empty square on a8.");
        }

        [Test]
        public void ShouldReturnNull_OnValidMove_EmptySquare_Middle()
        {
            var move = MoveHelpers.GenerateMove(2, 26);
            var boardInfo = BoardInfo.BoardInfoFromFen("4k3/8/8/8/8/8/8/2R1K3 w - - 0 1");
            Assert.IsNull(Validate(boardInfo, postMoveBoard, move), "Should return null when moving to empty square on c4.");
        }

        [Test]
        public void ShouldReturnNull_OnValidMove_OpponentPieceAttacked_Edge()
        {
            var move = MoveHelpers.GenerateMove(2, 58);
            var boardInfo = BoardInfo.BoardInfoFromFen("2q1k3/8/8/8/8/8/8/2R1K3 w - - 0 1");
            Assert.IsNull(Validate(boardInfo, postMoveBoard, move), "Should return null when capturing on c8.");

        }

        [Test]
        public void ShouldReturnNull_OnValidMove_OpponentPieceAttacked_Middle()
        {
            var move = MoveHelpers.GenerateMove(2, 26);
            var boardInfo = BoardInfo.BoardInfoFromFen("4k3/8/8/8/2q5/8/8/2R1K3 w - - 0 1");
            Assert.IsNull(Validate(boardInfo, postMoveBoard, move), "Should return null when capturing on c4.");
        }

        [Test]
        public void ShouldReturnProperError_WhenActivePlayerHasNoPieceAtSource()
        {
            var move = MoveHelpers.GenerateMove(0, 56);
            var boardInfo = BoardInfo.BoardInfoFromFen("4k3/8/8/8/2q5/8/8/2R1K3 w - - 0 1");
            Assert.AreEqual(MoveExceptionType.ActivePlayerHasNoPieceOnSourceSquare, Validate(boardInfo, postMoveBoard, move), "Should return error when active color has no piece at source.");
        }

        [Test]
        public void ShouldReturnProperError_WhenSlidingPieceBlocked()
        {
            var move = MoveHelpers.GenerateMove(2, 26);
            var boardInfo = BoardInfo.BoardInfoFromFen("4k3/8/8/8/2q5/2p5/8/2R1K3 w - - 0 1");
            Assert.AreEqual(MoveExceptionType.BadDestination, Validate(boardInfo, postMoveBoard, move), "Should return error when capturing on c4 but is blocked.");
        }

        [Test]
        public void ShouldReturnProperError_WhenDestinationIsInvalidForPiece()
        {
            var move = MoveHelpers.GenerateMove(2, 11);
            var boardInfo = BoardInfo.BoardInfoFromFen("4k3/8/8/8/2q5/8/8/2R1K3 w - - 0 1");
            Assert.AreEqual(MoveExceptionType.BadDestination, Validate(boardInfo, postMoveBoard, move), "Should return error when the destination is invalid.");
        }

        class PawnMoves : MagicBitboard.MoveValidation.MoveRules.PieceCanMoveToDestination
        {
            private const string InitialFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            private BoardInfo boardInfo;
            private ulong[][] pmb = new ulong[2][];
            [SetUp]
            public void Setup()
            {
                boardInfo = BoardInfo.BoardInfoFromFen(InitialFEN);
            }
            #region Validation Errors
            [Test]
            public void ShouldReturnProperError_WhenWhitePawnMovesBackwards()
            {
                var bi = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(28, 12);

                var actual = Validate(bi, pmb, move);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Pawns can't move backwards.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackPawnMovesBackwards()
            {
                var bi = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppp1ppp/8/4p3/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(36, 52);

                var actual = Validate(bi, pmb, move);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Pawns can't move backwards.");
            }

            [Test]
            public void ShouldReturnProperError_WhenWhitePawnAttacksEmptySquare()
            {
                var move = MoveHelpers.GenerateMove(12, 21);
                var actual = Validate(boardInfo, pmb, move);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Pawns can't capture an empty square to the NE.");
                move = MoveHelpers.GenerateMove(12, 19);
                actual = Validate(boardInfo, pmb, move);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Pawns can't capture an empty square to the NW.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackPawnAttacksEmptySquare()
            {
                boardInfo.ActivePlayerColor = Data.Types.Color.Black;
                var move = MoveHelpers.GenerateMove(52, 43);
                var actual = Validate(boardInfo, pmb, move);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Pawns can't capture an empty square to the SE.");
                move = MoveHelpers.GenerateMove(52, 45);
                actual = Validate(boardInfo, pmb, move);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Pawns can't capture an empty square to the SW.");
            }

            [Test]
            public void ShouldReturnProperError_WhenWhitePawnAttacksSquareOccupiedByWhite()
            {
                var bi = BoardInfo.BoardInfoFromFen("rnbqkbnr/pp1ppp1p/3p1p2/8/8/3P1P2/PP1PPP1P/RNBQKBNR w KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(12, 21);
                var actual = Validate(bi, pmb, move);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Pawns can't capture a friendly-occupied square to the NE.");
                move = MoveHelpers.GenerateMove(12, 19);
                actual = Validate(bi, pmb, move);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Pawns can't capture a friendly-occupied square to the NW.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackPawnAttacksSquareOccupiedByWhite()
            {
                var bi = BoardInfo.BoardInfoFromFen("rnbqkbnr/pp1ppp1p/3p1p2/8/8/3P1P2/PP1PPP1P/RNBQKBNR b KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(52, 43);
                var actual = Validate(bi, pmb, move);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Pawns can't capture a friendly-occupied square to the SE.");
                move = MoveHelpers.GenerateMove(52, 45);                                                                    
                actual = Validate(bi, pmb, move);                                                                    
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Pawns can't capture a friendly-occupied square to the SW.");
            }

            [Test]
            public void ShouldReturnProperError_WhenWhitePawnIsBlockedFromInitial()
            {
                var bi1 = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppp1ppp/8/8/8/4p3/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
                var bi2 = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppp1ppp/8/8/4p3/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
                var move1 = MoveHelpers.GenerateMove(12, 20);
                var move2 = MoveHelpers.GenerateMove(12, 28);
                var actual = Validate(bi1, pmb, move1);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Cannot move pawn when piece blocks its mobility.");
                actual = Validate(bi2, pmb, move2);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Cannot move pawn 2 squares in opening when piece blocks its mobility.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackPawnIsBlockedFromInitial()
            {
                var bi1 = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppppppp/4P3/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1");
                var bi2 = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppppppp/8/4P3/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1");
                var move1 = MoveHelpers.GenerateMove(52, 44);
                var move2 = MoveHelpers.GenerateMove(52, 36);
                var actual = Validate(bi1, pmb, move1);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Cannot move pawn when piece blocks its mobility.");
                actual = Validate(bi2, pmb, move2);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Cannot move pawn 2 squares in opening when piece blocks its mobility.");
            }

            [Test]
            public void ShouldReturnProperError_WhenWhitePawnMoves2From3rdRank()
            {
                var bi = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppp1ppp/4p3/8/8/4P3/PPPP1PPP/RNBQKBNR w KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(20, 36);
                var actual = Validate(bi, pmb, move);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Pawn cannot move 2 squares if not on the opening rank.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackPawnMoves2From6thRank()
            {
                var bi = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppp1ppp/4p3/8/8/4P3/PPPP1PPP/RNBQKBNR b KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(44, 28);
                var actual = Validate(bi, pmb, move);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Pawn cannot move 2 squares if not on the opening rank.");
            }

            [Test]
            public void ShouldReturnProperError_WhenWhitePawnIsBlockedInMiddle()
            {
                var bi1 = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1");
                var move1 = MoveHelpers.GenerateMove(28, 36);

                var actual = Validate(bi1, pmb, move1);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Cannot move pawn when piece blocks its mobility.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackPawnIsBlockedInMiddle()
            {
                var bi1 = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1");

                var move1 = MoveHelpers.GenerateMove(36, 28);

                var actual = Validate(bi1, pmb, move1);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Cannot move pawn when piece blocks its mobility.");
            }

            [Test]
            public void ShouldReturnProperError_WhenWhiteTryingToAttackAcrossBoard()
            {
                var bi1 = BoardInfo.BoardInfoFromFen("rnbqkbnr/ppppppp1/8/8/8/8/PPPPPPPp/RNBQKBNR w KQkq - 0 1");

                var move1 = MoveHelpers.GenerateMove(8, 15);

                var actual = Validate(bi1, pmb, move1);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Cannot capture across board.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackTryingToAttackAcrossBoard()
            {
                var bi1 = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppppppp/8/7P/8/8/PPPPPPP1/RNBQKBNR b KQkq - 0 1");

                var move1 = MoveHelpers.GenerateMove(48, 39);

                var actual = Validate(bi1, pmb, move1);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Cannot capture across board.");
            }

            [Test]
            public void ShouldReturnProperError_WhenWhitePawnMoves3()
            {
                var move = MoveHelpers.GenerateMove(12, 36);
                var actual = Validate(boardInfo, pmb, move);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Pawns can't move 3 squares.");
            }

            [Test]
            public void ShouldReturnProperError_WhenBlackPawnMoves3()
            {
                boardInfo.ActivePlayerColor = Data.Types.Color.Black;
                var move = MoveHelpers.GenerateMove(52, 28);
                var actual = Validate(boardInfo, pmb, move);
                Assert.AreEqual(MoveExceptionType.BadDestination, actual, "Expected validation error. Pawns can't move 3 squares.");
            }
            #endregion

            #region Validation Non-errors
            [Test]
            public void ShouldReturnNoError_WhenWhitePawnMovesForward1()
            {
                var move = MoveHelpers.GenerateMove(12, 20);
                var actual = Validate(boardInfo, pmb, move);
                Assert.IsNull(actual, "Expected no error. Pawns can move 1 forward.");
            }

            [Test]
            public void ShouldReturnNoError_WhenBlackPawnMovesForward1()
            {
                boardInfo.ActivePlayerColor = Data.Types.Color.Black;
                var move = MoveHelpers.GenerateMove(52, 44);
                var actual = Validate(boardInfo, pmb, move);
                Assert.IsNull(actual, "Expected no error. Pawns can move 1 forward.");
            }
            [Test]
            public void ShouldReturnNoError_WhenWhitePawnMoves2From2ndRank()
            {
                var bi = BoardInfo.BoardInfoFromFen("rnbqkbnr/pppp1ppp/4p3/8/8/4P3/PPPP1PPP/RNBQKBNR w KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(12, 28);
                var actual = Validate(boardInfo, pmb, move);
                Assert.IsNull(actual, "Pawn can move 2 squares from the opening rank.");
            }

            [Test]
            public void ShouldReturnNoError_WhenBlackPawnMoves2From7thRank()
            {
                boardInfo.ActivePlayerColor = Data.Types.Color.Black;
                var move = MoveHelpers.GenerateMove(52, 36);
                var actual = Validate(boardInfo, pmb, move);
                Assert.IsNull(actual, "Pawn can move 2 squares from the opening rank.");
            }
            [Test]
            public void ShouldReturnNoError_WhenWhitePawnAttacksEnemyOccupiedSquare()
            {
                var bi1 = BoardInfo.BoardInfoFromFen("rnbqkbnr/ppp1p1pp/3P1P2/8/8/3p1p2/PP2PPPP/RNBQKBNR w KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(12, 21);
                var actual = Validate(bi1, pmb, move);
                Assert.IsNull(actual, "Expected no error. Pawns can capture an enemy-occupied square to the NE.");
                move = MoveHelpers.GenerateMove(12, 19);
                actual = Validate(bi1, pmb, move);
                Assert.IsNull(actual, "Expected no error. Pawns can capture an enemy-occupied square to the NW.");
            }

            [Test]
            public void ShouldReturnNoError_WhenBlackPawnAttacksEnemyOccupiedSquare()
            {
                var bi1 = BoardInfo.BoardInfoFromFen("rnbqkbnr/ppp1p1pp/3P1P2/8/8/3p1p2/PP2PPPP/RNBQKBNR b KQkq - 0 1");
                var move = MoveHelpers.GenerateMove(52, 43);
                var actual = Validate(bi1, pmb, move);
                Assert.IsNull(actual, "Expected no error. Pawns can capture an enemy-occupied square to the SE.");
                move = MoveHelpers.GenerateMove(52, 45);
                actual = Validate(bi1, pmb, move);
                Assert.IsNull(actual, "Expected no error. Pawns can capture an enemy-occupied square to the SW.");
            }

            #endregion

        }
    }
}