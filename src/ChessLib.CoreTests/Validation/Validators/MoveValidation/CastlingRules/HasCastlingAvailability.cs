using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Exceptions;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.CastlingRules
{
    /// <summary>
    ///     Tests that appropriate castling availability flag is set for castling.
    /// </summary>
    [TestFixture(Description = "Tests that appropriate castling availability flag is set for castling.")]
    internal class
        HasCastlingAvailability : Core.Validation.Validators.MoveValidation.CastlingRules.HasCastlingAvailability
    {
        private Board _bi;

        [Test(Description =
            "Test that no exception is thrown when Black castles Queenside with BlackQueenside flag set.")]
        public void ValidateMove_CastleKeyPiecesMoved_ShouldNotThrowExceptionWhenCastlingFlagIsSet_q()
        {
            _bi = MakeCastlingBoard(CastlingAvailability.BlackQueenside);
            var move = MoveHelpers.GenerateMove(60, 58, MoveType.Castle);
            AssertCastlingAvailabilityExceptionNotThrown(move);
        }

        [Test(Description =
            "Test that an exception is thrown when Black castles Queenside with BlackQueenside flag not set.")]
        public void ValidateMove_CastleKeyPiecesMoved_ShouldThrowExceptionWhenCastlingFlagIsNotSet_q()
        {
            _bi = MakeCastlingBoard(CastlingAvailability.BlackKingside | CastlingAvailability.WhiteKingside |
                                    CastlingAvailability.WhiteQueenside);
            var move = MoveHelpers.GenerateMove(60, 58, MoveType.Castle);
            AssertCastlingAvailabilityExceptionThrown(move);
        }

        [Test(Description =
            "Test that no exception is thrown when Black castles Kingside with BlackKingside flag set.")]
        public void ValidateMove_CastleKeyPiecesMoved_ShouldNotThrowExceptionWhenCastlingFlagIsSet_k()
        {
            _bi = MakeCastlingBoard(CastlingAvailability.BlackKingside);
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            AssertCastlingAvailabilityExceptionNotThrown(move);
        }

        [Test(Description =
            "Test that an exception is thrown when Black castles Kingside with BlackKingside flag not set.")]
        public void ValidateMove_CastleKeyPiecesMoved_ShouldThrowExceptionWhenCastlingFlagIsNotSet_k()
        {
            _bi = MakeCastlingBoard(CastlingAvailability.BlackQueenside | CastlingAvailability.WhiteKingside |
                                    CastlingAvailability.WhiteQueenside);
            var move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            AssertCastlingAvailabilityExceptionThrown(move);
        }

        [Test(Description =
            "Test that no exception is thrown when White castles Queenside with WhiteQueenside flag set.")]
        public void ValidateMove_CastleKeyPiecesMoved_ShouldNotThrowExceptionWhenCastlingFlagIsSet_Q()
        {
            _bi = MakeCastlingBoard(CastlingAvailability.WhiteQueenside);
            var move = MoveHelpers.GenerateMove(4, 2, MoveType.Castle);
            AssertCastlingAvailabilityExceptionNotThrown(move);
        }

        [Test(Description =
            "Test that an exception is thrown when White castles Queenside with WhiteQueenside flag not set.")]
        public void ValidateMove_CastleKeyPiecesMoved_ShouldThrowExceptionWhenCastlingFlagIsNotSet_Q()
        {
            _bi = MakeCastlingBoard(CastlingAvailability.WhiteKingside | CastlingAvailability.BlackQueenside |
                                    CastlingAvailability.BlackQueenside);
            var move = MoveHelpers.GenerateMove(4, 2, MoveType.Castle);
            AssertCastlingAvailabilityExceptionThrown(move);
        }

        [Test(Description =
            "Test that no exception is thrown when White castles Kingside with WhiteKingside flag set.")]
        public void ValidateMove_CastleKeyPiecesMoved_ShouldNotThrowExceptionWhenCastlingFlagIsSet_K()
        {
            _bi = MakeCastlingBoard(CastlingAvailability.WhiteKingside);
            var move = MoveHelpers.GenerateMove(4, 6, MoveType.Castle);
            AssertCastlingAvailabilityExceptionNotThrown(move);
        }

        [Test(Description =
            "Test that an exception is thrown when White castles Kingside with WhiteKingside flag not set.")]
        public void ValidateMove_CastleKeyPiecesMoved_ShouldThrowExceptionWhenCastlingFlagIsNotSet_K()
        {
            _bi = MakeCastlingBoard(CastlingAvailability.WhiteQueenside | CastlingAvailability.BlackKingside |
                                    CastlingAvailability.BlackQueenside);
            var move = MoveHelpers.GenerateMove(4, 6, MoveType.Castle);
            AssertCastlingAvailabilityExceptionThrown(move);
        }

        [Test(Description =
            "Test that an exception is thrown when White castles Kingside with WhiteKingside flag not set.")]
        public void ValidateMove_CastleKeyPiecesMoved_ShouldThrowExceptionWhenNoCastlingFlagIsSet()
        {
            _bi = MakeCastlingBoard(CastlingAvailability.NoCastlingAvailable);
            var move = MoveHelpers.GenerateMove(4, 6, MoveType.Castle);
            AssertCastlingAvailabilityExceptionThrown(move);
            move = MoveHelpers.GenerateMove(60, 58, MoveType.Castle);
            AssertCastlingAvailabilityExceptionThrown(move);
            move = MoveHelpers.GenerateMove(4, 2, MoveType.Castle);
            AssertCastlingAvailabilityExceptionThrown(move);
            move = MoveHelpers.GenerateMove(60, 62, MoveType.Castle);
            AssertCastlingAvailabilityExceptionThrown(move);
        }

        private static Board MakeCastlingBoard(CastlingAvailability ca, char color = 'b')
        {
            var baseBoard =
                $"r3k2r/8/8/8/8/8/8/R3K2R {color} {FENHelpers.MakeCastlingAvailabilityStringFromBitFlags(ca)} - 0 1";
            return fenTextToBoard.Translate(baseBoard);
        }

        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();

        [Test]
        public void Validate_ShouldReturnBadDestination()
        {
            var postMoveBoard = new ulong[2][];
            var badDestinations = new ushort[] { 57, 63, 1, 5, 13, 28 };
            var boardInfo = fenTextToBoard.Translate("4k2r/8/8/8/8/8/8/4K2R b kq - 1 2");
            foreach (var dest in badDestinations)
            {
                var move = MoveHelpers.GenerateMove(60, dest, MoveType.Castle);
                Assert.Throws(typeof(MoveException), () =>
                {
                    try
                    {
                        Validate(boardInfo, postMoveBoard, move);
                    }
                    catch (MoveException m)
                    {
                        Assert.AreEqual(m.Error, m.Error);
                        throw;
                    }
                });
            }
        }

        private void AssertCastlingAvailabilityExceptionThrown(Move move)
        {
            const MoveError expectedMoveExceptionType = MoveError.CastleUnavailable;
            var postBoard = new ulong[2][];
            var actual = Validate(_bi, postBoard, move);
            Assert.AreEqual(expectedMoveExceptionType, actual);
        }

        private void AssertCastlingAvailabilityExceptionNotThrown(Move move)
        {
            var postBoard = new ulong[2][];
            Assert.AreEqual(MoveError.NoneSet, Validate(_bi, postBoard, move));
        }
    }
}