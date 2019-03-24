using MagicBitboard;
using MagicBitboard.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Data.Helpers.Tests
{

    [TestFixture]
    public class CardinalDirectionShiftTests
    {
        #region Cardinal Direction Shifts

        #region Cardinal Direction Test All Methods
        [Test]
        public void ShiftETestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftE(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.ShiftE()");
                }
            }
        }

        [Test]
        public void ShiftSTestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftS(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.ShiftS()");
                }
            }
        }

        [Test]
        public void ShiftWTestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftW(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.ShiftW()");
                }
            }
        }

        [Test]
        public void ShiftNTestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftN(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.ShiftN()");
                }
            }
        }

        #endregion

        #region Cardinal Direction Sanity Checks
        #region ShiftE Tests


        [Test]
        public void Shift2ETestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].Shift2E(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.Shift2E()");
                }
            }
        }
        #region ShiftE

        [Test]
        public void ShiftENormal()
        {
            ulong u = 1;
            ulong expected = 2;
            var actual = ShiftHelpers.ShiftE(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShiftEHFile()
        {
            ulong u = 128;
            ulong expected = 0;
            var display = BoardHelpers.FileMasks[7].GetDisplayBits();
            var actual = ShiftHelpers.ShiftE(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShiftEE4()
        {
            ulong u = 0x10000000;
            ulong expected = 0x20000000;
            var actual = ShiftHelpers.ShiftE(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region Shift2E


        [Test]
        public void Shift2ENormal()
        {
            ulong u = 0x10000000;//e4
            ulong expected = 0x40000000;//g4
            var actual = ShiftHelpers.Shift2E(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Shift2EFromGFile()
        {
            ulong u = 0x40000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.Shift2E(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Shift2EFromFFile()
        {
            ulong u = 0x80000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.Shift2E(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #endregion

        #region ShiftS Tests

        [Test]
        public void ShiftS2TestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].Shift2S(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.Shift2S()");
                }
            }
        }
        #region ShiftS

        [Test]
        public void ShiftSNormal()
        {
            ulong u = 0x100;
            ulong expected = 0x01;
            var actual = ShiftHelpers.ShiftS(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShiftS1stRank()
        {
            ulong u = 0x01;
            ulong expected = 0;
            var actual = ShiftHelpers.ShiftS(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShiftSD4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x80000;
            var actual = ShiftHelpers.ShiftS(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region Shift2S

        [Test]
        public void Shift2SNormal()
        {
            ulong u = 0x8000000;
            ulong expected = 0x800;
            var actual = ShiftHelpers.Shift2S(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Shift2SFrom2ndRank()
        {
            ulong u = 0x800;
            ulong expected = 0x00;
            var actual = ShiftHelpers.Shift2S(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Shift2SFrom1stRank()
        {
            ulong u = 0x08;
            ulong expected = 0x00;
            var actual = ShiftHelpers.Shift2S(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion
        #endregion

        #region ShiftW Tests


        [Test]
        public void Shift2WTestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].Shift2W(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.Shift2W()");
                }
            }
        }
        #region ShiftW

        [Test]
        public void ShiftWNormal()
        {
            ulong u = 0x2000000;
            ulong expected = 0x1000000;
            var actual = ShiftHelpers.ShiftW(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShiftWAFile()
        {
            ulong u = 0x1000000;
            ulong expected = 0;
            var actual = ShiftHelpers.ShiftW(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShiftWE4()
        {
            ulong u = 0x10000000;
            ulong expected = 0x8000000;
            var actual = ShiftHelpers.ShiftW(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region Shift2W

        [Test]
        public void Shift2WNormal()
        {
            ulong u = 0x10000000;
            ulong expected = 0x4000000;
            var actual = ShiftHelpers.Shift2W(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Shift2WFromBFile()
        {
            ulong u = 0x2000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.Shift2W(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Shift2WFromAFile()
        {
            ulong u = 0x1000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.Shift2W(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion
        #endregion

        #region ShiftN Tests


        [Test]
        public void ShiftN2TestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].Shift2N(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.Shift2N()");
                }
            }
        }
        #region ShiftN

        [Test]
        public void ShiftNNormal()
        {
            ulong u = 0x01;
            ulong expected = 0x100;
            var actual = ShiftHelpers.ShiftN(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShiftN8thRank()
        {
            ulong u = 0x100000000000000;
            ulong expected = 0;
            var actual = ShiftHelpers.ShiftN(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShiftND4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x800000000;
            var actual = ShiftHelpers.ShiftN(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region Shift2N


        [Test]
        public void Shift2NNormal()
        {
            ulong u = 0x8000000;
            ulong expected = 0x80000000000;
            var actual = ShiftHelpers.Shift2N(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Shift2NFrom7thRank()
        {
            ulong u = 0x8000000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.Shift2N(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Shift2NFrom8thRank()
        {
            ulong u = 0x800000000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.Shift2N(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion
        #endregion
        #endregion

        #endregion

        #region Divided Cardinal Direction Shifts
        #region Divided Cardinal Test All Methods
        [Test]
        public void ShiftNNETestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftNNE(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.ShiftNNE()");
                }
            }
        }

        [Test]
        public void ShiftNETestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftNE(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.Shift2E()");
                }
            }
        }

        [Test]
        public void ShiftENETestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftENE(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.ShiftENE()");
                }
            }
        }

        [Test]
        public void ShiftESETestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftESE(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.ShiftESE()");
                }
            }
        }

        [Test]
        public void ShiftSETestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftSE(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.ShiftSE()");
                }
            }
        }

        [Test]
        public void ShiftSSETestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftSSE(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.ShiftSSE()");
                }
            }
        }

        [Test]
        public void ShiftSSWTestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftSSW(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.ShiftSSW()");
                }
            }
        }

        [Test]
        public void ShiftSWTestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftSW(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.Shift2E()");
                }
            }
        }

        [Test]
        public void ShiftWSWTestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftWSW(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.ShiftWSW()");
                }
            }
        }

        [Test]
        public void ShiftWNWTestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftWNW(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.ShiftWNW()");
                }
            }
        }

        [Test]
        public void ShiftNWTestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftNW(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.Shift2E()");
                }
            }
        }

        [Test]
        public void ShiftNNWTestAll()
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
                    Assert.AreEqual(expectedValue, BoardHelpers.IndividualSquares[r, f].ShiftNNW(), $"Expected value of {expectedValue} from {(char)('a' + r)}{f + 1}.ShiftNNW()");
                }
            }
        }
        #endregion

        #region Divided Cardinal Direction Sanity Checks
        #region ShiftNNE

        [Test]
        public void ShiftNNENormal()
        {
            ulong u = 1;
            ulong expected = 0x20000;
            var actual = ShiftHelpers.ShiftNNE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNNEFromD4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x100000000000;
            var actual = ShiftHelpers.ShiftNNE(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShiftNNEFromH6()
        {
            ulong u = 0x800000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftNNE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNNEFromF8()
        {
            ulong u = 0x2000000000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftNNE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNNEFromH8()
        {
            ulong u = 0x8000000000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftNNE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNNEFromA8()
        {
            ulong u = 0x100000000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftNNE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNNEFromC1()
        {
            ulong u = 0x04;
            ulong expected = 0x80000;
            var actual = ShiftHelpers.ShiftNNE(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftNE Tests

        [Test]
        public void ShiftNENormal()
        {
            ulong u = 1;
            ulong expected = 0x0200;
            var actual = ShiftHelpers.ShiftNE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNEFromD4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x1000000000;
            var actual = ShiftHelpers.ShiftNE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNEFromH6()
        {
            ulong u = 0x800000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftNE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNEFromF8()
        {
            ulong u = 0x2000000000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftNE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNEFromH8()
        {
            ulong u = 0x8000000000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftNE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNEFromA8()
        {
            ulong u = 0x100000000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftNE(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftENE Tests

        [Test]
        public void ShiftENENormal()
        {
            ulong u = 1;
            ulong expected = 0x0400;
            var actual = ShiftHelpers.ShiftENE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftENEFromD4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x2000000000;
            var actual = ShiftHelpers.ShiftENE(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShiftENEFromH6()
        {
            ulong u = 0x800000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftENE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftENEFromF8()
        {
            ulong u = 0x2000000000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftENE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftENEFromH8()
        {
            ulong u = 0x8000000000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftENE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftENEFromA8()
        {
            ulong u = 0x100000000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftENE(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftESE Tests

        [Test]
        public void ShiftESENormal()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 0];
            ulong expected = BoardHelpers.IndividualSquares[6, 2];
            var actual = ShiftHelpers.ShiftESE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftESEFromD4()
        {
            ulong u = BoardHelpers.IndividualSquares[3, 3];
            ulong expected = BoardHelpers.IndividualSquares[2, 5];
            var actual = ShiftHelpers.ShiftESE(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShiftESEFromH6()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 6];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftESE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftESEFromF8()
        {
            ulong u = BoardHelpers.IndividualSquares[6, 6];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftESE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftESEFromH8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 5];
            ulong expected = BoardHelpers.IndividualSquares[6, 7];
            var actual = ShiftHelpers.ShiftESE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftESEFromG2()
        {
            ulong u = 0x4000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftESE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftESEFromA8()
        {
            ulong u = 0x100000000000000;
            ulong expected = 0x4000000000000;
            var actual = ShiftHelpers.ShiftESE(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftSE Tests

        [Test]
        public void ShiftSENormal()
        {
            ulong u = 0x100000000000000;
            ulong expected = 0x2000000000000;
            var actual = ShiftHelpers.ShiftSE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSEFromD4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x100000;
            var actual = ShiftHelpers.ShiftSE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSEFromH6()
        {
            ulong u = 0x800000000000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftSE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSEFromH2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 7];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftSE(u);
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public void ShiftSEFromG1()
        {
            ulong u = BoardHelpers.IndividualSquares[0, 6];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftSE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSEFromH1()
        {
            ulong u = BoardHelpers.IndividualSquares[0, 7];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftSE(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftSSE Tests

        [Test]
        public void ShiftSSENormal()
        {
            ulong u = 0x400000; //G3
            ulong expected = 0x80;//H1
            var actual = ShiftHelpers.ShiftSSE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSSEFromD4()
        {
            ulong u = BoardHelpers.IndividualSquares[3, 3];
            ulong expected = BoardHelpers.IndividualSquares[1, 4];
            var actual = ShiftHelpers.ShiftSSE(u);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShiftSSEFromF2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 5];
            ulong expected = 0;
            var actual = ShiftHelpers.ShiftSSE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSSEFromG2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 6];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftSSE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSSEFromH8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 7];
            ulong expected = 0;
            var actual = ShiftHelpers.ShiftSSE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSSEFromH3()
        {
            ulong u = 0x800000;
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftSSE(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSSEFromA2()
        {
            ulong u = 0x100;
            ulong expected = 0;
            var actual = ShiftHelpers.ShiftSSE(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftSSW Tests
        [Test]
        public void ShiftSSWNormal()
        {
            ulong u = 0x200000000000000;//b8
            ulong expected = 0x10000000000;//a6
            var actual = ShiftHelpers.ShiftSSW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSSWFromH3()
        {
            ulong u = 0x800000;//H3
            ulong expected = 0x40; //g1
            var actual = ShiftHelpers.ShiftSSW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSSWFromH8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 7];//h8
            ulong expected = BoardHelpers.IndividualSquares[5, 6]; //g6
            var actual = ShiftHelpers.ShiftSSW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSSWFromA8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 0];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftSSW(u);
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public void ShiftSSWFromA3()
        {
            ulong u = BoardHelpers.IndividualSquares[2, 0]; //a3
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftSSW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSSWFromH2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 7];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftSSW(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftSW Tests
        [Test]
        public void ShiftSWNormal()
        {
            ulong u = 0x8000000000000000;
            ulong expected = 0x40000000000000;
            var actual = ShiftHelpers.ShiftSW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSWFromD4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x40000; //c3
            var actual = ShiftHelpers.ShiftSW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSWFromH6()
        {
            ulong u = 0x800000000000;
            ulong expected = 0x4000000000;
            var actual = ShiftHelpers.ShiftSW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSWFromA2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 0];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftSW(u);
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public void ShiftSWFromG1()
        {
            ulong u = BoardHelpers.IndividualSquares[0, 6];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftSW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftSWFromH1()
        {
            ulong u = BoardHelpers.IndividualSquares[0, 7];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftSW(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftWSW Tests
        [Test]
        public void ShiftWSWNormal()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 2];//c8
            ulong expected = BoardHelpers.IndividualSquares[6, 0];//a7
            var actual = ShiftHelpers.ShiftWSW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftWSWFromH3()
        {
            ulong u = 0x800000;//H3
            ulong expected = 0x2000; //f2
            var actual = ShiftHelpers.ShiftWSW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftWSWFromH8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 7];//h8
            ulong expected = BoardHelpers.IndividualSquares[6, 5]; //f7
            var actual = ShiftHelpers.ShiftWSW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftWSWFromA8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 0];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftWSW(u);
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public void ShiftWSWFromA3()
        {
            ulong u = BoardHelpers.IndividualSquares[2, 0]; //a3
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftWSW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftWSWFromH2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 7];
            ulong expected = 0x01 << 5;
            var actual = ShiftHelpers.ShiftWSW(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftWNW Tests
        [Test]
        public void ShiftWNWNormal()
        {
            ulong u = 0x4000000000000; //c7
            ulong expected = 0x100000000000000; // a8
            var actual = ShiftHelpers.ShiftWNW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftWNWFromH3()
        {
            ulong u = 0x800000;//H3
            ulong expected = 0x20000000; //f4
            var actual = ShiftHelpers.ShiftWNW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftWNWFromH8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 7];//h8
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftWNW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftWNWFromA8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 0];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftWNW(u);
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public void ShiftWNWFromA3()
        {
            ulong u = BoardHelpers.IndividualSquares[2, 0]; //a3
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftWNW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftWNWFromH2()
        {
            ulong u = 0x8000; //h2
            ulong expected = 0x200000;
            var actual = ShiftHelpers.ShiftWNW(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftNW Tests
        [Test]
        public void ShiftNWNormal()
        {
            ulong u = 0x2000000000000;//b7
            ulong expected = 0x100000000000000;//a8
            var actual = ShiftHelpers.ShiftNW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNWFromD4()
        {
            ulong u = 0x8000000;
            ulong expected = 0x400000000; //c5
            var actual = ShiftHelpers.ShiftNW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNWFromH6()
        {
            ulong u = 0x800000000000;//h6
            ulong expected = 0x40000000000000;//g7
            var actual = ShiftHelpers.ShiftNW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNWFromA2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 0];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftNW(u);
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public void ShiftNWFromA8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 0];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftNW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNWFromH1()
        {
            ulong u = BoardHelpers.IndividualSquares[0, 7];
            ulong expected = 0x01 << 14;
            var actual = ShiftHelpers.ShiftNW(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region ShiftNNW Tests
        [Test]
        public void ShiftNNWNormal()
        {
            ulong u = 0x20000000000;//b6
            ulong expected = 0x100000000000000;//a8
            var actual = ShiftHelpers.ShiftNNW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNNWFromB7()
        {
            ulong u = 0x2000000000000;//b7
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftNNW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNNWFromH6()
        {
            ulong u = 0x800000000000;//h6
            ulong expected = 0x4000000000000000; //g6
            var actual = ShiftHelpers.ShiftNNW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNNWFromA8()
        {
            ulong u = BoardHelpers.IndividualSquares[7, 0];
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftNNW(u);
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public void ShiftNNWFromA3()
        {
            ulong u = BoardHelpers.IndividualSquares[2, 0]; //a3
            ulong expected = 0x00;
            var actual = ShiftHelpers.ShiftNNW(u);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShiftNNWFromH2()
        {
            ulong u = BoardHelpers.IndividualSquares[1, 7];
            ulong expected = 0x01 << 30;
            var actual = ShiftHelpers.ShiftNNW(u);
            Assert.AreEqual(expected, actual);
        }
        #endregion
        #endregion

        #endregion
    }
}
