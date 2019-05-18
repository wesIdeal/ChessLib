using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types;
using ChessLib.MagicBitboard.MoveValidation.CastlingRules;
using ChessLib.MagicBitboard.MoveValidation.EnPassantRules;
using ChessLib.MagicBitboard.MoveValidation.MoveRules;
using System;
using System.Collections.Generic;

namespace ChessLib.MagicBitboard.MoveValidation
{
    public class MoveValidator
    {
        readonly List<IMoveRule> _rules = new List<IMoveRule>();
        public readonly ulong[][] PostMoveBoard;
        private readonly BoardInfo _boardInfo;
        private readonly MoveExt _move;
        public static MoveValidator FromBitboards(in ulong[][] board, Color sideMoving, ushort? enPassantIndex, CastlingAvailability ca, MoveExt move, bool checkForStalemate = true)
        {
            var boardInfo = new BoardInfo()
            {
                PiecesOnBoard = board,
                ActivePlayerColor = sideMoving,
                CastlingAvailability = ca,
                EnPassantIndex = enPassantIndex
            };
            return new MoveValidator(boardInfo, move);
        }
        public MoveValidator(in BoardInfo board, in MoveExt move)
        {
            PostMoveBoard = BoardHelpers.GetBoardPostMove(board.PiecesOnBoard, board.ActivePlayerColor, move);
            _boardInfo = board;
            _move = move;
            _rules.Add(new PieceMovingIsActiveColor());
            _rules.Add(new KingNotInCheckAfterMove());
            switch (move.MoveType)
            {
                case MoveType.Normal:
                    _rules.Add(new PieceCanMoveToDestination());
                    _rules.Add(new DestinationNotOccupiedByActiveColor());
                    break;
                case MoveType.Promotion:
                    _rules.Add(new PieceCanMoveToDestination());
                    _rules.Add(new SourceIsPawn());
                    break;
                case MoveType.EnPassant:
                    _rules.Add(new EnPassantIsPossible());
                    _rules.Add(new SourceIsPawn());
                    _rules.Add(new SourceIsCorrectRank());
                    _rules.Add(new EnPassantSquareIsAttackedBySource());
                    break;
                case MoveType.Castle:
                    _rules.Add(new HasValidDestinationSquare());
                    _rules.Add(new HasCastlingAvailability());
                    _rules.Add(new KingNotInCheckBeforeMove());
                    _rules.Add(new CastlingSquaresNotAttacked());
                    _rules.Add(new CastlingHasNoPiecesBlocking());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public MoveExceptionType? Validate()
        {
            foreach (var rule in _rules)
            {
                var moveIssue = rule.Validate(_boardInfo, PostMoveBoard, _move);
                if (moveIssue.HasValue)
                {
                    return moveIssue;
                }
            }

            return null;
        }
    }


}
