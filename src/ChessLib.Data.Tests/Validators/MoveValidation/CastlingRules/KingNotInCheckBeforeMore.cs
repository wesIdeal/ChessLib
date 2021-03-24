﻿using ChessLib.Core;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Types.Helpers;
using ChessLib.Data.Helpers;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ChessLib.Data.Validators.MoveValidation.CastlingRules.Tests
{
    [TestFixture]
    class KingNotInCheckBeforeMove : Core.Validation.Validators.MoveValidation.CastlingRules.KingNotInCheckBeforeMove
    {
        private readonly ulong[][] _postBoard = new ulong[2][];
        private readonly Board _biNotInCheck = new Board("r3k2r/8/8/8/8/8/8/RRRRKRRR b KQkq - 0 1");
        private readonly Board _biInCheck = new Board("r3k2r/8/8/8/8/8/4Q3/RRRRKRRR b KQkq - 0 1");
        private readonly Move _move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
        [Test]
        public void Validate_ShouldReturnNullIfKingIsNotInCheckWhenCastling()
        {
            Assert.AreEqual(MoveError.NoneSet, this.Validate(_biNotInCheck, _postBoard, _move));
        }
        [Test]
        public void Validate_ShouldReturnErrorIfKingIsNotInCheckWhenCastling()
        {
            var expected = MoveError.CastleKingInCheck;
            Assert.AreEqual(expected, Validate(_biInCheck, _postBoard, _move));
        }
    }
}
