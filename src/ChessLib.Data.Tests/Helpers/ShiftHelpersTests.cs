using ChessLib.Data.Helpers;
using NUnit.Framework;
namespace ChessLib.Data.Tests.Helpers
{
    [TestFixture]
    public static class ShiftHelpersTests
    {
        #region Cardinal Direction Shifts

        #region Cardinal Direction Test All Methods
        [Test]
        public static void ShiftETestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (f != 7)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r, f + 1];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftE(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.ShiftE()");
                }
            }
        }

        [Test]
        public static void ShiftSTestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (r != 0)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r - 1, f];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftS(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.ShiftS()");
                }
            }
        }

        [Test]
        public static void ShiftWTestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (f != 0)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r, f - 1];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftW(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.ShiftW()");
                }
            }
        }

        [Test]
        public static void ShiftNTestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (r != 7)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r + 1, f];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftN(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.ShiftN()");
                }
            }
        }

        #endregion

        #region Cardinal Direction Sanity Checks
        #region ShiftE Tests

        [Test]
        public static void Shift2ETestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (f != 6 && f != 7)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r, f + 2];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].Shift2E(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.Shift2E()");
                }
            }
        }
        #region ShiftE

        [Test]
        public static void ShiftE_Normal()
        {
            ulong u = 1;
            ulong expected = 2;
            var actual = u.ShiftE();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void ShiftE_hFile()
        {
            ulong u = 128;
            ulong expected = 0;
            BoardHelpers.FileMasks[7].GetDisplayBits();
            var actual = u.ShiftE();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void ShiftEe4()
        {
            ulong u = 0x10000000;
            ulong expected = 0x20000000;
            var actual = u.ShiftE();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region Shift2E


        [Test]
        public static void Shift2E_Normal()
        {
            ulong u = 0x10000000;//e4
            ulong expected = 0x40000000;//g4
            var actual = u.Shift2E();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Shift2E_GFile()
        {
            ulong u = 0x40000000;
            ulong expected = 0x00;
            var actual = u.Shift2E();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Shift2E_FFile()
        {
            ulong u = 0x80000000;
            ulong expected = 0x00;
            var actual = u.Shift2E();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #endregion

        #region ShiftS Tests

        [Test]
        public static void ShiftS2TestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (r != 0 && r != 1)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r - 2, f];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].Shift2S(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.Shift2S()");
                }
            }
        }
        #region ShiftS

        [Test]
        public static void ShiftS_Normal()
        {
            ulong u = 0x100;
            ulong expected = 0x01;
            var actual = u.ShiftS();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void ShiftS_1stRank()
        {
            ulong u = 0x01;
            ulong expected = 0;
            var actual = u.ShiftS();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void ShiftS_d4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x80000;
            var actual = u.ShiftS();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region Shift2S

        [Test]
        public static void Shift2S_Normal()
        {
            ulong u = 0x8000000;
            ulong expected = 0x800;
            var actual = u.Shift2S();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Shift2S__2ndRank()
        {
            ulong u = 0x800;
            ulong expected = 0x00;
            var actual = u.Shift2S();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Shift2S_1stRank()
        {
            ulong u = 0x08;
            ulong expected = 0x00;
            var actual = u.Shift2S();
            Assert.AreEqual(expected, actual);
        }
        #endregion
        #endregion

        #region ShiftW Tests


        [Test]
        public static void Shift2WTestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (f != 1 && f != 0)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r, f - 2];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].Shift2W(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.Shift2W()");
                }
            }
        }
        #region ShiftW

        [Test]
        public static void ShiftW_Normal()
        {
            ulong u = 0x2000000;
            ulong expected = 0x1000000;
            var actual = u.ShiftW();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void ShiftW_aFile()
        {
            ulong u = 0x1000000;
            ulong expected = 0;
            var actual = u.ShiftW();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void ShiftW_e4()
        {
            ulong u = 0x10000000;
            ulong expected = 0x8000000;
            var actual = u.ShiftW();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region Shift2W

        [Test]
        public static void Shift2_Normal()
        {
            ulong u = 0x10000000;
            ulong expected = 0x4000000;
            var actual = u.Shift2W();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Shift2W_BFile()
        {
            ulong u = 0x2000000;
            ulong expected = 0x00;
            var actual = u.Shift2W();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Shift2W_AFile()
        {
            ulong u = 0x1000000;
            ulong expected = 0x00;
            var actual = u.Shift2W();
            Assert.AreEqual(expected, actual);
        }
        #endregion
        #endregion

        #region ShiftN Tests


        [Test]
        public static void ShiftN2TestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (r != 6 && r != 7)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r + 2, f];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].Shift2N(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.Shift2N()");
                }
            }
        }
        #region ShiftN

        [Test]
        public static void Shift_Normal()
        {
            ulong u = 0x01;
            ulong expected = 0x100;
            var actual = u.ShiftN();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void ShiftN_8thRank()
        {
            ulong u = 0x100000000000000;
            ulong expected = 0;
            var actual = u.ShiftN();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void ShiftN_d4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x800000000;
            var actual = u.ShiftN();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region Shift2N


        [Test]
        public static void Shift2N_Normal()
        {
            ulong u = 0x8000000;
            ulong expected = 0x80000000000;
            var actual = u.Shift2N();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Shift2N_7thRank()
        {
            ulong u = 0x8000000000000;
            ulong expected = 0x00;
            var actual = u.Shift2N();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Shift2N_8thRank()
        {
            ulong u = 0x800000000000000;
            ulong expected = 0x00;
            var actual = u.Shift2N();
            Assert.AreEqual(expected, actual);
        }
        #endregion
        #endregion
        #endregion

        #endregion

        #region Divided Cardinal Direction Shifts
        #region Divided Cardinal Test All Methods
        [Test]
        public static void ShiftNNETestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (r != 6 && r != 7 && f != 7)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r + 2, f + 1];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftNNE(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.ShiftNNE()");
                }
            }
        }

        [Test]
        public static void ShiftNE_TestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (f != 7 && r != 7)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r + 1, f + 1];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftNE(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.Shift2E()");
                }
            }
        }

        [Test]
        public static void ShiftENE_TestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (r != 7 && f != 7 && f != 6)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r + 1, f + 2];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftENE(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.ShiftENE()");
                }
            }
        }

        [Test]
        public static void ShiftESE_TestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (r != 0 && f != 7 && f != 6)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r - 1, f + 2];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftESE(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.ShiftESE()");
                }
            }
        }

        [Test]
        public static void ShiftSE_TestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (f != 7 && r != 0)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r - 1, f + 1];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftSE(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.ShiftSE()");
                }
            }
        }

        [Test]
        public static void ShiftSSE_TestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (r != 0 && r != 1 && f != 7)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r - 2, f + 1];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftSSE(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.ShiftSSE()");
                }
            }
        }

        [Test]
        public static void ShiftSSW_TestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (r != 0 && r != 1 && f != 0)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r - 2, f - 1];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftSSW(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.ShiftSSW()");
                }
            }
        }

        [Test]
        public static void ShiftSW_TestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (f != 0 && r != 0)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r - 1, f - 1];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftSW(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.Shift2E()");
                }
            }
        }

        [Test]
        public static void ShiftWSW_TestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (r != 0 && f != 0 && f != 1)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r - 1, f - 2];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftWSW(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.ShiftWSW()");
                }
            }
        }

        [Test]
        public static void ShiftWNW_TestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (r != 7 && f != 0 && f != 1)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r + 1, f - 2];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftWNW(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.ShiftWNW()");
                }
            }
        }

        [Test]
        public static void ShiftNW_TestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (f != 0 && r != 7)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r + 1, f - 1];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftNW(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.Shift2E()");
                }
            }
        }

        [Test]
        public static void ShiftNNW_TestAll()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    ulong expectedValue = 0;
                    if (r != 7 && r != 6 && f != 0)
                    {
                        expectedValue = BoardHelpers.IndividualSquares[r + 2, f - 1];
                    }
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftNNW(), $"Expected value of {expectedValue} _ {(char)('a' + r)}{f + 1}.ShiftNNW()");
                }
            }
        }
        #endregion

        #region Divided Cardinal Direction Sanity Checks
        #region ShiftNNE

        [Test]
        public static void ShiftNNE_Normal()
        {
            ulong u = 1;
            ulong expected = 0x20000;
            var actual = u.ShiftNNE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNNE_d4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x100000000000;
            var actual = u.ShiftNNE();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void ShiftNNE_h6()
        {
            ulong u = 0x800000000000;
            ulong expected = 0x00;
            var actual = u.ShiftNNE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNNE_F8()
        {
            ulong u = 0x2000000000000000;
            ulong expected = 0x00;
            var actual = u.ShiftNNE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNNE_H8()
        {
            ulong u = 0x8000000000000000;
            ulong expected = 0x00;
            var actual = u.ShiftNNE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNNE_A8()
        {
            ulong u = 0x100000000000000;
            ulong expected = 0x00;
            var actual = u.ShiftNNE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNNE_C1()
        {
            ulong u = 0x04;
            ulong expected = 0x80000;
            var actual = u.ShiftNNE();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftNE Tests

        [Test]
        public static void ShiftNE_Normal()
        {
            ulong u = 1;
            ulong expected = 0x0200;
            var actual = u.ShiftNE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNE__d4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x1000000000;
            var actual = u.ShiftNE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNE_h6()
        {
            ulong u = 0x800000000000;
            ulong expected = 0x00;
            var actual = u.ShiftNE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNE_f8()
        {
            ulong u = 0x2000000000000000;
            ulong expected = 0x00;
            var actual = u.ShiftNE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNE_H8()
        {
            ulong u = 0x8000000000000000;
            ulong expected = 0x00;
            var actual = u.ShiftNE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNE_a8()
        {
            ulong u = 0x100000000000000;
            ulong expected = 0x00;
            var actual = u.ShiftNE();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftENE Tests

        [Test]
        public static void ShiftENE_Normal()
        {
            ulong u = 1;
            ulong expected = 0x0400;
            var actual = u.ShiftENE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftENE_d4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x2000000000;
            var actual = u.ShiftENE();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void ShiftENE_h6()
        {
            ulong u = 0x800000000000;
            ulong expected = 0x00;
            var actual = u.ShiftENE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftENE_F8()
        {
            ulong u = 0x2000000000000000;
            ulong expected = 0x00;
            var actual = u.ShiftENE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftENE_H8()
        {
            ulong u = 0x8000000000000000;
            ulong expected = 0x00;
            var actual = u.ShiftENE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftENE_A8()
        {
            ulong u = 0x100000000000000;
            ulong expected = 0x00;
            var actual = u.ShiftENE();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftESE Tests

        [Test]
        public static void ShiftESE_Normal()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 0];
            ulong expected = BoardHelpers.IndividualSquares[6, 2];
            var actual = u.ShiftESE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftESE_d4()
        {
            ulong u = BoardHelpers.IndividualSquares[3, 3];
            ulong expected = BoardHelpers.IndividualSquares[2, 5];
            var actual = u.ShiftESE();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void ShiftESE_h6()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 6];
            ulong expected = 0x00;
            var actual = u.ShiftESE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftESE_F8()
        {
            ulong u = BoardHelpers.IndividualSquares[6, 6];
            ulong expected = 0x00;
            var actual = u.ShiftESE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftESE_H8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 5];
            ulong expected = BoardHelpers.IndividualSquares[6, 7];
            var actual = u.ShiftESE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftESE_G2()
        {
            ulong u = 0x4000;
            ulong expected = 0x00;
            var actual = u.ShiftESE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftESE_A8()
        {
            ulong u = 0x100000000000000;
            ulong expected = 0x4000000000000;
            var actual = u.ShiftESE();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftSE Tests

        [Test]
        public static void ShiftSE_Normal()
        {
            ulong u = 0x100000000000000;
            ulong expected = 0x2000000000000;
            var actual = u.ShiftSE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSE_d4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x100000;
            var actual = u.ShiftSE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSE_h6()
        {
            ulong u = 0x800000000000;
            ulong expected = 0x00;
            var actual = u.ShiftSE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSE_H2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 7];
            ulong expected = 0x00;
            var actual = u.ShiftSE();
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public static void ShiftSE_G1()
        {
            ulong u = BoardHelpers.IndividualSquares[0, 6];
            ulong expected = 0x00;
            var actual = u.ShiftSE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSE_H1()
        {
            ulong u = BoardHelpers.IndividualSquares[0, 7];
            ulong expected = 0x00;
            var actual = u.ShiftSE();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftSSE Tests

        [Test]
        public static void ShiftSSE_Normal()
        {
            ulong u = 0x400000; //G3
            ulong expected = 0x80;//H1
            var actual = u.ShiftSSE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSSE_d4()
        {
            ulong u = BoardHelpers.IndividualSquares[3, 3];
            ulong expected = BoardHelpers.IndividualSquares[1, 4];
            var actual = u.ShiftSSE();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void ShiftSSE_F2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 5];
            ulong expected = 0;
            var actual = u.ShiftSSE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSSE_G2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 6];
            ulong expected = 0x00;
            var actual = u.ShiftSSE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSSE_H8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 7];
            ulong expected = 0;
            var actual = u.ShiftSSE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSSE_H3()
        {
            ulong u = 0x800000;
            ulong expected = 0x00;
            var actual = u.ShiftSSE();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSSE_A2()
        {
            ulong u = 0x100;
            ulong expected = 0;
            var actual = u.ShiftSSE();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftSSW Tests
        [Test]
        public static void ShiftSS_Normal()
        {
            ulong u = 0x200000000000000;//b8
            ulong expected = 0x10000000000;//a6
            var actual = u.ShiftSSW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSSW_H3()
        {
            ulong u = 0x800000;//H3
            ulong expected = 0x40; //g1
            var actual = u.ShiftSSW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSSW_H8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 7];//h8
            ulong expected = BoardHelpers.IndividualSquares[5, 6]; //g6
            var actual = u.ShiftSSW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSSW_A8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 0];
            ulong expected = 0x00;
            var actual = u.ShiftSSW();
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public static void ShiftSSW_A3()
        {
            ulong u = BoardHelpers.IndividualSquares[2, 0]; //a3
            ulong expected = 0x00;
            var actual = u.ShiftSSW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSSW_H2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 7];
            ulong expected = 0x00;
            var actual = u.ShiftSSW();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftSW Tests
        [Test]
        public static void ShiftSW_Normal()
        {
            ulong u = 0x8000000000000000;
            ulong expected = 0x40000000000000;
            var actual = u.ShiftSW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSW_d4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x40000; //c3
            var actual = u.ShiftSW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSW_h6()
        {
            ulong u = 0x800000000000;
            ulong expected = 0x4000000000;
            var actual = u.ShiftSW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSW_A2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 0];
            ulong expected = 0x00;
            var actual = u.ShiftSW();
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public static void ShiftSW_G1()
        {
            ulong u = BoardHelpers.IndividualSquares[0, 6];
            ulong expected = 0x00;
            var actual = u.ShiftSW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftSW_H1()
        {
            ulong u = BoardHelpers.IndividualSquares[0, 7];
            ulong expected = 0x00;
            var actual = u.ShiftSW();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftWSW Tests
        [Test]
        public static void ShiftWS_Normal()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 2];//c8
            ulong expected = BoardHelpers.IndividualSquares[6, 0];//a7
            var actual = u.ShiftWSW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftWSW_H3()
        {
            ulong u = 0x800000;//H3
            ulong expected = 0x2000; //f2
            var actual = u.ShiftWSW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftWSW_H8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 7];//h8
            ulong expected = BoardHelpers.IndividualSquares[6, 5]; //f7
            var actual = u.ShiftWSW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftWSW_A8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 0];
            ulong expected = 0x00;
            var actual = u.ShiftWSW();
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public static void ShiftWSW_A3()
        {
            ulong u = BoardHelpers.IndividualSquares[2, 0]; //a3
            ulong expected = 0x00;
            var actual = u.ShiftWSW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftWSW_H2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 7];
            ulong expected = 0x01 << 5;
            var actual = u.ShiftWSW();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftWNW Tests
        [Test]
        public static void ShiftWN_Normal()
        {
            ulong u = 0x4000000000000; //c7
            ulong expected = 0x100000000000000; // a8
            var actual = u.ShiftWNW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftWNW_H3()
        {
            ulong u = 0x800000;//H3
            ulong expected = 0x20000000; //f4
            var actual = u.ShiftWNW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftWNW_H8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 7];//h8
            ulong expected = 0x00;
            var actual = u.ShiftWNW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftWNW_A8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 0];
            ulong expected = 0x00;
            var actual = u.ShiftWNW();
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public static void ShiftWNW_A3()
        {
            ulong u = BoardHelpers.IndividualSquares[2, 0]; //a3
            ulong expected = 0x00;
            var actual = u.ShiftWNW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftWNW_H2()
        {
            ulong u = 0x8000; //h2
            ulong expected = 0x200000;
            var actual = u.ShiftWNW();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftNW Tests
        [Test]
        public static void ShiftN_Normal()
        {
            ulong u = 0x2000000000000;//b7
            ulong expected = 0x100000000000000;//a8
            var actual = u.ShiftNW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNW_d4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x400000000; //c5
            var actual = u.ShiftNW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNW_h6()
        {
            ulong u = 0x800000000000;//h6
            ulong expected = 0x40000000000000;//g7
            var actual = u.ShiftNW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNW_A2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 0];
            ulong expected = 0x00;
            var actual = u.ShiftNW();
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public static void ShiftNW_A8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 0];
            ulong expected = 0x00;
            var actual = u.ShiftNW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNW_H1()
        {
            ulong u = BoardHelpers.IndividualSquares[0, 7];
            ulong expected = 0x01 << 14;
            var actual = u.ShiftNW();
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftNNW Tests
        [Test]
        public static void ShiftNN_Normal()
        {
            ulong u = 0x20000000000;//b6
            ulong expected = 0x100000000000000;//a8
            var actual = u.ShiftNNW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNNW_B7()
        {
            ulong u = 0x2000000000000;//b7
            ulong expected = 0x00;
            var actual = u.ShiftNNW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNNW_h6()
        {
            ulong u = 0x800000000000;//h6
            ulong expected = 0x4000000000000000; //g6
            var actual = u.ShiftNNW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNNW_A8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 0];
            ulong expected = 0x00;
            var actual = u.ShiftNNW();
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public static void ShiftNNW_A3()
        {
            ulong u = BoardHelpers.IndividualSquares[2, 0]; //a3
            ulong expected = 0x00;
            var actual = u.ShiftNNW();
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public static void ShiftNNW_H2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 7];
            ulong expected = 0x01 << 30;
            var actual = u.ShiftNNW();
            Assert.AreEqual(expected, actual);
        }
        #endregion
        #endregion

        #endregion
    }
}
