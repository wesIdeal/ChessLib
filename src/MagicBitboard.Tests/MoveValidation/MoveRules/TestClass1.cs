using MagicBitboard;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.Types;

namespace ChessLib.MagicBitboard.Tests.MoveValidation.MoveRules
{
    [TestFixture]
    public class PawnDestinationValid : ChessLib.MagicBitboard.MoveValidation.MoveRules.PawnDestinationValid
    {
        ulong[][] pmb = new ulong[2][];
        [Test]
        public void ShouldReturnNullWhenPawnMoveIsValid_White()
        {
            string fen = "8/PPPP1PPP/8/2k5/8/2K5/pppp1ppp/8 w - - 0 1";
            BoardInfo bi = BoardInfo.BoardInfoFromFen(fen);
            bi.ActivePlayerColor = Color.White;
            var move = MoveHelpers.GenerateMove(51, 59, MoveType.Promotion, PromotionPiece.Queen);
            var actual = Validate(bi, pmb, move);
            Assert.IsNull(actual);

        }
        [Test]
        public void ShouldReturnNullWhenPawnMoveIsValid_Black()
        {
            string fen = "8/PPPP1PPP/8/2k5/8/2K5/pppp1ppp/8 w - - 0 1";
            BoardInfo bi = BoardInfo.BoardInfoFromFen(fen);
            bi.ActivePlayerColor = Color.Black;
            var move = MoveHelpers.GenerateMove(12, 4, MoveType.Promotion, PromotionPiece.Queen);
            var actual = Validate(bi, pmb, move);
            Assert.IsNull(actual);
        }

        [Test]
        public void ShouldFailWhenAPieceBlocksMove_White()
        {
            string fen = "4q3/PPPPPPPP/8/2k5/8/2K5/pppppppp/4Q3 w - - 0 1";
            BoardInfo bi = BoardInfo.BoardInfoFromFen(fen);
            bi.ActivePlayerColor = Color.Black;
            var move = MoveHelpers.GenerateMove(51, 59, MoveType.Promotion, PromotionPiece.Queen);
            var actual = Validate(bi, pmb, move);
            Assert.AreEqual(MoveExceptionType.BadDestination, actual);
        }

        [Test]
        public void ShouldFailWhenAPieceBlocksMove_Black()
        {
            string fen = "4q3/PPPPPPPP/8/2k5/8/2K5/pppppppp/4Q3 w - - 0 1";
            BoardInfo bi = BoardInfo.BoardInfoFromFen(fen);
            bi.ActivePlayerColor = Color.Black;
            var move = MoveHelpers.GenerateMove(12, 4, MoveType.Promotion, PromotionPiece.Queen);
            var actual = Validate(bi, pmb, move);
            Assert.AreEqual(MoveExceptionType.BadDestination, actual);
        }
    }
}
