using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ChessLib.Validators.Tests.MoveValidation.CastlingRules
{
    [TestFixture]
    class KingNotInCheckBeforeMove : Data.Validators.MoveValidation.CastlingRules.KingNotInCheckBeforeMove
    {
        private readonly ulong[][] _postBoard = new ulong[2][];
        private readonly BoardInfo _biNotInCheck = new BoardInfo("r3k2r/8/8/8/8/8/8/RRRRKRRR b KQkq - 0 1");
        private readonly BoardInfo _biInCheck = new BoardInfo("r3k2r/8/8/8/8/8/4Q3/RRRRKRRR b KQkq - 0 1");
        private readonly MoveExt _move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
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
