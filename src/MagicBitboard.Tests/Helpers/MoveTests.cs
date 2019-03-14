using MagicBitboard;
using MagicBitboard.Enums;
using MagicBitboard.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Tests.Helpers
{
    [TestFixture]
    public class MoveTests
    {
        const string fenEP = "8/PPPPPPPP/8/2k5/8/2K5/pppppppp/8 w - - 0 1";
        BoardInfo biEnPassent = FENHelpers.BoardInfoFromFen(fenEP);
        ulong[] whitePiecesEnPassent, blackPiecesEnPassent;
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ShouldGetCorrectMoveWhenEnPassentIsSent()
        {
            for (ushort i = 48; i < 56; i++)
            {
                for (var pieceIdx = PromotionPiece.Knight; pieceIdx < PromotionPiece.Queen; pieceIdx++)
                {
                    var expected = MoveHelpers.GenerateMove(i, (ushort)(i + 8), MoveType.EnPassent, pieceIdx);
                    var input = BoardHelpers.IndexToSquareDisplay(i + 8) + $"={MoveHelpers.GetCharFromPromotionPiece(pieceIdx)}";
                    Assert.AreEqual(expected, MoveHelpers.GenerateMoveFromText(input, biEnPassent));
                }
            }
            biEnPassent.ActivePlayer = Color.Black;
            for (ushort i = 8; i < 16; i++)
            {
                for (var pieceIdx = PromotionPiece.Knight; pieceIdx < PromotionPiece.Queen; pieceIdx++)
                {
                    var expected = MoveHelpers.GenerateMove(i, (ushort)(i - 8), MoveType.EnPassent, pieceIdx);
                    var input = BoardHelpers.IndexToSquareDisplay(i - 8) + $"={MoveHelpers.GetCharFromPromotionPiece(pieceIdx)}";
                    Assert.AreEqual(expected, MoveHelpers.GenerateMoveFromText(input, biEnPassent));
                }
            }
        }

        [Test]
        public void ShouldFailWhenNoPawnIsIncapableOfEnPassent()
        {
            var fen = "8/PPPP1PPP/8/2k5/8/2K5/pppp1ppp/8 w - - 0 1";
            var bi = FENHelpers.BoardInfoFromFen(fen);
            Assert.Throws(typeof(MoveException), () =>
            {
                MoveHelpers.GenerateMoveFromText("e8=Q", bi);
            });
            Assert.Throws(typeof(MoveException), () =>
            {
                MoveHelpers.GenerateMoveFromText("e2=Q", bi);
            });
        }
        [Test]
        public void ShouldFailWhenAPieceBlocksEnPassent()
        {
            var fen = "4q3/PPPPPPPP/8/2k5/8/2K5/pppppppp/4Q3 w - - 0 1";
            var bi = FENHelpers.BoardInfoFromFen(fen);
            Assert.Throws(typeof(MoveException), () =>
            {
                MoveHelpers.GenerateMoveFromText("e8=Q", bi);
            });
            Assert.Throws(typeof(MoveException), () =>
            {
                MoveHelpers.GenerateMoveFromText("e2=Q", bi);
            });
        }
    }
}
