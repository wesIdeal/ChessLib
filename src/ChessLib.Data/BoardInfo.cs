using ChessLib.Data.Boards;
using ChessLib.Data.Exceptions;
using ChessLib.Data.Helpers;
using ChessLib.Data.Magic;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Enums;
using ChessLib.Types.Interfaces;
using ChessLib.Validators.BoardValidators;
using ChessLib.Validators.MoveValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ChessLib.Data
{
    public class BoardInfo : BoardBase
    {
        public MoveTree<MoveStorage> MoveTree { get; set; }

        public BoardInfo() : this(FENHelpers.FENInitial, false) { }

        public BoardInfo(ulong[][] occupancy, Color activePlayer, CastlingAvailability castlingAvailability,
            ushort? enPassantIdx, uint? halfMoveClock, uint fullMoveCount, bool validationException = true) : base(occupancy, activePlayer,
            castlingAvailability, enPassantIdx, halfMoveClock, fullMoveCount)
        {
            BoardValidator validator = new BoardValidator(this);
            validator.Validate(validationException);
        }

        public BoardInfo(string fen, bool is960 = false)
        {
            MoveTree = new MoveTree<MoveStorage>(null);
            _piecePlacement = FENHelpers.BoardFromFen(fen, out Color active, out CastlingAvailability ca, out ushort? enPassant, out uint hmClock, out uint fmClock);
            ActivePlayer = active;
            CastlingAvailability = ca;
            EnPassantSquare = enPassant;
            HalfmoveClock = hmClock;
            FullmoveCounter = fmClock;
            Chess960 = is960;
            InitialFEN = this.ToFEN();
        }

        public ulong ActiveRookOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Rook);
        public ulong ActiveKnightOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Knight);
        public ulong ActivePawnOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Pawn);
        public ulong ActiveQueenOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Queen);
        public ulong ActiveBishopOccupancy => GetPiecePlacement().Occupancy(ActivePlayer, Piece.Bishop);

        public void ApplySANMove(string moveText)
        {
            var moveTranslatorService = new MoveTranslatorService(this);
            var move = moveTranslatorService.GenerateMoveFromText(moveText);
            ApplyMove(move);
        }

        public MoveError? ApplyMove(MoveExt move)
        {
            var pocSource = this.GetPieceOfColorAtIndex(move.SourceIndex);
            var pocDestination = this.GetPieceOfColorAtIndex(move.DestinationIndex);
            if (pocSource == null) throw new ArgumentException("No piece at source.");
            var moveValidator = new MoveValidator(this, move);
            var validationError = moveValidator.Validate();
            if (validationError.HasValue)
                throw new MoveException("Error with move.", validationError.Value, move, ActivePlayer);
            ApplyValidatedMove(move);
            return null;
        }

        public void ApplyValidatedMove(MoveExt move)
        {
            var pocSource = this.GetPieceOfColorAtIndex(move.SourceIndex);
            var moveDisplayService = new MoveDisplayService(this);
            var shouldRecordResult = true;
            var san = moveDisplayService.MoveToSAN(move, shouldRecordResult);
            MoveTree.AddMove(new MoveStorage(this.ToFEN(), move, pocSource.Value.Piece, pocSource.Value.Color, san));
            var newBoard = this.ApplyMoveToBoard(move);
            this._piecePlacement = newBoard.GetPiecePlacement();
            this.ActivePlayer = newBoard.ActivePlayer;
            this.CastlingAvailability = newBoard.CastlingAvailability;
            this.EnPassantSquare = newBoard.EnPassantSquare;
            this.HalfmoveClock = newBoard.HalfmoveClock;
            this.FullmoveCounter = newBoard.FullmoveCounter;

        }

        public ulong GetPinnedPieces()
        {
            ulong pinned = 0;
            var xRayBishopAttacks = this.XRayBishopAttacks(ActivePlayerKingIndex);
            var xRayRookAttacks = this.XRayRookAttacks(ActivePlayerKingIndex);
            var oppQueenLocations = GetPiecePlacement().Occupancy(this.OpponentColor(), Piece.Queen);
            var oppBishopLocations = GetPiecePlacement().Occupancy(this.OpponentColor(), Piece.Bishop);
            var oppRookLocations = GetPiecePlacement().Occupancy(this.OpponentColor(), Piece.Rook);
            var bishopPinnedPieces = (oppBishopLocations | oppQueenLocations) & xRayBishopAttacks;
            var rookPinnedPieces = (oppRookLocations | oppQueenLocations) & xRayRookAttacks;
            var allPins = rookPinnedPieces | bishopPinnedPieces;
            while (allPins != 0)
            {
                var square = BitHelpers.BitScanForward(allPins);
                var squaresBetween = BoardHelpers.InBetween(square, ActivePlayerKingIndex);
                var piecesBetween = squaresBetween & GetPiecePlacement().Occupancy(ActivePlayer);
                if (piecesBetween.CountSetBits() == 1) pinned |= piecesBetween;
                allPins &= allPins - 1;
            }

            return pinned;
        }





        public bool DoesPieceAtSquareAttackSquare(ushort attackerSquare, ushort attackedSquare,
            Piece attackerPiece)
        {
            var attackedSquares = Bitboard.GetAttackedSquares(attackerPiece, attackerSquare, TotalOccupancy, Color.White);
            var attackedValue = attackedSquare.GetBoardValueOfIndex();
            return (attackedSquares & attackedValue) != 0;
        }

        public override object Clone()
        {
            return new BoardInfo(this.ToFEN());
        }
    }
}