using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;

namespace ChessLib.MagicBitboard.MoveValidation.MoveRules
{
    public class PositionIsNotStalemateAfterMove : IMoveRule
    {
        private KingNotInCheckAfterMove _kingNotInCheck = new KingNotInCheckAfterMove();
        public MoveExceptionType? Validate(in BoardInfo boardInfo, in ulong[][] postMoveBoard, in MoveExt move)
        {
            var oppColor = boardInfo.ActivePlayerColor;
            var activeColor = boardInfo.OpponentColor;
            var pieces = postMoveBoard[(int)boardInfo.ActivePlayerColor.Toggle()];
            var activeOcc = postMoveBoard.Occupancy(activeColor);
            var oppOcc = postMoveBoard.Occupancy(oppColor);
            for (int i = 0; i < pieces.Length; i++)
            {
                ulong pieceOccupancy = (ulong)pieces[i];
                Piece piece = (Piece)i;
                foreach (var individualPiece in pieceOccupancy.GetSetBits())
                {
                    var pseudoLegalMoves =
                        Bitboard.GetPseudoLegalMoves(piece, individualPiece, activeOcc, oppOcc, activeColor);
                    foreach (var psMove in pseudoLegalMoves.GetSetBits())
                    {
                        var moveExt = MoveHelpers.GenerateMove(individualPiece, psMove);
                        if (_kingNotInCheck.Validate(boardInfo, postMoveBoard, moveExt) == null)
                        {
                            return null;
                        }
                    }
                }
            }
            return MoveExceptionType.Stalemate;
        }
    }
}
