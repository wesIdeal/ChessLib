using ChessLib.Core.Tests.Validation.Validators.MoveValidation.CastlingRules.TestData;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using ChessLib.Core.Validation.Validators.MoveValidation.CastlingRules;
using NUnit.Framework;

namespace ChessLib.Core.Tests.Validation.Validators.MoveValidation.CastlingRules
{
    /// <summary>
    ///     Test cases for castling through occupied and non-occupied squares between castling King and Rook
    /// </summary>
    [TestFixture(TestOf = typeof(NoPieceBlocksCastlingMoveValidator))]
    internal class NoPieceBlocksCastlingMoveValidatorTests
    {
        [TestCaseSource(typeof(NoPieceBlocksCastlingMoveValidatorTestData),
            nameof(NoPieceBlocksCastlingMoveValidatorTestData.GetTestCases))]
        public MoveError TestCastlingWithPiecesBlocking(Board board, Move move)
        {
            TestContext.WriteLine(board.Fen);
            var validator = new NoPieceBlocksCastlingMoveValidator();
            return validator.Validate(board, null, move);
        }
    }
}