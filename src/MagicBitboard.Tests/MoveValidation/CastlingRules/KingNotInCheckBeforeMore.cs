using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using MagicBitboard;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ChessLib.MagicBitboard.Tests.MoveValidation.CastlingRules
{
    [TestFixture]
    class KingNotInCheckBeforeMove : ChessLib.MagicBitboard.MoveValidation.CastlingRules.KingNotInCheckBeforeMove
    {
        private ulong[][] postBoard = new ulong[2][];
        private BoardInfo biNotInCheck = BoardInfo.BoardInfoFromFen("r3k2r/8/8/8/8/8/8/RRRRKRRR b KQkq - 0 1");
        private BoardInfo biInCheck = BoardInfo.BoardInfoFromFen("r3k2r/8/8/8/8/8/4Q3/RRRRKRRR b KQkq - 0 1");
        private MoveExt move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
        [Test]
        public void Validate_ShouldReturnNullIfKingIsNotInCheckWhenCastling()
        {
            Assert.IsNull(this.Validate(biNotInCheck, postBoard, move));
        }
        [Test]
        public void Validate_ShouldReturnErrorIfKingIsNotInCheckWhenCastling()
        {
            var expected = MoveExceptionType.Castle_KingInCheck;
            Assert.AreEqual(expected, Validate(biInCheck, postBoard, move));
        }
    }
}
