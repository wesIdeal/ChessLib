using System.Linq;
using ChessLib.Core;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Data.Helpers;
using NUnit.Framework;

namespace ChessLib.Data.Tests.Helpers
{

    public partial class ShiftHelpers
    {
        
        //public static void TestPiecesAttackingSquare(string fen, int attackedSquare, params int[] attackingPieces)
        //{
        //    var board = new Board(fen);
        //    var actual = Bitboard.Instance.PiecesAttackingSquare(board.Occupancy, (ushort)attackedSquare);
        //    if (attackingPieces.Any())
        //    {
        //        foreach (var attackingPiece in attackingPieces)
        //        {
        //            Assert.IsTrue(actual.IsBitSet((ushort)attackingPiece), $"{attackingPiece.IndexToSquareDisplay()} should attack {attackedSquare.IndexToSquareDisplay()}");
        //        }
        //    }
        //    else
        //    {
        //        //no pieces attack square
        //        Assert.AreEqual(0, actual);
        //    }
        //}
        [TestCase("r3k2r/8/8/8/8/8/8/R3K2R w - - 0 1", CastlingAvailability.WhiteKingside, 64ul, 33ul)]
        [TestCase("r3k2r/8/8/8/8/8/8/R3K2R w - - 0 1", CastlingAvailability.WhiteQueenside, 4ul, 136ul)]
        [TestCase("r3k2r/8/8/8/8/8/8/R3K2R b - - 0 1", CastlingAvailability.BlackKingside, 0x4000000000000000ul, 0x2100000000000000ul)]
        [TestCase("r3k2r/8/8/8/8/8/8/R3K2R b - - 0 1", CastlingAvailability.BlackQueenside, 0x400000000000000ul, 0x8800000000000000ul)]
        public void ApplyMove_Castles(string fen, CastlingAvailability castlingMove, ulong expectedKingVal, ulong expectedRookVal)
        {
            var move = castlingMove == CastlingAvailability.WhiteKingside ? MoveHelpers.WhiteCastleKingSide
                : castlingMove == CastlingAvailability.WhiteQueenside ? MoveHelpers.WhiteCastleQueenSide
                : castlingMove == CastlingAvailability.BlackKingside ? MoveHelpers.BlackCastleKingSide
                : MoveHelpers.BlackCastleQueenSide;
            var board = new Board(fen);
            var postMove = BoardHelpers.ApplyMoveToBoard(board, move);
            var activeKingVal = postMove.Occupancy.Occupancy(board.ActivePlayer, Piece.King);
            var activeRookVal = postMove.Occupancy.Occupancy(board.ActivePlayer, Piece.Rook);
            Assert.AreEqual(expectedKingVal, activeKingVal);
            Assert.AreEqual(expectedRookVal, activeRookVal);
        }

        [TestCase("8/8/8/3pP2k/K7/8/8/8 w - d6 0 2", 36, 43, 0ul, 8796093022208ul)]
        public void ApplyMove_EnPassantCaptures(string fen, int src, int dst, ulong oppPawn, ulong actPawn)
        {
            var move = MoveHelpers.GenerateMove((ushort)src, (ushort)dst, MoveType.EnPassant);
            var board = new Board(fen);
            var activeColor = board.ActivePlayer;
            var oppColor = board.ActivePlayer.Toggle();
            var actual = BoardHelpers.ApplyMoveToBoard(board, move);
            Assert.AreEqual(actual.Occupancy.Occupancy(activeColor, Piece.Pawn), actPawn, $"{board.ActivePlayer}'s pawn structure incorrect after En Passant capture");
            Assert.AreEqual(actual.Occupancy.Occupancy(oppColor, Piece.Pawn), oppPawn, $"{board.ActivePlayer.Toggle()}'s pawn structure incorrect after En Passant capture");
        }

        //[TestCase("4k3/8/8/8/8/8/8/4K3 w - - 0 1", GameState.Drawn, false)]
        //[TestCase("4k3/8/8/8/8/4r3/4q3/4K3 w - - 0 1", GameState.Checkmate, true)]
        //[TestCase("4k3/8/8/7r/8/5q2/7r/6K1 w - - 0 1", GameState.StaleMate, true)]
        //[TestCase(FENHelpers.FENInitial, GameState.None, false)]
        //public void GameState_SetCorrectly(string fen, GameState expectedGameState, bool ended)
        //{
        //    var board = new Board(fen);
        //    board.ValidateBoard();
        //    Assert.AreEqual(expectedGameState,board.GameState);
        //    Assert.AreEqual(board.IsGameOver, ended);
        //}

        #region Cardinal Direction Shifts

