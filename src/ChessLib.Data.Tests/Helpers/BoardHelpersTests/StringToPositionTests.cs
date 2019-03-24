using NUnit.Framework;
using System;

namespace ChessLib.Data.Helpers.Tests
{
    [TestFixture]
    public partial class BoardHelpersTests
    {
        [TestFixture]
        public class StringToPositionTests
        {

            [Test]
            public void SquareTextToIndex_ShouldReturnIndex_OnGoodInput()
            {
                var counter = 0;
                for (char r = '1'; r <= '8'; r++)
                {
                    for (char f = 'a'; f <= 'h'; f++)
                    {
                        var text = new string(new[] { f, r });
                        var idx = BoardHelpers.SquareTextToIndex(text);
                        Assert.AreEqual(counter, idx);
                        counter++;
                    }
                }
            }

            [Test]
            public void SquareTextToIndex_ShouldReturnNull_OnUninterpretableSquare()
            {
                var expected = (ushort?)null;
                Assert.AreEqual(expected, BoardHelpers.SquareTextToIndex("-"));
            }

            [Test]
            public void SquareTextToIndex_ShouldThrowException_OnEmpty()
            {
                Assert.Throws(typeof(ArgumentException), () => { BoardHelpers.SquareTextToIndex(""); });
            }

            [Test]
            public void SquareTextToIndex_ShouldThrowException_OnInputLessThan2Chars()
            {
                Assert.Throws(typeof(ArgumentException), () => { BoardHelpers.SquareTextToIndex("a"); });
            }

            [Test]
            public void SquareTextToIndex_ShouldThrowException_OnInputGreaterThan2Chars()
            {
                var message = "";
                Assert.Throws(typeof(ArgumentException), () =>
                {
                    try
                    {
                        BoardHelpers.SquareTextToIndex("a22");
                    }
                    catch (ArgumentException a)
                    {
                        message = a.Message;
                        throw;
                    }
                });
                Console.Write(message);
            }

            [Test]
            public void SquareTextToIndex_ShouldThrowException_OnInvalidFile()
            {
                var message = "";
                Assert.Throws(typeof(ArgumentException), () =>
                {
                    try
                    {
                        BoardHelpers.SquareTextToIndex("i2");
                    }
                    catch (ArgumentException a)
                    {
                        message = a.Message;
                        throw;
                    }
                });
                Console.Write(message);
            }

            [Test]
            public void SquareTextToIndex_ShouldThrowException_OnRankZero()
            {
                var message = "";
                Assert.Throws(typeof(ArgumentException), () =>
                {
                    try
                    {
                        BoardHelpers.SquareTextToIndex("a0");
                    }
                    catch (ArgumentException a)
                    {
                        message = a.Message;
                        throw;
                    }
                });
                Console.Write(message);
            }

            [Test]
            public void SquareTextToIndex_ShouldThrowException_OnRankAboveValidRange()
            {
                var message = "";
                Assert.Throws(typeof(ArgumentException), () =>
                {
                    try
                    {
                        BoardHelpers.SquareTextToIndex("a9");
                    }
                    catch (ArgumentException a)
                    {
                        message = a.Message;
                        throw;
                    }
                });
                Console.Write(message);
            }

            [Test]
            public void IndexToFileDisplay_ShouldReturnCorrespondingFileChar_ValidInput()
            {
                for (ushort i = 0; i < 64; i++)
                {
                    char expected;
                    switch (i % 8)
                    {
                        case 0: expected = 'a'; break;
                        case 1: expected = 'b'; break;
                        case 2: expected = 'c'; break;
                        case 3: expected = 'd'; break;
                        case 4: expected = 'e'; break;
                        case 5: expected = 'f'; break;
                        case 6: expected = 'g'; break;
                        case 7: expected = 'h'; break;
                        default: throw new Exception("Error in IndexToChar test.");
                    }
                    Assert.AreEqual(expected, i.IndexToFileDisplay(), $"Did not return proper character for file index {(i % 8)}. Method returned {i.IndexToFileDisplay()}.");
                }
            }

            [Test]
            public void IndexToRankDisplay_ShouldReturnCorrespondingRankChar_ValidInput()
            {
                for (ushort i = 0; i < 64; i++)
                {
                    char expected;
                    switch (i / 8)
                    {
                        case 0: expected = '1'; break;
                        case 1: expected = '2'; break;
                        case 2: expected = '3'; break;
                        case 3: expected = '4'; break;
                        case 4: expected = '5'; break;
                        case 5: expected = '6'; break;
                        case 6: expected = '7'; break;
                        case 7: expected = '8'; break;
                        default: throw new Exception("Error in IndexToChar test.");
                    }
                    var actual = i.IndexToRankDisplay();
                    Assert.AreEqual(expected, actual, $"Did not return proper character for rank index of {(i / 8)}. Actual return value was {actual}.");
                }
            }

        }
    }
}
