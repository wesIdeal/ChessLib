﻿using ChessLib.Data;
using ChessLib.Types.Enums;
using NUnit.Framework;

namespace ChessLib.Validators.Tests.BoardValidation
{
    [TestFixture]
    public sealed class OpponentInCheckRule
    {
        [TestCase("8/8/8/8/8/8/6kQ/4K3 b - - 0 1", BoardException.None)]
        [TestCase("8/8/8/8/8/8/6kQ/4K3 w - - 0 1", BoardException.OppositeCheck)]
        public static void ValidateCheck(string fen, BoardException expectedException)
        {

            var board = new BoardInfo(fen);
            var rule = new ChessLib.Validators.BoardValidators.Rules.OpponentInCheckRule();
            var actual = rule.Validate(board);
            Assert.AreEqual(expectedException, actual);
        }
    }
}