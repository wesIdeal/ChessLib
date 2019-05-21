using ChessLib.Data;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Types;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using ChessLib.Validators.MoveValidation.CastlingRules;
using ChessLib.Validators.MoveValidation.EnPassantRules;
using ChessLib.Validators.MoveValidation.MoveRules;
using System;
using System.Collections.Generic;

namespace ChessLib.Validators.MoveValidation
{
    public class MoveValidator
    {
        readonly BoardFENInfo _boardInfo;
        readonly List<IMoveRule> _rules = new List<IMoveRule>();
        public readonly ulong[][] PostMoveBoard;
        private readonly MoveExt _move;
        public static MoveValidator FromBitboards(in ulong[][] board, Color sideMoving, ushort? enPassantIndex, CastlingAvailability ca, MoveExt move, bool checkForStalemate = true)
        {
            var boardInfo = new BoardFENInfo()
            {
                PiecePlacement = board,
                ActivePlayer = sideMoving,
                CastlingAvailability = ca,
                EnPassantSquare = enPassantIndex
            };
            return new MoveValidator(boardInfo, move);
        }
        public MoveValidator(in BoardFENInfo board, in MoveExt move)
        {
            PostMoveBoard = BoardHelpers.GetBoardPostMove(board.PiecePlacement, board.ActivePlayer, move);
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
