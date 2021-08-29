using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;
// ReSharper disable StringLiteralTypo

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.CastlingRules
{
    [TestFixture]
    class KingNotInCheckBeforeMove : Core.Validation.Validators.MoveValidation.CastlingRules.KingNotInCheckBeforeMove
    {
        private static readonly FenTextToBoard fenToBoard = new FenTextToBoard();
        private readonly ulong[][] _postBoard = new ulong[2][];
        private readonly Board _biNotInCheck = fenToBoard.Translate("r3k2r/8/8/8/8/8/8/RRRRKRRR b KQkq - 0 1");
        private readonly Board _biInCheck = fenToBoard.Translate("r3k2r/8/8/8/8/8/4Q3/RRRRKRRR b KQkq - 0 1");
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