        #region Cardinal Direction Test All Methods
        [Test]
        public static void ShiftE_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (idx.GetFile() != 7)
                {
                    expectedValue = BoardConstants.IndividualSquares[idx + 1];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftE(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftE()");
            }
        }

        private static bool IsOnRank(ushort square, ushort rank) => square.GetRank() == rank;

        [Test]
        public static void ShiftS_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnRank(idx,0))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx - 8];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftS(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftS()");

            }
        }

        [Test]
        public static void ShiftW_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnFile(idx,0))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx - 1];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftW(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftW()");

            }
        }

        [Test]
        public static void ShiftN_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnRank(idx,7))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx + 8];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftN(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftN()");
            }
        }


        #endregion

        #region Cardinal Direction Sanity Checks
        #region ShiftE Tests


        [Test]
        public static void Shift2E_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (idx.GetFile() != 6 && idx.GetFile() != 7)
                {
                    expectedValue = BoardConstants.IndividualSquares[idx + 2];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].Shift2E(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.Shift2E()");
            }

        }

        #endregion

        #region ShiftS Tests

        [Test]
        public static void ShiftS2_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnRank(idx,1) && !IsOnRank(idx,0))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx - 16];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].Shift2S(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.Shift2S()");
            }
        }

        #endregion

        #region ShiftW Tests


        [Test]
        public static void Shift2W_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (idx.GetFile() != 1 && idx.GetFile() != 0)
                {
                    expectedValue = BoardConstants.IndividualSquares[idx - 2];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].Shift2W(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.Shift2W()");
            }
        }

        #endregion

        #region ShiftN Tests


        [Test]
        public static void ShiftN2_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnRank(idx,6) && !IsOnRank(idx,7))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx + 16];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].Shift2N(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.Shift2N()");
            }
        }


        #endregion
        #endregion

        #endregion

        #region Divided Cardinal Direction Shifts
        #region Divided Cardinal Test All Methods
        [Test]
        public static void ShiftNNE_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if ((!IsOnRank(idx,6) && !IsOnRank(idx,7)) && idx.GetFile() != 7)
                {
                    expectedValue = BoardConstants.IndividualSquares[idx + 17];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftNNE(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftNNE()");

            }
        }

        [Test]
        public static void ShiftNE_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnFile(idx, 7) && !IsOnRank(idx,7))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx + 9];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftNE(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.Shift2E()");

            }
        }

        private static bool IsOnFile(ushort index, ushort file) => index.GetFile() == file;
        [Test]
        public static void ShiftENE_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnRank(idx,7) && !IsOnFile(idx, 7) && !IsOnFile(idx, 6))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx + 10];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftENE(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftENE()");
            }
        }


        [Test]
        public static void ShiftESE_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnRank(idx,0) && !IsOnFile(idx,7) && !IsOnFile(idx,6))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx - 6];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftESE(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftESE()");

            }
        }

        [Test]
        public static void ShiftSE_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnFile(idx,7) && !IsOnRank(idx,0))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx - 7];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftSE(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftSE()");
            }
        }

        [Test]
        public static void ShiftSSE_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnRank(idx,0) && !IsOnRank(idx,1) && !IsOnFile(idx,7))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx - 15];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftSSE(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftSSE()");
            }
        }

        [Test]
        public static void ShiftSSW_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnRank(idx,0) && !IsOnRank(idx,1) && !IsOnFile(idx,0))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx - 17];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftSSW(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftSSW()");

            }
        }

        [Test]
        public static void ShiftSW_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnFile(idx,0) && !IsOnRank(idx,0))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx - 9];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftSW(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.Shift2E()");

            }
        }

        [Test]
        public static void ShiftWSW_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnRank(idx,0) && !IsOnFile(idx,0) && !IsOnFile(idx,1))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx - 10];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftWSW(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftWSW()");

            }
        }

        [Test]
        public static void ShiftWNW_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnRank(idx,7) && !IsOnFile(idx,0) && !IsOnFile(idx,1))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx + 6];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftWNW(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftWNW()");
            }
        }

        [Test]
        public static void ShiftNW_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnFile(idx,0) && !IsOnRank(idx,7))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx + 7];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftNW(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.Shift2E()");
            }
        }

        [Test]
        public static void ShiftNNW_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!IsOnRank(idx,7) && !IsOnRank(idx,6) && !IsOnFile(idx,0))
                {
                    expectedValue = BoardConstants.IndividualSquares[idx + 15];
                }
                Assert.AreEqual(expectedValue, BoardConstants.IndividualSquares[idx].ShiftNNW(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftNNW()");
            }
        }
        #endregion

        #endregion
    }

}
