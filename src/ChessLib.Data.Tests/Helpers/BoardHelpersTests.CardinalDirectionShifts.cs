using System.Linq;
using ChessLib.Data.Helpers;
using NUnit.Framework;

namespace ChessLib.Data.Tests.Helpers
{

    public partial class BoardHelpersTests
    {
        [TestCase("4k3/8/8/2pP4/8/8/8/4K3 w - c6 0 1", 42, 35)]
        [TestCase("4k3/8/8/2pP4/8/8/8/4K3 w - c6 0 1", 43, new int[] { })]
        public static void TestPiecesAttackingSquare(string fen, int attackedSquare, params int[] attackingPieces)
        {
            var board = new BoardInfo(fen);
            var actual = board.PiecesAttackingSquare((ushort)attackedSquare);
            if (attackingPieces.Any())
            {
                foreach (var attackingPiece in attackingPieces)
                {
                    Assert.IsTrue(actual.IsBitSet((ushort)attackingPiece), $"{attackingPiece.IndexToSquareDisplay()} should attack {attackedSquare.IndexToSquareDisplay()}");
                }
            }
            else
            {
                //no pieces attack square
                Assert.AreEqual(0, actual);
            }
        }

        [TestFixture]
        public static class CardinalDirectionShifts
        {
            #region Cardinal Direction Shifts

            #region Cardinal Direction Test All Methods
            [Test]
            public static void ShiftE_TestAll()
            {
                for (ushort idx = 0; idx < 64; idx++)
                {
                    ulong expectedValue = 0;
                    if (!idx.IsIndexOnFile(7))
                    {
                        expectedValue = BoardHelpers.IndividualSquares[idx + 1];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftE(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftE()");
                }
            }

            [Test]
            public static void ShiftS_TestAll()
            {
                for (ushort idx = 0; idx < 64; idx++)
                {
                    ulong expectedValue = 0;
                    if (!idx.IsIndexOnRank(0))
                    {
                        expectedValue = BoardHelpers.IndividualSquares[idx - 8];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftS(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftS()");

                }
            }

            [Test]
            public static void ShiftW_TestAll()
            {
                for (ushort idx = 0; idx < 64; idx++)
                {
                    ulong expectedValue = 0;
                    if (!idx.IsIndexOnFile(0))
                    {
                        expectedValue = BoardHelpers.IndividualSquares[idx - 1];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftW(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftW()");

                }
            }

            [Test]
            public static void ShiftN_TestAll()
            {
                for (ushort idx = 0; idx < 64; idx++)
                {
                    ulong expectedValue = 0;
                    if (!idx.IsIndexOnRank(7))
                    {
                        expectedValue = BoardHelpers.IndividualSquares[idx + 8];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftN(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftN()");
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
                    if (!idx.IsIndexOnFile(6) && !idx.IsIndexOnFile(7))
                    {
                        expectedValue = BoardHelpers.IndividualSquares[idx + 2];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].Shift2E(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.Shift2E()");
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
                    if (!idx.IsIndexOnRank(1) && !idx.IsIndexOnRank(0))
                    {
                        expectedValue = BoardHelpers.IndividualSquares[idx - 16];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].Shift2S(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.Shift2S()");
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
                    if (!idx.IsIndexOnFile(1) && !idx.IsIndexOnFile(0))
                    {
                        expectedValue = BoardHelpers.IndividualSquares[idx - 2];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].Shift2W(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.Shift2W()");
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
                    if (!idx.IsIndexOnRank(6) && !idx.IsIndexOnRank(7))
                    {
                        expectedValue = BoardHelpers.IndividualSquares[idx + 16];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].Shift2N(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.Shift2N()");
                }
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
                if ((!idx.IsIndexOnRank(6) && !idx.IsIndexOnRank(7)) && !idx.IsIndexOnFile(7))
                {
                    expectedValue = BoardHelpers.IndividualSquares[idx + 17];
                }
                Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftNNE(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftNNE()");

            }
        }

        [Test]
        public static void ShiftNE_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!idx.IsIndexOnFile(7) && !idx.IsIndexOnRank(7))
                {
                    expectedValue = BoardHelpers.IndividualSquares[idx + 9];
                }
                Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftNE(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.Shift2E()");

            }
        }

        [Test]
        public static void ShiftENE_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!idx.IsIndexOnRank(7) && !idx.IsIndexOnFile(7) && !idx.IsIndexOnFile(6))
                {
                    expectedValue = BoardHelpers.IndividualSquares[idx + 10];
                }
                Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftENE(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftENE()");
            }
        }


        [Test]
        public static void ShiftESE_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!idx.IsIndexOnRank(0) && !idx.IsIndexOnFile(7) && !idx.IsIndexOnFile(6))
                {
                    expectedValue = BoardHelpers.IndividualSquares[idx - 6];
                }
                Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftESE(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftESE()");

            }
        }

        [Test]
        public static void ShiftSE_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!idx.IsIndexOnFile(7) && !idx.IsIndexOnRank(0))
                {
                    expectedValue = BoardHelpers.IndividualSquares[idx - 7];
                }
                Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftSE(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftSE()");
            }
        }

        [Test]
        public static void ShiftSSE_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!idx.IsIndexOnRank(0) && !idx.IsIndexOnRank(1) && !idx.IsIndexOnFile(7))
                {
                    expectedValue = BoardHelpers.IndividualSquares[idx - 15];
                }
                Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftSSE(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftSSE()");
            }
        }

        [Test]
        public static void ShiftSSW_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!idx.IsIndexOnRank(0) && !idx.IsIndexOnRank(1) && !idx.IsIndexOnFile(0))
                {
                    expectedValue = BoardHelpers.IndividualSquares[idx - 17];
                }
                Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftSSW(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftSSW()");

            }
        }

        [Test]
        public static void ShiftSW_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!idx.IsIndexOnFile(0) && !idx.IsIndexOnRank(0))
                {
                    expectedValue = BoardHelpers.IndividualSquares[idx - 9];
                }
                Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftSW(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.Shift2E()");

            }
        }

        [Test]
        public static void ShiftWSW_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!idx.IsIndexOnRank(0) && !idx.IsIndexOnFile(0) && !idx.IsIndexOnFile(1))
                {
                    expectedValue = BoardHelpers.IndividualSquares[idx - 10];
                }
                Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftWSW(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftWSW()");

            }
        }

        [Test]
        public static void ShiftWNW_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!idx.IsIndexOnRank(7) && !idx.IsIndexOnFile(0) && !idx.IsIndexOnFile(1))
                {
                    expectedValue = BoardHelpers.IndividualSquares[idx + 6];
                }
                Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftWNW(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftWNW()");
            }
        }

        [Test]
        public static void ShiftNW_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!idx.IsIndexOnFile(0) && !idx.IsIndexOnRank(7))
                {
                    expectedValue = BoardHelpers.IndividualSquares[idx + 7];
                }
                Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftNW(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.Shift2E()");
            }
        }

        [Test]
        public static void ShiftNNW_TestAll()
        {
            for (ushort idx = 0; idx < 64; idx++)
            {
                ulong expectedValue = 0;
                if (!idx.IsIndexOnRank(7) && !idx.IsIndexOnRank(6) && !idx.IsIndexOnFile(0))
                {
                    expectedValue = BoardHelpers.IndividualSquares[idx + 15];
                }
                Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[idx].ShiftNNW(), $"Expected value of {expectedValue} _From {idx.IndexToSquareDisplay()}.ShiftNNW()");
            }
        }
        #endregion

        #endregion
    }

}
