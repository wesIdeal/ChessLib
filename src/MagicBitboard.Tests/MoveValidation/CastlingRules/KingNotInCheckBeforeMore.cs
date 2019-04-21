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
        private readonly ulong[][] _postBoard = new ulong[2][];
        private readonly BoardInfo _biNotInCheck = BoardInfo.BoardInfoFromFen("r3k2r/8/8/8/8/8/8/RRRRKRRR b KQkq - 0 1");
        private readonly BoardInfo _biInCheck = BoardInfo.BoardInfoFromFen("r3k2r/8/8/8/8/8/4Q3/RRRRKRRR b KQkq - 0 1");
        private readonly MoveExt _move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
        [Test]
        public void Validate_ShouldReturnNullIfKingIsNotInCheckWhenCastling()
        {
            Assert.IsNull(this.Validate(_biNotInCheck, _postBoard, _move));
        }
        [Test]
        public void Validate_ShouldReturnErrorIfKingIsNotInCheckWhenCastling()
        {
            var expected = MoveExceptionType.Castle_KingInCheck;
            Assert.AreEqual(expected, Validate(_biInCheck, _postBoard, _move));
        }
    }
}
