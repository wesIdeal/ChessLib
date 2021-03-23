using ChessLib.Data.Boards;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Types.Exceptions;
using EnumsNET;

namespace ChessLib.Data.Tests.Validators.MoveValidation.CastlingRules
{
    internal class CastlingTestCase
    {
        public CastlingTestCase(string fen, MoveError error, MoveExt castlingMove)
        {
            Fen = fen;
            ExpectedError = error;
            CastlingMove = castlingMove;
            Board = new Board(fen);
        }

        public string Fen { get; set; }
        public MoveError ExpectedError { get; set; }
        public MoveExt CastlingMove { get; set; }
        public Board Board { get; }

        public override string ToString()
        {
            string move;
            if (CastlingMove.Equals(MoveHelpers.WhiteCastleKingSide) ||
                CastlingMove.Equals(MoveHelpers.BlackCastleKingSide))
            {
                move = "O-O";
            }
            else if (CastlingMove.Equals(MoveHelpers.WhiteCastleQueenSide) ||
                     CastlingMove.Equals(MoveHelpers.BlackCastleQueenSide))
            {
                move = "0-0-0";
            }
            else
            {
                move = "Non-Castling move.";
            }

            return
                $"{Board.ActivePlayer.AsString()}'s move is {move}, Expecting {ExpectedError.AsString()}\r\n\tFEN: {Fen}";
        }
    }
}